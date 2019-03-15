namespace EWU_TEALS_Draw {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.ImageBox_VideoCapture = new Emgu.CV.UI.ImageBox();
            this.ImageBox_VideoCapture_Gray = new Emgu.CV.UI.ImageBox();
            this.ImageBox_Drawing = new Emgu.CV.UI.ImageBox();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImageBox_VideoCapture
            // 
            this.ImageBox_VideoCapture.Location = new System.Drawing.Point(8, 8);
            this.ImageBox_VideoCapture.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.ImageBox_VideoCapture.Name = "ImageBox_VideoCapture";
            this.ImageBox_VideoCapture.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture.TabIndex = 2;
            this.ImageBox_VideoCapture.TabStop = false;
            // 
            // ImageBox_VideoCapture_Gray
            // 
            this.ImageBox_VideoCapture_Gray.Location = new System.Drawing.Point(8, 262);
            this.ImageBox_VideoCapture_Gray.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.ImageBox_VideoCapture_Gray.Name = "ImageBox_VideoCapture_Gray";
            this.ImageBox_VideoCapture_Gray.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture_Gray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture_Gray.TabIndex = 2;
            this.ImageBox_VideoCapture_Gray.TabStop = false;
            // 
            // ImageBox_Drawing
            // 
            this.ImageBox_Drawing.Location = new System.Drawing.Point(8, 8);
            this.ImageBox_Drawing.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.ImageBox_Drawing.Name = "ImageBox_Drawing";
            this.ImageBox_Drawing.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_Drawing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_Drawing.TabIndex = 3;
            this.ImageBox_Drawing.TabStop = false;
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(281, 3);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(67, 23);
            this.btnPlay.TabIndex = 4;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(195, 3);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 5;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(99, 3);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(3, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.52331F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.47669F));
            this.tableLayoutPanel1.Controls.Add(this.ImageBox_Drawing, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(14, 15);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1416, 828);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.ImageBox_VideoCapture, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ImageBox_VideoCapture_Gray, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1057, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.90381F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.09619F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(356, 579);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tableLayoutPanel3.Controls.Add(this.btnExit, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnReset, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnPause, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnPlay, 3, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(2, 491);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(352, 81);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1443, 857);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.Text = "EWU TEALS Draw";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBox_VideoCapture;
        private Emgu.CV.UI.ImageBox ImageBox_VideoCapture_Gray;
        private Emgu.CV.UI.ImageBox ImageBox_Drawing;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}