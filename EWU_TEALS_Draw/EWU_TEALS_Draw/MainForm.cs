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

namespace EWU_TEALS_Draw
{
    public partial class MainForm : Form
    {
        private List<IDisposable> Disposables;
        private VideoCapture VideoCapture;
        private bool IsRunning;

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
        // Red Low Threshold
        private bool RedLowOn;
        private IInputArray RedHsvMin_LowRange = new ScalarArray(new MCvScalar(0, 50, 100));
        private IInputArray RedHsvMax_LowRange = new ScalarArray(new MCvScalar(5, 255, 255));
        // Red High Threshold
        private bool RedOn = true;
        private static MCvScalar RedHsvMin_HighRange_Editable = new MCvScalar(157, 105, 115);
        private static MCvScalar RedHsvMax_HighRange_Editable = new MCvScalar(180, 255, 255);
        private IInputArray RedHsvMin_HighRange = new ScalarArray(RedHsvMin_HighRange_Editable);
        private IInputArray RedHsvMax_HighRange = new ScalarArray(RedHsvMax_HighRange_Editable);
        private MCvScalar RedDrawColor = new MCvScalar(60, 60, 230);
        private Point RedLastPosition;

        // Orange
        private bool OrangeOn = true;
        private static MCvScalar OrangeHsvMin_Editable = new MCvScalar(10, 175, 65);
        private static MCvScalar OrangeHsvMax_Editable = new MCvScalar(18, 255, 255);
        private IInputArray OrangeHsvMin = new ScalarArray(OrangeHsvMin_Editable);
        private IInputArray OrangeHsvMax = new ScalarArray(OrangeHsvMax_Editable);
        private MCvScalar OrangeDrawColor = new MCvScalar(60, 140, 255);
        private Point OrangeLastPosition;

        // Yellow
        private bool YellowOn = true;
        private static MCvScalar YellowHsvMin_Editable = new MCvScalar(25, 50, 120);
        private static MCvScalar YellowHsvMax_Editable = new MCvScalar(35, 255, 255);
        private IInputArray YellowHsvMin = new ScalarArray(YellowHsvMin_Editable);
        private IInputArray YellowHsvMax = new ScalarArray(YellowHsvMax_Editable);
        private MCvScalar YellowDrawColor = new MCvScalar(100, 240, 240);
        private Point YellowLastPosition;

        // Green
        private bool GreenOn = true;
        private static MCvScalar GreenHsvMin_Editable = new MCvScalar(45, 35, 66);
        private static MCvScalar GreenHsvMax_Editable = new MCvScalar(95, 255, 255);
        private IInputArray GreenHsvMin = new ScalarArray(GreenHsvMin_Editable);
        private IInputArray GreenHsvMax = new ScalarArray(GreenHsvMax_Editable);
        private MCvScalar GreenDrawColor = new MCvScalar(135, 230, 135);
        private Point GreenLastPosition;

        // Blue
        private bool BlueOn = true;
        private static MCvScalar BlueHsvMin_Editable = new MCvScalar(95, 109, 65);
        private static MCvScalar BlueHsvMax_Editable = new MCvScalar(117, 255, 255);
        private IInputArray BlueHsvMin = new ScalarArray(BlueHsvMin_Editable);
        private IInputArray BlueHsvMax = new ScalarArray(BlueHsvMax_Editable);
        private MCvScalar BlueDrawColor = new MCvScalar(255, 140, 85);
        private Point BlueLastPosition;

        // Purple
        private bool PurpleOn = true;
        private static MCvScalar PurpleHsvMin_Editable = new MCvScalar(125, 100, 100);
        private static MCvScalar PurpleHsvMax_Editable = new MCvScalar(140, 255, 255);
        private IInputArray PurpleHsvMin = new ScalarArray(PurpleHsvMin_Editable);
        private IInputArray PurpleHsvMax = new ScalarArray(PurpleHsvMax_Editable);
        private MCvScalar PurpleDrawColor = new MCvScalar(255, 135, 135);
        private Point PurpleLastPosition;

        // Special Color
        private bool SpecialOn = false;
        private static MCvScalar SpecialHsvMin_Editable = new MCvScalar(0, 0, 0);
        private static MCvScalar SpecialHsvMax_Editable = new MCvScalar(180, 255, 255);
        private IInputArray SpecialHsvMin = new ScalarArray(SpecialHsvMin_Editable);
        private IInputArray SpecialHsvMax = new ScalarArray(SpecialHsvMax_Editable);
        private MCvScalar SpecialDrawColor = new MCvScalar(0,0,0);
        private Point SpecialLastPosition;
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

                if (RedOn)
                {
                    Mat redThreshImage_High = DetectColor(videoFrame, RedHsvMin_HighRange, RedHsvMax_HighRange, RedDrawColor, RedLastPosition, "Red");
                    CvInvoke.Add(redThreshImage_High, combinedThreshImage, combinedThreshImage);
                }
                if (RedLowOn)
                {
                    Mat redThreshImage_Low = DetectColor(videoFrame, RedHsvMin_LowRange, RedHsvMax_LowRange, RedDrawColor, RedLastPosition, "Red");
                    CvInvoke.Add(redThreshImage_Low, combinedThreshImage, combinedThreshImage);
                }
                if (YellowOn)
                {
                    Mat yellowThreshImage = DetectColor(videoFrame, YellowHsvMin, YellowHsvMax, YellowDrawColor, YellowLastPosition, "Yellow");
                    CvInvoke.Add(yellowThreshImage, combinedThreshImage, combinedThreshImage);
                }
                if (GreenOn)
                {
                    Mat greenThreshImage = DetectColor(videoFrame, GreenHsvMin, GreenHsvMax, GreenDrawColor, GreenLastPosition, "Green");
                    CvInvoke.Add(greenThreshImage, combinedThreshImage, combinedThreshImage);
                }
                if (BlueOn)
                {
                    Mat blueThreshImage = DetectColor(videoFrame, BlueHsvMin, BlueHsvMax, BlueDrawColor, BlueLastPosition, "Blue");
                    CvInvoke.Add(blueThreshImage, combinedThreshImage, combinedThreshImage);
                }
                if (OrangeOn)
                {
                    Mat orangeThreshImage = DetectColor(videoFrame, OrangeHsvMin, OrangeHsvMax, OrangeDrawColor, OrangeLastPosition, "Orange");
                    CvInvoke.Add(orangeThreshImage, combinedThreshImage, combinedThreshImage);
                }
                if (PurpleOn)
                {
                    Mat purpleThreshImage = DetectColor(videoFrame, PurpleHsvMin, PurpleHsvMax, PurpleDrawColor, PurpleLastPosition, "Purple");
                    CvInvoke.Add(purpleThreshImage, combinedThreshImage, combinedThreshImage);
                }
                if (SpecialOn)
                {
                    Mat specialThreshImage = DetectColor(videoFrame, SpecialHsvMin, SpecialHsvMax, SpecialDrawColor, SpecialLastPosition, "Special");
                    CvInvoke.Add(specialThreshImage, combinedThreshImage, combinedThreshImage);
                }

                ImageBox_VideoCapture_Gray.Image = combinedThreshImage;

                if (DisposableQueue.Count > 8)
                {
                    DisposableQueue.Dequeue().Dispose();
                    DisposableQueue.Dequeue().Dispose();
                }
            }
        }

        private Mat DetectColor(Mat inputImage, IInputArray hsvThreshMin, IInputArray hsvThreshMax, MCvScalar drawColor, Point thisColorLastPosition, string color)
        {
            Mat ThreshImage_Temp = GetColorThreshImage_Temp(color);

            CvInvoke.CvtColor(inputImage, HsvImage_Temp, ColorConversion.Bgr2Hsv);

            // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
            CvInvoke.InRange(HsvImage_Temp, hsvThreshMin, hsvThreshMax, ThreshImage_Temp);

            // Get contours of thresh image
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                //Mat hierarchy = new Mat(); Use this if need hierarchy parameter in FindContours
                CvInvoke.FindContours(ThreshImage_Temp, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                // Find largest contour
                double maxArea = 0;
                int maxContourIndex = -1;
                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    {
                        double area = CvInvoke.ContourArea(contour);
                        if (area > maxArea)
                        {
                            maxArea = area;
                            maxContourIndex = i;
                        }
                    }
                }

                // If at least one contour was found and its area is at least 300 pixels
                if (maxContourIndex >= 0 && maxArea >= 600)
                {
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
                    int width = GetWidthBySpeed(thisColorLastPosition, contourCenter);
                    DrawLineTo(contourCenter, drawColor, thisColorLastPosition, width);

                    UpdateColorLastPosition(color, contourCenter.X, contourCenter.Y);
                }
                else
                {
                    UpdateColorLastPosition(color, 0, 0);
                }
            }
            
            return ThreshImage_Temp;
        }

        private Mat GetColorThreshImage_Temp(string color)
        {
            switch (color)
            {
                case "Blue":
                    return BlueThreshImage_Temp;

                case "Green":
                    return GreenThreshImage_Temp;

                case "Yellow":
                    return YellowThreshImage_Temp;

                case "Orange":
                    return OrangeThreshImage_Temp;

                case "Purple":
                    return PurpleThreshImage_Temp;

                case "Red":
                    return RedThreshImage_Temp;

                case "Special":
                    return SpecialThreshImage_Temp;

                default:
                    return null;
            }
        }

        private void UpdateColorLastPosition(string color, int x, int y)
        {
            switch (color)
            {
                case "Blue":
                    BlueLastPosition.X = x;
                    BlueLastPosition.Y = y;
                    break;

                case "Green":
                    GreenLastPosition.X = x;
                    GreenLastPosition.Y = y;
                    break;

                case "Yellow":
                    YellowLastPosition.X = x;
                    YellowLastPosition.Y = y;
                    break;

                case "Orange":
                    OrangeLastPosition.X = x;
                    OrangeLastPosition.Y = y;
                    break;

                case "Purple":
                    PurpleLastPosition.X = x;
                    PurpleLastPosition.Y = y;
                    break;

                case "Red":
                    RedLastPosition.X = x;
                    RedLastPosition.Y = y;
                    break;

                case "Special":
                    SpecialLastPosition.X = x;
                    SpecialLastPosition.Y = y;
                    break;
            }
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

                UpdateColorLastPosition("Blue", 0, 0);
                UpdateColorLastPosition("Green", 0, 0);
                UpdateColorLastPosition("Yellow", 0, 0);
                UpdateColorLastPosition("Orange", 0, 0);
                UpdateColorLastPosition("Purple", 0, 0);
                UpdateColorLastPosition("Red", 0, 0);
                UpdateColorLastPosition("Special", 0, 0);
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

        private void ColorPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckBox_IsMin.Checked = true;
            UpdateSliderValues(sender, e);

            if (ColorPicker.Text == "Red")
            {
                CheckBox_ColorOn.Checked = RedOn;
            }
            else if (ColorPicker.Text == "Blue")
            {
                CheckBox_ColorOn.Checked = BlueOn;
            }
            else if (ColorPicker.Text == "Green")
            {
                CheckBox_ColorOn.Checked = GreenOn;
            }
            else if (ColorPicker.Text == "Yellow")
            {
                CheckBox_ColorOn.Checked = YellowOn;
            }
            else if (ColorPicker.Text == "Orange")
            {
                CheckBox_ColorOn.Checked = OrangeOn;
            }
            else if (ColorPicker.Text == "Purple")
            {
                CheckBox_ColorOn.Checked = PurpleOn;
            }
            else if (ColorPicker.Text == "Special")
            {
                CheckBox_ColorOn.Checked = SpecialOn;
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
            if (ColorPicker.Text == "Red")
            {
                RedOn = CheckBox_ColorOn.Checked;
            }
            else if (ColorPicker.Text == "Blue")
            {
                BlueOn = CheckBox_ColorOn.Checked;
            }
            else if (ColorPicker.Text == "Green")
            {
                GreenOn = CheckBox_ColorOn.Checked;
            }
            else if (ColorPicker.Text == "Yellow")
            {
                YellowOn = CheckBox_ColorOn.Checked;
            }
            else if (ColorPicker.Text == "Orange")
            {
                OrangeOn = CheckBox_ColorOn.Checked;
            }
            else if (ColorPicker.Text == "Purple")
            {
                PurpleOn = CheckBox_ColorOn.Checked;
            }
            else if (ColorPicker.Text == "Special")
            {
                SpecialOn = CheckBox_ColorOn.Checked;
            }
        }

        private void UpdateSliderValues(object sender, EventArgs e)
        {
            double[] hsvValues = null;
            
            if (ColorPicker.Text == "Red")
            {
                if (CheckBox_IsMin.Checked) hsvValues = RedHsvMin_HighRange_Editable.ToArray();
                else hsvValues = RedHsvMax_HighRange_Editable.ToArray();
            }
            else if (ColorPicker.Text == "Blue")
            {
                if (CheckBox_IsMin.Checked) hsvValues = BlueHsvMin_Editable.ToArray();
                else hsvValues = BlueHsvMax_Editable.ToArray();
            }
            else if (ColorPicker.Text == "Green")
            {
                if (CheckBox_IsMin.Checked) hsvValues = GreenHsvMin_Editable.ToArray();
                else hsvValues = GreenHsvMax_Editable.ToArray();
            }
            else if (ColorPicker.Text == "Yellow")
            {
                if (CheckBox_IsMin.Checked) hsvValues = YellowHsvMin_Editable.ToArray();
                else hsvValues = YellowHsvMax_Editable.ToArray();
            }
            else if (ColorPicker.Text == "Orange")
            {
                if (CheckBox_IsMin.Checked) hsvValues = OrangeHsvMin_Editable.ToArray();
                else hsvValues = OrangeHsvMax_Editable.ToArray();
            }
            else if (ColorPicker.Text == "Purple")
            {
                if (CheckBox_IsMin.Checked) hsvValues = PurpleHsvMin_Editable.ToArray();
                else hsvValues = PurpleHsvMax_Editable.ToArray();
            }
            else if (ColorPicker.Text == "Special")
            {
                if (CheckBox_IsMin.Checked) hsvValues = SpecialHsvMin_Editable.ToArray();
                else hsvValues = SpecialHsvMax_Editable.ToArray();
            }

            if (hsvValues != null)
            {
                HSlider.Value = (int)hsvValues[0];
                SSlider.Value = (int)hsvValues[1];
                VSlider.Value = (int)hsvValues[2];
            }
        }

        private void UpdateHSVCodes()
        {
            if (ColorPicker.Text == "Red")
            {
                if (CheckBox_IsMin.Checked)
                {
                    RedHsvMin_HighRange_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    RedHsvMin_HighRange = new ScalarArray(RedHsvMin_HighRange_Editable);
                }
                else
                {
                    RedHsvMax_HighRange_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    RedHsvMax_HighRange = new ScalarArray(RedHsvMax_HighRange_Editable);
                }
            }
            else if (ColorPicker.Text == "Blue")
            {
                if (CheckBox_IsMin.Checked)
                {
                    BlueHsvMin_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    BlueHsvMin = new ScalarArray(BlueHsvMin_Editable);
                }
                else
                {
                    BlueHsvMax_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    BlueHsvMax = new ScalarArray(BlueHsvMax_Editable);
                }
            }
            else if (ColorPicker.Text == "Green")
            {
                if (CheckBox_IsMin.Checked)
                {
                    GreenHsvMin_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    GreenHsvMin = new ScalarArray(GreenHsvMin_Editable);
                }
                else
                {
                    GreenHsvMax_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    GreenHsvMax = new ScalarArray(GreenHsvMax_Editable);
                }
            }
            else if (ColorPicker.Text == "Yellow")
            {
                if (CheckBox_IsMin.Checked)
                {
                    YellowHsvMin_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    YellowHsvMin = new ScalarArray(YellowHsvMin_Editable);
                }
                else
                {
                    YellowHsvMax_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    YellowHsvMax = new ScalarArray(YellowHsvMax_Editable);
                }
            }
            else if (ColorPicker.Text == "Orange")
            {
                if (CheckBox_IsMin.Checked)
                {
                    OrangeHsvMin_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    OrangeHsvMin = new ScalarArray(OrangeHsvMin_Editable);
                }
                else
                {
                    OrangeHsvMax_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    OrangeHsvMax = new ScalarArray(OrangeHsvMax_Editable);
                }
            }
            else if (ColorPicker.Text == "Purple")
            {
                if (CheckBox_IsMin.Checked)
                {
                    PurpleHsvMin_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    PurpleHsvMin = new ScalarArray(PurpleHsvMin_Editable);
                }
                else
                {
                    PurpleHsvMax_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    PurpleHsvMax = new ScalarArray(PurpleHsvMax_Editable);
                }
            }
            else if (ColorPicker.Text == "Special")
            {
                if (CheckBox_IsMin.Checked)
                {
                    SpecialHsvMin_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    SpecialHsvMin = new ScalarArray(SpecialHsvMin_Editable);
                }
                else
                {
                    SpecialHsvMax_Editable = new MCvScalar(HSlider.Value, SSlider.Value, VSlider.Value);
                    SpecialHsvMax = new ScalarArray(SpecialHsvMax_Editable);
                }
            }
        }
    }
}
