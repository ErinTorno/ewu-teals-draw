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

namespace EWU_TEALS_Draw
{
    public partial class MainForm : Form
    {
        private VideoCapture VideoCapture;
        public CascadeClassifier HaarCascade = null;

        private const int CameraToUse = 0; // Default Camera: 0

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
            SetupVideoCapture();

            Application.Idle += ProcessFrame;
        }

        private void SetupVideoCapture()
        {
            VideoCapture = new VideoCapture(CameraToUse);
            // HaarCascade = new CascadeClassifier("Downloads\\hand.xml");

            VideoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, 30);
            VideoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 1080/4);
            VideoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 1920/4);
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (VideoCapture != null)
            {
                ImageBox_VideoCapture.Image = VideoCapture.QueryFrame();
                ImageBox_VideoCapture.Show();

                ImageBox_Drawing.Image = new Image<Bgr, byte>(1920/4, 1080/4, new Bgr(255,255,255));
                
                MCvScalar color = GetColor();
                List<Point> points = GetPoints();

                DrawLine(points[0], points[1], color);
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
            if (VideoCapture != null)
            {
                VideoCapture.Dispose();
                MessageBox.Show("Resources Released");
            }
        }
    }
}
