using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pdftron.SDF;
using pdftron.Common;
using pdftron;
using pdftron.PDF;
using System.IO;
using System.Drawing.Imaging;
using System.Xml;

namespace PDFNavigator
{
    public partial class BatchCreationForm : Form
    {
        private string sourceFilePathText;
        private string objectFilePathText;
        private System.Drawing.Font selectedFont;

        private double pageHeight;
        private double pageWidth;
        private double A4_HEIGHT_IN = 11.69;
        private double A4_WIDTH_IN = 8.27;

        public BatchCreationForm()
        {
            PDFNet.Initialize();
            InitializeComponent();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            DialogResult result = this.folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.sourceFilePathText = this.sourceFilePathTextBox.Text 
                    = this.folderBrowserDialog.SelectedPath;                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = this.folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.objectFilePathText = this.objectFilePathTextBox.Text
                    = this.folderBrowserDialog.SelectedPath;
            }
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if (this.sourceFilePathText != string.Empty && this.isSelectedFont)
            {
                //获取dpi设置（在PDF定义中为userInit参数）
                double DPI;
                if (this.DPITextbox.Text != String.Empty)
                {
                    DPI = Double.Parse(this.DPITextbox.Text);
                    this.userInit = 1 / DPI;
                }
                else
                {
                    DPI = 72;
                    this.userInit = 1 / 72;
                }

                //4.25.2011修改
                //根据DPI的设定动态改变（纸张满足A4规格）
                this.pageHeight = this.A4_HEIGHT_IN * DPI;
                this.pageWidth = this.A4_WIDTH_IN * DPI;


                DirectoryInfo directoryInfo = new DirectoryInfo(this.sourceFilePathTextBox.Text);

                double count = 0;
                int sum = directoryInfo.GetFiles().Length;
                foreach (FileInfo info in directoryInfo.GetFiles())
                {
                    if (info.Extension == ".txt" || info.Extension == ".TXT")
                    {
                        executeChange(info.FullName, info.Name);
                        this.progressBar.Value = (int)(++count / sum) * 100;
                    }
                }
                MessageBox.Show("已完成");
            }
            else
                MessageBox.Show("请选择文件夹路径或字体");
        }

        private void executeChange(string fileName, String shortName)
        {
            //首先打开文本文件
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                this.statusLabel.Text = "现在状态：正在执行文本转换";

                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, (int)fileStream.Length);
                String StringData = System.Text.Encoding.Default.GetString(bytes);
                StringData = StringData.Replace("\r\n", "\n");

                StringBuilder sb = new StringBuilder(StringData);
                ///于7.9添加，在文字的结尾处添加一个换行符，以满足下面断行的完整性
                sb.Insert(sb.Length, '\n');
                
                //于4.25.2011修改，将页面宽度也作为一个参量
                float fontSize = this.selectedFont.Size;
                int charNumPerLine = (int)(580 / fontSize);

                int index = 0;
                int lastIndex = 0;
                while (index < sb.Length)
                {
                    if (sb[index] != '\n')
                    {
                        ++index;
                        continue;
                    }
                    else
                    {
                        while (lastIndex + charNumPerLine < index)
                        {
                            sb.Insert(lastIndex + charNumPerLine, '\n');
                            lastIndex += charNumPerLine;
                            ++index;
                        }
                        lastIndex = index;
                        ++index;
                    }
                }

                string[] strLineArray = sb.ToString().Split('\n');
                
                //进行转化
                PDFDoc doc = new PDFDoc();

                ElementBuilder eb = new ElementBuilder();
                ElementWriter writer = new ElementWriter();

                
                pdftron.PDF.Font fnt;
                try
                {
                    // Full font embedding
                    fnt = pdftron.PDF.Font.CreateCIDTrueTypeFont(doc, this.selectedFont, true, true);
                }
                catch (PDFNetException ee)
                {
                    PDFNet.Terminate();
                    return;
                }

                Page page = doc.PageCreate(new Rect(0, 0, this.pageWidth, this.pageHeight));
                //添加DPI的设置（于4/17早）
                page.SetUserUnitSize(this.userInit);
                writer.Begin(page);	// begin writing to this page
                ///于4.27添加，因为下面的字体不随DPI而改变（始终默认为96），故将其按照用户的设定将字体进行缩放
                fontSize = (float)(fontSize / 96 / this.userInit);
                Element element = eb.CreateTextBegin(fnt, fontSize);

                ///5.19日修改，将文字居中
                double offsetInWidth = fontSize * 1.4;
                element.SetTextMatrix(1, 0, 0, 1, this.pageWidth * 0.1 + offsetInWidth, this.pageHeight * 0.9);
                element.GetGState().SetLeading(fontSize);		 // Set the spacing between lines
                writer.WriteElement(element);
                //添加定位点
                //于5.4.2011添加，用于屏蔽掉不必要的定位点添加
                if(this.isAppendLocationPoint.Checked)
                    appendLocationPoint(eb, writer, doc, page);
                
                this.statusLabel.Text = "现在状态：正在写入PDF文档";

                //写入string值
                //修改于4.4.2011，计划每页的行数与字体大小，页面相匹配，成反比，即y=a/x,而a由三号字体的测定为600
                int lineNumPerPage = (int)(900 / fontSize * this.pageHeight * 0.0008);
                int count = 0;
                foreach (string line in strLineArray)
                {
                    if ((++count) % lineNumPerPage == 0)
                    {
                        //结束一个旧的页面
                        // Finish the block of text
                        writer.WriteElement(eb.CreateTextEnd());

                        writer.End();  // save changes to the current page
                        doc.PagePushBack(page);

                        // 开始一个新的页面
                        page = doc.PageCreate(new Rect(0, 0, this.pageWidth, this.pageHeight));
                        //添加DPI的设置（于4/17早）
                        page.SetUserUnitSize(this.userInit);
                        writer.Begin(page);	// begin writing to this page
                        ///于4.27添加，因为下面的字体不随DPI而改变（始终默认为96），故将其按照用户的设定将字体进行缩放
                        element = eb.CreateTextBegin(fnt, fontSize);
                        element.SetTextMatrix(1, 0, 0, 1, this.pageWidth * 0.1 + offsetInWidth, this.pageHeight * 0.9);
                        element.GetGState().SetLeading(fontSize);
                        writer.WriteElement(element);
                        //添加定位点
                        //于5.4.2011添加，用于屏蔽掉不必要的定位点添加
                        if (this.isAppendLocationPoint.Checked)
                            appendLocationPoint(eb, writer, doc, page);
                    }
                    writer.WriteElement(eb.CreateUnicodeTextRun(line));
                    writer.WriteElement(eb.CreateTextNewLine());
                }

                //结束最后一个页面
                writer.WriteElement(eb.CreateTextEnd());

                writer.End();  // save changes to the current page
                doc.PagePushBack(page);              
                    


                // Calling Dispose() on ElementReader/Writer/Builder can result in increased performance and lower memory consumption.
                eb.Dispose();
                writer.Dispose();
                
                String saveFileName = this.objectFilePathTextBox.Text + "\\" + shortName.Split('.')[0] + ".pdf";
                doc.Save(saveFileName, SDFDoc.SaveOptions.e_remove_unused | SDFDoc.SaveOptions.e_hex_strings);
                doc.Close();

                //6月18日添加，生成图像格式的文档
                ///于6月30日添加，将图像格式的pdf文件进行简化，直接截图
                if (this.createPageImageCheckbox.Checked)
                {
                    createPageImage(saveFileName, shortName);
                }

                if (this.createXMLCheckBox.Checked)
                {
                    this.statusLabel.Text = "现在状态：正在生成标引文件";

                    ///于7.9日修改，同时添加标引文件
                    PDFExtractor.PDFExtractor extractor = new PDFExtractor.PDFExtractor();
                    extractor.init(saveFileName);
                    extractor.processPDF();
                    extractor.endProcess();
                }
                if (this.createPageImageCheckbox.Checked)
                {
                    createImageXmlFile(saveFileName);
                }
            }
        }

        private void createImageXmlFile(string saveFileName)
        {
            this.statusLabel.Text = "现在状态：正在生成截图的标引文件";

            //1.打开源文档
            XmlDocument doc = new XmlDocument();
            doc.Load(saveFileName.Split('.')[0]+".xml");

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
            doc.Save(saveFileName.Split('.')[0] + "-img.xml"); 
            
            this.statusLabel.Text = "现在状态：已完成";
        }

        private void createPageImage(string orginalFileName, string shortName)
        {
            this.statusLabel.Text = "现在状态：正在生成页面截图";

            string pageImageSavePath = this.pageImageSavePathTextbox.Text + @"\" + shortName.Split('.')[0] + "_每页图像";
            Directory.CreateDirectory(pageImageSavePath); 
            PDFDoc originalDoc = new PDFDoc(orginalFileName);
            PDFDraw draw = new PDFDraw();

            PDFDoc imageDoc = new PDFDoc();
            ElementBuilder eb = new ElementBuilder();
            ElementWriter ew = new ElementWriter();

            for (int index = 1; index <= originalDoc.GetPageCount(); ++index)
            {
                Page page = originalDoc.GetPage(index);
                draw.SetImageSize((int)page.GetPageWidth(), (int)page.GetPageHeight());
                //获取截图
                Bitmap srcImage = new Bitmap((int)page.GetPageWidth(), (int)page.GetPageHeight(), PixelFormat.Format8bppIndexed);
                srcImage = draw.GetBitmap(page);
                pdftron.PDF.Image image = pdftron.PDF.Image.Create(imageDoc, srcImage);
                Element element = eb.CreateImage(image, new Matrix2D(image.GetImageWidth(), 0, 0, image.GetImageHeight(), 0, 0));
                //新建一个页面
                Page newPage = imageDoc.PageCreate(new Rect(0, 0, page.GetPageWidth(), page.GetPageHeight()));
                ew.Begin(newPage);
                ew.WritePlacedElement(element);

                //如果添加标记点，则添加
                if (this.isAppendLocationPoint.Checked)
                    appendLocationPoint(eb, ew, imageDoc, newPage);
                ew.End();
                ///于7.9修改，将每页图像保存成图片
                srcImage = draw.GetBitmap(newPage);
                makeGreyImage(srcImage, pageImageSavePath + @"\" + index + ".bmp");
            }
            originalDoc.Close();
            imageDoc.Close();
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

        //private void createImageFormatDoc(String shortName, String[] strArray, int lineNumPerPage, System.Drawing.Font font, double offsetInWidth)
        //{
        //    //1.生成doc
        //    PDFDoc docWithImage = new PDFDoc();

        //    ElementBuilder builder = new ElementBuilder();	// Used to build new Element objects
        //    ElementWriter writer = new ElementWriter();

        //    Graphics g;

        //    //为了在图像中放开文字，减少每页的行数
        //    lineNumPerPage = (int)(lineNumPerPage / 1.2);

        //    Page page = docWithImage.PageCreate(new Rect(0, 0, this.pageWidth, this.pageHeight));
        //    //添加DPI的设置（于4/17早）
        //    page.SetUserUnitSize(this.userInit);

        //    writer.Begin(page);

        //    //2.循环每一行，如果到了一页的行数，则新生成一个页面
        //    String inputStr = String.Empty;
        //    int count = 0;
        //    Bitmap textBmp;
        //    System.Drawing.Font adjustedFont;
        //    Element element;
        //    pdftron.PDF.Image outputImg;
        //    while(count < strArray.Length)
        //    {
        //        if ((count + 1) % lineNumPerPage == 0)
        //        {
        //            //结束一个旧的页面
        //            textBmp = new Bitmap((int)this.pageWidth, (int)this.pageHeight);
        //            textBmp.SetResolution((float)(72), (float)(72));
        //            g = Graphics.FromImage(textBmp);
        //            adjustedFont = new System.Drawing.Font(font.FontFamily, (float)(font.Size / 96 / this.userInit));
        //            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                   
        //            g.DrawString(
        //                inputStr,
        //                adjustedFont,
        //                new SolidBrush(Color.Black),
        //                new PointF((float)(this.pageWidth * 0.1 + offsetInWidth), (float)(this.pageHeight * 0.1))
        //                );
        //            SolidBrush d = new SolidBrush(Color.DarkBlue);

        //            //输出图像
        //            outputImg = pdftron.PDF.Image.Create(docWithImage, textBmp);
        //            element = builder.CreateImage(
        //                outputImg,
        //                //new Matrix2D(textBmp.Width, 0, 0, textBmp.Height, 10, 800));
        //                new Matrix2D(textBmp.Width, 0, 0, textBmp.Height, 0, 0));
        //            writer.WritePlacedElement(element);                    
        //            docWithImage.PagePushBack(page);

        //            //清零各种局部变量
        //            inputStr = String.Empty;
        //            writer.End();
                    

        //            // 开始一个新的页面
        //            page = docWithImage.PageCreate(new Rect(0, 0, this.pageWidth, this.pageHeight));
        //            //添加DPI的设置（于4/17早）
        //            page.SetUserUnitSize(this.userInit);
        //            writer.Begin(page);
        //        }
        //        else
        //        {
        //            inputStr += strArray[count] + "\n";
        //        }
        //        ++count;
        //    }

        //    //生成最后一个页面
        //    textBmp = new Bitmap((int)this.pageWidth, (int)this.pageWidth);
        //    textBmp.SetResolution((float)(72), (float)(72));
        //    g = Graphics.FromImage(textBmp);
        //    adjustedFont = new System.Drawing.Font(font.FontFamily, (float)(font.Size / 96 / this.userInit));
        //    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        //    g.DrawString(
        //        inputStr,
        //        adjustedFont,
        //        new SolidBrush(Color.Black),
        //        new PointF(0, 0)
        //        );

        //    //输出图像
        //    outputImg = pdftron.PDF.Image.Create(docWithImage, textBmp);
        //    element = builder.CreateImage(
        //        outputImg,
        //        new Matrix2D(textBmp.Width, 0, 0, textBmp.Height, (float)(this.pageWidth * 0.1 + offsetInWidth), (float)(this.pageHeight * 0.9)));
        //    writer.WritePlacedElement(element);
        //    docWithImage.PagePushBack(page);

        //    docWithImage.Save(this.objectFilePathTextBox.Text + "\\" + shortName + "-img" + ".pdf", SDFDoc.SaveOptions.e_remove_unused | SDFDoc.SaveOptions.e_hex_strings);
        //    docWithImage.Close();
        //}

        private void appendLocationPoint(ElementBuilder eb, ElementWriter writer, PDFDoc doc, Page page)
        {
            this.statusLabel.Text = "现在状态：正在添加定位点";

            pdftron.PDF.Image image = pdftron.PDF.Image.Create(doc, Application.StartupPath +　"\\point2.bmp");
            double imageHeight = (double)image.GetImageHeight();
            //计算四个点的坐标
            //首先是定位点距离页边的距离
            double pageHeight = page.GetPageHeight();
            double pageWidth = page.GetPageWidth();
            double percentage = 0.05;
            double offset_x = pageWidth * percentage;
            double offset_y = pageHeight * percentage;


            ///重要标注：x1，y1其实是贴图左下角的坐标，而不是中心坐标
            ///
            int _x;
            int _y;
            //左上角
            double x1 = offset_x + imageHeight / 2;
            double y1 = pageHeight - (imageHeight / 2 + offset_y);
            ///于7.8添加，由于double的误差问题，将定位点的坐标位置强制转换成整数
            _x = (int)x1;
            _y = (int)y1;
            //Element element = eb.CreateImage(image, new Matrix2D(1, 0, 0, 1, x1, y1));
            Element element = eb.CreateImage(image, new Matrix2D(imageHeight, 0, 0, imageHeight, _x, _y));
            writer.WritePlacedElement(element);
            
            //右上角
            double x2 = pageWidth - (offset_x + imageHeight / 2);
            double y2 = pageHeight - (imageHeight / 2 + offset_y);
            _x = (int)x2;
            _y = (int)y2;
            element = eb.CreateImage(image, new Matrix2D(imageHeight, 0, 0, imageHeight, _x, _y));
            writer.WritePlacedElement(element);
            //右下角
            double x3 = pageWidth - (offset_x + imageHeight / 2);
            double y3 = imageHeight / 2 + offset_y;
            _x = (int)x3;
            _y = (int)y3;
            //element = eb.CreateImage(image, new Matrix2D(1, 0, 0, 1, x3, y3));
            element = eb.CreateImage(image, new Matrix2D(imageHeight, 0, 0, imageHeight, _x, _y));
            writer.WritePlacedElement(element);

            //左下角
            double x4 = offset_x + imageHeight / 2;
            double y4 = imageHeight / 2 + offset_y;
            _x = (int)x4;
            _y = (int)y4;
            element = eb.CreateImage(image, new Matrix2D(imageHeight, 0, 0, imageHeight, _x, _y));
            //element = eb.CreateImage(image, new Matrix2D(1, 0, 0, 1, x4, y4));
            writer.WritePlacedElement(element);
        }

        bool isSelectedFont = false;
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = this.fontDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.selectedFont = fontDialog1.Font;
                isSelectedFont = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.sourceFilePathText = this.objectFilePathText = @"D:\hello";
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BatchCreationForm_Load(object sender, EventArgs e)
        {

        }

        public double userInit { get; set; }

        private void isCreateImageFormatDoc_CheckedChanged(object sender, EventArgs e)
        {
            if (this.createPageImageCheckbox.Checked)
            {
                this.pageImageLabel.Visible = this.pageImageSavePathTextbox.Visible = this.pageImageFolderButton.Visible = true;
            }
            else
                this.pageImageLabel.Visible = this.pageImageSavePathTextbox.Visible = this.pageImageFolderButton.Visible = false;
        }

        private void createXMLCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void objectFilePathTextBox_Leave(object sender, EventArgs e)
        {
            this.pageImageSavePathTextbox.Text = this.objectFilePathTextBox.Text;
        }

        private void pageImageFolderButton_Click(object sender, EventArgs e)
        {

        }
    }
}
