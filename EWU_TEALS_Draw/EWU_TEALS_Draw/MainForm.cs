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
using System.IO;

namespace EWU_TEALS_Draw
{
    public partial class MainForm : Form
    {
        private VideoCapture VideoCapture;
        public CascadeClassifier CascadeClassifier = null;
        
        private const int ImageWidth = 1920 / 4;
        private const int ImageHeight = 1080 / 4;
        private const int CameraToUse = 0; // Default Camera: 0

        private List<IDisposable> Disposables;


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

            CascadeClassifier = new CascadeClassifier(@"../../HaarCascades/Hand.xml");
            //CascadeClassifier = new CascadeClassifier(@"../../HaarCascades/Hand_haar_cascade.xml");
            //CascadeClassifier = new CascadeClassifier(@"../../HaarCascades/haarcascade_profileface.xml");
            //CascadeClassifier = new CascadeClassifier(@"../../HaarCascades/haarcascade_frontalface_alt.xml");
            Disposables.Add(CascadeClassifier);

            ImageBox_VideoCapture_Gray.Image = new Image<Bgr, byte>(ImageWidth, ImageHeight, new Bgr(255, 255, 255));
            ImageBox_Drawing.Image = new Image<Bgr, byte>(1920/2, 1080/2, new Bgr(255, 255, 255));
            Disposables.Add(ImageBox_VideoCapture_Gray.Image);
            Disposables.Add(ImageBox_Drawing.Image);
            Disposables.Add(ImageBox_VideoCapture.Image);

            SetupVideoCapture();
            
            Application.Idle += ProcessFrame;
        }

        private void SetupVideoCapture()
        {
            VideoCapture = new VideoCapture(CameraToUse);
            Disposables.Add(VideoCapture);

            VideoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, 30); // These things don't actually work...
            VideoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, ImageHeight);
            VideoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, ImageWidth);
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (VideoCapture != null)
            {
                //using (ImageBox_VideoCapture.Image = VideoCapture.QueryFrame())
                //{
                    ImageBox_VideoCapture.Image = VideoCapture.QueryFrame();

                    //MCvScalar color = GetColor();
                    //List<Point> points = GetPoints();
                    //DrawLine(points[0], points[1], color);

                    DetectHand();
                //}
            }
        }

        private void DetectHand()
        {
            var color_image = new Image<Bgr, byte>(ImageBox_VideoCapture.Image.Bitmap);
            var gray_image = GetGrayImage(color_image);

            Rectangle[] hands = CascadeClassifier.DetectMultiScale(gray_image);
            Point handCenter = new Point();
            foreach (Rectangle hand in hands)
            {
                CvInvoke.Rectangle(gray_image, hand, new MCvScalar(240, 140, 0), 2);
                handCenter.X = hand.X + (hand.Width / 2);
                handCenter.Y = hand.Y + (hand.Height / 2);
                CvInvoke.Circle(gray_image, handCenter, 4, new MCvScalar(255,255,255),2);

                // Draw center on drawing
                handCenter.X = handCenter.X * 2; // scale to drawing size
                handCenter.Y = handCenter.Y * 2;
                CvInvoke.Circle(ImageBox_Drawing.Image, handCenter, 4, new MCvScalar(240,140,0), 2);
            }

            ImageBox_VideoCapture_Gray.Image = gray_image;
            ImageBox_Drawing.Refresh();
        }

        private IImage GetGrayImage(Image<Bgr, byte> color_image)
        {
            var image = new Image<Gray, byte>(color_image.Bitmap);

            return image;
        }

        private void DrawLine(Point point1, Point point2, MCvScalar color)
        {   
            CvInvoke.Line(ImageBox_VideoCapture_Gray.Image, point1, point2, color, 10, Emgu.CV.CvEnum.LineType.AntiAlias);
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
