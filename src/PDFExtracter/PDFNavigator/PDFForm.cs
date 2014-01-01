using System.Windows.Forms;
using pdftron;
using PDFExtractor;
using pdftron.PDF;
using System.Xml;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading;

namespace PDFNavigator
{
    public partial class PDFForm : Form
    {
        string fileName = string.Empty;
        string pdfDescName = string.Empty;

        string targetImageFileName;
        string targetImagePath;
        string targetImageFormat;
        string targetImageNamingOption;
        int sequencialImageNum;
        
        bool isTestingGroundData = false;
        
        int pageHeight = 0;
        int pageWidth = 0;

        int picHeight = 0;
        int picWidth = 0;

        Bitmap pageImage = null;

        float zoomLevel = 1;

        //生成画图类
        Graphics graphics;

        Thread extractorThread;

        public delegate void MyDelegate(XmlDocument xmlDoc, TreeNode rootNode);

        public PDFForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.Opaque, false);

            this.zoomInButton.Enabled = this.zoomOutButton.Enabled = false;

        }

        private void toolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PDF文件|*.pdf";
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.fileName = dialog.FileName;
                //设置截图的缺省目标位置
                setTargetImagePath();
                this.pdfDescName = this.fileName.Substring(0, this.fileName.IndexOf('.')) + ".xml";

                #region 将这两个函数封装在一个单独的线程里面在后台运行
                //设置tree的提示语
                this.setTreeMessage();
                this.extractorThread = new Thread(new ThreadStart(this.extractFunction));
                extractorThread.Start();
                #endregion

                //将PDF文件转化成图像                
                convertPDFToImage();

                //生成画图工具
                graphics = this.pictureBox.CreateGraphics();

            }
        }

        private void setTreeMessage()
        {
            this.pdfTreeView.Nodes.Add("正在生成索引…");
        }

        private void extractFunction()
        {
            //读入文件，调用PDFExtractor进行解析，从而生成XML文件
            FileInfo fi = new FileInfo(this.pdfDescName);
            if (!fi.Exists)
                extractFile();
            //将XML文件解析，并生成treeview
            createTreeView();

            this.extractorThread.Abort();
        }

        private void setTargetImagePath()
        {
            this.targetImagePath = new FileInfo(this.fileName).DirectoryName;
        }

        private void createTreeView()
        {
            //加载xml描述文件
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.pdfDescName);

            //生成根节点
            TreeNode rootNode = new TreeNode();
            
            //在多线程处理时，将下面一句话封装成一个函数，用delegate承载，以使主线程（即创建控件的线程）调用它，而不是子线程
            //递归遍历xml文档，生成完整的treeview
            MyDelegate createTreeViewDeleage = new MyDelegate(this.CreateTreeView);
            this.Invoke(createTreeViewDeleage, new object[] { xmlDoc, rootNode });
        }

        private void CreateTreeView(XmlDocument xmlDoc, TreeNode rootNode)
        {
            this.pdfTreeView.Nodes.Clear();
            this.pdfTreeView.Nodes.Add(visitNode(xmlDoc.DocumentElement, rootNode));
        }

        private TreeNode visitNode(XmlElement xmlElement, TreeNode treeNode)
        {
            //首先根据此节点修改treeNode的属性
            treeNode.Name = treeNode.Text = xmlElement.Name;

            //把此节点的所有属性抽象成没有子节点的treeNode
            foreach (XmlAttribute attribute in xmlElement.Attributes)
            {
                TreeNode attrNode = new TreeNode(attribute.Name + "-" + attribute.Value);
                treeNode.Nodes.Add(attrNode);
            }

            //对于该节点的每一个子节点，创建一个treeNode，递归创建
            foreach (XmlElement child in xmlElement.ChildNodes)
            {
                treeNode.Nodes.Add(visitNode(child, new TreeNode()));
            }

            return treeNode;
        }

        private void convertPDFToImage(int pageIndex = 1)
        {
            PDFDraw draw = new PDFDraw();
            PDFDoc pdfDoc = new PDFDoc(this.fileName);
            Page page = pdfDoc.GetPage(pageIndex);
            draw.SetImageSize((int)page.GetPageWidth(), (int)page.GetPageHeight());

            //获取当前的页面截图
            this.pageImage = draw.GetBitmap(page);

            //将页面截图贴到imagebox中
            this.pictureBox.Image = this.pageImage;

            //使缩放功能有效
            this.zoomOutButton.Enabled = this.zoomInButton.Enabled = true;

            //重新获取pictureBox的高度、宽度
            this.picHeight = this.pictureBox.Height;
            this.picWidth = this.pictureBox.Width;

            //获取page的高度、宽度
            this.pageHeight = (int)pdfDoc.GetPage(1).GetPageHeight();
            this.pageWidth = (int)pdfDoc.GetPage(1).GetPageWidth();

        }

        private void extractFile()
        {
            //读入文件，调用PDFExtractor进行解析，从而生成XML文件
            PDFExtractor.PDFExtractor pdfExtractor = new PDFExtractor.PDFExtractor();
            pdfExtractor.init(this.fileName);

            //开始分析PDF文档
            pdfExtractor.processPDF();

            //结束分析，关闭文档
            pdfExtractor.endProcess();
        }

        private void pdfTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //根据所点击的节点烈性进行相应的操作
            TreeNode selectedNode = e.Node;
            switch (selectedNode.Name)
            {
                //如果点击的是Page
                case "Page":
                    //首先把缩放改回初始值
                    this.setZoomDefault();
                    if (!this.isTestingGroundData)
                    {
                        this.currentPage = System.Convert.ToInt32(selectedNode.Nodes[3].Text.Split('-')[1]);
                        this.convertPDFToImage(this.currentPage);
                    }
                    else
                    {
                        this.currentPage = System.Convert.ToInt32(selectedNode.Nodes[3].Text.Split('-')[1]);
                        setDisplayImage(currentPage);
                    }
                    break;

                //如果点击的是Line
                case "Line":
                    //根据左下、右上画框
                    ///7.6日添加，根据四个点来画框
                    //左下
                    double[] left_down = new double[2];
                    TreeNode vertexNode = selectedNode.Nodes["LineCorners"].Nodes["left-down"];
                    left_down[0] = System.Convert.ToDouble(vertexNode.Nodes[0].Text.Split('-')[1]);
                    left_down[1] = System.Convert.ToDouble(vertexNode.Nodes[1].Text.Split('-')[1]);
                    //右下
                    double[] right_down = new double[2];
                    vertexNode = selectedNode.Nodes["LineCorners"].Nodes["right-down"];
                    right_down[0] = System.Convert.ToDouble(vertexNode.Nodes[0].Text.Split('-')[1]);
                    right_down[1] = System.Convert.ToDouble(vertexNode.Nodes[1].Text.Split('-')[1]);
                    //右上
                    double[] upper_right = new double[2];
                    vertexNode = selectedNode.Nodes["LineCorners"].Nodes["upper-right"];
                    upper_right[0] = System.Convert.ToDouble(vertexNode.Nodes[0].Text.Split('-')[1]);
                    upper_right[1] = System.Convert.ToDouble(vertexNode.Nodes[1].Text.Split('-')[1]);
                    //左上
                    double[] upper_left = new double[2];
                    vertexNode = selectedNode.Nodes["LineCorners"].Nodes["upper-left"];
                    upper_left[0] = System.Convert.ToDouble(vertexNode.Nodes[0].Text.Split('-')[1]);
                    upper_left[1] = System.Convert.ToDouble(vertexNode.Nodes[1].Text.Split('-')[1]);

                    //设置左下方坐标textbox的值
                    //resetCoordinateTextBox(left_down, upper_right);
                    //设置截图名字
                    //setTargetImageFileName(selectedNode.Nodes[2].Text.Split('-')[1], left_down, upper_right);
                    //画出矩形框
                    //如果是测试用，则使用_的函数，否则，正常使用
                    if(this.isTestingGroundData)
                        this._drawRect(left_down, right_down, upper_right, upper_left, "Line");
                    else
                        this.drawRect(left_down, upper_right, "Line");
                    break;

                //如果点击的是Word
                case "Word":
                    //根据左下、右上画框
                    ///7.6修改，用四个点画线
                    double[] w_left_down = new double[2];
                    TreeNode w_vertexNode = selectedNode.Nodes["WordCorners"].Nodes["left-down"];
                    w_left_down[0] = System.Convert.ToDouble(w_vertexNode.Nodes[0].Text.Split('-')[1]);
                    w_left_down[1] = System.Convert.ToDouble(w_vertexNode.Nodes[1].Text.Split('-')[1]);

                    ///7.6修改，用四个点画线
                    double[] w_right_down = new double[2];
                    w_vertexNode = selectedNode.Nodes["WordCorners"].Nodes["right-down"];
                    w_right_down[0] = System.Convert.ToDouble(w_vertexNode.Nodes[0].Text.Split('-')[1]);
                    w_right_down[1] = System.Convert.ToDouble(w_vertexNode.Nodes[1].Text.Split('-')[1]);

                    double[] w_upper_right = new double[2];
                    w_vertexNode = selectedNode.Nodes["WordCorners"].Nodes["upper-right"];
                    w_upper_right[0] = System.Convert.ToDouble(w_vertexNode.Nodes[0].Text.Split('-')[1]);
                    w_upper_right[1] = System.Convert.ToDouble(w_vertexNode.Nodes[1].Text.Split('-')[1]);

                    ///7.6修改，用四个点画线
                    double[] w_upper_left = new double[2];
                    w_vertexNode = selectedNode.Nodes["WordCorners"].Nodes["upper-left"];
                    w_upper_left[0] = System.Convert.ToDouble(w_vertexNode.Nodes[0].Text.Split('-')[1]);
                    w_upper_left[1] = System.Convert.ToDouble(w_vertexNode.Nodes[1].Text.Split('-')[1]);

                    //设置左下方坐标textbox的值
                    //resetCoordinateTextBox(w_left_down, w_upper_right);
                    //设置截图名字
                    //setTargetImageFileName(selectedNode.Nodes[0].Text.Split('-')[1], w_left_down, w_upper_right);
                    //画出矩形框

                    //如果是测试用，则使用_的函数，否则，正常使用
                    if (this.isTestingGroundData)
                        this._drawRect(w_left_down, w_right_down, w_upper_right, w_upper_left, "Word");
                    else
                        this.drawRect(w_left_down, w_upper_right, "Word");
                    break;

                //如果点击的是Char
                case "Char":
                    //根据左下、右上画框
                    double[] c_left_down = new double[2];
                    TreeNode c_vertexNode = selectedNode.Nodes["CharCorners"].Nodes["left-down"];
                    c_left_down[0] = System.Convert.ToDouble(c_vertexNode.Nodes[0].Text.Split('-')[1]);
                    c_left_down[1] = System.Convert.ToDouble(c_vertexNode.Nodes[1].Text.Split('-')[1]);
                    
                    double[] c_right_down = new double[2];
                    c_vertexNode = selectedNode.Nodes["CharCorners"].Nodes["right-down"];
                    c_right_down[0] = System.Convert.ToDouble(c_vertexNode.Nodes[0].Text.Split('-')[1]);
                    c_right_down[1] = System.Convert.ToDouble(c_vertexNode.Nodes[1].Text.Split('-')[1]);

                    double[] c_upper_right = new double[2];
                    c_vertexNode = selectedNode.Nodes["CharCorners"].Nodes["upper-right"];
                    c_upper_right[0] = System.Convert.ToDouble(c_vertexNode.Nodes[0].Text.Split('-')[1]);
                    c_upper_right[1] = System.Convert.ToDouble(c_vertexNode.Nodes[1].Text.Split('-')[1]);

                    double[] c_upper_left = new double[2];
                    c_vertexNode = selectedNode.Nodes["CharCorners"].Nodes["upper-left"];
                    c_upper_left[0] = System.Convert.ToDouble(c_vertexNode.Nodes[0].Text.Split('-')[1]);
                    c_upper_left[1] = System.Convert.ToDouble(c_vertexNode.Nodes[1].Text.Split('-')[1]);

                    //设置左下方坐标textbox的值
                    //resetCoordinateTextBox(c_left_down, c_upper_right);
                    //设置截图名字
                    //setTargetImageFileName(selectedNode.Nodes[0].Text.Split('-')[1], c_left_down, c_upper_right);
                    //画出矩形框
                    //如果是测试用，则使用_的函数，否则，正常使用
                    if (this.isTestingGroundData)
                        this._drawRect(c_left_down, c_right_down, c_upper_right, c_upper_left, "Char");
                    else
                        this.drawRect(c_left_down, c_upper_right, "Char");
                    break;

                //其他情况忽略
                default:
                    break;
            }
        }

        private void setDisplayImage(int index)
        {
            this.pictureBox.Image = new Bitmap(this.imagePageFileName[index]);
            this.pageImage = new Bitmap(this.pictureBox.Image);

            graphics = this.pictureBox.CreateGraphics();

            //使缩放功能有效
            this.zoomOutButton.Enabled = this.zoomInButton.Enabled = true;

            //重新获取pictureBox的高度、宽度
            this.picHeight = this.pictureBox.Height;
            this.picWidth = this.pictureBox.Width;

            //获取page的高度、宽度
            this.pageHeight = this.picHeight;
            this.pageWidth = this.picWidth;
        }

        private void setZoomDefault()
        {
            this.zoomLabel.Text = "100%";
            this.zoomLevel = 1f;
        }

        private void setTargetImageFileName(string value, double[] left_down, double[] upper_right)
        {
            this.targetImageFileName = new FileInfo(this.fileName).Name
                + "__" + value
                + "__" + "(" + (int)left_down[0] + "," + (int)left_down[1] + ")"
                + "(" + (int)upper_right[0] + "," + (int)upper_right[1] + ")";
        }

        //private void resetCoordinateTextBox(double[] upper_left, double[] right_down)
        //{
        //    this.left_downTextbox.Text = (int)upper_left[0] + ", " + (int)(fitY(upper_left[1]));
        //    this.upper_rightTextbox.Text = (int)right_down[0] + ", " + (int)(fitY(right_down[1]));
        //}

        private void drawRect(double[] left_down, double[] upper_right, String type)
        {
            #region 折叠
            //首先清除所画的图形!!!!!!!!!!!!!!
            if ((!this.displayAllLinesCheckbox.Checked) && (!this.displayAllWordsCheckbox.Checked))
                this.pictureBox.Refresh();
            //根据type确定画线的颜色
            Color color;
            switch (type)
            {
                case "Line" :
                    color = Color.Red;
                    break;
                case "Word" :
                    color = Color.Green;
                    break;
                case "Char" :
                    color = Color.Yellow;
                    break;
                default :
                    color = Color.Black;
                    break;
            }
            #endregion 
            Pen pen = new Pen(color);
            graphics.DrawRectangle
                (   
                    pen,
                    (float)left_down[0] * this.zoomLevel,//起始点x
                    (fitY(left_down[1]) - (float)((upper_right[1] - left_down[1]))) * this.zoomLevel + 1,//起始点y
                    ((float)((upper_right[0] - left_down[0]))) * this.zoomLevel,//宽度
                    ((float)((upper_right[1] - left_down[1]))) * this.zoomLevel//高度
                );
        }

        private void _drawRect(double[] left_down, double[] right_down, double[] upper_right, double[] upper_left, String type)
        {
            #region 折叠
            PDFDoc d = new PDFDoc();
            //首先清除所画的图形!!!!!!!!!!!!!!
            if((!this.displayAllLinesCheckbox.Checked) && (!this.displayAllWordsCheckbox.Checked))
                this.pictureBox.Refresh();
            //根据type确定画线的颜色
            Color color;
            switch (type)
            {
                case "Line":
                    color = Color.Red;
                    break;
                case "Word":
                    color = Color.Green;
                    break;
                case "Char":
                    color = Color.Yellow;
                    break;
                default:
                    color = Color.Black;
                    break;
            }
            #endregion
            Pen pen = new Pen(color);

            //生成point
            System.Drawing.Point[] points = new System.Drawing.Point[4];
            points[0] = new System.Drawing.Point((int)(left_down[0] * this.zoomLevel), (int)(left_down[1] * this.zoomLevel));
            points[1] = new System.Drawing.Point((int)(right_down[0] * this.zoomLevel), (int)(right_down[1] * this.zoomLevel));
            points[2] = new System.Drawing.Point((int)(upper_right[0] * this.zoomLevel), (int)(upper_right[1] * this.zoomLevel));
            points[3] = new System.Drawing.Point((int)(upper_left[0] * this.zoomLevel), (int)(upper_left[1] * this.zoomLevel));

            graphics.DrawLines(pen, points);
            graphics.DrawLine(pen, points[0], points[3]);            
        }

        private float fitY(double y)
        {
            return (float)(this.pageHeight - y) - 1;//1个象素的偏移量
        }

        //private void getRectImageButton_Click(object sender, EventArgs e)
        //{
        //    String left_down_coor = this.left_downTextbox.Text;
        //    String upper_right_coor = this.upper_rightTextbox.Text;
        //    int startX = System.Convert.ToInt32(left_down_coor.Split(',')[0]);
        //    int endX = System.Convert.ToInt32(upper_right_coor.Split(',')[0]);
        //    int width = endX - startX;
        //    int startY = System.Convert.ToInt32(upper_right_coor.Split(',')[1]);
        //    int endY = System.Convert.ToInt32(left_down_coor.Split(',')[1]);
        //    int height = endY - startY;
        //    GetImageRect(startX, startY, width, height);

            
        //}

        private void GetImageRect(int startX, int startY, int width, int height)
        {
            String fileName;
            switch (this.targetImageNamingOption)
            { 
                case "按\"源文件_字符值_坐标值\"" :
                    fileName = this.targetImagePath + "\\" + this.targetImageFileName + ".bmp";
                    break;
                case "按\"递增序号\"" :
                    fileName = this.targetImagePath + "\\"  + (++sequencialImageNum).ToString() + ".bmp";
                    break;
                case "按\"随机命名\"" :
                    fileName = this.targetImagePath + "\\" + Guid.NewGuid().ToString() + ".bmp";
                    break;
                default :
                    fileName = this.targetImagePath + "\\" + Guid.NewGuid().ToString() + ".bmp";
                    break;
            }
                        
            PixelFormat format;
            switch (this.targetImageFormat)
            {
                case "16位-RGB555":
                    format = PixelFormat.Format16bppRgb555;
                    break;
                case "16位-RGB565":
                    format = PixelFormat.Format16bppRgb565;
                    break;
                case "24位":
                    format = PixelFormat.Format24bppRgb;
                    break;
                case "32位":
                    format = PixelFormat.Format32bppRgb;
                    break;
                default:
                    format = PixelFormat.Format16bppRgb555;
                    break;
            }
            Bitmap image = this.pageImage.Clone(new Rectangle(startX, startY, width, height + 1), format);

            image.Save(fileName, ImageFormat.Bmp);

            MessageBox.Show("已将截图保存至：" + fileName + "\r格式为" + format.ToString());
        }

        private int changeY(int endY)
        {
            return this.pageImage.Height - endY - 2;
        }

        private void 批量生成XMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchXMLForm form = new BatchXMLForm();

            form.Show();
        }

        //private void settingButton_Click(object sender, EventArgs e)
        //{
        //    this.settingPanel.Visible = !this.settingPanel.Visible;
        //}

        //private void imageSettingfoldButton_Click(object sender, EventArgs e)
        //{
        //    this.settingPanel.Visible = false;
        //}

        //private void browseImageTargetPathButton_Click(object sender, EventArgs e)
        //{
        //    if (this.imageTargetfolderBrowserDialog.ShowDialog().Equals(DialogResult.OK))
        //    {
        //       this.targetImagePathTextbox.Text = imageTargetfolderBrowserDialog.SelectedPath;
        //    }

        //    PDFDoc doc = new PDFDoc();
        //    pdftron.PDF.Convert.ToPdf(doc, "d");
            
        //}

        //private void imageSettingExecuteButton_Click(object sender, EventArgs e)
        //{
        //    this.targetImagePath = 
        //        this.targetImagePathTextbox.Text.Equals("") 
        //        ? 
        //        new FileInfo(this.fileName).DirectoryName 
        //        : 
        //        this.targetImagePathTextbox.Text;

        //    this.targetImageFormat = this.imageFormatOption.SelectedText;
        //    this.targetImageNamingOption = this.namingOption.SelectedText;
        //    this.settingPanel.Visible = false;
        //}

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            this.mouseCoordinateTextbox.Text = e.X * this.zoomLevel + ", " + e.Y * this.zoomLevel;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new BatchCreationForm().Show();
        }

        private void 批量生成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new BatchCreationForm().Show();
        }

        private void 关键词搜索ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new KeywordSearchForm().Show();
        }

        private void zoomInButton_Click(object sender, EventArgs e)
        {
            if (this.pictureBox.Image != null)
            {
                this.zoomLevel += (float)0.2;

                this.setZoomImage();
            }
        }

        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            if (this.pictureBox.Image != null)
            {
                this.zoomLevel -= (float)0.2;

                this.setZoomImage();
            }
        }

        private void setZoomImage()
        {

            this.zoomLabel.Text = (100 * zoomLevel).ToString() + "%";

            int zoomedWidth = (int)(this.pageImage.Width * this.zoomLevel);
            int zoomedHeight = (int)(this.pageImage.Height * this.zoomLevel);

            this.pictureBox.Image = this.pageImage.GetThumbnailImage(zoomedWidth, zoomedHeight, null, IntPtr.Zero);
        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new GroundDataTestForm().Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.isTestingGroundData = true;
            this.pictureBox.Image = new Bitmap(@"D:\hello\新建文件夹\含噪声文档图像\ideals4_1.bmp");

            graphics = this.pictureBox.CreateGraphics();
            //加载xml描述文件
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"D:\hello\新建文件夹\标引信息存储路径\ideals4.xml");

            //生成根节点
            TreeNode rootNode = new TreeNode();
            CreateTreeView(xmlDoc, rootNode);
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isTestingGroundData)
                {
                    MessageBox.Show("请分别选择图像及其XML标引文件");
                    this.isTestingGroundData = true;

                    //选择图像文件
                    if (this.settingTestingGroundDataFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        this.pictureBox.Image = new Bitmap(this.settingTestingGroundDataFileDialog.FileName);
                        graphics = this.pictureBox.CreateGraphics();

                        //选择标引文件
                        if (this.settingTestingGroundDataFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(this.settingTestingGroundDataFileDialog.FileName);

                            //生成根节点
                            TreeNode rootNode = new TreeNode();
                            CreateTreeView(xmlDoc, rootNode);
                            
                            //使缩放功能有效
                            this.zoomOutButton.Enabled = this.zoomInButton.Enabled = true;
                        }
                    }
                }
                else
                    this.isTestingGroundData = false;
            }
            catch (Exception e1)
            {
                MessageBox.Show("程序运行环境不正确，无法测试。");
            }
        }

        private void 测试基准数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("请分别选择图像及其XML标引文件");
                this.isTestingGroundData = true;

                //选择图像文件
                if (this.settingTestingGroundDataFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.pictureBox.Image = new Bitmap(this.settingTestingGroundDataFileDialog.FileName);
                    graphics = this.pictureBox.CreateGraphics();

                    //选择标引文件
                    if (this.settingTestingGroundDataFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(this.settingTestingGroundDataFileDialog.FileName);

                        //生成根节点
                        TreeNode rootNode = new TreeNode();
                        CreateTreeView(xmlDoc, rootNode);

                        //使缩放功能有效
                        this.zoomOutButton.Enabled = this.zoomInButton.Enabled = true;
                    }
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show("程序运行环境不正确，无法测试。");
            }
        }

        private void 退化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DegrationForm().Show();
        }

        private void 打开页面图像集合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("请选择页面图像集合所在的路径");
            if (this.imageTargetfolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = this.imageTargetfolderBrowserDialog.SelectedPath;
                DirectoryInfo di = new DirectoryInfo(path);
                this.imagePageFileName = new string[di.GetFiles().Length + 1];
                
                foreach (FileInfo fi in di.GetFiles())
                {
                    int index = Int32.Parse(fi.Name.Split('.')[0]);
                    this.imagePageFileName[index] = fi.FullName;
                }

                MessageBox.Show("请选择标引XML文件");
                if (this.settingTestingGroundDataFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //加载xml描述文件
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(this.settingTestingGroundDataFileDialog.FileName);

                    this.pdfDescName = this.settingTestingGroundDataFileDialog.FileName;
                    //生成根节点
                    TreeNode rootNode = new TreeNode();

                    //在多线程处理时，将下面一句话封装成一个函数，用delegate承载，以使主线程（即创建控件的线程）调用它，而不是子线程
                    //递归遍历xml文档，生成完整的treeview
                    MyDelegate createTreeViewDeleage = new MyDelegate(this.CreateTreeView);
                    this.Invoke(createTreeViewDeleage, new object[] { xmlDoc, rootNode });

                    //使缩放功能有效
                    this.zoomOutButton.Enabled = this.zoomInButton.Enabled = true;

                    this.isTestingGroundData = true;
                }
            }
        }

        public string[] imagePageFileName { get; set; }

        public bool isDisplayImage { get; set; }

        private void displayAllLinesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        public int currentPage { get; set; }

        private void displayAllWordsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void displayAllLinesCheckbox_CheckedChanged_1(object sender, EventArgs e)
        {
            //循环遍历每一个坐标
            if (this.displayAllLinesCheckbox.Checked)
            {
                if (this.pdfDescName != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(this.pdfDescName);
                    XmlNode pageNode = doc.GetElementsByTagName("Page")[this.currentPage - 1];

                    foreach (XmlNode line in pageNode.ChildNodes)
                    {
                        XmlNode lineCorner = line.FirstChild;

                        XmlNode left_downNode = lineCorner.ChildNodes[0];
                        XmlNode right_downNode = lineCorner.ChildNodes[1];
                        XmlNode upper_rightNode = lineCorner.ChildNodes[2];
                        XmlNode upper_leftNode = lineCorner.ChildNodes[3];

                        double[] left_down = new double[2] { Double.Parse(left_downNode.Attributes["x"].Value), Double.Parse(left_downNode.Attributes["y"].Value) };
                        double[] right_down = new double[2] { Double.Parse(right_downNode.Attributes["x"].Value), Double.Parse(right_downNode.Attributes["y"].Value) };
                        double[] upper_right = new double[2] { Double.Parse(upper_rightNode.Attributes["x"].Value), Double.Parse(upper_rightNode.Attributes["y"].Value) };
                        double[] upper_left = new double[2] { Double.Parse(upper_leftNode.Attributes["x"].Value), Double.Parse(upper_leftNode.Attributes["y"].Value) };

                        if (this.isTestingGroundData)
                            this._drawRect(left_down, right_down, upper_right, upper_left, "Line");
                        else
                            this.drawRect(left_down, upper_right, "Line");
                    }
                }
            }
            else
                this.pictureBox.Refresh();
        }

        private void displayAllWordsCheckbox_CheckedChanged_1(object sender, EventArgs e)
        {
            //循环遍历每一个坐标
            if (this.displayAllWordsCheckbox.Checked)
            {
                if (this.pdfDescName != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(this.pdfDescName);
                    XmlNode pageNode = doc.GetElementsByTagName("Page")[this.currentPage - 1];

                    foreach (XmlNode line in pageNode.ChildNodes)
                    {
                        for (int index = 1; index < line.ChildNodes.Count; ++index)
                        {
                            XmlNode wordCorner = line.ChildNodes[index].ChildNodes[1];

                            XmlNode left_downNode = wordCorner.ChildNodes[0];
                            XmlNode right_downNode = wordCorner.ChildNodes[1];
                            XmlNode upper_rightNode = wordCorner.ChildNodes[2];
                            XmlNode upper_leftNode = wordCorner.ChildNodes[3];

                            double[] left_down = new double[2] { Double.Parse(left_downNode.Attributes["x"].Value), Double.Parse(left_downNode.Attributes["y"].Value) };
                            double[] right_down = new double[2] { Double.Parse(right_downNode.Attributes["x"].Value), Double.Parse(right_downNode.Attributes["y"].Value) };
                            double[] upper_right = new double[2] { Double.Parse(upper_rightNode.Attributes["x"].Value), Double.Parse(upper_rightNode.Attributes["y"].Value) };
                            double[] upper_left = new double[2] { Double.Parse(upper_leftNode.Attributes["x"].Value), Double.Parse(upper_leftNode.Attributes["y"].Value) };

                            if (this.isTestingGroundData)
                                this._drawRect(left_down, right_down, upper_right, upper_left, "Word");
                            else
                                this.drawRect(left_down, upper_right, "Word");
                        }
                    }
                }
            }
            else
                this.pictureBox.Refresh();
        }

        private void PDFForm_Load(object sender, EventArgs e)
        {

        }
    }
}