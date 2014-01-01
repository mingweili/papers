namespace PDFNavigator
{
    partial class BatchXMLForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.sourcePathTextbox = new System.Windows.Forms.TextBox();
            this.sourceBrowseButton = new System.Windows.Forms.Button();
            this.targetBrowseButton = new System.Windows.Forms.Button();
            this.targetPathTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.executeButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.sourceBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.targetBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.createPageImageCheckbox = new System.Windows.Forms.CheckBox();
            this.createImageXMLCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "源文件路径";
            // 
            // sourcePathTextbox
            // 
            this.sourcePathTextbox.Location = new System.Drawing.Point(84, 21);
            this.sourcePathTextbox.Name = "sourcePathTextbox";
            this.sourcePathTextbox.Size = new System.Drawing.Size(155, 21);
            this.sourcePathTextbox.TabIndex = 1;
            // 
            // sourceBrowseButton
            // 
            this.sourceBrowseButton.Location = new System.Drawing.Point(245, 19);
            this.sourceBrowseButton.Name = "sourceBrowseButton";
            this.sourceBrowseButton.Size = new System.Drawing.Size(26, 23);
            this.sourceBrowseButton.TabIndex = 2;
            this.sourceBrowseButton.Text = "…";
            this.sourceBrowseButton.UseVisualStyleBackColor = true;
            this.sourceBrowseButton.Click += new System.EventHandler(this.sourceBrowseButton_Click);
            // 
            // targetBrowseButton
            // 
            this.targetBrowseButton.Location = new System.Drawing.Point(245, 74);
            this.targetBrowseButton.Name = "targetBrowseButton";
            this.targetBrowseButton.Size = new System.Drawing.Size(26, 23);
            this.targetBrowseButton.TabIndex = 5;
            this.targetBrowseButton.Text = "…";
            this.targetBrowseButton.UseVisualStyleBackColor = true;
            this.targetBrowseButton.Click += new System.EventHandler(this.targetBrowseButton_Click);
            // 
            // targetPathTextbox
            // 
            this.targetPathTextbox.Location = new System.Drawing.Point(84, 76);
            this.targetPathTextbox.Name = "targetPathTextbox";
            this.targetPathTextbox.Size = new System.Drawing.Size(155, 21);
            this.targetPathTextbox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "目标文件路径";
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(83, 135);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(75, 23);
            this.executeButton.TabIndex = 6;
            this.executeButton.Text = "开始转化";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(164, 135);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 7;
            this.ExitButton.Text = "取消";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(9, 175);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(256, 23);
            this.progressBar.TabIndex = 8;
            // 
            // createPageImageCheckbox
            // 
            this.createPageImageCheckbox.AutoSize = true;
            this.createPageImageCheckbox.Location = new System.Drawing.Point(3, 113);
            this.createPageImageCheckbox.Name = "createPageImageCheckbox";
            this.createPageImageCheckbox.Size = new System.Drawing.Size(108, 16);
            this.createPageImageCheckbox.TabIndex = 16;
            this.createPageImageCheckbox.Text = "并生成每页图像";
            this.createPageImageCheckbox.UseVisualStyleBackColor = true;
            // 
            // createImageXMLCheckbox
            // 
            this.createImageXMLCheckbox.AutoSize = true;
            this.createImageXMLCheckbox.Location = new System.Drawing.Point(118, 113);
            this.createImageXMLCheckbox.Name = "createImageXMLCheckbox";
            this.createImageXMLCheckbox.Size = new System.Drawing.Size(144, 16);
            this.createImageXMLCheckbox.TabIndex = 17;
            this.createImageXMLCheckbox.Text = "并生成图像的标引信息";
            this.createImageXMLCheckbox.UseVisualStyleBackColor = true;
            // 
            // BatchXMLForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 225);
            this.Controls.Add(this.createImageXMLCheckbox);
            this.Controls.Add(this.createPageImageCheckbox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.targetBrowseButton);
            this.Controls.Add(this.targetPathTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sourceBrowseButton);
            this.Controls.Add(this.sourcePathTextbox);
            this.Controls.Add(this.label1);
            this.Name = "BatchXMLForm";
            this.Text = "批量转化";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sourcePathTextbox;
        private System.Windows.Forms.Button sourceBrowseButton;
        private System.Windows.Forms.Button targetBrowseButton;
        private System.Windows.Forms.TextBox targetPathTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.FolderBrowserDialog sourceBrowserDialog;
        private System.Windows.Forms.FolderBrowserDialog targetBrowserDialog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox createPageImageCheckbox;
        private System.Windows.Forms.CheckBox createImageXMLCheckbox;
    }
}