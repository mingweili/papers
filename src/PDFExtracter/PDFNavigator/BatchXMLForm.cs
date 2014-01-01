using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using pdftron.PDF;
using System.Drawing.Imaging;
using pdftron.Common;
using System.Xml;

namespace PDFNavigator
{
    public partial class BatchXMLForm : Form
    {
        String sourcePath;
        String targetPath;

        public BatchXMLForm()
        {
            InitializeComponent();
        }

        private void sourceBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = this.sourceBrowserDialog.ShowDialog();
            
            if (result.Equals(DialogResult.OK))
                this.sourcePathTextbox.Text = this.sourcePath = sourceBrowserDialog.SelectedPath;
        }

        private void targetBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = this.targetBrowserDialog.ShowDialog();

            if (result.Equals(DialogResult.OK))
                this.targetPathTextbox.Text = this.targetPath = targetBrowserDialog.SelectedPath;
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if (this.sourcePathTextbox.Text != "" && this.targetPathTextbox.Text != "")
            {
                PDFExtractor.PDFExtractor extractor = new PDFExtractor.PDFExtractor();

                DirectoryInfo sourceDirectory = new DirectoryInfo(this.sourcePathTextbox.Text);
                double count = 0;
                double sum = (double)sourceDirectory.GetFiles().Count();
                foreach (FileInfo fileInfo in sourceDirectory.GetFiles())
                {
                    if (fileInfo.Extension.ToLower().Equals(".pdf"))
                    {
                        this.progressBar.Value = (int)(++count / sum * 100);

                        //初始化
                        extractor.init(fileInfo.FullName);

                        //开始分析PDF文档
                        extractor.processPDF();

                        //结束分析，关闭文档
                        string XmlFileName = this.targetPathTextbox.Text + "\\" + fileInfo.Name.Split('.')[0] + ".xml";
                        extractor.endProcess(XmlFileName);

                        ///于7.16日添加，用于生成截图
                        if (this.createPageImageCheckbox.Checked)
                        {
                            createPageImage(fileInfo.FullName, fileInfo.Name);
                        }
                        ///于7.17添加，并生成图像的XML标引文件
                        if(this.createImageXMLCheckbox.Checked)
                        {
                            createImageXML(XmlFileName);
                        }
                    }
                }
                this.progressBar.Value = 100;
                MessageBox.Show("转换完成");
            }
            else
                MessageBox.Show("请选择/填写文件夹路径");
        }

        private void createImageXML(string XmlFileName)
        {
            //1.打开源文档
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlFileName);

            //2.对于每一页
            foreach (XmlElement pageNode in doc.GetElementsByTagName("Page"))
            {
                double pageHeight = Double.Parse(pageNode.Attributes["Height"].Value);

                foreach (XmlElement left_down in pageNode.GetElementsByTagName("left-down"))
                {
                    double ori_y = Double.Parse(left_down.GetAttribute("y"));

                    //计算新值
                    double new_y = pageHeight - ori_y;

                    left_down.SetAttribute("y", new_y.ToString());
                }
                foreach (XmlElement left_down in pageNode.GetElementsByTagName("right-down"))
                {
                    double ori_y = Double.Parse(left_down.GetAttribute("y"));

                    //计算新值
                    double new_y = pageHeight - ori_y;

                    left_down.SetAttribute("y", new_y.ToString());
                }
                foreach (XmlElement left_down in pageNode.GetElementsByTagName("upper-right"))
                {
                    double ori_y = Double.Parse(left_down.GetAttribute("y"));

                    //计算新值
                    double new_y = pageHeight - ori_y;

                    left_down.SetAttribute("y", new_y.ToString());
                }
                foreach (XmlElement left_down in pageNode.GetElementsByTagName("upper-left"))
                {
                    double ori_y = Double.Parse(left_down.GetAttribute("y"));

                    //计算新值
                    double new_y = pageHeight - ori_y;

                    left_down.SetAttribute("y", new_y.ToString());
                }
            }
            //保存
            doc.Save(XmlFileName.Split('.')[0] + "-img.xml"); 
        }

        private void createPageImage(string orginalFileName, string shortName)
        {
            string pageImageSavePath = this.targetPathTextbox.Text + @"\" + shortName.Split('.')[0] + "_每页图像";
            Directory.CreateDirectory(pageImageSavePath);
            PDFDoc originalDoc = new PDFDoc(orginalFileName);
            PDFDraw draw = new PDFDraw();

            for (int index = 1; index <= originalDoc.GetPageCount(); ++index)
            {
                Page page = originalDoc.GetPage(index);
                draw.SetImageSize((int)page.GetPageWidth(), (int)page.GetPageHeight());
                //获取截图
                Bitmap srcImage = new Bitmap((int)page.GetPageWidth(), (int)page.GetPageHeight(), PixelFormat.Format8bppIndexed);
                srcImage = draw.GetBitmap(page);

                double[] locationPoints = new double[8];
                if (this.isAppendedLocationPoint(originalDoc, locationPoints))
                    srcImage = appendLocationPoint(srcImage, locationPoints);
                makeGreyImage(srcImage, pageImageSavePath + @"\" + index + ".bmp");
            }
            originalDoc.Close();
        }

        private Bitmap appendLocationPoint(Bitmap srcImage, double[] locationPoints)
        {
            Graphics g = Graphics.FromImage(srcImage);
            SolidBrush sb = new SolidBrush(Color.Black);
            for (int index = 0; index < 4; ++index)
            {
                g.FillEllipse(sb, new Rectangle((int)locationPoints[index*2]-15, (int)locationPoints[index*2+1]-15, 30, 30));
            }
            return srcImage;
        }

        

        private bool isAppendedLocationPoint(PDFDoc originalDoc, double[] locationPoints)
        {
            ElementReader reader = new ElementReader();
            reader.Begin(originalDoc.GetPage(1));
            Element element;
            int count = 0;
            while ((element = reader.Next()) != null)
            {
                switch (element.GetType())
                {
                    case Element.Type.e_image:
                        {
                            Matrix2D ctm = element.GetCTM();
                            double x2 = 1, y2 = 1;
                            ctm.Mult(ref x2, ref y2);
                            //分别是定位点的中点
                            Console.WriteLine("    Coords: x1={0}, y1={1}, x2={2}, y2={3}", ctm.m_h, ctm.m_v, x2, y2);

                            ///重要标注：ctm.m_h，ctm.m_v其实是贴图左下角的坐标，而非中心坐标
                            ///转化为中心坐标
                            double center_x = ctm.m_h + element.GetImageWidth() / 2;
                            double center_y = ctm.m_v + element.GetImageHeight() / 2;
                            locationPoints[count*2] = center_x;
                            locationPoints[count * 2 + 1] = center_y;
                            if (++count == 4)
                            {
                                return true;
                            }
                            break;
                        }
                }
            }

            return false;
        }

        private unsafe void makeGreyImage(Bitmap srcImage, string fileName)
        {
            if (srcImage.PixelFormat != PixelFormat.Format8bppIndexed)
            {
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

                desImage.Save(fileName);
                desImage.Dispose();
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
