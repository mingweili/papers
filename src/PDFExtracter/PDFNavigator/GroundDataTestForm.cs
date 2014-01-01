using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pdftron.PDF;
using pdftron.Filters;
using pdftron.Common;
using System.Xml;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Collections;

namespace PDFNavigator
{
    public partial class GroundDataTestForm : Form
    {
        public GroundDataTestForm()
        {
            InitializeComponent();
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            //读入文件夹,批量生成
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                this.srcDocPathTextbox.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        public string fileName;

        private void appendLocationPointButton_Click(object sender, EventArgs e)
        {
            PDFDoc doc = new PDFDoc(this.fileName);

            ElementBuilder eb = new ElementBuilder();
            ElementWriter writer = new ElementWriter();
            writer.Begin(doc.GetPage(1));

            pdftron.PDF.Image image = pdftron.PDF.Image.Create(doc, @"D:\hello\peppers.jpg");
            Element element = eb.CreateImage(image, new Matrix2D(200, 145, 20, 300, 200, 150));
            writer.WritePlacedElement(element);
            writer.End();
            writer.Dispose();
            eb.Dispose();
            doc.Save(this.fileName, pdftron.SDF.SDFDoc.SaveOptions.e_remove_unused);
            doc.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///思路
            ///1.应该是批量生成实际的文档标引信息
            ///2.对每一个标引信息：
            ///1)读入理想化的标引信息，读出定位点
            ///2)根据算法，算出未知的参数
            ///3)根据参数和读入的理想化坐标，生成实际的标引信息
            ///4)保存该标引信息
            //1.获取该标准PDF的页面的定位点信息
            this.fileName = @"D:\hello\新建文件夹\hello.txt.pdf";
            string descFileName = this.fileName.Split(new char[] { '.' })[0] + ".xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(descFileName);

            XmlNode locationPointsNode = xmlDoc.GetElementsByTagName("LocationPoints")[0];
            int locationPointsQuantity = locationPointsNode.ChildNodes.Count;

            double[] coordinates = new double[locationPointsQuantity * 2];

            for (int index = 0; index < locationPointsQuantity; ++index)
            {
                coordinates[index * 2] = Double.Parse(locationPointsNode.ChildNodes[index].Attributes["x"].Value);
                coordinates[index * 2 + 1] = Double.Parse(locationPointsNode.ChildNodes[index].Attributes["y"].Value);
            }

            //2.获取有噪声的页面的定位点信息，暂时将这个函数保留
            double[] disturbedCoordinates = getDisturbedCoordinates(coordinates);

            //3.生成矩阵
            //对应算法中的等式左侧一列
            double[] leftMatrix = new double[9];
            Array.Copy(disturbedCoordinates, 0, leftMatrix, 1, 8);

            //对应算法中的等式右侧的混合矩阵
            double[,] mixedMatrix = new double[9, 9];
            setMixedMatrix(mixedMatrix, disturbedCoordinates, coordinates);

            //计算未知数
            double[] paras = getParas(mixedMatrix, leftMatrix);

            //测试计算得结果是否正确
            testParas(paras, coordinates[0], coordinates[1], coordinates[2], coordinates[3]);
        }

        private void testParas(double[] paras, double rx1, double ry1, double rx2, double ry2)
        {
            Console.WriteLine("\n----------------------------------------测试参数-------------------------------------");
            double p1 = paras[6] * rx1 + paras[7] * ry1 + 1;

            double x1 = (paras[0] * rx1 + paras[1] * ry1 + paras[2]) / p1;
            double y1 = (paras[3] * rx1 + paras[4] * ry1 + paras[5]) / p1;

            Console.WriteLine("rx1:" + rx1.ToString() + " " + "ry1:" + ry1.ToString());
            Console.WriteLine("x1:" + x1.ToString() + " " + "y1:" + y1.ToString());


            double p2 = paras[6] * rx2 + paras[7] * ry2 + 1;

            double x2 = (paras[0] * rx2 + paras[1] * ry2 + paras[2]) / p2;
            double y2 = (paras[3] * rx2 + paras[4] * ry2 + paras[5]) / p2;

            Console.WriteLine("rx2:" + rx2.ToString() + " " + "ry2:" + ry2.ToString());
            Console.WriteLine("x2:" + x2.ToString() + " " + "y2:" + y2.ToString());
        }

        private double[] getParas(double[,] mixedMatrix, double[] leftMatrix)
        {
            //利用克莱姆法则求出
            //首先将mix放到8*8的矩阵中
            double[,] tempMatrix = new double[8,8];
            for (int x = 0; x < 8; ++x)
                for (int y = 0; y < 8; ++y)
                    tempMatrix[x, y] = mixedMatrix[x + 1, y + 1];


            double D = MatrixValue(tempMatrix, 8);
            double[] paras = new double[8];
            for (int index = 0; index < 8; ++index)
            { 
                //计算D1,D2，D3……
                double[,] Di_Matrix = new double[8, 8];
                for (int x = 0; x < 8; ++x)
                    for (int y = 0; y < 8; ++y)
                        Di_Matrix[x, y] = tempMatrix[x, y];

                //leftMatrix是9个长度
                for (int i = 0; i < 8; ++i)
                    Di_Matrix[i, index] = leftMatrix[i + 1];
                double Di = MatrixValue(Di_Matrix, 8);

                paras[index] = Di / D;
            }

            return paras;
        }

        private double[,] multiplyMatrix(double[,] a, double[,] b)
        {
            double[,] result = new double[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)            
                for (int j = 0; j < b.GetLength(1); j++)                
                    for (int k = 0; k < a.GetLength(1); k++)                    
                     result[i, j] += a[i, k] * b[k, j];
            
            return result;

        }
        private double[,] ReverseMatrix(double[,] dMatrix, int Level)
        {
            double dMatrixValue = MatrixValue(dMatrix, Level);
            if (dMatrixValue == 0) return null;

            double[,] dReverseMatrix = new double[Level, 2 * Level];
            double x, c;
            // Init Reverse matrix
            for (int i = 0; i < Level; i++)
            {
                for (int j = 0; j < 2 * Level; j++)
                {
                    if (j < Level)
                        dReverseMatrix[i, j] = dMatrix[i, j];
                    else
                        dReverseMatrix[i, j] = 0;
                }

                dReverseMatrix[i, Level + i] = 1;
            }

            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)
            {
                if (dReverseMatrix[i, j] == 0)
                {
                    int m = i;
                    for (; dMatrix[m, j] == 0; m++) ;
                    if (m == Level)
                        return null;
                    else
                    {
                        // Add i-row with m-row
                        for (int n = j; n < 2 * Level; n++)
                            dReverseMatrix[i, n] += dReverseMatrix[m, n];
                    }
                }

                // Format the i-row with "1" start
                x = dReverseMatrix[i, j];
                if (x != 1)
                {
                    for (int n = j; n < 2 * Level; n++)
                        if (dReverseMatrix[i, n] != 0)
                            dReverseMatrix[i, n] /= x;
                }

                // Set 0 to the current column in the rows after current row
                for (int s = Level - 1; s > i; s--)
                {
                    x = dReverseMatrix[s, j];
                    for (int t = j; t < 2 * Level; t++)
                        dReverseMatrix[s, t] -= (dReverseMatrix[i, t] * x);
                }
            }

            // Format the first matrix into unit-matrix
            for (int i = Level - 2; i >= 0; i--)
            {
                for (int j = i + 1; j < Level; j++)
                    if (dReverseMatrix[i, j] != 0)
                    {
                        c = dReverseMatrix[i, j];
                        for (int n = j; n < 2 * Level; n++)
                            dReverseMatrix[i, n] -= (c * dReverseMatrix[j, n]);
                    }
            }

            double[,] dReturn = new double[Level, Level];
            for (int i = 0; i < Level; i++)
                for (int j = 0; j < Level; j++)
                    dReturn[i, j] = dReverseMatrix[i, j + Level];
            return dReturn;
        }

        private void test()
        {
            //测试矩阵求值函数
            
        }

        private double MatrixValue(double[,] MatrixList, int Level)
        {
            double[,] dMatrix = new double[Level, Level];

            for (int i = 0; i < Level; i++)
            {
                for (int j = 0; j < Level; j++)
                {
                    dMatrix[i, j] = MatrixList[i, j];
                }
            }

            int sign = 1;

            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)
            {
                //判断改行dMatrix[i, j]是否为0，若是，则寻找i后的行（m,m>i，切dMatrix[m, j]!=0)进行交换
                if (dMatrix[i, j] == 0)
                {
                    if (i == Level - 1)
                    {
                        return 0;
                    }

                    int m = i + 1;

                    //获取一个dMatrix[m, j]不为为0的行
                    for (; dMatrix[m, j] == 0; m++)
                    {
                        if (m == Level - 1)
                        {
                            return 0;
                        }
                    }

                    //判断是否达到矩阵的最大行，若是，则返回0


                    //把i行和m行调换
                    double temp;
                    for (int n = j; n < Level; n++)
                    {
                        temp = dMatrix[i, n];
                        dMatrix[i, n] = dMatrix[m, n];
                        dMatrix[m, n] = temp;
                    }
                    sign *= (-1);
                }

                //把当前行以后的行所对应的列变成0
                double tmp;
                for (int s = Level - 1; s > i; s--)
                {
                    tmp = dMatrix[s, j];
                    //j行后面的所有行
                    for (int t = j; t < Level; t++)
                    {
                        dMatrix[s, t] -= dMatrix[i, t] * (tmp / dMatrix[i, j]);
                    }
                }
            }

            double result = 1;
            for (int i = 0; i < Level; i++)
            {
                if (dMatrix[i, i] != 0)
                {
                    result *= dMatrix[i, i];
                }
                else
                {
                    return 0;
                }
            }

            return sign * result;
        }

        private void setMixedMatrix(double[,] mixedMatrix, double[] disturbedCoordinates, double[] coordinates)
        {
            for (int index = 1; index <= 4; ++index)
            {
                //奇数行
                mixedMatrix[index * 2 - 1, 1] = coordinates[index * 2 - 2];
                mixedMatrix[index * 2 - 1, 2] = coordinates[index * 2 - 1];

                mixedMatrix[index * 2 - 1, 3] = 1;

                for (int i = 4; i <= 6; ++i)
                    mixedMatrix[index * 2 - 1, i] = 0;

                mixedMatrix[index * 2 - 1, 7] = -coordinates[index * 2 - 2] * disturbedCoordinates[index * 2 - 2];
                mixedMatrix[index * 2 - 1, 8] = -coordinates[index * 2 - 1] * disturbedCoordinates[index * 2 - 2];


                //偶数行
                for (int i = 1; i <= 3; ++i)
                    mixedMatrix[index * 2, i] = 0;

                mixedMatrix[index * 2, 4] = coordinates[index * 2 - 2];
                mixedMatrix[index * 2, 5] = coordinates[index * 2 - 1];

                mixedMatrix[index * 2, 6] = 1;

                mixedMatrix[index * 2, 7] = -coordinates[index * 2 - 2] * disturbedCoordinates[index * 2 - 1];
                mixedMatrix[index * 2, 8] = -coordinates[index * 2 - 1] * disturbedCoordinates[index * 2 - 1];
            }
        }

        private double[] getDisturbedCoordinates(double[] coordinates)
        {
            double[] hello = new double[8];
            for (int i = 0; i < 8; ++i)
                hello[i] = coordinates[i] + 3;
            return hello;
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if (this.srcDocPathTextbox.Text != String.Empty
                && this.disturbedDocImagePathTextbox.Text != String.Empty
                && this.desSavePathTextbox.Text != String.Empty)
            {
                int count = 0;
                DirectoryInfo di = new DirectoryInfo(this.srcDocPathTextbox.Text);
                int sum = di.GetFiles().Length;
                foreach (FileInfo fi in di.GetFiles())
                {
                    //如果是PDF文件，则执行（假设PDF理想文档和它的标引文件在同一目录下，且名字相同）
                    if (fi.Extension.Equals(".pdf"))
                    {
                        ++count;
                        doIt(fi);
                        setProgressBar(count / sum);
                    }
                }
                setProgressBar(1);
                MessageBox.Show("已完成");
            }
            else
                MessageBox.Show("请选择/填写文件夹路径");
        }

        private void setProgressBar(int ratio)
        {
            this.progressBar1.Value = ratio * 100;
        }

        private void doIt(FileInfo fi)
        {
            //1.读入定位点信息
            string pdfFileName = fi.FullName;
            string descFileName = pdfFileName.Split('.')[0] + ".xml";

            //理想化文档的定位点坐标，暂时设定位点只有4个
            double[] coordinates = new double[8];
            int pageCount = getIdealLocationPointsCoordinates(descFileName, coordinates);

            ///以下工作应该是针对每一页的含噪声图像
            //存储了一份文档中所有的paras
            double[,] paraGroup = new double[pageCount, 8];
            for (int index = 1; index <= pageCount; ++index)
            {
                //暂定每一页的含噪声图像的命名规则是："文件名_i.bmp",其中i是第几页
                string disturbedDocImageName = this.disturbedDocImagePathTextbox.Text + @"\" + index.ToString() + ".bmp";
                //2.获取有噪声的页面的定位点信息，暂时将这个函数保留,应该传入disturbedDocImageName而不是coordinates
                //double[] disturbedCoordinates = getDisturbedCoordinates(coordinates);
                //disturbedDocImageName = makeDegration(disturbedDocImageName);
                double[] disturbedCoordinates = getDisturbedCoordinates(disturbedDocImageName);
                Console.WriteLine("找到的定位点个数" + disturbedCoordinates.Length / 2);
                Console.WriteLine(">>({0}, {1});({2}, {3});({4}, {5});({6}, {7})", disturbedCoordinates[0], disturbedCoordinates[1], disturbedCoordinates[2], disturbedCoordinates[3], disturbedCoordinates[4], disturbedCoordinates[5], disturbedCoordinates[6], disturbedCoordinates[7]);
                //3.生成矩阵
                //对应算法中的等式左侧一列
                double[] leftMatrix = new double[9];
                Array.Copy(disturbedCoordinates, 0, leftMatrix, 1, 8);
                //对应算法中的等式右侧的混合矩阵
                double[,] mixedMatrix = new double[9, 9];
                setMixedMatrix(mixedMatrix, disturbedCoordinates, coordinates);

                //计算未知数
                double[] parasPerPage = getParas(mixedMatrix, leftMatrix);
                ///于7.8添加，测试参数的准确度
                testParas(coordinates, parasPerPage, pageHeight);
                for (int i = 0; i < 8; ++i)
                    paraGroup[index - 1, i] = parasPerPage[i];

            }
            //生成含有噪点的标引文件信息
            createDisturbedDescDoc(fi.FullName, fi.Name, paraGroup);
        }

        //private string makeDegration(string disturbedDocImageName)
        //{
        //    if (this.degrationGroup.Visible)
        //    {
        //        Degrator degrator = new Degrator();
        //        //检测用户的选择情况
        //        bool isDegration = false;
        //        if (this.showThroughCheckbox.Checked)
        //        {
        //            try
        //            {
        //                String backImageName = this.showThroughTextbox.Text;
        //                ShowThroughPara para = new ShowThroughPara();
        //                para.backFileName = backImageName;
        //                para.adjustment = Double.Parse(this.showThroughAdjust.Text);
        //                para.colorDifference = (int)this.showThroughColorDifferenceNumericUpDown.Value;

        //                degrator.isShowThrough = true;
        //                degrator.showThroughPara = para;

        //                isDegration = true;
        //            }
        //            catch (Exception ee)
        //            {
        //                MessageBox.Show("show-through退化参数输入不正确");
        //            }
        //        }
        //        if (this.specklesCheckbox.Checked)
        //        {
        //            try
        //            {
        //                SpecklePara para = new SpecklePara();
        //                para.count = (int)this.speckleNumericUpDown.Value;
        //                para.size = (int)this.speckleSizeNumericUpDown.Value;
        //                para.density = Double.Parse(this.speckleDensityTextbox.Text);

        //                degrator.isSpeckles = true;
        //                degrator.specklePara = para;

        //                isDegration = true;
        //            }
        //            catch (Exception ee)
        //            {
        //                MessageBox.Show("斑点退化参数输入不正确");
        //            }
        //        }
        //        if (this.blurCheckbox.Checked)
        //        {
        //            degrator.isBlur = true;

        //            isDegration = true;
        //        }
        //        if (this.addLineCheckbox.Checked)
        //        {
        //            try
        //            {
        //                AddLinePara para = new AddLinePara();
        //                para.count = (int)this.lineNumericUpDown.Value;
        //                int[] width = new int[2];
        //                width[0] = Int32.Parse(this.lineWidthMinTextbox.Text);
        //                width[1] = Int32.Parse(this.lineWidthMaxTextbox.Text);
        //                para.widthThreshold = width;
        //                int[] length = new int[2];
        //                length[0] = Int32.Parse(this.lineLengthMinTextbox.Text);
        //                length[1] = Int32.Parse(this.lineLengthMaxTextbox.Text);
        //                para.lengthThreshold = length;
        //                para.density = Double.Parse(this.lineDensityTextbox.Text);
        //                para.colorPara = Double.Parse(this.lineColorTextbox.Text);

        //                degrator.isAddLine = true;
        //                degrator.addLinePara = para;

        //                isDegration = true;
        //            }
        //            catch (Exception ee)
        //            {
        //                MessageBox.Show("线条退化参数输入不正确");
        //            }
        //        }
        //        if (this.jitterCheckbox.Checked)
        //        {
        //            try
        //            {
        //                JitterPara para = new JitterPara();
        //                para.jitterRadium = (int)this.jitterRadiumNumericUpDown.Value;

        //                degrator.isJitter = true;
        //                degrator.jitterPara = para;

        //                isDegration = true;
        //            }
        //            catch (Exception ee)
        //            {
        //                MessageBox.Show("抖动退化参数输入不正确");
        //            }
        //        }
        //        if (this.rotationCheckbox.Checked)
        //        {
        //            try
        //            {
        //                RotationPara para = new RotationPara();
        //                para.angle = Double.Parse(this.rotationAngleTextbox.Text);

        //                degrator.isRotation = true;
        //                degrator.rotationPara = para;

        //                isDegration = true;
        //            }
        //            catch (Exception ee)
        //            {
        //                MessageBox.Show("旋转退化参数输入不正确");
        //            }
        //        }
        //        if (this.modelCheckbox.Checked)
        //        {
        //            try
        //            {
        //                ModelPara para = new ModelPara();
        //                para.α = Double.Parse(this.alphaTextbox.Text);
        //                para.β = Double.Parse(this.beitaTextbox.Text);
        //                para.α0 = Double.Parse(this.alpha0Textbox.Text);
        //                para.β0 = Double.Parse(this.beita0Textbox.Text);
        //                para.model = Int32.Parse(this.modelSizeTextbox.Text);

        //                degrator.modelPara = para;

        //                return degrator.testDegrationModel(disturbedDocImageName);
        //            }
        //            catch (Exception ee)
        //            {
        //                MessageBox.Show("经典退化模型参数输入不正确");
        //            }
        //        }

        //        string degradedImageName = String.Empty;
        //        if (isDegration)
        //            degradedImageName = degrator.degration(disturbedDocImageName);
        //        else
        //            degradedImageName = fileName;

        //        return degradedImageName;
        //    }
        //    else
        //        return disturbedDocImageName;
        //}

        [DllImport("fschh.dll", CharSet = CharSet.Ansi, EntryPoint = "BWLabel_BYTE")]
        //IntPtr, int, int, int, int, ref int
        private extern static unsafe long BWLabel_BYTE(IntPtr data, int lHeight, int lWidth, int mode, int* dest);

        private unsafe double[] getDisturbedCoordinates(string disturbedDocImageName)
        {
            ///步骤：
            ///1.获取连通区域的结果，由BWLabel_BYTE中传入的参数data指向；
            ///2.对于每一个连通域：
            ///     1）想办法获取该连通域的长宽；
            ///     2）根据不等式的要求，对连通域进行筛选
            
            //首先进行灰度处理
            makeGreyImage(disturbedDocImageName);
            //1.获取连通域的结果
            Bitmap image = new Bitmap(disturbedDocImageName);
            this.imageHeight = image.Height;

            //获取图像的BitmapData对像 
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
            Console.WriteLine("基本信息:宽：{0}, 高：{1}, Stride:{2}， 像素格式：{3}", data.Width, data.Height, data.Stride, image.PixelFormat.ToString());

            byte* rawDataPtr = (byte*)Marshal.AllocHGlobal(data.Width * data.Height);
            IntPtr rawData = new IntPtr(rawDataPtr);
            byte* location = rawDataPtr;
            //循环处理 
            unsafe
            {
                byte* ptr = (byte*)(data.Scan0);
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        // write the logic implementation here 
                        //进行二值化处理，根据算法的具体要求（BW_Label.c的75行），将内容（黑色）置为1，白色置为0
                        if (*ptr <= 128)
                            *ptr = 1;
                        else
                            *ptr = 0;
                        *location = *ptr;
                        ptr++;
                        location++;
                    }
                    ptr += data.Stride - data.Width;
                }
            }
            image.UnlockBits(data);
            int* dest = (int*)Marshal.AllocHGlobal(data.Height * data.Width * 4);

            BWLabel_BYTE(rawData, data.Height, data.Width, 8, dest);
            
            //读入dest数组，将每一个连通域读出，并读出他们的长宽、及顶点坐标
            double[] disturbedCoordinates = readConnectedDomain(dest, data.Width, data.Height);
            
            return disturbedCoordinates;
        }

        unsafe private void makeGreyImage(string disturbedDocImageName)
        {
            Bitmap srcImage = new Bitmap(disturbedDocImageName);
            int width = srcImage.Width;
            int height = srcImage.Height;
            BitmapData srcImageData = srcImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = srcImageData.Stride;
            byte* src_ptr = (byte*)srcImageData.Scan0.ToPointer();

            Bitmap desImage = new Bitmap(srcImage.Width, srcImage.Height, PixelFormat.Format8bppIndexed);
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

            srcImage.UnlockBits(srcImageData);
            srcImage.Dispose();
            desImage.UnlockBits(des_data);
            ColorPalette palette = desImage.Palette;
            for (int i = 0; i != palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            desImage.Palette = palette;

            desImage.Save(disturbedDocImageName);
            desImage.Dispose();
        }

        private void createDisturbedDescDoc(string fullName, string shortFileName, double[,] paraGroup)
        {
            ///思路：1.打开理想的标引文档，遍历之
            ///2.在遍历的同时，抽取出坐标信息，计算出实际坐标
            ///3.在遍历的同时，生成相同格式的含噪声的标引文件
            string disturbedDocName = this.desSavePathTextbox.Text + "\\" + shortFileName.Split('.')[0] + ".xml";

            //创建含噪点的XML标引文档
            XmlDocument disturbedDescDoc = new XmlDocument();

            //创建文件头
            XmlDeclaration d = disturbedDescDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            disturbedDescDoc.AppendChild(d);

            //创建根节点
            XmlNode newRootNode = disturbedDescDoc.CreateElement("Pdf");

            //打开理想化的标引文档
            XmlDocument idealDoc = new XmlDocument();

            idealDoc.Load(fullName.Split('.')[0] + ".xml");

            //复制info节点
            newRootNode.AppendChild(disturbedDescDoc.ImportNode(idealDoc.GetElementsByTagName("Info")[0], true));

            //新建content节点
            XmlElement newContentNode = disturbedDescDoc.CreateElement("Content");

            //开始读取理想化的content节点
            int index = -1;
            foreach (XmlNode pageNode in idealDoc.GetElementsByTagName("Page"))
            {
                //读取页面的高度，用于转化坐标
                double pageHeight = Double.Parse(pageNode.Attributes["Height"].Value);
                ++index;
                //对于每一页

                double[] paras = new double[8];
                int i = -1;
                while (++i < 8)
                    paras[i] = paraGroup[index, i];

                XmlElement newPageNode = disturbedDescDoc.CreateElement("Page");
                newPageNode.SetAttribute("LineCount", pageNode.Attributes["LineCount"].Value);
                newPageNode.SetAttribute("Height", pageNode.Attributes["Height"].Value);
                newPageNode.SetAttribute("Width", pageNode.Attributes["Width"].Value);
                newPageNode.SetAttribute("Index", pageNode.Attributes["Index"].Value);

                foreach (XmlNode lineNode in pageNode.ChildNodes)
                {
                    XmlElement newLineNode = disturbedDescDoc.CreateElement("Line");
                    newLineNode.SetAttribute("Index", lineNode.Attributes["Index"].Value);
                    newLineNode.SetAttribute("CharCount", lineNode.Attributes["CharCount"].Value);
                    newLineNode.SetAttribute("Value", lineNode.Attributes["Value"].Value);

                    //取行坐标
                    XmlNode lineCornersNode = lineNode.ChildNodes[0];

                    //左下
                    XmlNode left_downLineNode = lineCornersNode.ChildNodes[0];
                    double x = Double.Parse(left_downLineNode.Attributes["x"].Value);
                    double y = Double.Parse(left_downLineNode.Attributes["y"].Value);

                    double[] new_xy = new double[2];
                    caculateNewCoordinate(new_xy, paras, x, y, pageHeight);

                    //设置新值
                    XmlElement newLeft_downLineNode = disturbedDescDoc.CreateElement("left-down");
                    newLeft_downLineNode.SetAttribute("x", new_xy[0].ToString());
                    newLeft_downLineNode.SetAttribute("y", new_xy[1].ToString());
                    
                    ///于7.6日添加，添加右下，左上两点
                    //右下
                    XmlNode right_downLineNode = lineCornersNode.ChildNodes[1];
                    x = Double.Parse(right_downLineNode.Attributes["x"].Value);
                    y = Double.Parse(right_downLineNode.Attributes["y"].Value);

                    new_xy = new double[2];
                    caculateNewCoordinate(new_xy, paras, x, y, pageHeight);

                    //设置新值
                    XmlElement newRight_downLineNode = disturbedDescDoc.CreateElement("right-down");
                    newRight_downLineNode.SetAttribute("x", new_xy[0].ToString());
                    newRight_downLineNode.SetAttribute("y", new_xy[1].ToString());

                    //右上
                    XmlNode upper_rightLineNode = lineCornersNode.ChildNodes[2];
                    x = Double.Parse(upper_rightLineNode.Attributes["x"].Value);
                    y = Double.Parse(upper_rightLineNode.Attributes["y"].Value);

                    new_xy = new double[2];
                    caculateNewCoordinate(new_xy, paras, x, y, pageHeight);

                    //设置新值
                    XmlElement newUpper_rightLineNode = disturbedDescDoc.CreateElement("upper-right");
                    newUpper_rightLineNode.SetAttribute("x", new_xy[0].ToString());
                    newUpper_rightLineNode.SetAttribute("y", new_xy[1].ToString());

                    //左上
                    XmlNode upper_leftLineNode = lineCornersNode.ChildNodes[3];
                    x = Double.Parse(upper_leftLineNode.Attributes["x"].Value);
                    y = Double.Parse(upper_leftLineNode.Attributes["y"].Value);

                    new_xy = new double[2];
                    caculateNewCoordinate(new_xy, paras, x, y, pageHeight);

                    //设置新值
                    XmlElement newUpper_leftLineNode = disturbedDescDoc.CreateElement("upper-left");
                    newUpper_leftLineNode.SetAttribute("x", new_xy[0].ToString());
                    newUpper_leftLineNode.SetAttribute("y", new_xy[1].ToString());

                    
                    //添加节点
                    XmlElement newLineCornerNode = disturbedDescDoc.CreateElement("LineCorners");
                    newLineCornerNode.AppendChild(newLeft_downLineNode);
                    newLineCornerNode.AppendChild(newRight_downLineNode);
                    newLineCornerNode.AppendChild(newUpper_rightLineNode);
                    newLineCornerNode.AppendChild(newUpper_leftLineNode);

                    newLineNode.AppendChild(newLineCornerNode);

                    //对于每个字（word）
                    //<Word Value="智">
                    foreach (XmlElement wordNode in lineNode.ChildNodes)
                    {
                        if (wordNode.Name.Equals("LineCorners"))
                            continue;
                        else 
                        {
                            XmlElement newWordNode = disturbedDescDoc.CreateElement("Word");
                            newWordNode.SetAttribute("Value", wordNode.Attributes["Value"].Value);

                            //加入字体节点
                            newWordNode.AppendChild(disturbedDescDoc.ImportNode(wordNode.ChildNodes[0], true));

                            XmlNode wordCornerNode = wordNode.ChildNodes[1];
                            XmlNode newWordCornerNode = disturbedDescDoc.CreateElement("WordCorners");
                            foreach(XmlNode coorNode in wordCornerNode.ChildNodes)
                            {
                                XmlElement newcoorNode = disturbedDescDoc.CreateElement(coorNode.Name);
                                x = Double.Parse(coorNode.Attributes["x"].Value);
                                y = Double.Parse(coorNode.Attributes["y"].Value);
                                caculateNewCoordinate(new_xy, paras, x, y, pageHeight);
                                newcoorNode.SetAttribute("x", new_xy[0].ToString());
                                newcoorNode.SetAttribute("y", new_xy[1].ToString());

                                newWordCornerNode.AppendChild(newcoorNode);
                            }

                            newWordNode.AppendChild(newWordCornerNode);

                            //如果有char的节点，继续添加
                            if (wordCornerNode.LastChild.Name.Equals("Char"))                            
                            {
                                foreach (XmlElement charNode in wordNode.ChildNodes)
                                {
                                    if (charNode.Name.Equals("Char"))
                                    {
                                        XmlElement newCharNode = disturbedDescDoc.CreateElement("Char");
                                        newCharNode.SetAttribute("Value", charNode.Attributes["Value"].Value);

                                        //加入字体节点
                                        newCharNode.AppendChild(charNode.ChildNodes[0]);

                                        XmlNode charCornerNode = charNode.ChildNodes[1];
                                        XmlNode newCharCornerNode = disturbedDescDoc.CreateElement("CharCorners");
                                        foreach (XmlNode coorNode in charCornerNode.ChildNodes)
                                        {
                                            XmlElement newcoorNode = disturbedDescDoc.CreateElement(coorNode.Name);
                                            x = Double.Parse(coorNode.Attributes["x"].Value);
                                            y = Double.Parse(coorNode.Attributes["y"].Value);
                                            caculateNewCoordinate(new_xy, paras, x, y, pageHeight);
                                            newcoorNode.SetAttribute("x", new_xy[0].ToString());
                                            newcoorNode.SetAttribute("y", new_xy[1].ToString());

                                            newCharCornerNode.AppendChild(newcoorNode);
                                        }

                                        //将每个char的corners添加到char
                                        newCharNode.AppendChild(newCharCornerNode);

                                        //将每个char添加到word
                                        newWordNode.AppendChild(newCharNode);
                                    }
                                }
                            }

                            //将每个word添加到line
                            newLineNode.AppendChild(newWordNode);
                        }
                    }

                    //将每个line添加到page
                    newPageNode.AppendChild(newLineNode);
                }

                //将每个page添加到content
                newContentNode.AppendChild(newPageNode);
            }

            //将content添加到根节点
            newRootNode.AppendChild(newContentNode);

            //将根节点添加到文档
            disturbedDescDoc.AppendChild(newRootNode);

            //保存
            disturbedDescDoc.Save(disturbedDocName);
        }

        private void caculateNewCoordinate(double[] new_xy, double[] paras, double x, double y, double pageHeight)
        {
            y = pageHeight - y;
            double p = paras[6] * x + paras[7] * y + 1;
            new_xy[0] = (paras[0] * x + paras[1] * y + paras[2]) / p;
            new_xy[1] = (paras[3] * x + paras[4] * y + paras[5]) / p;
        }

        private void testParas(double[] loc, double[] paras, double pageh)
        {
            Console.Write(">>");
            for (int index = 0; index < 7; index += 2)
            {
                double p = paras[6] * loc[index] + paras[7] * loc[index + 1] + 1;
                Console.Write("({0}, {1}), ", ((paras[0] * loc[index] + paras[1] * loc[index + 1] + paras[2]) / p).ToString(), ((paras[3] * loc[index] + paras[4] * loc[index + 1] + paras[5]) / p).ToString());
            }
        }

        private int getIdealLocationPointsCoordinates(string descFileName, double[] coordinates)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(descFileName);

            //读出文档的高，以作坐标变换
            XmlNode pageHeightNode = xmlDoc.GetElementsByTagName("Page")[0];
            this.pageHeight = Double.Parse(pageHeightNode.Attributes["Height"].Value);

            XmlNode locationPointsNode = xmlDoc.GetElementsByTagName("LocationPoints")[0];
            int locationPointsQuantity = locationPointsNode.ChildNodes.Count;

            for (int index = 0; index < locationPointsQuantity; ++index)
            {
                //此定位点的顺序为：左上角、右上角、右下角、左下角
                coordinates[index * 2] = Double.Parse(locationPointsNode.ChildNodes[index].Attributes["x"].Value);
                coordinates[index * 2 + 1] = Double.Parse(locationPointsNode.ChildNodes[index].Attributes["y"].Value);

                Console.WriteLine("理想化坐标  x:{0}, y:{1}", coordinates[index * 2], pageHeight - coordinates[index * 2 + 1]);
            }
            //顺序不变，但改为以左上角为坐标系（原为左下角）
            changeOriginPoint(coordinates, pageHeight);

            XmlNode pageCountNode = xmlDoc.GetElementsByTagName("PageCount")[0];
            int pageCount = Int32.Parse(pageCountNode.Attributes["Value"].Value);

            return pageCount;
        }

        private void changeOriginPoint(double[] coordinates, double pageHeight)
        {
            for (int index = 1; index < coordinates.Length; index += 2)
            {
                coordinates[index] = pageHeight - coordinates[index] - 1;
            }
        }

        private unsafe double[] readConnectedDomain(int* dest, int width, int height)
        {
            int* ptr = dest;

            //存储连通域的列表，仅用于记录所有联通域的面积
            Hashtable domainList = new Hashtable();
            for(int y = 0; y < height; ++y)
                for(int x = 0; x < width; ++x)
                {
                    //统计所有连通域的面积，并记录所有连通域的点坐标
                    int domainIndex = *ptr;
                    if (domainList.ContainsKey(domainIndex))
                        ((DomainNode)domainList[domainIndex]).add(x, y);
                    else
                        domainList.Add(domainIndex, new DomainNode(domainIndex));

                    ++ptr;
                }

            //筛选符合面积要求的连通域
            Hashtable filteredDomainList = new Hashtable();
            foreach (DictionaryEntry de in domainList)
            {
                int domainCount = ((DomainNode)de.Value).count;

                ///于7.1党的生日添加，将阈值与图像的缩放比例相一致
                double ratio = Math.Pow(((double)this.imageHeight) / this.pageHeight, 2);
                int c = domainCount;
                if (
                        (domainCount <= STARDARD_LOCATION_POINT_DOMAIN_COUNT * ratio * 1.2) &&
                        (domainCount >= STARDARD_LOCATION_POINT_DOMAIN_COUNT * ratio * 0.8)
                        //(domainCount <= 7000) &&
                        //(domainCount >= 5000)
                   )
                    filteredDomainList.Add(de.Key, de.Value);
            }
            domainList.Clear();

            //再计算出符合初步条件的连通域的宽高
            //此new_filteredDomainList是真正符合要求的连通域
            Hashtable new_filteredDomainList = new Hashtable();
            foreach (DictionaryEntry de in filteredDomainList)
            {
                int domainWidth = ((DomainNode)de.Value).getDomainWidth();
                int domainHeight = ((DomainNode)de.Value).getDomainHeight();

                double Threshold_left = 0.628;
                double Threshold_right = 0.942;
                
                double area = (double)( ((DomainNode)de.Value).count );
                double ratio = area / ( (double)domainWidth * (double)domainHeight );

                if (ratio <= Threshold_right && ratio >= Threshold_left)
                    new_filteredDomainList.Add(de.Key, de.Value);
            }

            filteredDomainList.Clear();

            //最后读出每一个中心点的坐标
            double[] disturbedCoordinates = new double[new_filteredDomainList.Count * 2];
            int i = 0;
            foreach (DictionaryEntry de in new_filteredDomainList)
            {
                ((DomainNode)de.Value).getDomainCenter().CopyTo(disturbedCoordinates, i);
                
                i += 2;
            }

            //由于根据算法找到的定位点的相互顺序不一定，需要调整顺序，使其成为按照：左上角、右上角、右下角、左下角排序
            changeDisturbedLocationPointsOrder(disturbedCoordinates);

            return disturbedCoordinates;
        }

        private void changeDisturbedLocationPointsOrder(double[] disturbedCoordinates)
        {
            double[] oldCoordinate = new double[disturbedCoordinates.Length];
            disturbedCoordinates.CopyTo(oldCoordinate, 0);

            double[,] coordinates = new double[disturbedCoordinates.Length / 2, 2];
            for (int index = 0; index < disturbedCoordinates.Length / 2; ++index)
            {
                coordinates[index, 0] = disturbedCoordinates[index * 2];
                coordinates[index, 1] = disturbedCoordinates[index * 2 + 1];
            }

            //左上角
            double min_u_l_Value = Double.MaxValue;
            int u_l_index = -1;
            //右上角
            double max_u_r_Value = Double.MinValue;
            int u_r_index = -1;
            //右下角
            double max_r_d_Value = Double.MinValue;
            int r_d_index = -1;
            //左下角
            double max_l_d_Value = Double.MinValue;
            int l_d_index = -1;

            for (int index = 0; index < coordinates.GetLength(0); ++index)
            { 
                //先判断是否是左上角
                if (coordinates[index, 0] + coordinates[index, 1] < min_u_l_Value) 
                {
                    min_u_l_Value = coordinates[index, 0] + coordinates[index, 1];
                    u_l_index = index;
                }
                //判断是否是右上角
                if (coordinates[index, 0] - coordinates[index, 1] > max_u_r_Value)
                {
                    max_u_r_Value = coordinates[index, 0] - coordinates[index, 1];
                    u_r_index = index;
                }
                //判断是否是右下角
                if (coordinates[index, 0] + coordinates[index, 1] > max_r_d_Value)
                {
                    max_r_d_Value = coordinates[index, 0] + coordinates[index, 1];
                    r_d_index = index;
                }
                //判断是否是左下角
                if (coordinates[index, 1] - coordinates[index, 0] > max_l_d_Value)
                {
                    max_l_d_Value = coordinates[index, 1] - coordinates[index, 0];
                    l_d_index = index;
                }
            }

            //重新排列
            disturbedCoordinates[0] = oldCoordinate[u_l_index * 2];
            disturbedCoordinates[1] = oldCoordinate[u_l_index * 2 + 1];
            disturbedCoordinates[2] = oldCoordinate[u_r_index * 2];
            disturbedCoordinates[3] = oldCoordinate[u_r_index * 2 + 1];
            disturbedCoordinates[4] = oldCoordinate[r_d_index * 2];
            disturbedCoordinates[5] = oldCoordinate[r_d_index * 2 + 1];
            disturbedCoordinates[6] = oldCoordinate[l_d_index * 2];
            disturbedCoordinates[7] = oldCoordinate[l_d_index * 2 + 1];
        }

        private void d_Click(object sender, EventArgs e)
        {
            DialogResult r = this.folderBrowserDialog1.ShowDialog();

            if (r == System.Windows.Forms.DialogResult.OK)
            {
                this.disturbedDocImagePath = this.disturbedDocImagePathTextbox.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        public string disturbedDocImagePath { get; set; }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult r = this.folderBrowserDialog1.ShowDialog();
            if (r == System.Windows.Forms.DialogResult.OK)
            {
                this.desSavePathText = this.desSavePathTextbox.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        public string desSavePathText { get; set; }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.srcDocPathTextbox.Text = @"D:\hello\新建文件夹\理想化文档路径";
            this.disturbedDocImagePathTextbox.Text = @"D:\hello\新建文件夹\含噪声文档图像";
            this.desSavePathTextbox.Text = @"D:\hello\新建文件夹\标引信息存储路径";
        }

        public int imageHeight { get; set; }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public double pageHeight { get; set; }

        private int STARDARD_LOCATION_POINT_DOMAIN_COUNT = 706;

        private void GroundDataTestForm_Load(object sender, EventArgs e)
        {
            if (Environment.UserName.Contains("Phoenix"))
                this.button1.Visible = true;
        }

        //private void addDegrationButton_Click(object sender, EventArgs e)
        //{
        //    if (!this.degrationGroup.Visible)
        //    {
        //        this.addDegrationButton.Text = "取消退化效果<<";
        //        this.degrationGroup.Visible = true;
        //        this.Height = 820;
        //    }
        //    else
        //    {
        //        this.addDegrationButton.Text = "添加退化效果>>";
        //        this.degrationGroup.Visible = false;
        //        this.Height = 280;
        //    }
        //}

        //private void showThroughCheckbox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.showThroughCheckbox.Checked)
        //    {
        //        this.showThroughGroup.Enabled = true;

        //        this.modelCheckbox.Checked = false;
        //        this.modelGroupBox.Enabled = false;
        //    }
        //    else
        //    {
        //        this.showThroughGroup.Enabled = false;
        //        this.showThroughTextbox.Text = String.Empty;
        //    }

        //}

        //private void specklesCheckbox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.specklesCheckbox.Checked)
        //    {
        //        this.speckleGroup.Enabled = true;

        //        this.modelCheckbox.Checked = false;
        //        this.modelGroupBox.Enabled = false;
        //    }
        //    else
        //    {
        //        this.speckleGroup.Enabled = false;
        //    }
        //}

        //private void blurCheckbox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.blurCheckbox.Checked)
        //    {
        //        this.blurGroup.Enabled = true;
        //        this.jitterGroup.Enabled = false;
        //        this.jitterCheckbox.Checked = false;

        //        this.modelCheckbox.Checked = false;
        //        this.modelGroupBox.Enabled = false;
        //    }
        //    else
        //    {
        //        this.blurGroup.Enabled = false;
        //        this.jitterGroup.Enabled = true;
        //    }
        //}

        //private void addLineCheckbox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.addLineCheckbox.Checked)
        //    {
        //        this.addLineGroup.Enabled = true;

        //        this.modelCheckbox.Checked = false;
        //        this.modelGroupBox.Enabled = false;
        //    }
        //    else
        //        this.addLineGroup.Enabled = false;
        //}

        //private void jitterCheckbox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.jitterCheckbox.Checked)
        //    {
        //        this.jitterGroup.Enabled = true;
        //        this.blurGroup.Enabled = false;
        //        this.blurCheckbox.Checked = false;

        //        this.modelCheckbox.Checked = false;
        //        this.modelGroupBox.Enabled = false;
        //    }
        //    else
        //    {
        //        this.jitterGroup.Enabled = false;
        //        this.blurGroup.Enabled = true;
        //    }
        //}

        //private void rotationCheckbox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.rotationCheckbox.Checked)
        //    {
        //        this.rotationGroup.Enabled = true;

        //        this.modelCheckbox.Checked = false;
        //        this.modelGroupBox.Enabled = false;
        //    }
        //    else
        //        this.rotationGroup.Enabled = false;
        //}

        //private void showThroughButton_Click(object sender, EventArgs e)
        //{
        //    if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        this.showThroughTextbox.Text = this.openFileDialog1.FileName;
        //    }
        //}

        //private void modelGroupBox_Enter(object sender, EventArgs e)
        //{

        //}

        //private void modelCheckbox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.modelCheckbox.Checked)
        //    {
        //        this.showThroughCheckbox.Checked = false;
        //        this.showThroughGroup.Enabled = false;
        //        this.specklesCheckbox.Checked = false;
        //        this.speckleGroup.Enabled = false;
        //        this.blurCheckbox.Checked = false;
        //        this.blurGroup.Enabled = false;
        //        this.addLineCheckbox.Checked = false;
        //        this.addLineGroup.Enabled = false;
        //        this.jitterCheckbox.Checked = false;
        //        this.jitterGroup.Enabled = false;
        //        this.rotationCheckbox.Checked = false;
        //        this.rotationGroup.Enabled = false;

        //        this.modelGroupBox.Enabled = true;
        //    }
        //    else
        //        this.modelGroupBox.Enabled = false;
        //}

        //bool formulaClicked = false;

        //private void formulaPictureBox_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (!this.formulaClicked)
        //    {
        //        this.formulaPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
        //        this.formulaPictureBox.Height = this.formulaPictureBox.Image.Height;
        //        this.formulaPictureBox.Width = this.formulaPictureBox.Image.Width;
        //        this.formulaClicked = true;
        //    }
        //    else
        //    {
        //        this.formulaPictureBox.Width = 108;
        //        this.formulaPictureBox.Height = 37;
        //        this.formulaPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        //        this.formulaClicked = false;
        //    }
        //}
    }
}
