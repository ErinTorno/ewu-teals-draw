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
using Emgu.CV.Structure;
using System.IO;

namespace EWU_TEALS_Draw
{
    public partial class MainForm : Form
    {
        private VideoCapture VideoCapture;
        public CascadeClassifier HaarCascade = null;

        
        private const int ImageWidth = 1920 / 4;
        private const int ImageHeight = 1080 / 4;
        private const int CameraToUse = 0; // Default Camera: 0

        private List<IDisposable> Disposables;

        public MainForm()
        {
            InitializeComponent();

            Startup();
        }
        
        private void TestCode()
        {
            Emgu.CV.Image<Bgr, Byte> image = new Image<Bgr, byte>(500, 500);
            
        }

        private void Startup()
        {
            Disposables = new List<IDisposable>();

            //HaarCascade = new CascadeClassifier(@"../../HaarCascades/HandHaarCascade.xml");
            HaarCascade = new CascadeClassifier(@"../../HaarCascades/Hand.xml");
            //HaarCascade = new CascadeClassifier(@"../../HaarCascades/Handy.xml");
            Disposables.Add(HaarCascade);
            

            SetupVideoCapture();

            Application.Idle += ProcessFrame;
        }

        private void SetupVideoCapture()
        {
            VideoCapture = new VideoCapture(CameraToUse);
            Disposables.Add(VideoCapture);

            VideoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, 30);
            VideoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, ImageHeight);
            VideoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, ImageWidth);
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (VideoCapture != null)
            {
                ImageBox_VideoCapture.Image = VideoCapture.QueryFrame();

                ImageBox_Drawing.Image = new Image<Bgr, byte>(ImageWidth,ImageHeight, new Bgr(255,255,255));
                
                MCvScalar color = GetColor();
                List<Point> points = GetPoints();

                DrawLine(points[0], points[1], color);

                //DrawContours();
                OutlineHand();
            }
        }

        private void DrawContours()
        {
            Point[] points1 = new Point[] { new Point(10, 10), new Point(150, 150) };
            Point[] points2 = new Point[] { new Point(150, 150 ), new Point(10, 150) };

            Emgu.CV.Util.VectorOfPoint vector1 = new Emgu.CV.Util.VectorOfPoint(points1);
            Emgu.CV.Util.VectorOfPoint vector2 = new Emgu.CV.Util.VectorOfPoint(points2);

            Emgu.CV.Util.VectorOfPoint[] vectors = new Emgu.CV.Util.VectorOfPoint[2];
            vectors[0] = vector1;

            //CvInvoke.DrawContours(ImageBox_VideoCapture.Image, vector1, -1, new MCvScalar(), 3);
        }

        private void OutlineHand()
        {
            Image<Bgr, byte> imageFrame = new Image<Bgr, byte>(ImageBox_VideoCapture.Image.Bitmap);
            Image<Gray, byte> grayFrame = imageFrame.Convert<Gray, byte>();

            Rectangle[] rectangles = HaarCascade.DetectMultiScale(grayFrame);

            foreach (Rectangle rectangle in rectangles)
            {
                CvInvoke.Rectangle(ImageBox_VideoCapture.Image, rectangle, new MCvScalar(240, 140, 0), 3, Emgu.CV.CvEnum.LineType.FourConnected, 0);
            }
        }

        private void DrawLine(Point point1, Point point2, MCvScalar color)
        {
            CvInvoke.Line(ImageBox_Drawing.Image, point1, point2, color, 10, Emgu.CV.CvEnum.LineType.AntiAlias, 0);
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
                    disposable.Dispose();
                }

                Disposables = null;
            }
        }
    }
}
