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
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox_VideoCapture
            // 
            this.ImageBox_VideoCapture.Location = new System.Drawing.Point(676, 9);
            this.ImageBox_VideoCapture.Margin = new System.Windows.Forms.Padding(8);
            this.ImageBox_VideoCapture.Name = "ImageBox_VideoCapture";
            this.ImageBox_VideoCapture.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture.TabIndex = 2;
            this.ImageBox_VideoCapture.TabStop = false;
            // 
            // ImageBox_VideoCapture_Gray
            // 
            this.ImageBox_VideoCapture_Gray.Location = new System.Drawing.Point(676, 231);
            this.ImageBox_VideoCapture_Gray.Margin = new System.Windows.Forms.Padding(8);
            this.ImageBox_VideoCapture_Gray.Name = "ImageBox_VideoCapture_Gray";
            this.ImageBox_VideoCapture_Gray.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture_Gray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture_Gray.TabIndex = 2;
            this.ImageBox_VideoCapture_Gray.TabStop = false;
            // 
            // ImageBox_Drawing
            // 
            this.ImageBox_Drawing.Location = new System.Drawing.Point(19, 9);
            this.ImageBox_Drawing.Margin = new System.Windows.Forms.Padding(8);
            this.ImageBox_Drawing.Name = "ImageBox_Drawing";
            this.ImageBox_Drawing.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_Drawing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_Drawing.TabIndex = 3;
            this.ImageBox_Drawing.TabStop = false;
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(676, 444);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 4;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(757, 444);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 5;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(838, 444);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(921, 444);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1026, 609);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.ImageBox_Drawing);
            this.Controls.Add(this.ImageBox_VideoCapture_Gray);
            this.Controls.Add(this.ImageBox_VideoCapture);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.Text = "EWU TEALS Draw";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBox_VideoCapture;
        private Emgu.CV.UI.ImageBox ImageBox_VideoCapture_Gray;
        private Emgu.CV.UI.ImageBox ImageBox_Drawing;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExit;
    }
}