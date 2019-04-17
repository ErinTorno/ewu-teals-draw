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
using EwuTeals.Draw;
using System.Collections.ObjectModel;

namespace EWU_TEALS_Draw
{
    public partial class MainForm : Form
    {
        private List<IDisposable> Disposables;
        private VideoCapture VideoCapture;
        private bool IsRunning;

        #region Resolution Properties
        private const int FPS = 30;
        private const int ActualCameraWidth = 1920;
        private const int ActualCameraHeight = 1080;

        private const int DisplayedCameraWidth = ActualCameraWidth / 6;
        private const int DisplayedCameraHeight = ActualCameraHeight / 6;

        private int CanvasWidth = (int)Math.Floor(DisplayedCameraWidth * 3.76);
        private int CanvasHeight = (int)Math.Floor(DisplayedCameraHeight * 3.76);
        #endregion

        private ObservableCollection<Detectable> Detectables = new ObservableCollection<Detectable>();
        
        private AutoColor AutoColor;

        #region Re-used Objects for saving memory
        Queue<Mat> DisposableQueue = new Queue<Mat>();
        #endregion

        #region Controls
        private const Keys KeyReset = Keys.R;
        private const Keys KeyExit = Keys.Q;
        private const Keys KeySave = Keys.S;
        private const Keys KeyOpen = Keys.O;
        private const Keys KeyToggleAuto = Keys.A;
        private const Keys KeyCaptureColor = Keys.Space;
        private const Keys KeyClearDetectables = Keys.C;
        #endregion

        public MainForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            Startup();
        }

        private void Startup()
        {
            SetupVideoCapture();
            ImageBox_Drawing.Image = new Image<Bgr, byte>(CanvasWidth, CanvasHeight, new Bgr(255, 255, 255));
            AutoColor = new AutoColor(ImageBox_VideoCapture);

            Application.Idle += ProcessFrame;
            IsRunning = true;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            ColorPicker.SelectedIndex = 0;

            Detectables.Add(new DetectableColor("Red",     true,  inkColor: new MCvScalar(60, 60, 230),   minHsv: new MCvScalar(0, 125, 180),   maxHsv: new MCvScalar(6, 255, 255)));
            Detectables.Add(new DetectableColor("Orange",  true,  inkColor: new MCvScalar(60, 140, 255),  minHsv: new MCvScalar(10, 175, 65),   maxHsv: new MCvScalar(18, 255, 255)));
            Detectables.Add(new DetectableColor("Yellow",  true,  inkColor: new MCvScalar(100, 240, 240), minHsv: new MCvScalar(19, 50, 195),   maxHsv: new MCvScalar(35, 255, 255)));
            Detectables.Add(new DetectableColor("Green",   true,  inkColor: new MCvScalar(135, 230, 135), minHsv: new MCvScalar(70, 70, 75),    maxHsv: new MCvScalar(95, 255, 255)));
            Detectables.Add(new DetectableColor("Blue",    true,  inkColor: new MCvScalar(255, 140, 185), minHsv: new MCvScalar(99, 111, 66),   maxHsv: new MCvScalar(117, 255, 255)));
            Detectables.Add(new DetectableColor("Purple",  true,  inkColor: new MCvScalar(255, 135, 135), minHsv: new MCvScalar(125, 100, 100), maxHsv: new MCvScalar(140, 255, 255)));
            Detectables.Add(new DetectableColor("Special", false, inkColor: new MCvScalar(0, 0, 0),       minHsv: new MCvScalar(0, 0, 0),       maxHsv: new MCvScalar(180, 255, 255)));

            Detectables.CollectionChanged += (sender, e) => {
                ColorPicker.Items.Clear();
                foreach (var d in Detectables)
                    ColorPicker.Items.Add(d.Name);
            };

            AutoColor.OnColorCapture += (sender, e) => {
                Detectables.Add(e.Color);
            };
        }

        private void SetupVideoCapture()
        {
            // attempt to use this type; if it fails, we go to default
            VideoCapture = new VideoCapture(1 + CaptureType.DShow); // Need DShow backend for Logitech Webcam
            if (VideoCapture.Width == 0 || VideoCapture.Height == 0)
                VideoCapture = new VideoCapture(0);
            
            VideoCapture.SetCaptureProperty(CapProp.FrameWidth, DisplayedCameraWidth);
            VideoCapture.SetCaptureProperty(CapProp.FrameHeight, DisplayedCameraHeight);
            VideoCapture.SetCaptureProperty(CapProp.Autofocus, 0);
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (VideoCapture != null)
            {
                Mat videoFrame = VideoCapture.QueryFrame(); // If not managed, video frame causes .2mb/s Loss, does not get cleaned up by GC. Must manually dispose.
                CvInvoke.Flip(videoFrame, videoFrame, FlipType.Horizontal);
                ImageBox_VideoCapture.Image = videoFrame;
                DisposableQueue.Enqueue(videoFrame); // Add Video Frames to a queue to be disposed when NOT in use

                Mat combinedThreshImage = Mat.Zeros(videoFrame.Rows, videoFrame.Cols, DepthType.Cv8U, 1);
                DisposableQueue.Enqueue(combinedThreshImage);

                AutoColor.Update(videoFrame);

                foreach (var d in Detectables) {
                    if (d.IsEnabled) {
                        Mat curThresh = d.Draw(ImageBox_Drawing, videoFrame);
                        CvInvoke.Add(curThresh, combinedThreshImage, combinedThreshImage);
                    }
                }

                ImageBox_VideoCapture_Gray.Image = combinedThreshImage;

                if (DisposableQueue.Count > 8)
                {
                    DisposableQueue.Dequeue().Dispose();
                    DisposableQueue.Dequeue().Dispose();
                }
            }
        }

        private IImage GetGrayImage(Mat color_image)
        {
            Image<Gray, byte> grayImage = new Image<Gray, byte>(color_image.Bitmap);

            return grayImage;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ReleaseResources();
        }

        private void ReleaseResources()
        {
            if (Disposables != null)
            {
                foreach (IDisposable disposable in Disposables)
                {
                    if (disposable != null) disposable.Dispose();
                }

                Disposables = null;
            }

            if (DisposableQueue != null)
            {
                foreach (IDisposable disposable in DisposableQueue)
                {
                    if (disposable != null) disposable.Dispose();
                }
            }
        }
        
        void MainForm_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case KeyToggleAuto:
                    AutoColor.IsActive = !AutoColor.IsActive;
                    break;
                case KeyCaptureColor:
                    AutoColor.CaptureNextUpdate = true;
                    break;
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
                case KeyClearDetectables:
                    Detectables.Clear();
                    break;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (IsRunning == false)
            {
                Application.Idle += ProcessFrame;
                IsRunning = true;
                btnPlay.Text = "Pause";
            }
            else {
                Application.Idle -= ProcessFrame;
                IsRunning = false;
                btnPlay.Text = "Play";

                // we reset each of these to prevent weird line issues when unpausing at far away locations
                foreach (var d in Detectables) d.ResetLastPosition();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to exit the beautiful application?",
                    "Important Question",
                    MessageBoxButtons.YesNo);
            if(result == DialogResult.Yes)
                Application.Exit();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ImageBox_Drawing.Image = new Image<Bgr, byte>(CanvasWidth, CanvasHeight, new Bgr(255, 255, 255));
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            tableLayoutPanel_Sliders.Visible = !tableLayoutPanel_Sliders.Visible;
        }

        private void ColorPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (ColorPicker.SelectedIndex < Detectables.Count) {
                CheckBox_IsMin.Checked = true;
                UpdateSliderValues(sender, e);

                CheckBox_ColorOn.Checked = Detectables[ColorPicker.SelectedIndex].IsEnabled;
            }
        }

        private void HSlider_ValueChanged(object sender, EventArgs e)
        {
            TrackBar bar = (TrackBar)sender;
            lblH.Text = "H(" + bar.Value + ")";

            UpdateHSVCodes();
        }

        private void SSlider_ValueChanged(object sender, EventArgs e)
        {
            TrackBar bar = (TrackBar)sender;
            lblS.Text = "S(" + bar.Value + ")";

            UpdateHSVCodes();
        }

        private void VSlider_ValueChanged(object sender, EventArgs e)
        {
            TrackBar bar = (TrackBar)sender;
            lblV.Text = "V(" + bar.Value + ")";

            UpdateHSVCodes();
        }

        private void CheckBox_ColorOn_CheckedChanged(object sender, EventArgs e)
        {
            if (ColorPicker.SelectedIndex < Detectables.Count)
                Detectables[ColorPicker.SelectedIndex].IsEnabled = CheckBox_ColorOn.Checked;
        }

        private void UpdateSliderValues(object sender, EventArgs e) {
            if (ColorPicker.SelectedIndex < Detectables.Count) {
                var drawable = Detectables[ColorPicker.SelectedIndex];
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
            if (ColorPicker.SelectedIndex < Detectables.Count) {
                var drawable = Detectables[ColorPicker.SelectedIndex];
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
            var settings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All
            };
            var json = JsonConvert.SerializeObject(Detectables, settings);
            File.WriteAllText(path, json);
        }

        public void LoadHsvFromFile(string path) {
            var settings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All
            };
            var ser = JsonConvert.DeserializeObject<ObservableCollection<Detectable>>(File.ReadAllText(path), settings);
            // for all availale colors, we update them
            Detectables.Clear();
            foreach (var d in ser)
                Detectables.Add(d);
        }
    }
}
