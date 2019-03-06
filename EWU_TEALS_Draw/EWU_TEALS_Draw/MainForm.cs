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
        public CascadeClassifier CascadeClassifier = null;
        private Point LastHandPosition;
        private Drawing drawing = new Drawing(1280, 720);


        // Resolution Properties
        private const int CameraToUse = 0; // Default Camera: 0
        private const int FPS = 30;
        private const int ActualCameraWidth = 1280;
        private const int ActualCameraHeight = 720;

        private const int DisplayedCameraWidth = ActualCameraWidth / 4;
        private const int DisplayedCameraHeight = ActualCameraHeight / 4;

        private const int CanvasWidth = DisplayedCameraWidth * 2;
        private const int CanvasHeight = DisplayedCameraHeight * 2;


        

        // Thresholding Properties
        private IInputArray HsvThreshMin = new ScalarArray(new MCvScalar(100, 150, 100)); // Blue min
        private IInputArray HsvThreshMax = new ScalarArray(new MCvScalar(135, 255, 255)); // Blue max
        private Mat Hsv_image;
        private Mat ThreshLow_image;
        private Mat ThreshHigh_image;
        private Mat Thresh_image;

        
        // TimeSlicing Properties
        private int LastTime = DateTime.Now.Millisecond;
        private int Now;
        private DateTime NowDateTime;
        private DateTime LastDateTime = DateTime.Now;


        public MainForm()
        {
            InitializeComponent();

            Startup();
        }

        private void Startup()
        {
            Disposables = new List<IDisposable>();

            //CascadeClassifier = new CascadeClassifier(@"../../HaarCascades/haarcascade_upperbody.xml");
            //CascadeClassifier = new CascadeClassifier(@"../../HaarCascades/haarcascade_eye.xml");
            CascadeClassifier = new CascadeClassifier(@"../../HaarCascades/haarcascade_lefteye_2splits.xml");

            Disposables.Add(CascadeClassifier);

            ImageBox_VideoCapture_Gray.Image = new Image<Bgr, byte>(DisplayedCameraWidth, DisplayedCameraHeight, new Bgr(255, 255, 255));

            ImageBox_Drawing.Image = new Image<Bgr, byte>(CanvasWidth, CanvasHeight, new Bgr(255, 255, 255));
            
            
            Disposables.Add(ImageBox_VideoCapture_Gray.Image);
            Disposables.Add(ImageBox_Drawing.Image);
            Disposables.Add(ImageBox_VideoCapture.Image);

            SetupVideoCapture();



            Mat color_image = VideoCapture.QueryFrame();
            Hsv_image = new Mat(color_image.Size, DepthType.Cv8U, 3);
            ThreshLow_image = new Mat(color_image.Size, DepthType.Cv8U, 1);
            ThreshHigh_image = new Mat(color_image.Size, DepthType.Cv8U, 1);
            Thresh_image = new Mat(color_image.Size, DepthType.Cv8U, 1);


            Application.Idle += ProcessFrame;
        }

        private void SetupVideoCapture()
        {
            VideoCapture = new VideoCapture(CameraToUse); // Webcam is 1280x720
            Disposables.Add(VideoCapture);

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
                

                DetectColor(flippedVideoFrame);
                //DetectHand(flippedVideoFrame);   
            }
        }

        private Mat FlipImage(Mat inputImage)
        {
            Mat flippedImage = new Mat(inputImage.Size, DepthType.Cv8U, inputImage.NumberOfChannels);
            CvInvoke.Flip(inputImage, flippedImage, FlipType.Horizontal);
            return flippedImage;
        }

        private void DetectColor(Mat inputImage)
        {
            CvInvoke.CvtColor(inputImage, Hsv_image, ColorConversion.Bgr2Hsv);
            
            // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
            CvInvoke.InRange(Hsv_image, HsvThreshMin, HsvThreshMax, Thresh_image);

            // Find average of white pixels
            Mat points = new Mat(inputImage.Size, DepthType.Cv8U, 1);
            CvInvoke.FindNonZero(Thresh_image, points);

            // An alternative approach to averaging would be to use the K-means 
            // algorithm to find clusters, since average is significantly influenced by outliers.
            MCvScalar avg = CvInvoke.Mean(points);
            Point avgPoint = new Point((int)avg.V0, (int)avg.V1);

            int sumWhitePixels = CvInvoke.CountNonZero(Thresh_image);

            // Now we check if there are more than x pixels of detected color, since we don't want to draw
            // if all we detect is noise.
            if (sumWhitePixels > 100)
            {
                // Draw circle on camera feed
                CvInvoke.Circle(inputImage, avgPoint, 5, new MCvScalar(0, 10, 220), 2);

                // Draw on canvas
                avgPoint = ScaleToCanvas(avgPoint);
                MCvScalar color = GetColorBySpeed(avgPoint);
                DrawLineTo(avgPoint, color);
            }
            // If not enough pixels to count as an object, reset LastHandPosition to 0 so it will be 
            // updated when we do find it.
            else
            {
                LastHandPosition.X = 0;
                LastHandPosition.Y = 0;
            }

            ImageBox_VideoCapture_Gray.Image = Thresh_image;
        }

        private MCvScalar GetColorBySpeed(Point canvasScaledPoint)
        {
            MCvScalar color = new MCvScalar(255, 255, 255);

            if (LastHandPosition.X != 0 && LastHandPosition.Y != 0) // We are moving
            {
                int maxIntensity = 255;

                int dx = canvasScaledPoint.X - LastHandPosition.X;
                int dy = canvasScaledPoint.Y - LastHandPosition.Y;
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

        private Point ScaleToCanvas(Point point)
        {
            double widthMultiplier = (CanvasWidth * 1.0) / DisplayedCameraWidth;
            double heightMultiplier = (CanvasHeight * 1.0) / DisplayedCameraHeight;

            point.X = (int)(point.X * widthMultiplier);
            point.Y = (int)(point.Y * heightMultiplier);

            return point;
        }

        private void DrawLineTo(Point point, MCvScalar color)
        {
            if (LastHandPosition.X != 0 && LastHandPosition.Y != 0)
            {
                CvInvoke.Line(ImageBox_Drawing.Image, LastHandPosition, point, color, 8, LineType.AntiAlias);
                ImageBox_Drawing.Refresh();
            }

            LastHandPosition = new Point(point.X, point.Y);
        }

        private void DetectHand(Mat inputImage)
        {
            //color_image._EqualizeHist();
            //color_image._GammaCorrect(1.2d);
            var gray_image = GetGrayImage(inputImage);

            Rectangle[] hands = CascadeClassifier.DetectMultiScale(gray_image);
            Point handCenter = new Point();
            if (hands.Length > 0)
            {
                foreach (Rectangle hand in hands)
                {
                    CvInvoke.Rectangle(gray_image, hand, new MCvScalar(240, 140, 0), 2);
                    handCenter.X = hand.X + (hand.Width / 2);
                    handCenter.Y = hand.Y + (hand.Height / 2);
                    //CvInvoke.Circle(gray_image, handCenter, 4, new MCvScalar(255,255,255),2);

                    // Draw center on drawing
                    //drawing.AddPoint(ImageBox_VideoCapture, handCenter.X, handCenter.Y); // Draw with Drawing.cs

                    //handCenter.X = ImageBox_Drawing.Width - handCenter.X * 2; // scale to drawing size
                    //handCenter.Y = handCenter.Y * 2;
                    //CvInvoke.Circle(ImageBox_Drawing.Image, handCenter, 4, new MCvScalar(240,140,0), 2);

                    // Draw without Drawing.cs:
                    handCenter = ScaleToCanvas(handCenter);
                    MCvScalar color = new MCvScalar(240, 140, 0);
                    DrawPoint(handCenter, color);
                }
            }
            else if (hands.Length == 0)
            {
                LastHandPosition.X = 0;
                LastHandPosition.Y = 0;
            }
            

            drawing.Update(ImageBox_Drawing.Image);
            ImageBox_VideoCapture_Gray.Image = gray_image;
            ImageBox_Drawing.Refresh();
            ImageBox_VideoCapture.Image = inputImage;
        }

        private void DrawPoint(Point point, MCvScalar color)
        {
            if (LastHandPosition.X != 0 && LastHandPosition.Y != 0)
            {
                CvInvoke.Circle(ImageBox_Drawing.Image, point, 5, color, -1); // -1 indicates filled circle
                ImageBox_Drawing.Refresh();
            }

            LastHandPosition = new Point(point.X, point.Y);
        }

        private IImage GetGrayImage(Mat color_image)
        {
            var image = new Image<Gray, byte>(color_image.Bitmap);

            return image;
        }

        private void DrawLine(Point point1, Point point2, MCvScalar color)
        {   
            CvInvoke.Line(ImageBox_VideoCapture_Gray.Image, point1, point2, color, 10, LineType.AntiAlias);
            ImageBox_VideoCapture_Gray.Refresh();
        }

        private MCvScalar GetColor()
        {
            // Logic to determine what color to draw with...

            // MCvScalar is a Color object, that takes rgb values but in the order of bgr !!!!!!
            return new MCvScalar(125, 125, 200);
        }

        private List<Point> GetPoints()
        {
            // Logic to determine the points from hand movement...

            List<Point> points = new List<Point>();
            points.Add(new Point(50, 50));
            points.Add(new Point(100, 300));
            return points;
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

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Application.Idle += ProcessFrame;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Application.Idle -= ProcessFrame;
            ImageBox_VideoCapture_Gray.Image = new Image<Bgr, byte>(DisplayedCameraWidth, DisplayedCameraHeight, new Bgr(255, 255, 255));
            LastHandPosition.X = 0;
            LastHandPosition.Y = 0;

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

        /*
        private bool NextTimeSlice()
        {
            double FPS = 30;
            // 30FPS has a frame length of 1s/ 30 * 1000 ms
            int frameLength = (int)((1.0 / FPS) * 1000);

            NowDateTime = DateTime.Now;

            TimeSpan timeSpan = NowDateTime - LastDateTime;
            int msPassed = timeSpan.Seconds * 1000 + timeSpan.Milliseconds;

            if (msPassed < frameLength) return false;
            else
            {
                LastDateTime = DateTime.Now;
                //MessageBox.Show("NextTimeSlice");
                return true;
            }
        }
        */
        /*
        private bool NextTimeSlice()
        {
            Now = DateTime.Now.Millisecond;

            double FPS = 30;
            // 30FPS has a frame length of 1s/ 30 * 1000 ms
            int frameLength = (int)((1.0/FPS) * 1000);

            int msPassed;
            if (Now > LastTime)
            {
                msPassed = Now - LastTime;
            }
            else
            {
                msPassed = 1000 - LastTime + Now;
            }
            



            if (msPassed < frameLength) return false;
            else
            {
                LastTime = Now;
                //MessageBox.Show("NextTimeSlice");
                return true;
            }

        }
        */

        //
        /*
        // pre-using
        private void ProcessFrame(object sender, EventArgs e)
        {
            if (VideoCapture != null)
            {
                ImageBox_VideoCapture.Image = VideoCapture.QueryFrame();
                
                MCvScalar color = GetColor();
                List<Point> points = GetPoints();

                DrawLine(points[0], points[1], color);

                //DrawContours();
                //OutlineHand();

                ImageBox_VideoCapture.Image.Dispose();
            }
        }
        */
    }
}
