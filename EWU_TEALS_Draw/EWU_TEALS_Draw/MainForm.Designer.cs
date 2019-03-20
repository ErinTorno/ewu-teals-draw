﻿namespace EWU_TEALS_Draw
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
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.tableLayoutPanel_Drawing = new System.Windows.Forms.TableLayoutPanel();
            this.ImageBox_Drawing = new Emgu.CV.UI.ImageBox();
            this.tableLayoutPanel_VideoFeeds = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_Sliders = new System.Windows.Forms.TableLayoutPanel();
            this.ColorPicker = new System.Windows.Forms.ComboBox();
            this.VSlider = new System.Windows.Forms.TrackBar();
            this.SSlider = new System.Windows.Forms.TrackBar();
            this.HSlider = new System.Windows.Forms.TrackBar();
            this.ColorOn = new System.Windows.Forms.CheckBox();
            this.lblH = new System.Windows.Forms.Label();
            this.lblS = new System.Windows.Forms.Label();
            this.lblV = new System.Windows.Forms.Label();
            this.tableLayoutPanel_Buttons = new System.Windows.Forms.TableLayoutPanel();
            this.btnEdit = new System.Windows.Forms.Button();
            this.ImageBox_VideoCapture = new Emgu.CV.UI.ImageBox();
            this.ImageBox_VideoCapture_Gray = new Emgu.CV.UI.ImageBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel_Drawing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).BeginInit();
            this.tableLayoutPanel_VideoFeeds.SuspendLayout();
            this.tableLayoutPanel_Sliders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HSlider)).BeginInit();
            this.tableLayoutPanel_Buttons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlay.ForeColor = System.Drawing.Color.Black;
            this.btnPlay.Location = new System.Drawing.Point(0, 0);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(325, 34);
            this.btnPlay.TabIndex = 4;
            this.btnPlay.Text = "Play/Pause";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.Black;
            this.btnExit.Location = new System.Drawing.Point(0, 68);
            this.btnExit.Margin = new System.Windows.Forms.Padding(0);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(325, 34);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.Black;
            this.btnReset.Location = new System.Drawing.Point(0, 34);
            this.btnReset.Margin = new System.Windows.Forms.Padding(0);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(325, 34);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Clear Canvas";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tableLayoutPanel_Drawing
            // 
            this.tableLayoutPanel_Drawing.AutoScroll = true;
            this.tableLayoutPanel_Drawing.AutoSize = true;
            this.tableLayoutPanel_Drawing.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel_Drawing.ColumnCount = 1;
            this.tableLayoutPanel_Drawing.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Drawing.Controls.Add(this.ImageBox_Drawing, 0, 0);
            this.tableLayoutPanel_Drawing.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel_Drawing.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Drawing.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_Drawing.MaximumSize = new System.Drawing.Size(1611, 0);
            this.tableLayoutPanel_Drawing.Name = "tableLayoutPanel_Drawing";
            this.tableLayoutPanel_Drawing.RowCount = 1;
            this.tableLayoutPanel_Drawing.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Drawing.Size = new System.Drawing.Size(480, 1033);
            this.tableLayoutPanel_Drawing.TabIndex = 8;
            // 
            // ImageBox_Drawing
            // 
            this.ImageBox_Drawing.Location = new System.Drawing.Point(0, 100);
            this.ImageBox_Drawing.Margin = new System.Windows.Forms.Padding(0, 100, 0, 0);
            this.ImageBox_Drawing.Name = "ImageBox_Drawing";
            this.ImageBox_Drawing.Size = new System.Drawing.Size(480, 573);
            this.ImageBox_Drawing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox_Drawing.TabIndex = 3;
            this.ImageBox_Drawing.TabStop = false;
            // 
            // tableLayoutPanel_VideoFeeds
            // 
            this.tableLayoutPanel_VideoFeeds.AutoSize = true;
            this.tableLayoutPanel_VideoFeeds.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel_VideoFeeds.ColumnCount = 1;
            this.tableLayoutPanel_VideoFeeds.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_VideoFeeds.Controls.Add(this.tableLayoutPanel_Sliders, 0, 3);
            this.tableLayoutPanel_VideoFeeds.Controls.Add(this.tableLayoutPanel_Buttons, 0, 2);
            this.tableLayoutPanel_VideoFeeds.Controls.Add(this.ImageBox_VideoCapture, 0, 0);
            this.tableLayoutPanel_VideoFeeds.Controls.Add(this.ImageBox_VideoCapture_Gray, 0, 1);
            this.tableLayoutPanel_VideoFeeds.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel_VideoFeeds.Location = new System.Drawing.Point(1572, 0);
            this.tableLayoutPanel_VideoFeeds.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_VideoFeeds.MinimumSize = new System.Drawing.Size(320, 0);
            this.tableLayoutPanel_VideoFeeds.Name = "tableLayoutPanel_VideoFeeds";
            this.tableLayoutPanel_VideoFeeds.RowCount = 4;
            this.tableLayoutPanel_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_VideoFeeds.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_VideoFeeds.Size = new System.Drawing.Size(330, 1033);
            this.tableLayoutPanel_VideoFeeds.TabIndex = 9;
            // 
            // tableLayoutPanel_Sliders
            // 
            this.tableLayoutPanel_Sliders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_Sliders.ColumnCount = 2;
            this.tableLayoutPanel_Sliders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel_Sliders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 363F));
            this.tableLayoutPanel_Sliders.Controls.Add(this.ColorPicker, 1, 0);
            this.tableLayoutPanel_Sliders.Controls.Add(this.VSlider, 1, 3);
            this.tableLayoutPanel_Sliders.Controls.Add(this.SSlider, 1, 2);
            this.tableLayoutPanel_Sliders.Controls.Add(this.HSlider, 1, 1);
            this.tableLayoutPanel_Sliders.Controls.Add(this.ColorOn, 0, 0);
            this.tableLayoutPanel_Sliders.Controls.Add(this.lblH, 0, 1);
            this.tableLayoutPanel_Sliders.Controls.Add(this.lblS, 0, 2);
            this.tableLayoutPanel_Sliders.Controls.Add(this.lblV, 0, 3);
            this.tableLayoutPanel_Sliders.Location = new System.Drawing.Point(3, 609);
            this.tableLayoutPanel_Sliders.Name = "tableLayoutPanel_Sliders";
            this.tableLayoutPanel_Sliders.RowCount = 4;
            this.tableLayoutPanel_Sliders.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 41.11111F));
            this.tableLayoutPanel_Sliders.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58.88889F));
            this.tableLayoutPanel_Sliders.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel_Sliders.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel_Sliders.Size = new System.Drawing.Size(324, 166);
            this.tableLayoutPanel_Sliders.TabIndex = 15;
            this.tableLayoutPanel_Sliders.Visible = false;
            // 
            // ColorPicker
            // 
            this.ColorPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ColorPicker.FormattingEnabled = true;
            this.ColorPicker.Items.AddRange(new object[] {
            "Blue",
            "Red",
            "Yellow",
            "Green"});
            this.ColorPicker.Location = new System.Drawing.Point(63, 3);
            this.ColorPicker.Name = "ColorPicker";
            this.ColorPicker.Size = new System.Drawing.Size(357, 24);
            this.ColorPicker.TabIndex = 11;
            this.ColorPicker.SelectedIndexChanged += new System.EventHandler(this.ColorPicker_SelectedIndexChanged);
            // 
            // VSlider
            // 
            this.VSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.VSlider.Location = new System.Drawing.Point(63, 122);
            this.VSlider.Maximum = 179;
            this.VSlider.Name = "VSlider";
            this.VSlider.Size = new System.Drawing.Size(357, 41);
            this.VSlider.TabIndex = 14;
            this.VSlider.ValueChanged += new System.EventHandler(this.VSlider_ValueChanged);
            // 
            // SSlider
            // 
            this.SSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.SSlider.Location = new System.Drawing.Point(63, 78);
            this.SSlider.Maximum = 255;
            this.SSlider.Name = "SSlider";
            this.SSlider.Size = new System.Drawing.Size(357, 38);
            this.SSlider.TabIndex = 13;
            this.SSlider.ValueChanged += new System.EventHandler(this.SSlider_ValueChanged);
            // 
            // HSlider
            // 
            this.HSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.HSlider.Location = new System.Drawing.Point(63, 34);
            this.HSlider.Maximum = 255;
            this.HSlider.Name = "HSlider";
            this.HSlider.Size = new System.Drawing.Size(357, 38);
            this.HSlider.TabIndex = 12;
            this.HSlider.ValueChanged += new System.EventHandler(this.HSlider_ValueChanged);
            // 
            // ColorOn
            // 
            this.ColorOn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ColorOn.AutoSize = true;
            this.ColorOn.Checked = true;
            this.ColorOn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ColorOn.Location = new System.Drawing.Point(3, 5);
            this.ColorOn.Name = "ColorOn";
            this.ColorOn.Size = new System.Drawing.Size(49, 21);
            this.ColorOn.TabIndex = 12;
            this.ColorOn.Text = "On";
            this.ColorOn.UseVisualStyleBackColor = true;
            this.ColorOn.CheckedChanged += new System.EventHandler(this.ColorOn_CheckedChanged);
            // 
            // lblH
            // 
            this.lblH.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblH.AutoSize = true;
            this.lblH.Location = new System.Drawing.Point(3, 44);
            this.lblH.Name = "lblH";
            this.lblH.Size = new System.Drawing.Size(36, 17);
            this.lblH.TabIndex = 15;
            this.lblH.Text = "H(0)";
            // 
            // lblS
            // 
            this.lblS.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblS.AutoSize = true;
            this.lblS.Location = new System.Drawing.Point(3, 88);
            this.lblS.Name = "lblS";
            this.lblS.Size = new System.Drawing.Size(35, 17);
            this.lblS.TabIndex = 16;
            this.lblS.Text = "S(0)";
            // 
            // lblV
            // 
            this.lblV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblV.AutoSize = true;
            this.lblV.Location = new System.Drawing.Point(3, 134);
            this.lblV.Name = "lblV";
            this.lblV.Size = new System.Drawing.Size(35, 17);
            this.lblV.TabIndex = 17;
            this.lblV.Text = "V(0)";
            // 
            // tableLayoutPanel_Buttons
            // 
            this.tableLayoutPanel_Buttons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel_Buttons.AutoSize = true;
            this.tableLayoutPanel_Buttons.ColumnCount = 1;
            this.tableLayoutPanel_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Buttons.Controls.Add(this.btnPlay, 0, 0);
            this.tableLayoutPanel_Buttons.Controls.Add(this.btnEdit, 0, 3);
            this.tableLayoutPanel_Buttons.Controls.Add(this.btnExit, 0, 2);
            this.tableLayoutPanel_Buttons.Controls.Add(this.btnReset, 0, 1);
            this.tableLayoutPanel_Buttons.Location = new System.Drawing.Point(5, 470);
            this.tableLayoutPanel_Buttons.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.tableLayoutPanel_Buttons.Name = "tableLayoutPanel_Buttons";
            this.tableLayoutPanel_Buttons.RowCount = 4;
            this.tableLayoutPanel_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Buttons.Size = new System.Drawing.Size(325, 136);
            this.tableLayoutPanel_Buttons.TabIndex = 10;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.ForeColor = System.Drawing.Color.Black;
            this.btnEdit.Location = new System.Drawing.Point(0, 102);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(0);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(325, 34);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "Edit Detection";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
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
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.pictureBox1.ErrorImage = global::EWU_TEALS_Draw.Properties.Resources.logo_uh_color;
            this.pictureBox1.Image = global::EWU_TEALS_Draw.Properties.Resources.logo_uh_color;
            this.pictureBox1.Location = new System.Drawing.Point(4, 4);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1893, 87);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1902, 1033);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tableLayoutPanel_Drawing);
            this.Controls.Add(this.tableLayoutPanel_VideoFeeds);
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "EWU TEALS Draw";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.tableLayoutPanel_Drawing.ResumeLayout(false);
            this.tableLayoutPanel_Drawing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_Drawing)).EndInit();
            this.tableLayoutPanel_VideoFeeds.ResumeLayout(false);
            this.tableLayoutPanel_VideoFeeds.PerformLayout();
            this.tableLayoutPanel_Sliders.ResumeLayout(false);
            this.tableLayoutPanel_Sliders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HSlider)).EndInit();
            this.tableLayoutPanel_Buttons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox_VideoCapture_Gray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBox_VideoCapture;
        private Emgu.CV.UI.ImageBox ImageBox_VideoCapture_Gray;
        private Emgu.CV.UI.ImageBox ImageBox_Drawing;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Drawing;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_VideoFeeds;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Buttons;
        private System.Windows.Forms.Button btnEdit;
        public System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox ColorPicker;
        private System.Windows.Forms.TrackBar HSlider;
        private System.Windows.Forms.TrackBar SSlider;
        private System.Windows.Forms.TrackBar VSlider;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Sliders;
        private System.Windows.Forms.CheckBox ColorOn;
        private System.Windows.Forms.Label lblH;
        private System.Windows.Forms.Label lblS;
        private System.Windows.Forms.Label lblV;
    }
}

