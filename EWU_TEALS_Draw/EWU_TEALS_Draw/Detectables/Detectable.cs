using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Newtonsoft.Json;
using System;
using System.Drawing;

namespace EwuTeals.Detectables {
    public abstract class Detectable : IDisposable {
        protected static readonly Point InvalidPoint = new Point(-255, -255);

        public string Name { get; }
        public bool IsEnabled { get; set; }
        public MCvScalar InkColor { get; set; }

        [JsonIgnore]
        private Mat threshMat = new Mat();
        [JsonIgnore]
        public Point LastPosition { get; protected set; }

        public Detectable(string name) {
            Name = name;
            LastPosition = InvalidPoint;
        }

        public abstract void ResetLastPosition();

        public abstract Mat Draw(ImageBox canvas, Mat videoCapture, bool drawOnCanvas = true);

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

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool callFromDispose) {
            threshMat.Dispose();
        }
    }
}
