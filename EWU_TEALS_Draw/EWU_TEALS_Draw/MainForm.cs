using EwuTeals.Draw;
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
        private Point LastHandPosition;

        private Drawing drawing = new Drawing(ActualCameraWidth, ActualCameraHeight, FPS);
        private ColorDetector colorDetector;

        // Resolution Properties
        private const int CameraToUse = 0; // Default Camera: 0
        private const int FPS = 30;
        private const int ActualCameraWidth = 1280;
        private const int ActualCameraHeight = 720;

        private const int DisplayedCameraWidth = ActualCameraWidth / 4;
        private const int DisplayedCameraHeight = ActualCameraHeight / 4;

        private const int CanvasWidth = DisplayedCameraWidth * 2;
        private const int CanvasHeight = DisplayedCameraHeight * 3;

        // Thresholding Properties
        private static readonly ColorRange DefaultColorRange = new ColorRange(
            inkColor: new MCvScalar(255, 135, 135),
            minHsvColor: new MCvScalar(100, 150, 100),
            maxHsvColor: new MCvScalar(135, 255, 255)
        );
        private static readonly ColorRange DefaultColorRangeGreen = new ColorRange(
            inkColor: new MCvScalar(135, 230, 135),
            minHsvColor: new MCvScalar(65, 75, 100),
            maxHsvColor: new MCvScalar(85, 255, 255)
        );

        public MainForm()
        {
            InitializeComponent();

            Startup();
        }

        private void Startup()
        {
            Disposables = new List<IDisposable>();
            ImageBox_VideoCapture_Gray.Image = new Image<Bgr, byte>(DisplayedCameraWidth, DisplayedCameraHeight, new Bgr(255, 255, 255));

            ImageBox_Drawing.Image = new Image<Bgr, byte>(CanvasWidth, CanvasHeight, new Bgr(255, 255, 255));
            
            Disposables.Add(ImageBox_VideoCapture_Gray.Image);
            Disposables.Add(ImageBox_Drawing.Image);
            Disposables.Add(ImageBox_VideoCapture.Image);

            SetupVideoCapture();
            
            Mat color_image = VideoCapture.QueryFrame();
            colorDetector = new ColorDetector(ImageBox_VideoCapture, ImageBox_Drawing, color_image.Size, DefaultColorRange, DefaultColorRangeGreen);

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

                // The canvases are checked for each color that the detect has been told, and then those points are added to the drawing
                colorDetector.UpdateDrawing(flippedVideoFrame, drawing);
                ImageBox_VideoCapture_Gray.Image = colorDetector.ThreshImage;
                drawing.Update(ImageBox_Drawing);
            }
        }

        private Mat FlipImage(Mat inputImage)
        {
            Mat flippedImage = new Mat(inputImage.Size, DepthType.Cv8U, inputImage.NumberOfChannels);
            CvInvoke.Flip(inputImage, flippedImage, FlipType.Horizontal);
            return flippedImage;
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
            drawing.Reset();
        }
    }
}
