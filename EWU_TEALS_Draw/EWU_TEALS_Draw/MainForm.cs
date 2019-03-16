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

        #region Color Threshold Properties
        // Blue
        private IInputArray BlueHsvMin = new ScalarArray(new MCvScalar(105, 125, 65)); // Blue min
        private IInputArray BlueHsvMax = new ScalarArray(new MCvScalar(117, 255, 255)); // Blue max
        private MCvScalar BlueDrawColor = new MCvScalar(255, 140, 85); // Blue draw color
        private Point BlueLastPosition;

        // Green
        private IInputArray GreenHsvMin = new ScalarArray(new MCvScalar(60, 100, 20));
        private IInputArray GreenHsvMax = new ScalarArray(new MCvScalar(90, 255, 255));
        private MCvScalar GreenDrawColor = new MCvScalar(135, 230, 135);
        private Point GreenLastPosition;

        // Yellow
        private IInputArray YellowHsvMin = new ScalarArray(new MCvScalar(20, 50, 150));
        private IInputArray YellowHsvMax = new ScalarArray(new MCvScalar(35, 255, 255));
        private MCvScalar YellowDrawColor = new MCvScalar(100, 240, 240);
        private Point YellowLastPosition;

        // Orange
        private IInputArray OrangeHsvMin = new ScalarArray(new MCvScalar(12, 175, 65));
        private IInputArray OrangeHsvMax = new ScalarArray(new MCvScalar(18, 255, 255));
        private MCvScalar OrangeDrawColor = new MCvScalar(60, 140, 255);
        private Point OrangeLastPosition;

        // Purple
        private IInputArray PurpleHsvMin = new ScalarArray(new MCvScalar(125, 100, 100));
        private IInputArray PurpleHsvMax = new ScalarArray(new MCvScalar(140, 255, 255));
        private MCvScalar PurpleDrawColor = new MCvScalar(255, 135, 135);
        private Point PurpleLastPosition;

        // Red Lower Threshold
        private IInputArray RedHsvMin_Low = new ScalarArray(new MCvScalar(0, 50, 100));
        private IInputArray RedHsvMax_Low = new ScalarArray(new MCvScalar(5, 255, 255));
        // Red Upper Threshold
        private IInputArray RedHsvMin_High = new ScalarArray(new MCvScalar(170, 175, 45));
        private IInputArray RedHsvMax_High = new ScalarArray(new MCvScalar(180, 255, 255));

        private MCvScalar RedDrawColor = new MCvScalar(60, 60, 230);
        private Point RedLastPosition;
        #endregion

        #region Re-used Objects for saving memory
        private Mat HsvImage_Temp = new Mat();
        public Mat BlueThreshImage_Temp = new Mat();
        public Mat GreenThreshImage_Temp = new Mat();
        public Mat YellowThreshImage_Temp = new Mat();
        public Mat OrangeThreshImage_Temp = new Mat();
        public Mat PurpleThreshImage_Temp = new Mat();
        public Mat RedThreshImage_Temp = new Mat();
        private Mat CombinedThreshImage = new Mat();
        private Point AveragePoint = new Point();
        private Mat Points;
        Queue<Mat> VideoFrames = new Queue<Mat>();
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
            Points = new Mat(VideoCapture.QueryFrame().Size, DepthType.Cv8U, 1);

            Application.Idle += ProcessFrame;
            btnPlay.Enabled = false;
        }

        private void SetupVideoCapture()
        {
            VideoCapture = new VideoCapture(CameraToUse); // Webcam is 1280x720

            VideoCapture.SetCaptureProperty(CapProp.Fps, FPS); // The FPS property doesn't actually work...
            VideoCapture.SetCaptureProperty(CapProp.FrameWidth, DisplayedCameraWidth);  // 1280, 640, 320
            VideoCapture.SetCaptureProperty(CapProp.FrameHeight, DisplayedCameraHeight); // 720, 360, 180
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (VideoCapture != null)
            {
                Mat videoFrame = VideoCapture.QueryFrame(); // If not managed, video frame causes .2mb/s Loss, does not get cleaned up by GC. Must manually dispose.
                CvInvoke.Flip(videoFrame, videoFrame, FlipType.Horizontal);
                ImageBox_VideoCapture.Image = videoFrame;
                VideoFrames.Enqueue(videoFrame); // Add Video Frames to a queue to be disposed when NOT in use

                Mat blueThreshImage = DetectColor(videoFrame, BlueHsvMin, BlueHsvMax, BlueDrawColor, BlueLastPosition, "Blue");
                Mat greenThreshImage = DetectColor(videoFrame, GreenHsvMin, GreenHsvMax, GreenDrawColor, GreenLastPosition, "Green");
                Mat yellowThreshImage = DetectColor(videoFrame, YellowHsvMin, YellowHsvMax, YellowDrawColor, YellowLastPosition, "Yellow");
                Mat orangeThreshImage = DetectColor(videoFrame, OrangeHsvMin, OrangeHsvMax, OrangeDrawColor, OrangeLastPosition, "Orange");
                Mat purpleThreshImage = DetectColor(videoFrame, PurpleHsvMin, PurpleHsvMax, PurpleDrawColor, PurpleLastPosition, "Purple");
                Mat redThreshImage_Low = DetectColor(videoFrame, RedHsvMin_Low, RedHsvMax_Low, RedDrawColor, RedLastPosition, "Red");
                Mat redThreshImage_High = DetectColor(videoFrame, RedHsvMin_High, RedHsvMax_High, RedDrawColor, RedLastPosition, "Red");

                CvInvoke.Add(blueThreshImage, greenThreshImage, CombinedThreshImage);
                CvInvoke.Add(yellowThreshImage, CombinedThreshImage, CombinedThreshImage);
                CvInvoke.Add(orangeThreshImage, CombinedThreshImage, CombinedThreshImage);
                CvInvoke.Add(purpleThreshImage, CombinedThreshImage, CombinedThreshImage);
                CvInvoke.Add(redThreshImage_Low, CombinedThreshImage, CombinedThreshImage);
                CvInvoke.Add(redThreshImage_High, CombinedThreshImage, CombinedThreshImage);

                ImageBox_VideoCapture_Gray.Image = CombinedThreshImage;
                
                if (VideoFrames.Count > 5)
                {
                    VideoFrames.Dequeue().Dispose();
                }
            }
        }

        private Mat DetectColor(Mat inputImage, IInputArray hsvThreshMin, IInputArray hsvThreshMax, MCvScalar drawColor, Point thisColorLastPosition, string color)
        {
            Mat ThreshImage_Temp = GetColorThreshImage_Temp(color);

            CvInvoke.CvtColor(inputImage, HsvImage_Temp, ColorConversion.Bgr2Hsv);

            // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
            CvInvoke.InRange(HsvImage_Temp, hsvThreshMin, hsvThreshMax, ThreshImage_Temp);

            // Find average of white pixels
            //Mat Points = new Mat(inputImage.Size, DepthType.Cv8U, 1);
            CvInvoke.FindNonZero(ThreshImage_Temp, Points);

            // An alternative approach to averaging would be to use the K-means 
            // algorithm to find clusters, since average is significantly influenced by outliers.
            MCvScalar avg = CvInvoke.Mean(Points);
            AveragePoint.X = (int)avg.V0;
            AveragePoint.Y = (int)avg.V1;

            int sumWhitePixels = CvInvoke.CountNonZero(ThreshImage_Temp);

            // Now we check if there are more than x pixels of detected color, since we don't want to draw
            // if all we detect is noise.
            if (sumWhitePixels > 300)
            {
                // Draw circle on camera feed
                CvInvoke.Circle(inputImage, AveragePoint, 5, drawColor, 2);

                // Draw on canvas
                AveragePoint = ScaleToCanvas(AveragePoint);

                int width = GetWidthBySpeed(thisColorLastPosition, AveragePoint);
                DrawLineTo(AveragePoint, drawColor, thisColorLastPosition, width);

                UpdateColorLastPosition(color, AveragePoint.X, AveragePoint.Y);
            }
            // If not enough pixels to count as an object, reset lastColorPosition to 0 so it will be 
            // updated when we do find it.
            else
            {
                UpdateColorLastPosition(color, 0, 0);
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
            double maxAssumedSpeed = 1; // found this number through testing...
            double speedRatio = speed / maxAssumedSpeed;

            if (speedRatio > 1.0) speedRatio = 1.0;
            int maxWidth = 25;
            int strokeWidth = (int)Math.Round(speedRatio * maxWidth);


            // To flip to wider when slower:
            //strokeWidth = maxWidth - strokeWidth;

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
            var image = new Image<Gray, byte>(color_image.Bitmap);

            return image;
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

            if (VideoFrames != null)
            {
                foreach (IDisposable disposable in VideoFrames)
                {
                    if (disposable != null) disposable.Dispose();
                }
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Application.Idle += ProcessFrame;

            btnPlay.Enabled = false;
            btnPause.Enabled = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Application.Idle -= ProcessFrame;
            btnPlay.Enabled = true;
            btnPause.Enabled = false;

            UpdateColorLastPosition("Blue", 0, 0);
            UpdateColorLastPosition("Green", 0, 0);
            UpdateColorLastPosition("Yellow", 0, 0);
            UpdateColorLastPosition("Orange", 0, 0);
            UpdateColorLastPosition("Purple", 0, 0);
            UpdateColorLastPosition("Red", 0, 0);
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
    }
}
