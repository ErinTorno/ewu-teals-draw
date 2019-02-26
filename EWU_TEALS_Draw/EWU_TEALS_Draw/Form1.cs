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
    public partial class Form1 : Form
    {
        Timer timer;

        
        public Form1()
        {
            InitializeComponent();

            //Startup();
            RunIdle();
        }

        private void RunIdle()
        {
            ImageViewer viewer = new ImageViewer(); //create an image viewer
            VideoCapture capture = new VideoCapture(); //create a camera captue
            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
                viewer.Image = capture.QueryFrame(); //draw the image obtained from camera
            });
            //viewer.ShowDialog();
            viewer.Show();
        }

        private void Startup()
        {
            using (VideoCapture videoCapture = new VideoCapture())
            {
                Emgu.CV.Image<Bgr, Byte> image = videoCapture.QueryFrame().ToImage<Bgr, Byte>();

                imageBox1.Image = image;
                imageBox1.Refresh();

                image.Dispose();
            }
        }

        private Timer CreateTimer(int millis)
        {
            Timer timer = new Timer();
            timer.Interval = millis;
            timer.Tick += Timer_Tick;

            return timer;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            using (VideoCapture videoCapture = new VideoCapture())
            {
                Emgu.CV.Image<Bgr, Byte> image = videoCapture.QueryFrame().ToImage<Bgr, Byte>();

                imageBox1.Image = image;
                imageBox1.Refresh();

                image.Dispose();
            }
        }
    }
}
