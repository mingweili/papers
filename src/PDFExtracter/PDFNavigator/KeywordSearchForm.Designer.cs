namespace PDFNavigator
{
    partial class KeywordSearchForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sourceBrowseButton = new System.Windows.Forms.Button();
            this.sourcePathTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.keywordTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.executeButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.processTip = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statisticButton = new System.Windows.Forms.Button();
            this.ThresholdComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.keywordLengthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keywordLengthNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // sourceBrowseButton
            // 
            this.sourceBrowseButton.Location = new System.Drawing.Point(340, 24);
            this.sourceBrowseButton.Name = "sourceBrowseButton";
            this.sourceBrowseButton.Size = new System.Drawing.Size(26, 23);
            this.sourceBrowseButton.TabIndex = 5;
            this.sourceBrowseButton.Text = "…";
            this.sourceBrowseButton.UseVisualStyleBackColor = true;
            this.sourceBrowseButton.Click += new System.EventHandler(this.sourceBrowseButton_Click);
            // 
            // sourcePathTextbox
            // 
            this.sourcePathTextbox.Location = new System.Drawing.Point(134, 26);
            this.sourcePathTextbox.Name = "sourcePathTextbox";
            this.sourcePathTextbox.Size = new System.Drawing.Size(187, 21);
            this.sourcePathTextbox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "源文件夹路径";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "关键词";
            // 
            // keywordTextbox
            // 
            this.keywordTextbox.Location = new System.Drawing.Point(94, 24);
            this.keywordTextbox.Name = "keywordTextbox";
            this.keywordTextbox.Size = new System.Drawing.Size(187, 21);
            this.keywordTextbox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(92, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "（多个关键词之间请用空格隔开）";
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(202, 88);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(75, 23);
            this.executeButton.TabIndex = 9;
            this.executeButton.Text = "确定";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(287, 305);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "关闭";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(49, 79);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(317, 220);
            this.tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.keywordTextbox);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.executeButton);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(309, 194);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "关键词搜索";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.processTip);
            this.tabPage2.Controls.Add(this.progressBar);
            this.tabPage2.Controls.Add(this.statisticButton);
            this.tabPage2.Controls.Add(this.ThresholdComboBox);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.keywordLengthNumericUpDown);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(309, 194);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "关键词统计";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // processTip
            // 
            this.processTip.AutoSize = true;
            this.processTip.Location = new System.Drawing.Point(6, 165);
            this.processTip.Name = "processTip";
            this.processTip.Size = new System.Drawing.Size(41, 12);
            this.processTip.TabIndex = 6;
            this.processTip.Text = "状态：";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(6, 139);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(297, 23);
            this.progressBar.TabIndex = 5;
            // 
            // statisticButton
            // 
            this.statisticButton.Location = new System.Drawing.Point(164, 105);
            this.statisticButton.Name = "statisticButton";
            this.statisticButton.Size = new System.Drawing.Size(75, 23);
            this.statisticButton.TabIndex = 4;
            this.statisticButton.Text = "开始统计";
            this.statisticButton.UseVisualStyleBackColor = true;
            this.statisticButton.Click += new System.EventHandler(this.statisticButton_Click);
            // 
            // ThresholdComboBox
            // 
            this.ThresholdComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ThresholdComboBox.FormattingEnabled = true;
            this.ThresholdComboBox.Items.AddRange(new object[] {
            ">5",
            ">10",
            ">50",
            ">100",
            ">500",
            ">800",
            ">1000",
            ">5000",
            ">10000",
            ">20000",
            ">100000"});
            this.ThresholdComboBox.Location = new System.Drawing.Point(140, 58);
            this.ThresholdComboBox.Name = "ThresholdComboBox";
            this.ThresholdComboBox.Size = new System.Drawing.Size(99, 20);
            this.ThresholdComboBox.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(69, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "排序阈值";
            // 
            // keywordLengthNumericUpDown
            // 
            this.keywordLengthNumericUpDown.Location = new System.Drawing.Point(140, 25);
            this.keywordLengthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.keywordLengthNumericUpDown.Name = "keywordLengthNumericUpDown";
            this.keywordLengthNumericUpDown.Size = new System.Drawing.Size(35, 21);
            this.keywordLengthNumericUpDown.TabIndex = 1;
            this.keywordLengthNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "关键词长度";
            // 
            // KeywordSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 386);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.sourceBrowseButton);
            this.Controls.Add(this.sourcePathTextbox);
            this.Controls.Add(this.label1);
            this.Name = "KeywordSearchForm";
            this.Text = "KeywordSearchForm";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.keywordLengthNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sourceBrowseButton;
        private System.Windows.Forms.TextBox sourcePathTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox keywordTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown keywordLengthNumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ThresholdComboBox;
        private System.Windows.Forms.Button statisticButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label processTip;
    }
}