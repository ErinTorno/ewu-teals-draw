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
        private const int ActualCameraWidth = 1280;
        private const int ActualCameraHeight = 720;

        private const int DisplayedCameraWidth = ActualCameraWidth / 4;
        private const int DisplayedCameraHeight = ActualCameraHeight / 4;

        private const int CanvasWidth = DisplayedCameraWidth * 3;
        private const int CanvasHeight = DisplayedCameraHeight * 3;
        #endregion

        #region Color Threshold Properties
        private IInputArray BlueHsvMin = new ScalarArray(new MCvScalar(100, 150, 100)); // Blue min
        private IInputArray BlueHsvMax = new ScalarArray(new MCvScalar(135, 255, 255)); // Blue max
        private MCvScalar BlueDrawColor = new MCvScalar(255, 135, 135); // Blue draw
        private Point BlueLastPosition;

        private IInputArray GreenHsvMin = new ScalarArray(new MCvScalar(65, 75, 100)); // Green min
        private IInputArray GreenHsvMax = new ScalarArray(new MCvScalar(85, 255, 255)); // Green max
        private MCvScalar GreenDrawColor = new MCvScalar(135, 230, 135); // Green draw
        private Point GreenLastPosition;
        #endregion

        public MainForm()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;

            Startup();
        }

        private void Startup()
        {
            SetupVideoCapture();
            ImageBox_VideoCapture_Gray.Image = new Image<Bgr, byte>(DisplayedCameraWidth, DisplayedCameraHeight, new Bgr(255, 255, 255));
            ImageBox_Drawing.Image = new Image<Bgr, byte>(CanvasWidth, CanvasHeight, new Bgr(255, 255, 255));
            
            Application.Idle += ProcessFrame;
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

                Mat combinedImage = new Mat();
                CvInvoke.Add(blueThreshImage, greenThreshImage, combinedImage);
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

                switch (color)
                {
                    case "Blue":
                        BlueLastPosition.X = avgPoint.X;
                        BlueLastPosition.Y = avgPoint.Y;
                        break;

                    case "Green":
                        GreenLastPosition.X = avgPoint.X;
                        GreenLastPosition.Y = avgPoint.Y;
                        break;
                }
            }
            // If not enough pixels to count as an object, reset lastColorPosition to 0 so it will be 
            // updated when we do find it.
            else
            {
                switch (color)
                {
                    case "Blue":
                        BlueLastPosition.X = 0;
                        BlueLastPosition.Y = 0;
                        break;

                    case "Green":
                        GreenLastPosition.X = 0;
                        GreenLastPosition.Y = 0;
                        break;
                }
            }

            return threshImage;
        }

        private MCvScalar GetColorBySpeed(Point canvasScaledPoint)
        {
            MCvScalar color = new MCvScalar(255, 255, 255);

            if (BlueLastPosition.X != 0 && BlueLastPosition.Y != 0) // We are moving
            {
                int maxIntensity = 255;

                int dx = canvasScaledPoint.X - BlueLastPosition.X;
                int dy = canvasScaledPoint.Y - BlueLastPosition.Y;
                double travelDistance = Math.Sqrt(dx * dx + dy * dy);

                double speed = travelDistance / (1000 / FPS); // Speed as a ratio of pixels/ms
                int colorAllotment = (int)(speed * (maxIntensity * 3));

                int r = 0;
                int g = 0;
                int b = 0;
                if (colorAllotment <= (1 * maxIntensity)) // r:0, g:0, b:+
                {
                    b = colorAllotment;
                }

                else if (colorAllotment > (1 * maxIntensity) && colorAllotment <= (2 * maxIntensity)) // r:0, g:+, b:max
                {
                    b = maxIntensity;
                    g = colorAllotment - maxIntensity;

                }

                else if (colorAllotment > (2 * maxIntensity) && colorAllotment <= (3 * maxIntensity)) // r:0, g:max, b:-
                {
                    g = maxIntensity;
                    b = maxIntensity - (colorAllotment - maxIntensity);
                }

                else if (colorAllotment > (3 * maxIntensity) && colorAllotment <= (4 * maxIntensity)) // r:+, g:max, b:0
                {
                    g = maxIntensity;
                    r = colorAllotment - maxIntensity;
                }

                else if (colorAllotment > (4 * maxIntensity) && colorAllotment <= (5 * maxIntensity)) // r:max, g:-, b:0
                {
                    r = maxIntensity;
                    g = maxIntensity - (colorAllotment - maxIntensity);
                }

                else if (colorAllotment > 5)
                {
                    r = maxIntensity;
                }

                color.V0 = b;
                color.V1 = g;
                color.V2 = r;
            }

            return color;
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

        private void DrawLineTo(Point point, MCvScalar color, Point thisColorLastPosition, int strokeWidth)
        {
            if (strokeWidth <= 0) strokeWidth = 1;
            if (thisColorLastPosition.X != 0 && thisColorLastPosition.Y != 0)
            {
                CvInvoke.Line(ImageBox_Drawing.Image, thisColorLastPosition, point, color, strokeWidth, LineType.AntiAlias);
                ImageBox_Drawing.Refresh();
            }
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
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Application.Idle -= ProcessFrame;
            ImageBox_VideoCapture_Gray.Image = new Image<Bgr, byte>(DisplayedCameraWidth, DisplayedCameraHeight, new Bgr(255, 255, 255));
            BlueLastPosition.X = 0;
            BlueLastPosition.Y = 0;
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
