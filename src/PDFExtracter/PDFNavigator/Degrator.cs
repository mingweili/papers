using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;

namespace PDFNavigator
{
    public struct RotationData
    {
        public int center_x;
        public int center_y;
        public int new_center_x;
        public int new_center_y;
        public double new_image_width;
        public double new_image_height;
        public double cos_angle;
        public double sin_angle;
    }
    public struct ShowThroughPara
    {
        public String backFileName{set;get;}
        public double adjustment { set; get; }
        public int colorDifference { set; get; }
    }
    public struct JitterPara
    {
        public int jitterRadium { set; get; }
    }
    public struct SpecklePara
    {
        public int count { set; get; }
        public int size { set; get; }
        public double density { set; get; }
    }
    public struct AddLinePara
    {
        public int count { set; get; }
        public int[] widthThreshold { set; get; }
        public int[] lengthThreshold { set; get; }
        public double density { set; get; }
        public double colorPara { set; get; }
    }
    public struct RotationPara
    {
        public double angle { set; get; }
    }
    public struct ModelPara
    {
        public double α { set; get; }
        public double β { set; get; }
        public double α0 { set; get; }
        public double β0 { set; get; }

        public int model { set; get; }
    }
    unsafe public class Degrator
    {
        public bool isShowThrough = false;
        public ShowThroughPara showThroughPara;

        public bool isBlur = false;

        public bool isJitter = false;
        public JitterPara jitterPara;

        public bool isSpeckles = false;
        public SpecklePara specklePara;

        public bool isAddLine = false;
        public AddLinePara addLinePara;
        
        public bool isRotation = false;
        public RotationPara rotationPara;

        public ModelPara modelPara;

        public Bitmap srcImage;
        public BitmapData srcImageData;

        unsafe public RotationData degration(string fileName, string desSavePath)
        {
            this.fileName = fileName;
            this.srcImage = new Bitmap(fileName);
            this.makeGreyImage();

            RotationData rd = new RotationData();
            //是否show-through
            if (this.isShowThrough)
                testShowThrough();
            if (this.isBlur)
                testBlur();
            if (this.isJitter)
                testJitter();
            if (this.isSpeckles)
                testSpeckles();
            if (this.isAddLine)
                testAddLine();
            if (this.isRotation)
                rd = testRotation();

            String degradedImageName = desSavePath + @"\" + new DirectoryInfo(fileName).Name;
            this.srcImage.Save(degradedImageName);

            return rd;
        }

        unsafe public string testDegrationModel(string fileName, string desSavePath)
        {
            //1.将图像灰度化、二值化

            this.srcImage = new Bitmap(fileName);
            this.fileName = fileName;
            makeGreyImage();
            makeBinaryImage();

            int width = this.srcImage.Width;
            int height = this.srcImage.Height;
            this.srcImageData = this.srcImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* ptr = (byte*)this.srcImageData.Scan0.ToPointer();
            int stride = this.srcImageData.Stride;

            Console.WriteLine("进入前/背景取反过程");
            Random random = new Random();
            //2.将背景、前景按照概率翻转，取α=β=2，α0=β0=1
            double α = this.modelPara.α;
            double β = this.modelPara.β;
            double α0 = this.modelPara.α0;
            double β0 = this.modelPara.β0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    //如果是前景
                    if (ptr[y * stride + x] == 0)
                    {
                        int[] bound_xy = new int[2];
                        int step = 0;
                        for
                        (
                        step = 1;
                        !findBound(255, x, y, step, ptr, stride, width, height, bound_xy);
                        ++step
                        ) ;

                        //找到之后，计算d（该点离背景的距离）
                        double d =
                            Math.Pow((x - bound_xy[0]), 2) + Math.Pow((y - bound_xy[1]), 2);

                        double p = α0 * Math.Pow(Math.E, -α * d);

                        //按照概率来翻转
                        if (random.NextDouble() < p)
                            ptr[y * stride + x] = 255;
                    }

                    //暂时把背景的像素翻转去掉，太耗时间
                    //else
                    //{
                    //    int[] bound_xy = new int[2];
                    //    int step = 0;
                    //    for
                    //    (
                    //    step = 1;
                    //    !findBound(0, x, y, step, ptr, stride, width, height, bound_xy);
                    //    ++step
                    //    ) ;
                    //    Console.WriteLine("\t背景步长：{0}", step);
                    //    //找到之后，计算d（该点离背景的距离）
                    //    double d =
                    //        Math.Pow((x - bound_xy[0]), 2) + Math.Pow((y - bound_xy[1]), 2);

                    //    double p = β0 * Math.Pow(Math.E, -β * d);

                    //    //按照概率来翻转
                    //    if (random.NextDouble() < p)
                    //        ptr[y * stride + x] = 0;
                    //}
                }
            }

            Console.WriteLine("离开前/背景取反过程");

            Console.WriteLine("进入膨胀处理");
            //3.做闭运算
            //1)膨胀
            //做膨胀、腐蚀的结构元素大小
            //申请目标指针，用于保存新的像素值
            byte[] des_ptr = new byte[height * stride];
            int model = this.modelPara.model;
            if (model == 2)
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        des_ptr[y * stride + x] = ptr[y * stride + x];
                        //遍历前景色，将以(x,y)点为中心、以结构元素为结构的像素矩阵改成前景色
                        if (ptr[y * stride + x] == 0)
                        {
                            if (x + 1 < width)
                                des_ptr[y * stride + (x + 1)] = 0;
                            if (y - 1 >= 0)
                                des_ptr[(y - 1) * stride + x] = 0;
                            if (x + 1 < width && y - 1 >= 0)
                                des_ptr[(y - 1) * stride + (x + 1)] = 0;
                        }
                    }
                }
            }
            //按照自定义的结构元素大小，以该结构的中心元素为中心
            else
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        des_ptr[y * stride + x] = ptr[y * stride + x];
                        if (ptr[y * stride + x] == 0)
                        {
                            for (int step_y = y - (int)Math.Floor((double)model / 2); step_y <= y + Math.Floor((double)model / 2); ++step_y)
                            {
                                for (int step_x = x - (int)Math.Floor((double)model / 2); step_x <= x + Math.Floor((double)model / 2); ++step_x)
                                {
                                    if (step_x >= 0 && step_x < width && step_y >= 0 && step_y < width)
                                    {
                                        des_ptr[step_y * stride + step_x] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Marshal.Copy(des_ptr, 0, new IntPtr(ptr), width * height);

            Console.WriteLine("离开膨胀处理");
            Console.WriteLine("进入腐蚀处理");
            //2)做腐蚀处理
            if (model == 2)
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        //遍历前景色，若图像中的前景色不能与结构元素想匹配，则去掉该中心点
                        if (ptr[y * stride + x] == 0)
                        {
                            if (x + 1 < width && ptr[y * stride + (x + 1)] != 0)
                            {
                                des_ptr[y * stride + x] = 255;
                                continue;
                            }
                            if (x + 1 < width && y - 1 >= 0 && ptr[(y - 1) * stride + (x + 1)] != 0)
                            {
                                des_ptr[y * stride + x] = 255;
                                continue;
                            }
                            if (y - 1 >= 0 && ptr[(y - 1) * stride + x] != 0)
                            {
                                des_ptr[y * stride + x] = 255;
                                continue;
                            }
                        }
                    }
                }
            }
            //按照自定义的结构元素大小，以该结构的中心元素为中心
            else
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        if (ptr[y * stride + x] == 0)
                        {
                            bool flag = false;
                            for (int step_y = y - (int)Math.Floor((double)model / 2); step_y <= y + Math.Floor((double)model / 2) && flag; ++step_y)
                            {
                                for (int step_x = x - (int)Math.Floor((double)model / 2); step_x <= x + Math.Floor((double)model / 2) && flag; ++step_x)
                                {
                                    if (step_x >= 0 && step_x < width && step_y >= 0 && step_y < width && ptr[step_y * stride + step_x] != 0)
                                    {
                                        des_ptr[y * stride + x] = 255;
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("离开腐蚀处理");

            //最后，深度拷贝
            Marshal.Copy(des_ptr, 0, new IntPtr(ptr), width * height);
            
            //最后，保存，查看效果
            this.srcImage.UnlockBits(this.srcImageData);
            String degradedImageName = desSavePath + @"\" + new DirectoryInfo(fileName).Name;
            this.srcImage.Save(degradedImageName);
            
            return fileName;
        }

        unsafe private bool findBound(byte bound, int x0, int y0, int R, byte* ptr, int stride, int width, int height, int[] bound_xy)
        {
            //通过以x0,y0为圆心，找圆上的点
            int x = 0, y = R;
            int d = 1 - R;

            while (y > x)
            {
                //判断是否在图像内，是否是背景
                int x_now = 0;
                int y_now = 0;

                x_now = x + x0;
                y_now = y + y0;
                if (ptr[y_now * stride + x_now] == bound)
                {
                    bound_xy[0] = x_now;
                    bound_xy[1] = y_now;
                    return true;
                }

                x_now = (y + x0);
                y_now = (x + y0);
                if (ptr[y_now * stride + x_now] == bound)
                {
                    bound_xy[0] = x_now;
                    bound_xy[1] = y_now;
                    return true;
                }

                x_now = -x + x0;
                y_now = y + y0;
                if (ptr[y_now * stride + x_now] == bound)
                {
                    bound_xy[0] = x_now;
                    bound_xy[1] = y_now;
                    return true;
                }

                x_now = -y + x0;
                y_now = x + y0;
                if (ptr[y_now * stride + x_now] == bound)
                {
                    bound_xy[0] = x_now;
                    bound_xy[1] = y_now;
                    return true;
                }

                x_now = -x + x0;
                y_now = -y + y0;
                if (ptr[y_now * stride + x_now] == bound)
                {
                    bound_xy[0] = x_now;
                    bound_xy[1] = y_now;
                    return true;
                }

                x_now = -y + x0;
                y_now = -x + y0;
                if (ptr[y_now * stride + x_now] == bound)
                {
                    bound_xy[0] = x_now;
                    bound_xy[1] = y_now;
                    return true;
                }

                x_now = x + x0;
                y_now = -y + y0;
                if (ptr[y_now * stride + x_now] == bound)
                {
                    bound_xy[0] = x_now;
                    bound_xy[1] = y_now;
                    return true;
                }

                x_now = y + x0;
                y_now = -x + y0;
                if (ptr[y_now * stride + x_now] == bound)
                {
                    bound_xy[0] = x_now;
                    bound_xy[1] = y_now;
                    return true;
                }

                if (d < 0)
                    d = d + 2 * x + 3;                        // d的变化
                else
                {
                    d = d + 2 * (x - y) + 5;                  // d <= 0时,d的变化
                    y--;                                      // y坐标减
                }
                x++;                                           // x坐标加
            }

            return false;
        }

        private void makeBinaryImage()
        {
            int width = this.srcImage.Width;
            int height = this.srcImage.Height;
            this.srcImageData = this.srcImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* ptr = (byte*)this.srcImageData.Scan0.ToPointer();
            int stride = this.srcImageData.Stride;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (ptr[y * stride + x] < 200)
                        ptr[y * stride + x] = 0;
                    else
                        ptr[y * stride + x] = 255;
                }
            }

            this.srcImage.UnlockBits(this.srcImageData);
            this.srcImage.Save(@"D:\bi.bmp");
        }

        public void testJitter()
        {
            int width = this.srcImage.Width;
            int height = this.srcImage.Height;

            this.srcImageData =
                this.srcImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* ptr = (byte*)this.srcImageData.Scan0.ToPointer();
            int stride = this.srcImageData.Stride;

            byte[] modified_ptr = new byte[stride * height];

            Random random = new Random();
            //为抖动的范围
            int radium = jitterPara.jitterRadium;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int r_x = random.Next(-radium, radium);
                    int r_y = random.Next(-radium, radium);
                    if (x + r_x >= 0 && x + r_x < width && y + r_y >= 0 && y + r_y < height)
                        modified_ptr[y * stride + x] = ptr[(y + r_y) * stride + (x + r_x)];
                    else
                        modified_ptr[y * stride + x] = ptr[y * stride + x];
                }
            }

            Marshal.Copy(modified_ptr, 0, new IntPtr(ptr), stride * height);

            this.srcImage.UnlockBits(this.srcImageData);
        }

        unsafe public void testShowThrough()
        {
            //1.将背面图片翻转
            Bitmap back_image = new Bitmap(showThroughPara.backFileName);
            int width = back_image.Width;
            BitmapData back_image_data =
                back_image.LockBits(new Rectangle(0, 0, back_image.Width, back_image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* back_image_ptr = (byte*)back_image_data.Scan0.ToPointer();

            Bitmap back_image_op = new Bitmap(back_image.Width, back_image.Height, PixelFormat.Format8bppIndexed);
            BitmapData back_image_op_data = back_image_op.LockBits(new Rectangle(0, 0, back_image.Width, back_image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* back_image_op_ptr = (byte*)back_image_op_data.Scan0.ToPointer();

            int stride = back_image_data.Stride;
            int rev_stride = back_image_op_data.Stride;
            for (int y = 0; y < back_image.Height; ++y)
            {
                for (int x = 0; x < back_image.Width; ++x)
                {
                    *back_image_op_ptr = back_image_ptr[y * stride + (width - x - 1)];
                    ++back_image_op_ptr;
                }
                back_image_op_ptr += (rev_stride - width);
            }
            back_image_op.UnlockBits(back_image_op_data);

            //2.模糊处理
            Bitmap done_back_image = makeBlur(back_image_op);


            //3.将两张图片合成
            BitmapData done_back_image_data = done_back_image.LockBits(new Rectangle(0, 0, done_back_image.Width, done_back_image.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* done_back_image_ptr = (byte*)done_back_image_data.Scan0.ToPointer();
            int back_stride = done_back_image_data.Stride;

            this.srcImageData = this.srcImage.LockBits(new Rectangle(0, 0, this.srcImage.Width, this.srcImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* front_image_ptr = (byte*)this.srcImageData.Scan0.ToPointer();
            int front_stride = this.srcImageData.Stride;

            int mix_width = done_back_image.Width;
            int mix_height = done_back_image.Height;

            //如果正反两面的像素大于此阈值，则生成浸透效果，否则，只保留正面图像
            int threshold = this.showThroughPara.colorDifference;
            //表示两者的差值失真的比例
            double adjustment = this.showThroughPara.adjustment;

            for (int y = 0; y < mix_height; ++y)
            {
                for (int x = 0; x < mix_width; ++x)
                {
                    byte back_color = done_back_image_ptr[y * back_stride + x];
                    byte front_color = front_image_ptr[y * front_stride + x];
                    byte difference = (byte)Math.Abs(front_color - back_color);
                    if (difference >= threshold)
                    {
                        front_image_ptr[y * front_stride + x] = (byte)(front_image_ptr[y * front_stride + x] - (byte)(adjustment * difference));
                    }
                }
            }

            done_back_image.UnlockBits(done_back_image_data);
            done_back_image.Dispose();
            this.srcImage.UnlockBits(this.srcImageData);
        }

        unsafe public Bitmap makeBlur(Bitmap back_image_op)
        {
            BitmapData data =
                back_image_op.LockBits(new Rectangle(0, 0, srcImage.Width, srcImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* src_ptr = (byte*)data.Scan0.ToPointer();
            int stride = data.Stride;
            //2. 开始转化
            //1) 申请同样大小的内存，用于存储模糊后的值
            byte[] des_ptr = new byte[data.Stride * data.Height];
            int max_x = data.Width;
            int max_y = data.Height;
            int temp_x, temp_y = 0;
            for (int y = 0; y < max_y; ++y)
            {
                for (int x = 0; x < max_x; ++x)
                {
                    int count = 0;
                    int mixedColor = 0;
                    //左上
                    temp_x = x - 1;
                    temp_y = y - 1;
                    if (temp_x >= 0 && temp_y >= 0)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //上
                    temp_x = x;
                    temp_y = y - 1;
                    if (temp_y >= 0)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //右上
                    temp_x = x + 1;
                    temp_y = y - 1;
                    if (temp_x < max_x && temp_y >= 0)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //右
                    temp_x = x + 1;
                    temp_y = y;
                    if (temp_x < max_x)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //右下
                    temp_x = x + 1;
                    temp_y = y + 1;
                    if (temp_x < max_x && temp_y < max_y)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //下
                    temp_x = x;
                    temp_y = y + 1;
                    if (temp_x < max_x && temp_y < max_y)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //左下
                    temp_x = x - 1;
                    temp_y = y + 1;
                    if (temp_x >= 0 && temp_y < max_y)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //左
                    temp_x = x - 1;
                    temp_y = y;
                    if (temp_x >= 0)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }
                    //if (mixedColor / count < 180)
                    //    des_ptr[y * stride + x] = 0;
                    //else
                    //    des_ptr[y * stride + x] = 255;
                    des_ptr[y * stride + x] = (byte)(mixedColor / count);
                }
            }
            //值拷贝，深度拷贝
            Marshal.Copy(des_ptr, 0, new IntPtr(src_ptr), max_y * max_x);
            back_image_op.UnlockBits(data);

            return back_image_op;
        }

        unsafe public void testSpeckles()
        {
            //1.打开图像
            this.srcImageData =
                this.srcImage.LockBits(new Rectangle(0, 0, srcImage.Width, srcImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            int stride = this.srcImageData.Stride;
            byte* ptr = (byte*)this.srcImageData.Scan0.ToPointer();

            //2.确定几个随机参数
            Random random = new Random();
            //1)斑点个数
            int maxSpeckleCount = this.specklePara.count;
            double speckleCount = random.Next(0, maxSpeckleCount);

            int width = srcImage.Width;
            int height = srcImage.Height;

            int count = 0;
            while (++count <= speckleCount)
            {
                //1)随机生成斑点大小
                int speckleSize = random.Next(1, this.specklePara.size);

                //2)随机生成斑点中心位置
                int position_x = random.Next(0 + speckleSize, width - speckleSize);
                int position_y = random.Next(0 + speckleSize, height - speckleSize);

                //3)随机生成斑点密度
                double speckleDensity = this.specklePara.density;

                //随机生成一个数字，表示像素密度的扩大倍数
                int n = random.Next();

                for (int y = position_y; y < position_y + speckleSize; ++y)
                {
                    for (int x = position_x; x < position_x + speckleSize; ++x)
                    {
                        //根据密度
                        if (random.NextDouble() <= speckleDensity)
                            ptr[y * stride + x] = (byte)(ptr[y * stride + x] * (1 + n));
                    }
                }
            }

            this.srcImage.UnlockBits(this.srcImageData);
        }

        //处理的图像为二值化图像，且打开方式为：Format8bppIndexed
        unsafe public void testBlur()
        {
            //1. 打开图像
            this.srcImageData =
                this.srcImage.LockBits(new Rectangle(0, 0, srcImage.Width, srcImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            byte* src_ptr = (byte*)this.srcImageData.Scan0.ToPointer();
            int stride = this.srcImageData.Stride;
            //2. 开始转化
            //1) 申请同样大小的内存，用于存储模糊后的值
            byte[] des_ptr = new byte[this.srcImageData.Stride * this.srcImageData.Height];
            int max_x = this.srcImageData.Width;
            int max_y = this.srcImageData.Height;
            int temp_x, temp_y = 0;
            for (int y = 0; y < max_y; ++y)
            {
                for (int x = 0; x < max_x; ++x)
                {
                    int count = 0;
                    int mixedColor = 0;

                    //像素本身
                    temp_x = x;
                    temp_y = y;
                    mixedColor += 4 * (int)src_ptr[temp_y * stride + temp_x];
                    count += 4;

                    //左上
                    temp_x = x - 1;
                    temp_y = y - 1;
                    if (temp_x >= 0 && temp_y >= 0)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //上
                    temp_x = x;
                    temp_y = y - 1;
                    if (temp_y >= 0)
                    {
                        mixedColor += 2 * (int)src_ptr[temp_y * stride + temp_x];
                        count += 2;
                    }

                    //右上
                    temp_x = x + 1;
                    temp_y = y - 1;
                    if (temp_x < max_x && temp_y >= 0)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //右
                    temp_x = x + 1;
                    temp_y = y;
                    if (temp_x < max_x)
                    {
                        mixedColor += 2 * (int)src_ptr[temp_y * stride + temp_x];
                        count += 2;
                    }

                    //右下
                    temp_x = x + 1;
                    temp_y = y + 1;
                    if (temp_x < max_x && temp_y < max_y)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //下
                    temp_x = x;
                    temp_y = y + 1;
                    if (temp_x < max_x && temp_y < max_y)
                    {
                        mixedColor += 2 * (int)src_ptr[temp_y * stride + temp_x];
                        count += 2;
                    }

                    //左下
                    temp_x = x - 1;
                    temp_y = y + 1;
                    if (temp_x >= 0 && temp_y < max_y)
                    {
                        mixedColor += (int)src_ptr[temp_y * stride + temp_x];
                        ++count;
                    }

                    //左
                    temp_x = x - 1;
                    temp_y = y;
                    if (temp_x >= 0)
                    {
                        mixedColor += 2 * (int)src_ptr[temp_y * stride + temp_x];
                        count += 2;
                    }
                    //if (mixedColor / count < 180)
                    //    des_ptr[y * stride + x] = 0;
                    //else
                    //    des_ptr[y * stride + x] = 255;
                    des_ptr[y * stride + x] = (byte)(mixedColor / count);
                }
            }
            //值拷贝，深度拷贝
            Marshal.Copy(des_ptr, 0, new IntPtr(src_ptr), max_y * max_x);
            this.srcImage.UnlockBits(this.srcImageData);
        }

        unsafe public void testAddLine()
        {
            //1.规定好参数
            //1)线条数量
            int lineCount = this.addLinePara.count;
            //2)线条宽度阈值
            int[] lineWidthThreshold = this.addLinePara.widthThreshold;
            //3)线条长度阈值
            int[] lineLengthThreshold = this.addLinePara.lengthThreshold;
            //4)线条密度
            double lineDensity = this.addLinePara.density;
            //5)线条横竖的概率
            double lineHorizontalOrVertical = 0.3;
            //6)线条颜色的概率
            double lineColor = this.addLinePara.colorPara;

            //2.读入原图像
            this.srcImageData =
                this.srcImage.LockBits(new Rectangle(0, 0, this.srcImage.Width, this.srcImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            int stride = this.srcImageData.Stride;
            byte* ptr = (byte*)this.srcImageData.Scan0.ToPointer();

            //随机生成
            Random random = new Random();
            int lineLength = 0;
            int lineWidth = 0;
            int lineXLocation = 0;
            int lineYLocation = 0;

            //3.开始画线
            for (int index = 0; index < lineCount; ++index)
            {
                lineLength = random.Next(lineLengthThreshold[0], lineLengthThreshold[1]);
                //生成线段宽度
                lineWidth = random.Next(lineWidthThreshold[0], lineWidthThreshold[1]);
                //随机生成线的颜色
                byte color = 0;
                if (random.NextDouble() < lineColor)
                    color = 0;
                else
                    color = 255;

                //如果是横线
                if (random.NextDouble() < lineHorizontalOrVertical)
                {
                    //随机生成线段的位置
                    lineXLocation = random.Next(0, srcImage.Width - lineLength - 1);
                    lineYLocation = random.Next(0, srcImage.Height - lineWidth - 1);

                    //开始画线
                    for (int y = lineYLocation; y < lineYLocation + lineWidth; ++y)
                    {
                        for (int x = lineXLocation; x < lineXLocation + lineLength; ++x)
                        {
                            //线条密度的判断
                            if (random.NextDouble() < lineDensity)
                            {
                                ptr[y * stride + x] = color;
                            }
                        }
                    }
                }
                else
                {
                    //随机生成线段的位置
                    lineXLocation = random.Next(0, srcImage.Width - lineWidth - 1);
                    lineYLocation = random.Next(0, srcImage.Height - lineLength - 1);

                    //开始画线
                    for (int y = lineYLocation; y < lineYLocation + lineLength; ++y)
                    {
                        for (int x = lineXLocation; x < lineXLocation + lineWidth; ++x)
                        {
                            //线条密度的判断
                            if (random.NextDouble() < lineDensity)
                            {
                                ptr[y * stride + x] = color;
                            }
                        }
                    }
                }

            }

            this.srcImage.UnlockBits(this.srcImageData);
        }

        unsafe public RotationData testRotation()
        {
            ///以图像中心为坐标系原点
            double angle = this.rotationPara.angle;
            angle = angle / 180 * Math.PI;
            double sin_angle = Math.Sin(angle);
            double cos_angle = Math.Cos(angle);

            //1.获取原图像的宽、高
            int srcHeight = this.srcImage.Height;
            int srcWidth = this.srcImage.Width;

            int center_x = srcWidth / 2;
            int center_y = srcHeight / 2;

            //2.获取原图四个角的坐标（以图像中心为坐标系原点）
            //1)
            double bottom_left_x = (double)(-srcWidth - 1) / 2;
            double bottom_left_y = (double)(srcHeight - 1) / 2;
            //2)
            double bottom_right_x = (double)(srcWidth - 1) / 2;
            double bottom_right_y = (double)(srcHeight - 1) / 2;
            //3)
            double upper_left_x = (double)(-srcWidth - 1) / 2;
            double upper_left_y = (double)(-srcHeight - 1) / 2;
            //4)
            double upper_right_x = (double)(srcWidth - 1) / 2;
            double upper_right_y = (double)(-srcHeight - 1) / 2;

            //3.计算旋转后的图像的四个顶点坐标
            //1)
            double new_bottom_left_x = cos_angle * bottom_left_x + sin_angle * bottom_left_y;
            double new_bottom_left_y = -sin_angle * bottom_left_x + cos_angle * bottom_left_y;
            //2)
            double new_bottom_right_x = cos_angle * bottom_right_x + sin_angle * bottom_right_y;
            double new_bottom_right_y = -sin_angle * bottom_right_x + cos_angle * bottom_right_y;
            //3)
            double new_upper_left_x = cos_angle * upper_left_x + sin_angle * upper_left_y;
            double new_upper_left_y = -sin_angle * upper_left_x + cos_angle * upper_left_y;
            //4)
            double new_upper_right_x = cos_angle * upper_right_x + sin_angle * upper_right_y;
            double new_upper_right_y = -sin_angle * upper_right_x + cos_angle * upper_right_y;


            //4.计算新图像的宽高
            int new_image_width = (int)(Math.Max(
                Math.Abs(new_upper_left_x - new_bottom_right_x),
                Math.Abs(new_upper_right_x - new_bottom_left_x)) + 0.5);

            int new_image_height = (int)(Math.Max(
                Math.Abs(new_upper_left_y - new_bottom_right_y),
                Math.Abs(new_upper_right_y - new_bottom_left_y)) + 0.5);

            //预先生成两个常量
            double f1 = -0.5 * (new_image_width - 1) * cos_angle - 0.5 * (new_image_height - 1) * sin_angle + 0.5 * (srcWidth - 1);
            double f2 = 0.5 * (new_image_width - 1) * sin_angle - 0.5 * (new_image_height - 1) * cos_angle + 0.5 * (srcHeight - 1);

            //读出原图像的指针，直接用指针操作
            this.srcImageData = srcImage.LockBits(new Rectangle(0, 0, srcWidth, srcHeight), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            int stride = this.srcImageData.Stride;
            byte* src_ptr = (byte*)this.srcImageData.Scan0.ToPointer();

            //5.新建一个图像
            Bitmap new_image = new Bitmap(new_image_width, new_image_height, PixelFormat.Format8bppIndexed);
            BitmapData new_image_data =
                new_image.LockBits(new Rectangle(0, 0, new_image_width, new_image_height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            byte* ptr = (byte*)(new_image_data.Scan0.ToPointer());
            for (int y = 0; y < new_image_height; ++y)
            {
                for (int x = 0; x < new_image_width; ++x)
                {
                    //计算该像素在原图中的位置坐标
                    ///于7月5日注释，改为双线性插值
                    double x0 = x * cos_angle + y * sin_angle + f1;
                    double y0 = -x * sin_angle + y * cos_angle + f2;
                    if (x0 >= 0 && x0 < srcWidth && y0 >= 0 && y0 < srcHeight)
                    {
                        //获取小数部分
                        int x0_round = (int)Math.Truncate(x0);
                        int y0_round = (int)Math.Truncate(y0);
                        double x0_dec = Math.Abs(x0 - x0_round);
                        double y0_dec = Math.Abs(y0 - y0_round);
                        //按照双线性插值，取四邻域的点，按照小数部分的比重，计算新的值
                        //本点
                        double p1 = (1 - x0_dec) * (1 - y0_dec) * src_ptr[y0_round * stride + x0_round];
                        //下点
                        double p2 = 0;
                        if (y0 + 1 < srcHeight)
                            p2 = (1 - x0_dec) * y0_dec * src_ptr[(y0_round + 1) * stride + x0_round];
                        //右点
                        double p3 = 0;
                        if (x0 + 1 < srcWidth)
                            p3 = x0_dec * (1 - y0_dec) * src_ptr[y0_round * stride + (x0_round + 1)];
                        //右下点
                        double p4 = 0;
                        if (y0 + 1 < srcHeight && x0 + 1 < srcWidth)
                            p4 = x0_dec * y0_dec * src_ptr[(y0_round + 1) * stride + (x0_round + 1)];

                        *ptr = (byte)(p1 + p2 + p3 + p4);
                    }
                    else
                        *ptr = 255;
                    ptr++;
                }
                ptr += (new_image_data.Stride - new_image_width);
            }

            this.srcImage.UnlockBits(this.srcImageData);
            new_image.UnlockBits(new_image_data);

            new_image.Palette = this.srcImage.Palette;
            this.srcImage = new_image;

            //打包生成需要转化的工具
            RotationData rotationData = new RotationData();
            rotationData.center_x = center_x;
            rotationData.center_y = center_y;
            rotationData.new_center_x = new_image_width / 2;
            rotationData.new_center_y = new_image_height / 2;
            rotationData.new_image_width = new_image_width;
            rotationData.new_image_height = new_image_height;
            rotationData.cos_angle = cos_angle;
            rotationData.sin_angle = sin_angle;

            return rotationData;
        }

        private void modifyXmlDoc(string xmlDocFileName, int new_image_width, int new_image_height, double cos_angle, double sin_angle)
        {
            //1.打开源文档
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlDocFileName);

            //2.修改页面的宽、高
            foreach (XmlElement pageNode in doc.GetElementsByTagName("Page"))
            {
                pageNode.SetAttribute("Width", Math.Truncate((double)new_image_width).ToString());
                pageNode.SetAttribute("Height",  Math.Truncate((double)new_image_height).ToString());
            }

            //3.修改坐标信息
            foreach (XmlElement left_down in doc.GetElementsByTagName("left-down"))
            {
                double ori_x = Double.Parse(left_down.GetAttribute("x"));
                double ori_y = Double.Parse(left_down.GetAttribute("y"));

                //计算新值
                double new_x = ori_x * cos_angle + ori_y * sin_angle;
                double new_y = -ori_x * sin_angle + ori_y * cos_angle;

                left_down.SetAttribute("x", new_x.ToString());
                left_down.SetAttribute("y", new_y.ToString());
            }

            //保存
            doc.Save(xmlDocFileName);
        }

        //此函数用于将图像灰度化，默认源图像格式为8位索引图像
        unsafe public void makeGreyImage()
        {
            if (this.srcImage.PixelFormat != PixelFormat.Format8bppIndexed)
                make8IndexedGreyFormat();
            else if(!isGreyFormat())
            {
                int height = this.srcImage.Height;
                int width = this.srcImage.Width;
                this.srcImageData =
                    srcImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                byte* ptr = (byte*)this.srcImageData.Scan0.ToPointer();
                int stride = this.srcImageData.Stride;

                ColorPalette palette = srcImage.Palette;

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int index = (int)ptr[y * stride + x];
                        Color color = palette.Entries[index];
                        //0.3R+0.59G+0.11B
                        byte newIndex = (byte)(0.3 * (double)color.R + 0.59 * (double)color.G + 0.11 * (double)color.B);
                        ptr[y * stride + x] = newIndex;
                    }
                }

                this.srcImage.UnlockBits(this.srcImageData);
                //修改调色板
                for (int i = 0; i < palette.Entries.Length; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                this.srcImage.Palette = palette;
            }
        }

        private bool isGreyFormat()
        {
            ColorPalette palette = this.srcImage.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                if (palette.Entries[i] != Color.FromArgb(i, i, i))
                    return false;
            }
            return true;
        }

        unsafe private void make8IndexedGreyFormat()
        {            
            int width = this.srcImage.Width;
            int height = this.srcImage.Height;
            this.srcImageData = this.srcImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = this.srcImageData.Stride;
            byte* src_ptr = (byte*)this.srcImageData.Scan0.ToPointer();

            Bitmap desImage = new Bitmap(this.srcImage.Width, this.srcImage.Height, PixelFormat.Format8bppIndexed);
            BitmapData des_data = desImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            int stride2 = des_data.Stride;
            byte* des_ptr = (byte*)des_data.Scan0.ToPointer();

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    float value = 0.11F * src_ptr[y * stride + x * 3] + 0.59F * src_ptr[y * stride + x * 3 + 1] + 0.3F * src_ptr[y * stride + x * 3 + 2];
                    des_ptr[y * stride2 + x] = (byte)value;
                }
            }

            this.srcImage.UnlockBits(this.srcImageData);
            this.srcImage.Dispose();
            desImage.UnlockBits(des_data);
            ColorPalette palette = desImage.Palette;
            for (int i = 0; i != palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            desImage.Palette = palette;

            desImage.Save(this.fileName);
            desImage.Dispose();

            //重新获取该资源
            this.srcImage = new Bitmap(this.fileName);
        }

        public string fileName { get; set; }

        public void saveGreyImage()
        {
            this.srcImage.Save(this.fileName);
        }
    }  
}
