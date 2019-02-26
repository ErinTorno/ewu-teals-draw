using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Emgu;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.Drawing;


namespace EWU_TEALS_Draw
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            VideoCapture videoCapture = new VideoCapture();
            Emgu.CV.Image<Bgr, Byte> image = videoCapture.QueryFrame().ToImage<Bgr, Byte>();
            //image1.Source
            ImageBox imageBox = new ImageBox();



            /*
            Emgu.CV.VideoCapture videoCapture = new VideoCapture();

            
            using (var nextFrame = videoCapture.QueryFrame())
            {
                if (nextFrame != null)
                {
                    var color = 
                    image1.Source = nextFrame.ToImage<>
                    image1.Image = nextFrame.ToBitmap();
                }
            }
            */
        }
    }
}
