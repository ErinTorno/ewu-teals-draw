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

        private const int DisplayedCameraWidth = ActualCameraWidth / 8;
        private const int DisplayedCameraHeight = ActualCameraHeight / 8;

        private int CanvasWidth = (int)Math.Floor(DisplayedCameraWidth * 4.7);
        private int CanvasHeight = (int)Math.Floor(DisplayedCameraHeight * 4.7);
        #endregion

        #region Color Threshold Properties
        // Blue
        private IInputArray BlueHsvMin = new ScalarArray(new MCvScalar(100, 150, 65)); // Blue min
        private IInputArray BlueHsvMax = new ScalarArray(new MCvScalar(117, 255, 255)); // Blue max
        private MCvScalar BlueDrawColor = new MCvScalar(255, 140, 85); // Blue draw color
        private Point BlueLastPosition;

        // Green
        private IInputArray GreenHsvMin = new ScalarArray(new MCvScalar(50, 100, 50));
        private IInputArray GreenHsvMax = new ScalarArray(new MCvScalar(80, 255, 255));
        private MCvScalar GreenDrawColor = new MCvScalar(135, 230, 135);
        private Point GreenLastPosition;

        // Yellow
        private IInputArray YellowHsvMin = new ScalarArray(new MCvScalar(25, 150, 65));
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
        private IInputArray RedHsvMin_Low = new ScalarArray(new MCvScalar(0, 175, 45));
        private IInputArray RedHsvMax_Low = new ScalarArray(new MCvScalar(5, 255, 255));
        // Red Upper Threshold
        private IInputArray RedHsvMin_High = new ScalarArray(new MCvScalar(170, 175, 45));
        private IInputArray RedHsvMax_High = new ScalarArray(new MCvScalar(180, 255, 255));

        private MCvScalar RedDrawColor = new MCvScalar(60, 60, 230);
        private Point RedLastPosition;
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
            ImageBox_VideoCapture_Gray.Image = new Image<Bgr, byte>(DisplayedCameraWidth, DisplayedCameraHeight, new Bgr(255, 255, 255));
            ImageBox_Drawing.Image = new Image<Bgr, byte>(CanvasWidth, CanvasHeight, new Bgr(255, 255, 255));
            
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
                Mat flippedVideoFrame = FlipImage(VideoCapture.QueryFrame());
                ImageBox_VideoCapture.Image = flippedVideoFrame;

                Mat blueThreshImage = DetectColor(flippedVideoFrame, BlueHsvMin, BlueHsvMax, BlueDrawColor, BlueLastPosition, "Blue");
                Mat greenThreshImage = DetectColor(flippedVideoFrame, GreenHsvMin, GreenHsvMax, GreenDrawColor, GreenLastPosition, "Green");
                Mat yellowThreshImage = DetectColor(flippedVideoFrame, YellowHsvMin, YellowHsvMax, YellowDrawColor, YellowLastPosition, "Yellow");
                Mat orangeThreshImage = DetectColor(flippedVideoFrame, OrangeHsvMin, OrangeHsvMax, OrangeDrawColor, OrangeLastPosition, "Orange");
                Mat purpleThreshImage = DetectColor(flippedVideoFrame, PurpleHsvMin, PurpleHsvMax, PurpleDrawColor, PurpleLastPosition, "Purple");
                Mat redThreshImage_Low = DetectColor(flippedVideoFrame, RedHsvMin_Low, RedHsvMax_Low, RedDrawColor, RedLastPosition, "Red");
                Mat redThreshImage_High = DetectColor(flippedVideoFrame, RedHsvMin_High, RedHsvMax_High, RedDrawColor, RedLastPosition, "Red");

                Mat combinedImage = new Mat();
                CvInvoke.Add(blueThreshImage, greenThreshImage, combinedImage);
                CvInvoke.Add(yellowThreshImage, combinedImage, combinedImage);
                CvInvoke.Add(orangeThreshImage, combinedImage, combinedImage);
                CvInvoke.Add(purpleThreshImage, combinedImage, combinedImage);
                CvInvoke.Add(redThreshImage_Low, combinedImage, combinedImage);
                CvInvoke.Add(redThreshImage_High, combinedImage, combinedImage);

                ImageBox_VideoCapture_Gray.Image = combinedImage;
            }
        }

        private Mat DetectColor(Mat inputImage, IInputArray hsvThreshMin, IInputArray hsvThreshMax, MCvScalar drawColor, Point thisColorLastPosition, string color)
        {
            Mat hsvImage = new Mat();
            Mat threshImage = new Mat();

            CvInvoke.CvtColor(inputImage, hsvImage, ColorConversion.Bgr2Hsv);

            // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
            CvInvoke.InRange(hsvImage, hsvThreshMin, hsvThreshMax, threshImage);

            // Find average of white pixels
            Mat points = new Mat(inputImage.Size, DepthType.Cv8U, 1);
            CvInvoke.FindNonZero(threshImage, points);

            // An alternative approach to averaging would be to use the K-means 
            // algorithm to find clusters, since average is significantly influenced by outliers.
            MCvScalar avg = CvInvoke.Mean(points);
            Point avgPoint = new Point((int)avg.V0, (int)avg.V1);

            int sumWhitePixels = CvInvoke.CountNonZero(threshImage);

            // Now we check if there are more than x pixels of detected color, since we don't want to draw
            // if all we detect is noise.
            if (sumWhitePixels > 100)
            {
                // Draw circle on camera feed
                CvInvoke.Circle(inputImage, avgPoint, 5, drawColor, 2);

                // Draw on canvas
                avgPoint = ScaleToCanvas(avgPoint);

                int width = GetWidthBySpeed(thisColorLastPosition, avgPoint);
                DrawLineTo(avgPoint, drawColor, thisColorLastPosition, width);

                UpdateColorLastPosition(color, avgPoint);
            }
            // If not enough pixels to count as an object, reset lastColorPosition to 0 so it will be 
            // updated when we do find it.
            else
            {
                UpdateColorLastPosition(color, new Point(0, 0));
            }

            return threshImage;
        }

        private void UpdateColorLastPosition(string color, Point currentPosition)
        {
            switch (color)
            {
                case "Blue":
                    BlueLastPosition.X = currentPosition.X;
                    BlueLastPosition.Y = currentPosition.Y;
                    break;

                case "Green":
                    GreenLastPosition.X = currentPosition.X;
                    GreenLastPosition.Y = currentPosition.Y;
                    break;

                case "Yellow":
                    YellowLastPosition.X = currentPosition.X;
                    YellowLastPosition.Y = currentPosition.Y;
                    break;

                case "Orange":
                    OrangeLastPosition.X = currentPosition.X;
                    OrangeLastPosition.Y = currentPosition.Y;
                    break;

                case "Purple":
                    PurpleLastPosition.X = currentPosition.X;
                    PurpleLastPosition.Y = currentPosition.Y;
                    break;

                case "Red":
                    RedLastPosition.X = currentPosition.X;
                    RedLastPosition.Y = currentPosition.Y;
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
            strokeWidth = maxWidth - strokeWidth;

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
        }

        private Mat FlipImage(Mat inputImage)
        {
            Mat flippedImage = new Mat(inputImage.Size, DepthType.Cv8U, inputImage.NumberOfChannels);
            CvInvoke.Flip(inputImage, flippedImage, FlipType.Horizontal);
            return flippedImage;
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

            Point point = new Point(0, 0);
            UpdateColorLastPosition("Blue", point);
            UpdateColorLastPosition("Green", point);
            UpdateColorLastPosition("Yellow", point);
            UpdateColorLastPosition("Orange", point);
            UpdateColorLastPosition("Purple", point);
            UpdateColorLastPosition("Red", point);
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
