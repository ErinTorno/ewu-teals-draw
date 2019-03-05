using Emgu.CV;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWU_TEALS_Draw {
    class Drawing {
        private const double CircleScale = 8.0;

        private List<Point> points = new List<Point>();

        public int Width { get; private set; }
        public int Height { get; private set; }
        public IReadOnlyList<Point> Points { get => points.AsReadOnly(); }

        public Drawing(int width, int height) {
            Width = width;
            Height = height;
        }

        public void AddPoint(ImageBox image, int x, int y) {
            var xscale = (double)image.Width / (double)Width;
            var yscale = (double)image.Height / (double)Height;
            points.Add(new Point((int)((image.Width - x) / xscale), (int)(y / yscale)));
        }

        public void Update(IImage image) {
            var xscale = (double)image.Bitmap.Width / (double)Width;
            var yscale = (double)image.Bitmap.Height / (double)Height;
            foreach (var p in points) {
                var relativePoint = new Point((int)(p.X * xscale), (int)(p.Y * yscale));
                CvInvoke.Circle(image, p, radius: (int)(CircleScale * (xscale + yscale) / 2.0), color: new Emgu.CV.Structure.MCvScalar(240, 140, 0));
            };
        }
    }
}
