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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.tableLayoutPanel_Drawing = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_VideoFeeds = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_Buttons = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).BeginInit();
            this.tableLayoutPanel_Drawing.SuspendLayout();
            this.tableLayoutPanel_VideoFeeds.SuspendLayout();
            this.tableLayoutPanel_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImageBox_VideoCapture
            // 
            this.ImageBox_VideoCapture.Location = new System.Drawing.Point(5, 100);
            this.ImageBox_VideoCapture.Margin = new System.Windows.Forms.Padding(5, 100, 5, 0);
            this.ImageBox_VideoCapture.MaximumSize = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture.Name = "ImageBox_VideoCapture";
            this.ImageBox_VideoCapture.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture.TabIndex = 2;
            this.ImageBox_VideoCapture.TabStop = false;
            // 
            // ImageBox_VideoCapture_Gray
            // 
            this.ImageBox_VideoCapture_Gray.Location = new System.Drawing.Point(5, 285);
            this.ImageBox_VideoCapture_Gray.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.ImageBox_VideoCapture_Gray.MaximumSize = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture_Gray.Name = "ImageBox_VideoCapture_Gray";
            this.ImageBox_VideoCapture_Gray.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture_Gray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture_Gray.TabIndex = 2;
            this.ImageBox_VideoCapture_Gray.TabStop = false;
            // 
            // ImageBox_Drawing
            // 
            this.ImageBox_Drawing.Location = new System.Drawing.Point(0, 100);
            this.ImageBox_Drawing.Margin = new System.Windows.Forms.Padding(0, 100, 0, 0);
            this.ImageBox_Drawing.Name = "ImageBox_Drawing";
            this.ImageBox_Drawing.Size = new System.Drawing.Size(480, 270);
            this.ImageBox_Drawing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_Drawing.TabIndex = 3;
            this.ImageBox_Drawing.TabStop = false;
            // 
            // btnPlay
            // 
            this.btnPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlay.Location = new System.Drawing.Point(0, 0);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(300, 35);
            this.btnPlay.TabIndex = 4;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(0, 105);
            this.btnExit.Margin = new System.Windows.Forms.Padding(0);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(300, 35);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(0, 35);
            this.btnPause.Margin = new System.Windows.Forms.Padding(0);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(300, 35);
            this.btnPause.TabIndex = 5;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(0, 70);
            this.btnReset.Margin = new System.Windows.Forms.Padding(0);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(300, 35);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Clear Canvas";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tableLayoutPanel_Drawing
            // 
            this.tableLayoutPanel_Drawing.AutoScroll = true;
            this.tableLayoutPanel_Drawing.AutoSize = true;
            this.tableLayoutPanel_Drawing.ColumnCount = 1;
            this.tableLayoutPanel_Drawing.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Drawing.Controls.Add(this.ImageBox_Drawing, 0, 0);
            this.tableLayoutPanel_Drawing.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel_Drawing.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Drawing.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_Drawing.MaximumSize = new System.Drawing.Size(1610, 0);
            this.tableLayoutPanel_Drawing.Name = "tableLayoutPanel_Drawing";
            this.tableLayoutPanel_Drawing.RowCount = 1;
            this.tableLayoutPanel_Drawing.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Drawing.Size = new System.Drawing.Size(480, 673);
            this.tableLayoutPanel_Drawing.TabIndex = 8;
            // 
            // tableLayoutPanel_VideoFeeds
            // 
            this.tableLayoutPanel_VideoFeeds.AutoSize = true;
            this.tableLayoutPanel_VideoFeeds.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel_VideoFeeds.ColumnCount = 1;
            this.tableLayoutPanel_VideoFeeds.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_VideoFeeds.Controls.Add(this.tableLayoutPanel_Buttons, 0, 2);
            this.tableLayoutPanel_VideoFeeds.Controls.Add(this.ImageBox_VideoCapture, 0, 0);
            this.tableLayoutPanel_VideoFeeds.Controls.Add(this.ImageBox_VideoCapture_Gray, 0, 1);
            this.tableLayoutPanel_VideoFeeds.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel_VideoFeeds.Location = new System.Drawing.Point(932, 0);
            this.tableLayoutPanel_VideoFeeds.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_VideoFeeds.MinimumSize = new System.Drawing.Size(320, 0);
            this.tableLayoutPanel_VideoFeeds.Name = "tableLayoutPanel_VideoFeeds";
            this.tableLayoutPanel_VideoFeeds.RowCount = 3;
            this.tableLayoutPanel_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_VideoFeeds.Size = new System.Drawing.Size(330, 673);
            this.tableLayoutPanel_VideoFeeds.TabIndex = 9;
            // 
            // tableLayoutPanel_Buttons
            // 
            this.tableLayoutPanel_Buttons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_Buttons.AutoSize = true;
            this.tableLayoutPanel_Buttons.ColumnCount = 1;
            this.tableLayoutPanel_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Buttons.Controls.Add(this.btnPlay, 0, 0);
            this.tableLayoutPanel_Buttons.Controls.Add(this.btnPause, 0, 1);
            this.tableLayoutPanel_Buttons.Controls.Add(this.btnReset, 0, 2);
            this.tableLayoutPanel_Buttons.Controls.Add(this.btnExit, 0, 3);
            this.tableLayoutPanel_Buttons.Location = new System.Drawing.Point(5, 470);
            this.tableLayoutPanel_Buttons.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.tableLayoutPanel_Buttons.Name = "tableLayoutPanel_Buttons";
            this.tableLayoutPanel_Buttons.RowCount = 4;
            this.tableLayoutPanel_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Buttons.Size = new System.Drawing.Size(325, 140);
            this.tableLayoutPanel_Buttons.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(110)))), ((int)(((byte)(130)))));
            this.ClientSize = new System.Drawing.Size(1262, 673);
            this.Controls.Add(this.tableLayoutPanel_Drawing);
            this.Controls.Add(this.tableLayoutPanel_VideoFeeds);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "EWU TEALS Draw";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).EndInit();
            this.tableLayoutPanel_Drawing.ResumeLayout(false);
            this.tableLayoutPanel_Drawing.PerformLayout();
            this.tableLayoutPanel_VideoFeeds.ResumeLayout(false);
            this.tableLayoutPanel_VideoFeeds.PerformLayout();
            this.tableLayoutPanel_Buttons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBox_VideoCapture;
        private Emgu.CV.UI.ImageBox ImageBox_VideoCapture_Gray;
        private Emgu.CV.UI.ImageBox ImageBox_Drawing;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Drawing;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_VideoFeeds;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Buttons;
    }
}

