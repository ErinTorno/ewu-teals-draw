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

        
        public Form1()
        {
            InitializeComponent();

            RunIdle();
        }

        private void RunIdle()
        {
            VideoCapture capture = new VideoCapture(); //create a camera captue
            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
                //viewer.Image = capture.QueryFrame(); //draw the image obtained from camera
                imageBox1.Image = capture.QueryFrame();
                imageBox1.Show();

               

                Point point1 = new Point(50, 50);
                Point point2 = new Point(100, 300);

                // Draw Line
                CvInvoke.Line(imageBox1.Image, point1, point2, new MCvScalar(), 10, Emgu.CV.CvEnum.LineType.AntiAlias, 0);
            });
        }
    }
}
