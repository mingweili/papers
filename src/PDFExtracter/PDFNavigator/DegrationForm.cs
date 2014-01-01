using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace PDFNavigator
{
    public partial class DegrationForm : Form
    {
        public DegrationForm()
        {
            InitializeComponent();
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if (this.srcImageTextbox.Text != String.Empty
                && this.desSavePath.Text != String.Empty)
            {
                double count = 0;
                DirectoryInfo di = new DirectoryInfo(this.srcImageTextbox.Text);
                int sum = di.GetFiles().Length;
                foreach (FileInfo fi in di.GetFiles())
                {
                    //如果是PDF文件，则执行（假设PDF理想文档和它的标引文件在同一目录下，且名字相同）
                    if (fi.Extension.Equals(".bmp") || fi.Extension.Equals(".BMP"))
                    {
                        ++count;
                        doIt(fi);
                        setProgressBar((int)(count / sum * 100));
                    }
                }
                if (this.rotationCheckbox.Checked)
                {
                    modifyXmlDoc(this.srcXMLDocTextbox.Text, this.rd);
                }

                setProgressBar(100);
                MessageBox.Show("已完成");
            }
            else
                MessageBox.Show("请选择/填写文件夹路径");
        }

        private void setProgressBar(int p)
        {
            this.progressBar.Value = p;
        }

        private void doIt(FileInfo fi)
        {
            Degrator degrator = new Degrator();
            //检测用户的选择情况
            bool isDegration = false;
            if (this.showThroughCheckbox.Checked)
            {
                try
                {
                    String backImageName = this.showThroughTextbox.Text;
                    ShowThroughPara para = new ShowThroughPara();
                    para.backFileName = backImageName;
                    para.adjustment = Double.Parse(this.showThroughAdjust.Text);
                    para.colorDifference = (int)this.showThroughColorDifferenceNumericUpDown.Value;

                    degrator.isShowThrough = true;
                    degrator.showThroughPara = para;

                    isDegration = true;
                }
                catch (Exception ee)
                {
                    MessageBox.Show("show-through退化出错");
                }
            }
            if (this.specklesCheckbox.Checked)
            {
                try
                {
                    SpecklePara para = new SpecklePara();
                    para.count = (int)this.speckleNumericUpDown.Value;
                    para.size = (int)this.speckleSizeNumericUpDown.Value;
                    para.density = Double.Parse(this.speckleDensityTextbox.Text);

                    degrator.isSpeckles = true;
                    degrator.specklePara = para;

                    isDegration = true;
                }
                catch (Exception ee)
                {
                    MessageBox.Show("斑点退化出错");
                }
            }
            if (this.blurCheckbox.Checked)
            {
                degrator.isBlur = true;

                isDegration = true;
            }
            if (this.addLineCheckbox.Checked)
            {
                try
                {
                    AddLinePara para = new AddLinePara();
                    para.count = (int)this.lineNumericUpDown.Value;
                    int[] width = new int[2];
                    width[0] = Int32.Parse(this.lineWidthMinTextbox.Text);
                    width[1] = Int32.Parse(this.lineWidthMaxTextbox.Text);
                    para.widthThreshold = width;
                    int[] length = new int[2];
                    length[0] = Int32.Parse(this.lineLengthMinTextbox.Text);
                    length[1] = Int32.Parse(this.lineLengthMaxTextbox.Text);
                    para.lengthThreshold = length;
                    para.density = Double.Parse(this.lineDensityTextbox.Text);
                    para.colorPara = Double.Parse(this.lineColorTextbox.Text);

                    degrator.isAddLine = true;
                    degrator.addLinePara = para;

                    isDegration = true;
                }
                catch (Exception ee)
                {
                    MessageBox.Show("线条退化出错");
                }
            }
            if (this.jitterCheckbox.Checked)
            {
                try
                {
                    JitterPara para = new JitterPara();
                    para.jitterRadium = (int)this.jitterRadiumNumericUpDown.Value;

                    degrator.isJitter = true;
                    degrator.jitterPara = para;

                    isDegration = true;
                }
                catch (Exception ee)
                {
                    MessageBox.Show("抖动退化出错");
                }
            }
            if (this.rotationCheckbox.Checked)
            {
                try
                {
                    RotationPara para = new RotationPara();
                    para.angle = Double.Parse(this.rotationAngleTextbox.Text);

                    degrator.isRotation = true;
                    degrator.rotationPara = para;

                    isDegration = true;
                }
                catch (Exception ee)
                {
                    MessageBox.Show("旋转出错");
                }
            }
            if (this.modelCheckbox.Checked)
            {
                try
                {
                    ModelPara para = new ModelPara();
                    para.α = Double.Parse(this.alphaTextbox.Text);
                    para.β = Double.Parse(this.beitaTextbox.Text);
                    para.α0 = Double.Parse(this.alpha0Textbox.Text);
                    para.β0 = Double.Parse(this.beita0Textbox.Text);
                    para.model = Int32.Parse(this.modelSizeTextbox.Text);

                    degrator.modelPara = para;

                    degrator.testDegrationModel(fi.FullName, this.desSavePath.Text);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("经典模型出错");
                }
            }

            if(isDegration)
            {
                this.rd = degrator.degration(fi.FullName, this.desSavePath.Text);
                
            }
        }

        private void modifyXmlDoc(String xmlDocFileName, RotationData rd)
        {
            //1.打开源文档
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlDocFileName);

            //2.修改页面的宽、高
            foreach (XmlElement pageNode in doc.GetElementsByTagName("Page"))
            {
                pageNode.SetAttribute("Width", Math.Truncate((double)rd.new_image_width).ToString());
                pageNode.SetAttribute("Height", Math.Truncate((double)rd.new_image_height).ToString());
            }

            //3.修改坐标信息
            foreach (XmlElement left_down in doc.GetElementsByTagName("left-down"))
            {
                double ori_x = Double.Parse(left_down.GetAttribute("x"));
                double ori_y = Double.Parse(left_down.GetAttribute("y"));

                //计算新值
                double new_x = (ori_x - rd.center_x) * rd.cos_angle - (ori_y - rd.center_y) * rd.sin_angle + rd.new_center_x;
                double new_y = (ori_x - rd.center_x) * rd.sin_angle + (ori_y - rd.center_y) * rd.cos_angle + rd.new_center_y;

                left_down.SetAttribute("x", new_x.ToString());
                left_down.SetAttribute("y", new_y.ToString());
            }

            foreach (XmlElement right_down in doc.GetElementsByTagName("right-down"))
            {
                double ori_x = Double.Parse(right_down.GetAttribute("x"));
                double ori_y = Double.Parse(right_down.GetAttribute("y"));

                //计算新值
                double new_x = (ori_x - rd.center_x) * rd.cos_angle - (ori_y - rd.center_y) * rd.sin_angle + rd.new_center_x;
                double new_y = (ori_x - rd.center_x) * rd.sin_angle + (ori_y - rd.center_y) * rd.cos_angle + rd.new_center_y;

                right_down.SetAttribute("x", new_x.ToString());
                right_down.SetAttribute("y", new_y.ToString());
            }

            foreach (XmlElement upper_right in doc.GetElementsByTagName("upper-right"))
            {
                double ori_x = Double.Parse(upper_right.GetAttribute("x"));
                double ori_y = Double.Parse(upper_right.GetAttribute("y"));

                //计算新值
                double new_x = (ori_x - rd.center_x) * rd.cos_angle - (ori_y - rd.center_y) * rd.sin_angle + rd.new_center_x;
                double new_y = (ori_x - rd.center_x) * rd.sin_angle + (ori_y - rd.center_y) * rd.cos_angle + rd.new_center_y;

                upper_right.SetAttribute("x", new_x.ToString());
                upper_right.SetAttribute("y", new_y.ToString());
            }

            foreach (XmlElement upper_left in doc.GetElementsByTagName("upper-left"))
            {
                double ori_x = Double.Parse(upper_left.GetAttribute("x"));
                double ori_y = Double.Parse(upper_left.GetAttribute("y"));

                //计算新值
                double new_x = (ori_x - rd.center_x) * rd.cos_angle - (ori_y - rd.center_y) * rd.sin_angle + rd.new_center_x;
                double new_y = (ori_x - rd.center_x) * rd.sin_angle + (ori_y - rd.center_y) * rd.cos_angle + rd.new_center_y;

                upper_left.SetAttribute("x", new_x.ToString());
                upper_left.SetAttribute("y", new_y.ToString());
            }
            
            //保存
            doc.Save(xmlDocFileName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            executeButton_Click(sender, e);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void modelCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.modelCheckbox.Checked)
            {
                this.showThroughCheckbox.Checked = false;
                this.showThroughGroup.Enabled = false;
                this.specklesCheckbox.Checked = false;
                this.speckleGroup.Enabled = false;
                this.blurCheckbox.Checked = false;
                this.blurGroup.Enabled = false;
                this.addLineCheckbox.Checked = false;
                this.addLineGroup.Enabled = false;
                this.jitterCheckbox.Checked = false;
                this.jitterGroup.Enabled = false;
                this.rotationCheckbox.Checked = false;
                this.rotationGroup.Enabled = false;

                this.modelGroupBox.Enabled = true;
            }
            else
                this.modelGroupBox.Enabled = false;
        }

        private void showThroughCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.showThroughCheckbox.Checked)
            {
                this.showThroughGroup.Enabled = true;

                this.modelCheckbox.Checked = false;
                this.modelGroupBox.Enabled = false;
            }
            else
            {
                this.showThroughGroup.Enabled = false;
                this.showThroughTextbox.Text = String.Empty;
            }
        }

        private void specklesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.specklesCheckbox.Checked)
            {
                this.speckleGroup.Enabled = true;

                this.modelCheckbox.Checked = false;
                this.modelGroupBox.Enabled = false;
            }
            else
            {
                this.speckleGroup.Enabled = false;
            }
        }

        private void addLineCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.addLineCheckbox.Checked)
            {
                this.addLineGroup.Enabled = true;

                this.modelCheckbox.Checked = false;
                this.modelGroupBox.Enabled = false;
            }
            else
                this.addLineGroup.Enabled = false;
        }

        private void blurCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.blurCheckbox.Checked)
            {
                this.blurGroup.Enabled = true;
                this.jitterGroup.Enabled = false;
                this.jitterCheckbox.Checked = false;

                this.modelCheckbox.Checked = false;
                this.modelGroupBox.Enabled = false;
            }
            else
            {
                this.blurGroup.Enabled = false;
                this.jitterGroup.Enabled = true;
            }
        }

        private void jitterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.jitterCheckbox.Checked)
            {
                this.jitterGroup.Enabled = true;
                this.blurGroup.Enabled = false;
                this.blurCheckbox.Checked = false;

                this.modelCheckbox.Checked = false;
                this.modelGroupBox.Enabled = false;
            }
            else
            {
                this.jitterGroup.Enabled = false;
                this.blurGroup.Enabled = true;
            }
        }

        private void rotationCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rotationCheckbox.Checked)
            {
                this.rotationGroup.Enabled = true;

                this.modelCheckbox.Checked = false;
                this.modelGroupBox.Enabled = false;
            }
            else
                this.rotationGroup.Enabled = false;
        }

        private void showThroughButton_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.showThroughTextbox.Text = this.openFileDialog1.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.srcImageTextbox.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.desSavePath.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.srcXMLDocTextbox.Text = this.openFileDialog1.FileName;
            }
        }

        public RotationData rd = new RotationData();
    }
}
