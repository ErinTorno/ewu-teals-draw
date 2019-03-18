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

namespace EWU_TEALS_Draw {
    public enum Color { Blue, Green, Yellow, Orange, Purple, Red }

    public partial class MainForm : Form {
        private List<IDisposable> Disposables;
        private VideoCapture VideoCapture;

        #region Resolution Properties
        private const int CameraToUse = 0; // Default Camera: 0
        private const int FPS = 30;
        private const int ActualCameraWidth = 1920;
        private const int ActualCameraHeight = 1080;

        private const int DisplayedCameraWidth = ActualCameraWidth / 6;
        private const int DisplayedCameraHeight = ActualCameraHeight / 6;

        private int CanvasWidth = (int)Math.Floor(DisplayedCameraWidth * 3.77);
        private int CanvasHeight = (int)Math.Floor(DisplayedCameraHeight * 3.77);
        #endregion

        #region KeyPress Properties
        private const Keys KeyReset = Keys.R;
        private const Keys KeyExit = Keys.Q;
        #endregion

        #region Color Threshold Properties
        // Red Low Threshold
        private IInputArray RedHsvMin_Low = new ScalarArray(new MCvScalar(0, 50, 100));
        private IInputArray RedHsvMax_Low = new ScalarArray(new MCvScalar(5, 255, 255));
        // Red High Threshold
        private IInputArray RedHsvMin_High = new ScalarArray(new MCvScalar(157, 105, 115));
        private IInputArray RedHsvMax_High = new ScalarArray(new MCvScalar(180, 255, 255));
        private MCvScalar RedDrawColor = new MCvScalar(60, 60, 230);
        private Point RedLastPosition;

        // Orange
        private IInputArray OrangeHsvMin = new ScalarArray(new MCvScalar(10, 175, 65));
        private IInputArray OrangeHsvMax = new ScalarArray(new MCvScalar(18, 255, 255));
        private MCvScalar OrangeDrawColor = new MCvScalar(60, 140, 255);
        private Point OrangeLastPosition;

        // Yellow
        private IInputArray YellowHsvMin = new ScalarArray(new MCvScalar(25, 50, 120));
        private IInputArray YellowHsvMax = new ScalarArray(new MCvScalar(35, 255, 255));
        private MCvScalar YellowDrawColor = new MCvScalar(100, 240, 240);
        private Point YellowLastPosition;

        // Green
        private IInputArray GreenHsvMin = new ScalarArray(new MCvScalar(85, 200, 70));
        private IInputArray GreenHsvMax = new ScalarArray(new MCvScalar(95, 255, 255));
        private MCvScalar GreenDrawColor = new MCvScalar(135, 230, 135);
        private Point GreenLastPosition;

        // Blue
        private IInputArray BlueHsvMin = new ScalarArray(new MCvScalar(95, 170, 110));
        private IInputArray BlueHsvMax = new ScalarArray(new MCvScalar(117, 255, 255));
        private MCvScalar BlueDrawColor = new MCvScalar(255, 140, 85);
        private Point BlueLastPosition;

        // Purple
        private IInputArray PurpleHsvMin = new ScalarArray(new MCvScalar(125, 100, 100));
        private IInputArray PurpleHsvMax = new ScalarArray(new MCvScalar(140, 255, 255));
        private MCvScalar PurpleDrawColor = new MCvScalar(255, 135, 135);
        private Point PurpleLastPosition;
        #endregion

        #region Re-used Objects for saving memory
        private Mat HsvImage_Temp = new Mat();
        private Mat RedThreshImage_Temp = new Mat();
        private Mat OrangeThreshImage_Temp = new Mat();
        private Mat YellowThreshImage_Temp = new Mat();
        private Mat GreenThreshImage_Temp = new Mat();
        private Mat BlueThreshImage_Temp = new Mat();
        private Mat PurpleThreshImage_Temp = new Mat();

        Queue<Mat> DisposableQueue = new Queue<Mat>();
        #endregion

        public MainForm() {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            Startup();
        }

        private void Startup() {
            SetupVideoCapture();
            ImageBox_Drawing.Image = new Image<Bgr, byte>(CanvasWidth, CanvasHeight, new Bgr(255, 255, 255));

            Application.Idle += ProcessFrame;
            btnPlay.Enabled = false;

            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void SetupVideoCapture() {
            if (CameraToUse == 0) VideoCapture = new VideoCapture(CameraToUse);
            else if (CameraToUse == 1) VideoCapture = new VideoCapture(CameraToUse + CaptureType.DShow); // Need DShow backend for Logitech Webcam

            VideoCapture.SetCaptureProperty(CapProp.Fps, FPS);
            VideoCapture.SetCaptureProperty(CapProp.FrameWidth, DisplayedCameraWidth);
            VideoCapture.SetCaptureProperty(CapProp.FrameHeight, DisplayedCameraHeight);
            VideoCapture.SetCaptureProperty(CapProp.Autofocus, 0);
            //VideoCapture.SetCaptureProperty(CapProp.AutoExposure, 0);
            //VideoCapture.SetCaptureProperty(CapProp.Contrast, 20);
        }

        private void ProcessFrame(object sender, EventArgs e) {
            if (VideoCapture != null) {
                Mat videoFrame = VideoCapture.QueryFrame(); // If not managed, video frame causes .2mb/s Loss, does not get cleaned up by GC. Must manually dispose.
                CvInvoke.Flip(videoFrame, videoFrame, FlipType.Horizontal);
                ImageBox_VideoCapture.Image = videoFrame;
                DisposableQueue.Enqueue(videoFrame); // Add Video Frames to a queue to be disposed when NOT in use

                //Mat redThreshImage_Low = DetectColor(videoFrame, RedHsvMin_Low, RedHsvMax_Low, RedDrawColor, RedLastPosition, Color.Red);
                Mat redThreshImage_High = DetectColor(videoFrame, RedHsvMin_High, RedHsvMax_High, RedDrawColor, RedLastPosition, Color.Red);
                //Mat orangeThreshImage = DetectColor(videoFrame, OrangeHsvMin, OrangeHsvMax, OrangeDrawColor, OrangeLastPosition, Color.Orange);
                Mat yellowThreshImage = DetectColor(videoFrame, YellowHsvMin, YellowHsvMax, YellowDrawColor, YellowLastPosition, Color.Yellow);
                Mat greenThreshImage = DetectColor(videoFrame, GreenHsvMin, GreenHsvMax, GreenDrawColor, GreenLastPosition, Color.Green);
                Mat blueThreshImage = DetectColor(videoFrame, BlueHsvMin, BlueHsvMax, BlueDrawColor, BlueLastPosition, Color.Blue);
                //Mat purpleThreshImage = DetectColor(videoFrame, PurpleHsvMin, PurpleHsvMax, PurpleDrawColor, PurpleLastPosition, Color.Purple);

                Mat combinedThreshImage = Mat.Zeros(videoFrame.Rows, videoFrame.Cols, DepthType.Cv8U, 1);
                DisposableQueue.Enqueue(combinedThreshImage);
                //CvInvoke.Add(redThreshImage_Low, combinedThreshImage, combinedThreshImage);
                CvInvoke.Add(redThreshImage_High, combinedThreshImage, combinedThreshImage);
                //CvInvoke.Add(orangeThreshImage, combinedThreshImage, combinedThreshImage);
                CvInvoke.Add(yellowThreshImage, combinedThreshImage, combinedThreshImage);
                CvInvoke.Add(greenThreshImage, combinedThreshImage, combinedThreshImage);
                CvInvoke.Add(blueThreshImage, combinedThreshImage, combinedThreshImage);
                //CvInvoke.Add(purpleThreshImage, combinedThreshImage, combinedThreshImage);

                ImageBox_VideoCapture_Gray.Image = combinedThreshImage;

                if (DisposableQueue.Count > 8) {
                    DisposableQueue.Dequeue().Dispose();
                    DisposableQueue.Dequeue().Dispose();
                }
            }
        }

        private Mat DetectColor(Mat inputImage, IInputArray hsvThreshMin, IInputArray hsvThreshMax, MCvScalar drawColor, Point thisColorLastPosition, Color color) {
            Mat ThreshImage_Temp = GetColorThreshImage_Temp(color);

            CvInvoke.CvtColor(inputImage, HsvImage_Temp, ColorConversion.Bgr2Hsv);

            // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
            CvInvoke.InRange(HsvImage_Temp, hsvThreshMin, hsvThreshMax, ThreshImage_Temp);

            // Get contours of thresh image
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint()) {
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
                if (maxContourIndex >= 0 && maxArea >= 1000) {
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
                else {
                    UpdateColorLastPosition(color, 0, 0);
                }
            }

            return ThreshImage_Temp;
        }

        private Mat GetColorThreshImage_Temp(Color color) {
            switch (color) {
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
                default:
                    return null;
            }
        }

        private void UpdateColorLastPosition(Color color, int x, int y) {
            switch (color) {
                case Color.Blue:
                    BlueLastPosition.X = x;
                    BlueLastPosition.Y = y;
                    break;

                case Color.Green:
                    GreenLastPosition.X = x;
                    GreenLastPosition.Y = y;
                    break;

                case Color.Yellow:
                    YellowLastPosition.X = x;
                    YellowLastPosition.Y = y;
                    break;

                case Color.Orange:
                    OrangeLastPosition.X = x;
                    OrangeLastPosition.Y = y;
                    break;

                case Color.Purple:
                    PurpleLastPosition.X = x;
                    PurpleLastPosition.Y = y;
                    break;

                case Color.Red:
                    RedLastPosition.X = x;
                    RedLastPosition.Y = y;
                    break;
            }
        }

        private void DrawLineTo(Point point, MCvScalar color, Point thisColorLastPosition, int strokeWidth) {
            if (strokeWidth <= 0) strokeWidth = 1;
            if (thisColorLastPosition.X != 0 && thisColorLastPosition.Y != 0) {
                CvInvoke.Line(ImageBox_Drawing.Image, thisColorLastPosition, point, color, strokeWidth, LineType.AntiAlias);
                ImageBox_Drawing.Refresh();
            }
        }

        private int GetWidthBySpeed(Point colorLastPosition, Point colorDestination) {
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

        private Point ScaleToCanvas(Point point) {
            double widthMultiplier = (CanvasWidth * 1.0) / DisplayedCameraWidth;
            double heightMultiplier = (CanvasHeight * 1.0) / DisplayedCameraHeight;

            point.X = (int)(point.X * widthMultiplier);
            point.Y = (int)(point.Y * heightMultiplier);

            return point;
        }

        private IImage GetGrayImage(Mat color_image) {
            var image = new Image<Gray, byte>(color_image.Bitmap);

            return image;
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

            if (DisposableQueue != null) {
                foreach (IDisposable disposable in DisposableQueue) {
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
            }
        }

        private void btnPlay_Click(object sender, EventArgs e) {
            Application.Idle += ProcessFrame;

            btnPlay.Enabled = false;
            btnPause.Enabled = true;
        }

        private void btnPause_Click(object sender, EventArgs e) {
            Application.Idle -= ProcessFrame;
            btnPlay.Enabled = true;
            btnPause.Enabled = false;

            UpdateColorLastPosition(Color.Blue, 0, 0);
            UpdateColorLastPosition(Color.Green, 0, 0);
            UpdateColorLastPosition(Color.Yellow, 0, 0);
            UpdateColorLastPosition(Color.Orange, 0, 0);
            UpdateColorLastPosition(Color.Purple, 0, 0);
            UpdateColorLastPosition(Color.Red, 0, 0);
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
    }
}
