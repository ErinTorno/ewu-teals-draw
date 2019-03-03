namespace EWU_TEALS_Draw
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.ImageBox_VideoCapture = new Emgu.CV.UI.ImageBox();
            this.ImageBox_VideoCapture_Gray = new Emgu.CV.UI.ImageBox();
            this.ImageBox_Drawing = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox_VideoCapture
            // 
            this.ImageBox_VideoCapture.Location = new System.Drawing.Point(25, 25);
            this.ImageBox_VideoCapture.Margin = new System.Windows.Forms.Padding(10);
            this.ImageBox_VideoCapture.Name = "ImageBox_VideoCapture";
            this.ImageBox_VideoCapture.Size = new System.Drawing.Size(480, 260);
            this.ImageBox_VideoCapture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture.TabIndex = 2;
            this.ImageBox_VideoCapture.TabStop = false;
            // 
            // ImageBox_VideoCapture_Gray
            // 
            this.ImageBox_VideoCapture_Gray.Location = new System.Drawing.Point(550, 25);
            this.ImageBox_VideoCapture_Gray.Margin = new System.Windows.Forms.Padding(10);
            this.ImageBox_VideoCapture_Gray.Name = "ImageBox_VideoCapture_Gray";
            this.ImageBox_VideoCapture_Gray.Size = new System.Drawing.Size(480, 260);
            this.ImageBox_VideoCapture_Gray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture_Gray.TabIndex = 2;
            this.ImageBox_VideoCapture_Gray.TabStop = false;
            // 
            // ImageBox_Drawing
            // 
            this.ImageBox_Drawing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBox_Drawing.Location = new System.Drawing.Point(25, 331);
            this.ImageBox_Drawing.Margin = new System.Windows.Forms.Padding(10);
            this.ImageBox_Drawing.Name = "ImageBox_Drawing";
            this.ImageBox_Drawing.Size = new System.Drawing.Size(960, 520);
            this.ImageBox_Drawing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_Drawing.TabIndex = 3;
            this.ImageBox_Drawing.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1220, 872);
            this.Controls.Add(this.ImageBox_Drawing);
            this.Controls.Add(this.ImageBox_VideoCapture_Gray);
            this.Controls.Add(this.ImageBox_VideoCapture);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(15);
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
    }
}

