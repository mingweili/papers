namespace PDFNavigator
{
    partial class PDFForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.pdfTreeView = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.kaiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.打开页面图像集合ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.批量生成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.高级功能ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.批量生成XMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关键词搜索ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.基准信息录入ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退化效果ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退化ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.imageTargetfolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.zoomLabel = new System.Windows.Forms.Label();
            this.zoomOutButton = new System.Windows.Forms.Button();
            this.zoomInButton = new System.Windows.Forms.Button();
            this.displayAllWordsCheckbox = new System.Windows.Forms.CheckBox();
            this.displayAllLinesCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mouseCoordinateTextbox = new System.Windows.Forms.TextBox();
            this.settingTestingGroundDataFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTabList;
            this.pictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox.Location = new System.Drawing.Point(3, 29);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(600, 600);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            // 
            // pdfTreeView
            // 
            this.pdfTreeView.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.pdfTreeView.Location = new System.Drawing.Point(12, 25);
            this.pdfTreeView.Name = "pdfTreeView";
            this.pdfTreeView.Size = new System.Drawing.Size(353, 656);
            this.pdfTreeView.TabIndex = 1;
            this.pdfTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.pdfTreeView_NodeMouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kaiToolStripMenuItem,
            this.高级功能ToolStripMenuItem,
            this.基准信息录入ToolStripMenuItem,
            this.退化效果ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1108, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // kaiToolStripMenuItem
            // 
            this.kaiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.打开页面图像集合ToolStripMenuItem,
            this.批量生成ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.kaiToolStripMenuItem.Name = "kaiToolStripMenuItem";
            this.kaiToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.kaiToolStripMenuItem.Text = "文件";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(182, 22);
            this.toolStripMenuItem1.Text = "打开PDF...";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // 打开页面图像集合ToolStripMenuItem
            // 
            this.打开页面图像集合ToolStripMenuItem.Name = "打开页面图像集合ToolStripMenuItem";
            this.打开页面图像集合ToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.打开页面图像集合ToolStripMenuItem.Text = "打开页面图像集合…";
            this.打开页面图像集合ToolStripMenuItem.Click += new System.EventHandler(this.打开页面图像集合ToolStripMenuItem_Click);
            // 
            // 批量生成ToolStripMenuItem
            // 
            this.批量生成ToolStripMenuItem.Name = "批量生成ToolStripMenuItem";
            this.批量生成ToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.批量生成ToolStripMenuItem.Text = "批量生成PDF...";
            this.批量生成ToolStripMenuItem.Click += new System.EventHandler(this.批量生成ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // 高级功能ToolStripMenuItem
            // 
            this.高级功能ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.批量生成XMLToolStripMenuItem,
            this.关键词搜索ToolStripMenuItem});
            this.高级功能ToolStripMenuItem.Name = "高级功能ToolStripMenuItem";
            this.高级功能ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.高级功能ToolStripMenuItem.Text = "高级功能";
            // 
            // 批量生成XMLToolStripMenuItem
            // 
            this.批量生成XMLToolStripMenuItem.Name = "批量生成XMLToolStripMenuItem";
            this.批量生成XMLToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.批量生成XMLToolStripMenuItem.Text = "批量生成XML索引文件...";
            this.批量生成XMLToolStripMenuItem.Click += new System.EventHandler(this.批量生成XMLToolStripMenuItem_Click);
            // 
            // 关键词搜索ToolStripMenuItem
            // 
            this.关键词搜索ToolStripMenuItem.Name = "关键词搜索ToolStripMenuItem";
            this.关键词搜索ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.关键词搜索ToolStripMenuItem.Text = "关键词搜索/统计...";
            this.关键词搜索ToolStripMenuItem.Click += new System.EventHandler(this.关键词搜索ToolStripMenuItem_Click);
            // 
            // 基准信息录入ToolStripMenuItem
            // 
            this.基准信息录入ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.测试ToolStripMenuItem});
            this.基准信息录入ToolStripMenuItem.Name = "基准信息录入ToolStripMenuItem";
            this.基准信息录入ToolStripMenuItem.Size = new System.Drawing.Size(92, 21);
            this.基准信息录入ToolStripMenuItem.Text = "基准信息录入";
            // 
            // 测试ToolStripMenuItem
            // 
            this.测试ToolStripMenuItem.Name = "测试ToolStripMenuItem";
            this.测试ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.测试ToolStripMenuItem.Text = "生成...";
            this.测试ToolStripMenuItem.Click += new System.EventHandler(this.测试ToolStripMenuItem_Click);
            // 
            // 退化效果ToolStripMenuItem
            // 
            this.退化效果ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.退化ToolStripMenuItem});
            this.退化效果ToolStripMenuItem.Name = "退化效果ToolStripMenuItem";
            this.退化效果ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.退化效果ToolStripMenuItem.Text = "退化效果";
            // 
            // 退化ToolStripMenuItem
            // 
            this.退化ToolStripMenuItem.Name = "退化ToolStripMenuItem";
            this.退化ToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.退化ToolStripMenuItem.Text = "退化…";
            this.退化ToolStripMenuItem.Click += new System.EventHandler(this.退化ToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(12, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.zoomLabel);
            this.splitContainer1.Panel1.Controls.Add(this.zoomOutButton);
            this.splitContainer1.Panel1.Controls.Add(this.zoomInButton);
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.displayAllWordsCheckbox);
            this.splitContainer1.Panel2.Controls.Add(this.displayAllLinesCheckbox);
            this.splitContainer1.Panel2.Controls.Add(this.pdfTreeView);
            this.splitContainer1.Size = new System.Drawing.Size(1084, 689);
            this.splitContainer1.SplitterDistance = 712;
            this.splitContainer1.TabIndex = 13;
            // 
            // zoomLabel
            // 
            this.zoomLabel.AutoSize = true;
            this.zoomLabel.Location = new System.Drawing.Point(117, 10);
            this.zoomLabel.Name = "zoomLabel";
            this.zoomLabel.Size = new System.Drawing.Size(29, 12);
            this.zoomLabel.TabIndex = 3;
            this.zoomLabel.Text = "100%";
            // 
            // zoomOutButton
            // 
            this.zoomOutButton.Location = new System.Drawing.Point(59, 3);
            this.zoomOutButton.Name = "zoomOutButton";
            this.zoomOutButton.Size = new System.Drawing.Size(48, 23);
            this.zoomOutButton.TabIndex = 2;
            this.zoomOutButton.Text = "缩小";
            this.zoomOutButton.UseVisualStyleBackColor = true;
            this.zoomOutButton.Click += new System.EventHandler(this.zoomOutButton_Click);
            // 
            // zoomInButton
            // 
            this.zoomInButton.Location = new System.Drawing.Point(3, 3);
            this.zoomInButton.Name = "zoomInButton";
            this.zoomInButton.Size = new System.Drawing.Size(48, 23);
            this.zoomInButton.TabIndex = 1;
            this.zoomInButton.Text = "放大";
            this.zoomInButton.UseVisualStyleBackColor = true;
            this.zoomInButton.Click += new System.EventHandler(this.zoomInButton_Click);
            // 
            // displayAllWordsCheckbox
            // 
            this.displayAllWordsCheckbox.AutoSize = true;
            this.displayAllWordsCheckbox.Location = new System.Drawing.Point(122, 9);
            this.displayAllWordsCheckbox.Name = "displayAllWordsCheckbox";
            this.displayAllWordsCheckbox.Size = new System.Drawing.Size(84, 16);
            this.displayAllWordsCheckbox.TabIndex = 7;
            this.displayAllWordsCheckbox.Text = "显示所有字";
            this.displayAllWordsCheckbox.UseVisualStyleBackColor = true;
            this.displayAllWordsCheckbox.CheckedChanged += new System.EventHandler(this.displayAllWordsCheckbox_CheckedChanged_1);
            // 
            // displayAllLinesCheckbox
            // 
            this.displayAllLinesCheckbox.AutoSize = true;
            this.displayAllLinesCheckbox.Location = new System.Drawing.Point(12, 9);
            this.displayAllLinesCheckbox.Name = "displayAllLinesCheckbox";
            this.displayAllLinesCheckbox.Size = new System.Drawing.Size(84, 16);
            this.displayAllLinesCheckbox.TabIndex = 6;
            this.displayAllLinesCheckbox.Text = "显示所有行";
            this.displayAllLinesCheckbox.UseVisualStyleBackColor = true;
            this.displayAllLinesCheckbox.CheckedChanged += new System.EventHandler(this.displayAllLinesCheckbox_CheckedChanged_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 725);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "坐标位置";
            // 
            // mouseCoordinateTextbox
            // 
            this.mouseCoordinateTextbox.Enabled = false;
            this.mouseCoordinateTextbox.Location = new System.Drawing.Point(70, 720);
            this.mouseCoordinateTextbox.Name = "mouseCoordinateTextbox";
            this.mouseCoordinateTextbox.Size = new System.Drawing.Size(73, 21);
            this.mouseCoordinateTextbox.TabIndex = 15;
            // 
            // settingTestingGroundDataFileDialog
            // 
            this.settingTestingGroundDataFileDialog.FileName = "openFileDialog2";
            // 
            // PDFForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1108, 749);
            this.Controls.Add(this.mouseCoordinateTextbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PDFForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PDF导航器";
            this.Load += new System.EventHandler(this.PDFForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TreeView pdfTreeView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem kaiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem 高级功能ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 批量生成XMLToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog imageTargetfolderBrowserDialog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox mouseCoordinateTextbox;
        private System.Windows.Forms.ToolStripMenuItem 批量生成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关键词搜索ToolStripMenuItem;
        private System.Windows.Forms.Button zoomOutButton;
        private System.Windows.Forms.Button zoomInButton;
        private System.Windows.Forms.Label zoomLabel;
        private System.Windows.Forms.ToolStripMenuItem 基准信息录入ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 测试ToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog settingTestingGroundDataFileDialog;
        private System.Windows.Forms.ToolStripMenuItem 退化效果ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退化ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开页面图像集合ToolStripMenuItem;
        private System.Windows.Forms.CheckBox displayAllWordsCheckbox;
        private System.Windows.Forms.CheckBox displayAllLinesCheckbox;
    }
}

