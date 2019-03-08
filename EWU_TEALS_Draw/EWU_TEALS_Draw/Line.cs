using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Draw {
    public struct WPoint {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }

        public WPoint(double x, double y, double width) {
            X = x; Y = y; Width = width;
        }

        public System.Drawing.Point ToStandardPoint() {
            return new System.Drawing.Point((int)X, (int)Y);
        }

        // we scale it to fit the new proportions
        public WPoint ToRelative(double xscale, double yscale, double wscale) {
            return new WPoint(xscale * X, yscale * Y, wscale * Width);
        }
    }

    public struct Line {
        public List<WPoint> Points { get; set; }
        public MCvScalar Color { get; set; }

        public Line(List<WPoint> p, MCvScalar c) {
            Points = p;
            Color = c;
        }
    }
}
