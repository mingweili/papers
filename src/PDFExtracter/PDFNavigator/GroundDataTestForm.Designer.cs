namespace PDFNavigator
{
    partial class GroundDataTestForm
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
            this.srcDocPathTextbox = new System.Windows.Forms.TextBox();
            this.selectFileButton = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.disturbedDocImagePathTextbox = new System.Windows.Forms.TextBox();
            this.d = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.desSavePathTextbox = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "理想化文档路径";
            // 
            // srcDocPathTextbox
            // 
            this.srcDocPathTextbox.Location = new System.Drawing.Point(168, 23);
            this.srcDocPathTextbox.Name = "srcDocPathTextbox";
            this.srcDocPathTextbox.Size = new System.Drawing.Size(236, 21);
            this.srcDocPathTextbox.TabIndex = 2;
            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(444, 21);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(37, 23);
            this.selectFileButton.TabIndex = 3;
            this.selectFileButton.Text = "...";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.selectFileButton_Click);
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(285, 152);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(89, 35);
            this.executeButton.TabIndex = 5;
            this.executeButton.Text = "开始生成";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "含噪声文档图像";
            // 
            // disturbedDocImagePathTextbox
            // 
            this.disturbedDocImagePathTextbox.Location = new System.Drawing.Point(168, 62);
            this.disturbedDocImagePathTextbox.Name = "disturbedDocImagePathTextbox";
            this.disturbedDocImagePathTextbox.Size = new System.Drawing.Size(236, 21);
            this.disturbedDocImagePathTextbox.TabIndex = 7;
            // 
            // d
            // 
            this.d.Location = new System.Drawing.Point(444, 60);
            this.d.Name = "d";
            this.d.Size = new System.Drawing.Size(37, 23);
            this.d.TabIndex = 8;
            this.d.Text = "...";
            this.d.UseVisualStyleBackColor = true;
            this.d.Click += new System.EventHandler(this.d_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "标引信息存储路径";
            // 
            // desSavePathTextbox
            // 
            this.desSavePathTextbox.Location = new System.Drawing.Point(168, 101);
            this.desSavePathTextbox.Name = "desSavePathTextbox";
            this.desSavePathTextbox.Size = new System.Drawing.Size(236, 21);
            this.desSavePathTextbox.TabIndex = 10;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(444, 99);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(37, 23);
            this.button4.TabIndex = 12;
            this.button4.Text = "...";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(392, 152);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(89, 35);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(50, 135);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "快捷生成目录";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(47, 206);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(434, 13);
            this.progressBar1.TabIndex = 16;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // GroundDataTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 245);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.desSavePathTextbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.d);
            this.Controls.Add(this.disturbedDocImagePathTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.selectFileButton);
            this.Controls.Add(this.srcDocPathTextbox);
            this.Controls.Add(this.label1);
            this.Name = "GroundDataTestForm";
            this.Text = "GroundDataTestForm";
            this.Load += new System.EventHandler(this.GroundDataTestForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox srcDocPathTextbox;
        private System.Windows.Forms.Button selectFileButton;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox disturbedDocImagePathTextbox;
        private System.Windows.Forms.Button d;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox desSavePathTextbox;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;

    }
}