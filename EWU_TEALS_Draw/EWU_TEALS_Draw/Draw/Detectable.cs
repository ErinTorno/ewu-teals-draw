using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Draw {
    public abstract class Detectable {
        protected static readonly Point InvalidPoint = new Point(-255, -255);

        public bool IsEnabled { get; set; }

        public string Name { get; }

        public Detectable(string name) {
            Name = name;
        }

        public abstract void ResetLastPosition();

        public abstract Mat Draw(ImageBox canvas, Mat videoCapture);

        protected void DrawLineTo(ImageBox canvas, Point orig, Point desti, MCvScalar color, int strokeWidth) {
            if (orig == InvalidPoint || desti == InvalidPoint)
                return;
            if (strokeWidth <= 0) strokeWidth = 1;
            CvInvoke.Line(canvas.Image, orig, desti, color, strokeWidth, LineType.AntiAlias);
            canvas.Refresh();
        }

        protected Point ScaleToFit(Point point, double oldW, double oldH, double newW, double newH) {
            point.X = (int)(point.X * newW / oldW);
            point.Y = (int)(point.Y * newH / oldH);

            return point;
        }
    }
}
