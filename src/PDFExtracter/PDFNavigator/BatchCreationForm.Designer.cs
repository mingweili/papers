namespace PDFNavigator
{
    partial class BatchCreationForm
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
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.sourceFilePathTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.objectFilePathTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.button3 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.DPITextbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.isAppendLocationPoint = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.createPageImageCheckbox = new System.Windows.Forms.CheckBox();
            this.createXMLCheckBox = new System.Windows.Forms.CheckBox();
            this.pageImageLabel = new System.Windows.Forms.Label();
            this.pageImageSavePathTextbox = new System.Windows.Forms.TextBox();
            this.pageImageFolderButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "txt文件夹路径";
            // 
            // sourceFilePathTextBox
            // 
            this.sourceFilePathTextBox.Location = new System.Drawing.Point(103, 42);
            this.sourceFilePathTextBox.Name = "sourceFilePathTextBox";
            this.sourceFilePathTextBox.Size = new System.Drawing.Size(267, 21);
            this.sourceFilePathTextBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(391, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(33, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "目标文件夹路径";
            // 
            // objectFilePathTextBox
            // 
            this.objectFilePathTextBox.Location = new System.Drawing.Point(104, 84);
            this.objectFilePathTextBox.Name = "objectFilePathTextBox";
            this.objectFilePathTextBox.Size = new System.Drawing.Size(266, 21);
            this.objectFilePathTextBox.TabIndex = 4;
            this.objectFilePathTextBox.Leave += new System.EventHandler(this.objectFilePathTextBox_Leave);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(391, 82);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(33, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(225, 265);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(92, 36);
            this.executeButton.TabIndex = 6;
            this.executeButton.Text = "开始生成";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(336, 265);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(88, 36);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(95, 128);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(91, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "字体设置...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "DPI";
            // 
            // DPITextbox
            // 
            this.DPITextbox.Location = new System.Drawing.Point(239, 128);
            this.DPITextbox.Name = "DPITextbox";
            this.DPITextbox.Size = new System.Drawing.Size(41, 21);
            this.DPITextbox.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label5.Location = new System.Drawing.Point(12, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "纸张规格：A4";
            // 
            // isAppendLocationPoint
            // 
            this.isAppendLocationPoint.AutoSize = true;
            this.isAppendLocationPoint.Location = new System.Drawing.Point(286, 132);
            this.isAppendLocationPoint.Name = "isAppendLocationPoint";
            this.isAppendLocationPoint.Size = new System.Drawing.Size(84, 16);
            this.isAppendLocationPoint.TabIndex = 13;
            this.isAppendLocationPoint.Text = "添加定位块";
            this.isAppendLocationPoint.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(14, 241);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(410, 12);
            this.progressBar.TabIndex = 14;
            // 
            // createPageImageCheckbox
            // 
            this.createPageImageCheckbox.AutoSize = true;
            this.createPageImageCheckbox.Location = new System.Drawing.Point(14, 201);
            this.createPageImageCheckbox.Name = "createPageImageCheckbox";
            this.createPageImageCheckbox.Size = new System.Drawing.Size(108, 16);
            this.createPageImageCheckbox.TabIndex = 15;
            this.createPageImageCheckbox.Text = "并生成每页图像";
            this.createPageImageCheckbox.UseVisualStyleBackColor = true;
            this.createPageImageCheckbox.CheckedChanged += new System.EventHandler(this.isCreateImageFormatDoc_CheckedChanged);
            // 
            // createXMLCheckBox
            // 
            this.createXMLCheckBox.AutoSize = true;
            this.createXMLCheckBox.Location = new System.Drawing.Point(14, 173);
            this.createXMLCheckBox.Name = "createXMLCheckBox";
            this.createXMLCheckBox.Size = new System.Drawing.Size(126, 16);
            this.createXMLCheckBox.TabIndex = 16;
            this.createXMLCheckBox.Text = "并生成XML标引文件";
            this.createXMLCheckBox.UseVisualStyleBackColor = true;
            this.createXMLCheckBox.CheckedChanged += new System.EventHandler(this.createXMLCheckBox_CheckedChanged);
            // 
            // pageImageLabel
            // 
            this.pageImageLabel.AutoSize = true;
            this.pageImageLabel.Location = new System.Drawing.Point(118, 201);
            this.pageImageLabel.Name = "pageImageLabel";
            this.pageImageLabel.Size = new System.Drawing.Size(107, 12);
            this.pageImageLabel.TabIndex = 17;
            this.pageImageLabel.Text = "  |  图像存储路径";
            this.pageImageLabel.Visible = false;
            // 
            // pageImageSavePathTextbox
            // 
            this.pageImageSavePathTextbox.Location = new System.Drawing.Point(225, 197);
            this.pageImageSavePathTextbox.Name = "pageImageSavePathTextbox";
            this.pageImageSavePathTextbox.Size = new System.Drawing.Size(142, 21);
            this.pageImageSavePathTextbox.TabIndex = 18;
            this.pageImageSavePathTextbox.Visible = false;
            // 
            // pageImageFolderButton
            // 
            this.pageImageFolderButton.Location = new System.Drawing.Point(392, 195);
            this.pageImageFolderButton.Name = "pageImageFolderButton";
            this.pageImageFolderButton.Size = new System.Drawing.Size(32, 23);
            this.pageImageFolderButton.TabIndex = 19;
            this.pageImageFolderButton.Text = "...";
            this.pageImageFolderButton.UseVisualStyleBackColor = true;
            this.pageImageFolderButton.Visible = false;
            this.pageImageFolderButton.Click += new System.EventHandler(this.pageImageFolderButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(14, 260);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(65, 12);
            this.statusLabel.TabIndex = 20;
            this.statusLabel.Text = "现在状态：";
            // 
            // BatchCreationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 326);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.pageImageFolderButton);
            this.Controls.Add(this.pageImageSavePathTextbox);
            this.Controls.Add(this.pageImageLabel);
            this.Controls.Add(this.createXMLCheckBox);
            this.Controls.Add(this.createPageImageCheckbox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.isAppendLocationPoint);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DPITextbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.objectFilePathTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.sourceFilePathTextBox);
            this.Controls.Add(this.label1);
            this.Name = "BatchCreationForm";
            this.Text = "批量生成";
            this.Load += new System.EventHandler(this.BatchCreationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sourceFilePathTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox objectFilePathTextBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox DPITextbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox isAppendLocationPoint;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox createPageImageCheckbox;
        private System.Windows.Forms.CheckBox createXMLCheckBox;
        private System.Windows.Forms.Label pageImageLabel;
        private System.Windows.Forms.TextBox pageImageSavePathTextbox;
        private System.Windows.Forms.Button pageImageFolderButton;
        private System.Windows.Forms.Label statusLabel;

    }
}