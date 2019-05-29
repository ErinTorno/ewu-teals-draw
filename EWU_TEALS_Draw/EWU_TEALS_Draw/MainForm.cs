using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using EwuTeals.Detectables;
using EwuTeals.Utils;
using EwuTeals.Games;
using EwuTeals.Games.WhackAMole;

namespace EWU_TEALS_Draw {
    public partial class MainForm : Form {
        private VideoCapture VideoCapture;
        private bool IsRunning;
        private DateTime LastTime;

        #region Resolution Properties
        private const int FPS = 30;
        private const int ActualCameraWidth = 1920;
        private const int ActualCameraHeight = 1080;

        private const int DisplayedCameraWidth = ActualCameraWidth / 6;
        private const int DisplayedCameraHeight = ActualCameraHeight / 6;

        private int CanvasWidth = (int)Math.Floor(DisplayedCameraWidth * 3.76);
        private int CanvasHeight = (int)Math.Floor(DisplayedCameraHeight * 3.76);
        #endregion

        private Game _game;
        private Game Game {
            get => _game;
            set {
                _game = value;
                // we need to update the menu options for each new game
                _game.Detectables.CollectionChanged += (sender, e) => {
                    ColorPicker.Items.Clear();
                    foreach (var d in _game.Detectables)
                        ColorPicker.Items.Add(d.Name);
                };
            }
        }

        #region Re-used Objects for saving memory
        Queue<Mat> DisposableQueue = new Queue<Mat>();
        #endregion

        #region Controls
        private const Keys KeyReset = Keys.R;
        private const Keys KeyExit = Keys.Q;
        private const Keys KeySave = Keys.S;
        private const Keys KeyOpen = Keys.O;
        #endregion

        JsonSerializerSettings JsonSettings = new JsonSerializerSettings() {
            TypeNameHandling = TypeNameHandling.All
        };

        public MainForm() {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            Startup();
        }

        private void Startup() {
            SetupVideoCapture();
            ImageBox_Drawing.Image = new Image<Bgr, byte>(CanvasWidth, CanvasHeight, new Bgr(255, 255, 255));
            // Game = new MostColorGame(this, ImageBox_Drawing, ImageBox_VideoCapture, GamePanel, Detectables);
            // Game = new FreeDrawGame(this, ImageBox_Drawing, ImageBox_VideoCapture_Gray);
            Game = new WhackAMoleGame(this, ImageBox_Drawing, ImageBox_VideoCapture, ImageBox_VideoCapture_Gray, GamePanel);

            Application.Idle += ProcessFrame;
            IsRunning = true;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            ColorPicker.SelectedIndex = 0;

            LastTime = DateTime.Now;
        }

        private void SetupVideoCapture() {
            // attempt to use this type; if it fails, we go to default
            VideoCapture = new VideoCapture(1 + CaptureType.DShow); // Need DShow backend for Logitech Webcam
            if (VideoCapture.Width == 0 || VideoCapture.Height == 0)
                VideoCapture = new VideoCapture(0);

            VideoCapture.SetCaptureProperty(CapProp.FrameWidth, DisplayedCameraWidth);
            VideoCapture.SetCaptureProperty(CapProp.FrameHeight, DisplayedCameraHeight);
            VideoCapture.SetCaptureProperty(CapProp.Autofocus, 0);
        }

        private void ProcessFrame(object sender, EventArgs e) {
            if (VideoCapture != null) {
                Mat videoFrame = VideoCapture.QueryFrame(); // If not managed, video frame causes .2mb/s Loss, does not get cleaned up by GC. Must manually dispose.
                CvInvoke.Flip(videoFrame, videoFrame, FlipType.Horizontal);
                ImageBox_VideoCapture.Image = videoFrame;
                DisposableQueue.Enqueue(videoFrame); // Add Video Frames to a queue to be disposed when NOT in use

                var curTime = DateTime.Now;
                var span = curTime.Subtract(LastTime);
                LastTime = curTime;

                Game.Update(span.TotalSeconds, videoFrame);
                
                if (DisposableQueue.Count > 4) {
                    DisposableQueue.Dequeue().Dispose();
                }
            }
        }

        private IImage GetGrayImage(Mat color_image) {
            Image<Gray, byte> grayImage = new Image<Gray, byte>(color_image.Bitmap);

            return grayImage;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (DisposableQueue != null) {
                foreach (IDisposable disposable in DisposableQueue) {
                    if (disposable != null) disposable.Dispose();
                }
            }
        }

        void MainForm_KeyDown(object sender, KeyEventArgs e) {
            // if the Game is requesting all key input, then we won't run this
            if (Game.ShouldYieldKeys) {
                switch (e.KeyCode) {
                    case KeyReset:
                        btnReset.PerformClick();
                        break;
                    case KeyExit:
                        btnExit.PerformClick();
                        break;
                    case KeySave:
                        SaveFileDialog();
                        break;
                    case KeyOpen:
                        OpenFileDialog();
                        break;
                }
            }
        }

        private void btnPlay_Click(object sender, EventArgs e) {
            if (IsRunning == false) {
                Application.Idle += ProcessFrame;
                IsRunning = true;
                btnPlay.Text = "Pause";
            }
            else {
                Application.Idle -= ProcessFrame;
                IsRunning = false;
                btnPlay.Text = "Play";

                // we reset each of these to prevent weird line issues when unpausing at far away locations
                foreach (var d in Game.Detectables) d.ResetLastPosition();
            }
        }

        private void btnExit_Click(object sender, EventArgs e) {
            DialogResult result = MessageBox.Show("Do you want to exit the beautiful application?",
                    "Important Question",
                    MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                Application.Exit();
        }

        private void btnReset_Click(object sender, EventArgs e) {
            Game.Reset();
        }

        private void btnEdit_Click(object sender, EventArgs e) {
            tableLayoutPanel_Sliders.Visible = !tableLayoutPanel_Sliders.Visible;
        }

        private void ColorPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (ColorPicker.SelectedIndex < Game.Detectables.Count) {
                CheckBox_IsMin.Checked = true;
                UpdateSliderValues(sender, e);

                CheckBox_ColorOn.Checked = Game.Detectables[ColorPicker.SelectedIndex].IsEnabled;
            }
        }

        private void HSlider_ValueChanged(object sender, EventArgs e) {
            TrackBar bar = (TrackBar)sender;
            lblH.Text = "H(" + bar.Value + ")";

            UpdateHSVCodes();
        }

        private void SSlider_ValueChanged(object sender, EventArgs e) {
            TrackBar bar = (TrackBar)sender;
            lblS.Text = "S(" + bar.Value + ")";

            UpdateHSVCodes();
        }

        private void VSlider_ValueChanged(object sender, EventArgs e) {
            TrackBar bar = (TrackBar)sender;
            lblV.Text = "V(" + bar.Value + ")";

            UpdateHSVCodes();
        }

        private void CheckBox_ColorOn_CheckedChanged(object sender, EventArgs e) {
            if (ColorPicker.SelectedIndex < Game.Detectables.Count)
                Game.Detectables[ColorPicker.SelectedIndex].IsEnabled = CheckBox_ColorOn.Checked;
        }

        private void UpdateSliderValues(object sender, EventArgs e) {
            if (ColorPicker.SelectedIndex < Game.Detectables.Count) {
                var drawable = Game.Detectables[ColorPicker.SelectedIndex];
                if (drawable is DetectableColor) {
                    var hsv = (DetectableColor)drawable;
                    double[] hsvValues = null;

                    if (CheckBox_IsMin.Checked)
                        hsvValues = hsv.MinHsv.ToArray();
                    else
                        hsvValues = hsv.MaxHsv.ToArray();

                    if (hsvValues != null) {
                        HSlider.Value = (int)hsvValues[0];
                        SSlider.Value = (int)hsvValues[1];
                        VSlider.Value = (int)hsvValues[2];
                    }
                }
            }
        }

        private void UpdateHSVCodes() {
            if (ColorPicker.SelectedIndex < Game.Detectables.Count) {
                var drawable = Game.Detectables[ColorPicker.SelectedIndex];
                if (drawable is DetectableColor) {
                    var hsv = (DetectableColor)drawable;
                    if (CheckBox_IsMin.Checked)
                        hsv.MinHsv = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    else
                        hsv.MaxHsv = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                }
            }
        }

        public void SaveFileDialog() {
            using (var diag = new SaveFileDialog()) {
                diag.Filter = "JSON File|*.json";
                diag.Title = "Save HSV Preset to File";
                var res = diag.ShowDialog();
                if (res == DialogResult.OK) {
                    SaveHsvToFile(diag.FileName);
                }
            }
        }

        public void OpenFileDialog() {
            using (var diag = new OpenFileDialog()) {
                diag.Filter = "JSON File|*.json";
                diag.Title = "Open HSV Preset from File";
                var res = diag.ShowDialog();
                if (res == DialogResult.OK) {
                    LoadHsvFromFile(diag.FileName);
                }
            }
        }

        // saving and loading disabled write now until we decide how to handle this

        public void SaveHsvToFile(string path) {
            var json = JsonConvert.SerializeObject(Game.Detectables, JsonSettings);
            File.WriteAllText(path, json);
        }

        public void LoadHsvFromFile(string path) {
            var ser = JsonConvert.DeserializeObject<ObservableCollection<Detectable>>(File.ReadAllText(path), JsonSettings);
            // for all availale colors, we update them
            Game.Detectables.Clear();
            foreach (var d in ser)
                Game.Detectables.Add(d);
            if (Game.Detectables.Count > 0)
                ColorPicker.Text = Game.Detectables.First().Name;
        }

        private void GameSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Text == "Free Draw")
            {

            }
            else if (this.Text == "Whack-A-Mole")
            {

            }
            else if (this.Text == "Color Fill Game")
            {

            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {

        }
    }
}
