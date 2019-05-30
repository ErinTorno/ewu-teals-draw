using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using EwuTeals.Detectables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace EwuTeals.Utils {
    public class AutoColor {
        private static readonly TimeSpan DelayBetweenCaptures = new TimeSpan(0, 0, 0, 0, 1000);
        private static readonly Point IncorrectPoint = new Point(-255, -255);
        private static readonly MCvScalar RectangleColor = new MCvScalar(255, 230, 230);
        private const double PointDistPercent = 0.19, DrawSquareDimPercent = 0.20;
        // we take a number of strips of points in the captured area, so that the total number of points are n^2
        private const double PointLinesTaken = 15.0;

        private List<DetectableColor> colors = new List<DetectableColor>();
        private DateTime lastCapure = DateTime.Now;

        public event EventHandler<ColorCaptureArgs> OnColorCapture;

        public IReadOnlyList<DetectableColor> Colors { get => colors.AsReadOnly(); }

        public ImageBox VideoCapture { get; set; }
        
        public bool IsActive { get; set; }

        public bool CaptureNextUpdate { get; set; }

        private string NextColorName { get => "Auto Color #" + colors.Count; }

        public AutoColor(ImageBox video) {
            VideoCapture = video;
            IsActive = false;
            CaptureNextUpdate = false;
        }

        public void Reset() {
            colors = new List<DetectableColor>();
        }


        public void Update(Mat source) {
            if (IsActive) {
                if (CaptureNextUpdate) {
                    CaptureNextUpdate = false;
                    if (lastCapure.Add(DelayBetweenCaptures) < DateTime.Now) {
                        lastCapure = DateTime.Now;
                        var avgC = GetColorFromSurrounding(source, new Point(source.Width / 2, source.Height / 2));
                        colors.Add(GenerateHsvConfig(NextColorName, avgC));
                        var ev = OnColorCapture;
                        if (ev != null) ev(this, new ColorCaptureArgs(colors.Last()));
                    }
                }

                var image = VideoCapture.Image;
                var bitmap = image.Bitmap;
                
                var squareSize = (int)(DrawSquareDimPercent * bitmap.Width);
                var rectangle = new Rectangle((bitmap.Width - squareSize) / 2, bitmap.Height / 2 - squareSize, squareSize, squareSize);
                CvInvoke.Rectangle(image, rectangle, RectangleColor, 3);

                VideoCapture.Refresh();
            }
        }

        /// <summary>
        /// Gets a scalar by averaging the colors surrounding a center point
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        private MCvScalar GetColorFromSurrounding(Mat source, Point center) {
            int dist = (int)(PointDistPercent * VideoCapture.Width);
            var pixelIncrement = (int)(dist / PointLinesTaken);
            var bitmap = source.Bitmap;
            var minX = center.X - dist < 0 ? 0 : center.X - dist;
            var maxX = center.X + dist >= source.Width ? source.Width - 1 : center.X + dist;
            var minY = center.Y - dist < 0 ? 0 : center.Y - dist;
            var maxY = center.Y + dist >= source.Height ? source.Height - 1 : center.Y + dist;

            // we just sum together every color component, then divide to find average; it's consistent enough that we don't have to use HSV
            int points = 0;
            var b = 0.0; var g = 0.0; var r = 0.0;
            for (int x = minX; x < maxX; x += pixelIncrement) {
                for (int y = minY; y < maxY; y += pixelIncrement) {
                    var col = bitmap.GetPixel(x, y);
                    b += col.B;
                    g += col.G;
                    r += col.R;
                    ++points;
                }
            }
            return new MCvScalar(b / points, g / points, r / points);
        }

        public static DetectableColor GenerateHsvConfig(string name, MCvScalar bgrColor) {
            var hsv = bgrColor.ToHsv();
            // we increase the saturation, since it appears duller on camera
            var ink = new MCvScalar(hsv.V0, Math.Min(255.0, hsv.V1 * 1.75), Math.Min(hsv.V2 * 1.75, 255.0)).ToBgr();
            var minHsv = new MCvScalar(hsv.V0 - 6.0, hsv.V1 * 0.5, 80.0);
            var maxHsv = new MCvScalar(hsv.V0 + 6.0, 255.0, 255.0);
            return new DetectableColor(name, true, ink, minHsv.RestrictHsvRanges(), maxHsv.RestrictHsvRanges());
        }

        public class ColorCaptureArgs : EventArgs {
            public DetectableColor Color { get; }

            public ColorCaptureArgs(DetectableColor config) {
                Color = config;
            }
        }
    }
}
