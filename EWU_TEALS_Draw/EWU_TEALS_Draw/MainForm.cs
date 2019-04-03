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

namespace EWU_TEALS_Draw
{
    public enum Color { Red, Yellow, Green, Blue, Orange, Purple, Special }

    public partial class MainForm : Form
    {
        private List<IDisposable> Disposables;
        private VideoCapture VideoCapture;
        private bool IsRunning;
        private int MinAreaToDetect = 600;

        #region Resolution Properties
        private const int CameraToUse = 0; // Default Camera: 0
        private const int FPS = 30;
        private const int ActualCameraWidth = 1920;
        private const int ActualCameraHeight = 1080;

        private const int DisplayedCameraWidth = ActualCameraWidth / 6;
        private const int DisplayedCameraHeight = ActualCameraHeight / 6;

        private int CanvasWidth = (int)Math.Floor(DisplayedCameraWidth * 3.76);
        private int CanvasHeight = (int)Math.Floor(DisplayedCameraHeight * 3.76);
        #endregion

        #region Color Threshold Properties
        private Dictionary<Color, HsvConfig> Colors = new Dictionary<Color, HsvConfig> {
            { Color.Red,     new HsvConfig(true,  inkColor: new MCvScalar(60, 60, 230),   minHsv: new MCvScalar(0, 125, 180),   maxHsv: new MCvScalar(6, 255, 255)) },
            { Color.Orange,  new HsvConfig(true,  inkColor: new MCvScalar(60, 140, 255),  minHsv: new MCvScalar(10, 175, 65),   maxHsv: new MCvScalar(18, 255, 255)) },
            { Color.Yellow,  new HsvConfig(true,  inkColor: new MCvScalar(100, 240, 240), minHsv: new MCvScalar(19, 50, 195),   maxHsv: new MCvScalar(35, 255, 255)) },
            { Color.Green,   new HsvConfig(true,  inkColor: new MCvScalar(135, 230, 135), minHsv: new MCvScalar(70, 70, 75),    maxHsv: new MCvScalar(95, 255, 255)) },
            { Color.Blue,    new HsvConfig(true,  inkColor: new MCvScalar(255, 140, 185), minHsv: new MCvScalar(99, 111, 66),   maxHsv: new MCvScalar(117, 255, 255)) },
            { Color.Purple,  new HsvConfig(true,  inkColor: new MCvScalar(255, 135, 135), minHsv: new MCvScalar(125, 100, 100), maxHsv: new MCvScalar(140, 255, 255)) },
            { Color.Special, new HsvConfig(false, inkColor: new MCvScalar(0, 0, 0),       minHsv: new MCvScalar(0, 0, 0),       maxHsv: new MCvScalar(180, 255, 255)) }
        };

        private Dictionary<Color, Point> LastPosition = new Dictionary<Color, Point> {
            { Color.Red, new Point(0, 0) },
            { Color.Orange, new Point(0, 0) },
            { Color.Yellow, new Point(0, 0) },
            { Color.Green, new Point(0, 0) },
            { Color.Blue, new Point(0, 0) },
            { Color.Purple, new Point(0, 0) },
            { Color.Special, new Point(0, 0) }
        };
       
        #endregion

        #region Re-used Objects for saving memory
        private Mat HsvImage_Temp = new Mat();
        private Mat RedThreshImage_Temp = new Mat();
        private Mat OrangeThreshImage_Temp = new Mat();
        private Mat YellowThreshImage_Temp = new Mat();
        private Mat GreenThreshImage_Temp = new Mat();
        private Mat BlueThreshImage_Temp = new Mat();
        private Mat PurpleThreshImage_Temp = new Mat();
        private Mat SpecialThreshImage_Temp = new Mat();

        Queue<Mat> DisposableQueue = new Queue<Mat>();
        #endregion

        #region Controls
        private const Keys KeyReset = Keys.R;
        private const Keys KeyExit = Keys.Q;
        private const Keys KeySave = Keys.S;
        private const Keys KeyOpen = Keys.O;
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

            Application.Idle += ProcessFrame;
            IsRunning = true;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            ColorPicker.SelectedIndex = 0;
        }

        private void SetupVideoCapture()
        {
            if (CameraToUse == 0) VideoCapture = new VideoCapture(CameraToUse);
            else if (CameraToUse == 1) VideoCapture = new VideoCapture(CameraToUse + CaptureType.DShow); // Need DShow backend for Logitech Webcam

            //VideoCapture.SetCaptureProperty(CapProp.Fps, FPS);
            VideoCapture.SetCaptureProperty(CapProp.FrameWidth, DisplayedCameraWidth);
            VideoCapture.SetCaptureProperty(CapProp.FrameHeight, DisplayedCameraHeight);
            VideoCapture.SetCaptureProperty(CapProp.Autofocus, 0);
            //VideoCapture.SetCaptureProperty(CapProp.AutoExposure, 0);
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

                foreach (var pair in Colors) {
                    var color = pair.Key;
                    var config = pair.Value;
                    if (config.IsEnabled) {
                        Mat curThresh = DetectColor(videoFrame, color);
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

        private Mat DetectColor(Mat inputImage, Color color)
        {
            var curColor = Colors[color];
            Mat ThreshImage_Temp = GetColorThreshImage_Temp(color);

            CvInvoke.CvtColor(inputImage, HsvImage_Temp, ColorConversion.Bgr2Hsv);

            // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
            CvInvoke.InRange(HsvImage_Temp, curColor.MinHsvRange, curColor.MaxHsvRange, ThreshImage_Temp);

            // Get contours of thresh image
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                //Mat hierarchy = new Mat(); Use this if need hierarchy parameter in FindContours
                CvInvoke.FindContours(ThreshImage_Temp, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                
                // Find largest contour
                double maxArea = 0;
                int maxContourIndex = -1;
                for (int i = 0; i < contours.Size; i++) {
                    using (VectorOfPoint contour = contours[i]) {
                        double area = CvInvoke.ContourArea(contour);
                        if (area > maxArea) {
                            maxArea = area;
                            maxContourIndex = i;
                        }
                    }
                }

                // If at least one contour was found and its area is at least 300 pixels
                if (maxContourIndex >= 0 && maxArea >= MinAreaToDetect) {
                    // Draw the contour in white
                    CvInvoke.DrawContours(inputImage, contours, maxContourIndex, new MCvScalar(255, 255, 255), 2);

                    // Get the contour center of mass
                    MCvMoments moments = CvInvoke.Moments(contours[maxContourIndex]);
                    Point contourCenter = new Point(
                        (int)(moments.M10 / moments.M00),
                        (int)(moments.M01 / moments.M00));

                    // Draw the contour center on video feed
                    CvInvoke.Circle(inputImage, contourCenter, 5, new MCvScalar(255, 255, 255), 2);

                    // Draw on canvas using contour center
                    contourCenter = ScaleToCanvas(contourCenter);
                    int width = GetWidthBySpeed(LastPosition[color], contourCenter);
                    DrawLineTo(contourCenter, curColor.InkColor, LastPosition[color], width);

                    UpdateColorLastPosition(color, contourCenter.X, contourCenter.Y);
                }
                else {
                    UpdateColorLastPosition(color, 0, 0);
                }

                /*
                // Find largest contour
                double maxRating = 0;
                var contourCenter = new Point(0, 0);
                int maxContourIndex = -1;
                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    {
                        double area = CvInvoke.ContourArea(contour);
                        // as long as area is large enough, we check if it is a better choice
                        if (area > MinAreaToDetect) {
                            // Get the contour center of mass
                            MCvMoments moments = CvInvoke.Moments(contour);
                            var center = new Point(
                                (int)(moments.M10 / moments.M00),
                                (int)(moments.M01 / moments.M00));
                            // we negate, so that more irregular shapes are negative, and so lower rating
                            var rating = -contour.IrregularityRating(contourCenter);
                            if (rating > maxRating) {
                                maxContourIndex = i;
                                contourCenter = center;
                            }
                        }
                    }
                }
                
                if (maxContourIndex >= 0) ...
                */
            }
            
            return ThreshImage_Temp;
        }

        private Mat GetColorThreshImage_Temp(Color color)
        {
            switch (color)
            {
                case Color.Blue:
                    return BlueThreshImage_Temp;

                case Color.Green:
                    return GreenThreshImage_Temp;

                case Color.Yellow:
                    return YellowThreshImage_Temp;

                case Color.Orange:
                    return OrangeThreshImage_Temp;

                case Color.Purple:
                    return PurpleThreshImage_Temp;

                case Color.Red:
                    return RedThreshImage_Temp;

                case Color.Special:
                    return SpecialThreshImage_Temp;

                default:
                    return null;
            }
        }

        private void UpdateColorLastPosition(Color color, int x, int y)
        {
            var lastPos = LastPosition[color];
            lastPos.X = x;
            lastPos.Y = y;
            LastPosition[color] = lastPos;
        }

        private void DrawLineTo(Point point, MCvScalar color, Point thisColorLastPosition, int strokeWidth)
        {
            if (strokeWidth <= 0) strokeWidth = 1;
            if (thisColorLastPosition.X != 0 && thisColorLastPosition.Y != 0)
            {
                CvInvoke.Line(ImageBox_Drawing.Image, thisColorLastPosition, point, color, strokeWidth, LineType.AntiAlias);
                ImageBox_Drawing.Refresh();
            }
        }

        private Color GetSelectedColor() {
            var name = ColorPicker.Text;
            return (Color)Enum.Parse(typeof(Color), ColorPicker.Text);
        }

        private int GetWidthBySpeed(Point colorLastPosition, Point colorDestination)
        {
            int dx = colorDestination.X - colorLastPosition.X;
            int dy = colorDestination.Y - colorLastPosition.Y;

            double travelDistance = Math.Sqrt(dx * dx + dy * dy);
            double speed = travelDistance / (1000 / FPS); // Speed as a ratio of pixels/frame length in ms
            double maxAssumedSpeed = 2; // found this number through testing...
            double speedRatio = speed / maxAssumedSpeed;

            if (speedRatio > 1.0) speedRatio = 1.0;
            int maxWidth = 25;
            int minWidth = 3;
            int strokeWidth = (int)Math.Ceiling(speedRatio * maxWidth);


            // To flip to wider when slower:
            //strokeWidth = maxWidth - strokeWidth;

            if (strokeWidth > maxWidth) strokeWidth = maxWidth;
            if (strokeWidth < minWidth) strokeWidth = minWidth;
            return strokeWidth;
        }

        private Point ScaleToCanvas(Point point)
        {
            double widthMultiplier = (CanvasWidth * 1.0) / DisplayedCameraWidth;
            double heightMultiplier = (CanvasHeight * 1.0) / DisplayedCameraHeight;

            point.X = (int)(point.X * widthMultiplier);
            point.Y = (int)(point.Y * heightMultiplier);

            return point;
        }

        private IImage GetGrayImage(Mat color_image)
        {
            Image<Gray, byte> grayImage = new Image<Gray, byte>(color_image.Bitmap);

            return grayImage;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            ReleaseResources();
        }

        private void ReleaseResources() {
            if (Disposables != null) {
                foreach (IDisposable disposable in Disposables) {
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

                UpdateColorLastPosition(Color.Blue, 0, 0);
                UpdateColorLastPosition(Color.Green, 0, 0);
                UpdateColorLastPosition(Color.Yellow, 0, 0);
                UpdateColorLastPosition(Color.Orange, 0, 0);
                UpdateColorLastPosition(Color.Purple, 0, 0);
                UpdateColorLastPosition(Color.Red, 0, 0);
                UpdateColorLastPosition(Color.Special, 0, 0);
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
            ImageBox_Drawing.Image = new Image<Bgr, byte>(CanvasWidth, CanvasHeight, new Bgr(255, 255, 255));
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            tableLayoutPanel_Sliders.Visible = !tableLayoutPanel_Sliders.Visible;
        }

        private void ColorPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckBox_IsMin.Checked = true;
            UpdateSliderValues(sender, e);

            var color = GetSelectedColor();
            CheckBox_ColorOn.Checked = Colors[color].IsEnabled;
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
            var color = GetSelectedColor();
            var config = Colors[color];
            config.IsEnabled = CheckBox_ColorOn.Checked;
            Colors[color] = config;
        }

        private void UpdateSliderValues(object sender, EventArgs e)
        {
            double[] hsvValues = null;

            var color = GetSelectedColor();
            var config = Colors[color];
            if (CheckBox_IsMin.Checked)
                hsvValues = config.MinHsv.ToArray();
            else
                hsvValues = config.MaxHsv.ToArray();
            
            if (hsvValues != null)
            {
                HSlider.Value = (int)hsvValues[0];
                SSlider.Value = (int)hsvValues[1];
                VSlider.Value = (int)hsvValues[2];
            }
        }

        private void UpdateHSVCodes()
        {
            var color = GetSelectedColor();
            var config = Colors[color];
            if (CheckBox_IsMin.Checked)
                config.MinHsv = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
            else
                config.MaxHsv = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
            Colors[color] = config;
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

        public void SaveHsvToFile(string path) {
            File.WriteAllText(path, JsonConvert.SerializeObject(Colors));
        }

        public void LoadHsvFromFile(string path) {
            var ser = JsonConvert.DeserializeObject<Dictionary<Color, HsvConfig>>(File.ReadAllText(path));

            Action<Color, HsvConfig> updateColor = (color, config) => {
                Action<MCvScalar> setSliders = scalar => {
                    HSlider.Value = (int)scalar.V0;
                    SSlider.Value = (int)scalar.V1;
                    VSlider.Value = (int)scalar.V2;
                };
                ColorPicker.Text = color.ToString();

                CheckBox_ColorOn.Checked = config.IsEnabled;
                CheckBox_IsMin.Checked = true;
                setSliders(config.MinHsv);
                CheckBox_IsMin.Checked = false;
                setSliders(config.MaxHsv);
            };
            // for all availale colors, we update them
            foreach (var pair in ser)
                updateColor(pair.Key, pair.Value);
            Colors = ser;
            ColorPicker.Text = Color.Red.ToString();
        }
    }
}
