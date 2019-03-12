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
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.tableLayout_Main = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayout_VideoFeeds = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayout_Buttons = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).BeginInit();
            this.tableLayout_Main.SuspendLayout();
            this.tableLayout_VideoFeeds.SuspendLayout();
            this.tableLayout_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImageBox_VideoCapture
            // 
            this.ImageBox_VideoCapture.Location = new System.Drawing.Point(0, 0);
            this.ImageBox_VideoCapture.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBox_VideoCapture.Name = "ImageBox_VideoCapture";
            this.ImageBox_VideoCapture.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture.TabIndex = 2;
            this.ImageBox_VideoCapture.TabStop = false;
            // 
            // ImageBox_VideoCapture_Gray
            // 
            this.ImageBox_VideoCapture_Gray.Location = new System.Drawing.Point(0, 289);
            this.ImageBox_VideoCapture_Gray.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBox_VideoCapture_Gray.Name = "ImageBox_VideoCapture_Gray";
            this.ImageBox_VideoCapture_Gray.Size = new System.Drawing.Size(320, 180);
            this.ImageBox_VideoCapture_Gray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_VideoCapture_Gray.TabIndex = 2;
            this.ImageBox_VideoCapture_Gray.TabStop = false;
            // 
            // ImageBox_Drawing
            // 
            this.ImageBox_Drawing.Location = new System.Drawing.Point(11, 10);
            this.ImageBox_Drawing.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.ImageBox_Drawing.Name = "ImageBox_Drawing";
            this.ImageBox_Drawing.Size = new System.Drawing.Size(640, 360);
            this.ImageBox_Drawing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_Drawing.TabIndex = 3;
            this.ImageBox_Drawing.TabStop = false;
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(4, 4);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(4);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(69, 28);
            this.btnPlay.TabIndex = 4;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(92, 4);
            this.btnPause.Margin = new System.Windows.Forms.Padding(4);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(69, 28);
            this.btnPause.TabIndex = 5;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(180, 4);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(78, 28);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(266, 4);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(50, 28);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // tableLayout_Main
            // 
            this.tableLayout_Main.AutoScroll = true;
            this.tableLayout_Main.AutoSize = true;
            this.tableLayout_Main.ColumnCount = 2;
            this.tableLayout_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.32275F));
            this.tableLayout_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.67725F));
            this.tableLayout_Main.Controls.Add(this.ImageBox_Drawing, 0, 0);
            this.tableLayout_Main.Controls.Add(this.tableLayout_VideoFeeds, 1, 0);
            this.tableLayout_Main.Location = new System.Drawing.Point(0, 0);
            this.tableLayout_Main.Name = "tableLayout_Main";
            this.tableLayout_Main.RowCount = 1;
            this.tableLayout_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout_Main.Size = new System.Drawing.Size(1890, 1080);
            this.tableLayout_Main.TabIndex = 8;
            // 
            // tableLayout_VideoFeeds
            // 
            this.tableLayout_VideoFeeds.ColumnCount = 1;
            this.tableLayout_VideoFeeds.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout_VideoFeeds.Controls.Add(this.ImageBox_VideoCapture, 0, 0);
            this.tableLayout_VideoFeeds.Controls.Add(this.tableLayout_Buttons, 0, 2);
            this.tableLayout_VideoFeeds.Controls.Add(this.ImageBox_VideoCapture_Gray, 0, 1);
            this.tableLayout_VideoFeeds.Location = new System.Drawing.Point(972, 3);
            this.tableLayout_VideoFeeds.MinimumSize = new System.Drawing.Size(450, 0);
            this.tableLayout_VideoFeeds.Name = "tableLayout_VideoFeeds";
            this.tableLayout_VideoFeeds.RowCount = 3;
            this.tableLayout_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.7326F));
            this.tableLayout_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.2674F));
            this.tableLayout_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayout_VideoFeeds.Size = new System.Drawing.Size(450, 701);
            this.tableLayout_VideoFeeds.TabIndex = 4;
            // 
            // tableLayout_Buttons
            // 
            this.tableLayout_Buttons.AutoSize = true;
            this.tableLayout_Buttons.ColumnCount = 4;
            this.tableLayout_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tableLayout_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayout_Buttons.Controls.Add(this.btnPlay, 0, 0);
            this.tableLayout_Buttons.Controls.Add(this.btnExit, 3, 0);
            this.tableLayout_Buttons.Controls.Add(this.btnPause, 1, 0);
            this.tableLayout_Buttons.Controls.Add(this.btnReset, 2, 0);
            this.tableLayout_Buttons.Location = new System.Drawing.Point(3, 573);
            this.tableLayout_Buttons.MinimumSize = new System.Drawing.Size(320, 40);
            this.tableLayout_Buttons.Name = "tableLayout_Buttons";
            this.tableLayout_Buttons.RowCount = 1;
            this.tableLayout_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout_Buttons.Size = new System.Drawing.Size(320, 40);
            this.tableLayout_Buttons.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.tableLayout_Main);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "EWU TEALS Draw";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).EndInit();
            this.tableLayout_Main.ResumeLayout(false);
            this.tableLayout_Main.PerformLayout();
            this.tableLayout_VideoFeeds.ResumeLayout(false);
            this.tableLayout_VideoFeeds.PerformLayout();
            this.tableLayout_Buttons.ResumeLayout(false);
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
        private System.Windows.Forms.TableLayoutPanel tableLayout_Main;
        private System.Windows.Forms.TableLayoutPanel tableLayout_VideoFeeds;
        private System.Windows.Forms.TableLayoutPanel tableLayout_Buttons;
    }
}

