namespace PDF_Page_Counter
{
    partial class FormOptimized
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.headerFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerFilesize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerPagescount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerFilepath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.txt_path = new System.Windows.Forms.TextBox();
            this.btn_run = new System.Windows.Forms.Button();
            this.Total = new System.Windows.Forms.Label();
            this.lbl_files = new System.Windows.Forms.Label();
            this.lbl_pages = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.headerFile,
            this.headerFilesize,
            this.columnHeader1,
            this.headerPagescount,
            this.headerFilepath,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 55);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1165, 578);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // headerFile
            // 
            this.headerFile.Text = "File";
            this.headerFile.Width = 175;
            // 
            // headerFilesize
            // 
            this.headerFilesize.Text = "Filesize";
            this.headerFilesize.Width = 100;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File Status";
            this.columnHeader1.Width = 100;
            // 
            // headerPagescount
            // 
            this.headerPagescount.Text = "Pages Count";
            this.headerPagescount.Width = 100;
            // 
            // headerFilepath
            // 
            this.headerFilepath.Text = "File Path";
            this.headerFilepath.Width = 494;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Errors";
            this.columnHeader2.Width = 200;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBox1.Location = new System.Drawing.Point(1028, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox1.Size = new System.Drawing.Size(137, 18);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Include Subdirectories";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Clear List";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txt_path
            // 
            this.txt_path.Location = new System.Drawing.Point(106, 14);
            this.txt_path.Name = "txt_path";
            this.txt_path.Size = new System.Drawing.Size(586, 20);
            this.txt_path.TabIndex = 11;
            this.txt_path.Text = "C:\\PRUEBA CONTENCIOSOS MAYOR CUANTIA";
            // 
            // btn_run
            // 
            this.btn_run.Location = new System.Drawing.Point(698, 14);
            this.btn_run.Name = "btn_run";
            this.btn_run.Size = new System.Drawing.Size(75, 23);
            this.btn_run.TabIndex = 12;
            this.btn_run.Text = "Run";
            this.btn_run.UseVisualStyleBackColor = true;
            this.btn_run.Click += new System.EventHandler(this.button1_Click);
            // 
            // Total
            // 
            this.Total.AutoSize = true;
            this.Total.Location = new System.Drawing.Point(9, 638);
            this.Total.Name = "Total";
            this.Total.Size = new System.Drawing.Size(31, 13);
            this.Total.TabIndex = 13;
            this.Total.Text = "Total";
            // 
            // lbl_files
            // 
            this.lbl_files.AutoSize = true;
            this.lbl_files.Location = new System.Drawing.Point(46, 638);
            this.lbl_files.Name = "lbl_files";
            this.lbl_files.Size = new System.Drawing.Size(13, 13);
            this.lbl_files.TabIndex = 14;
            this.lbl_files.Text = "0";
            // 
            // lbl_pages
            // 
            this.lbl_pages.AutoSize = true;
            this.lbl_pages.Location = new System.Drawing.Point(140, 638);
            this.lbl_pages.Name = "lbl_pages";
            this.lbl_pages.Size = new System.Drawing.Size(13, 13);
            this.lbl_pages.TabIndex = 16;
            this.lbl_pages.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(103, 638);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Paginas";
            // 
            // FormOptimized
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 660);
            this.Controls.Add(this.lbl_pages);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbl_files);
            this.Controls.Add(this.Total);
            this.Controls.Add(this.btn_run);
            this.Controls.Add(this.txt_path);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.listView1);
            this.Name = "FormOptimized";
            this.Text = "FormOptimized";
            this.Load += new System.EventHandler(this.FormOptimized_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader headerFile;
        private System.Windows.Forms.ColumnHeader headerFilesize;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader headerPagescount;
        private System.Windows.Forms.ColumnHeader headerFilepath;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txt_path;
        private System.Windows.Forms.Button btn_run;
        private System.Windows.Forms.Label Total;
        private System.Windows.Forms.Label lbl_files;
        private System.Windows.Forms.Label lbl_pages;
        private System.Windows.Forms.Label label3;
    }
}