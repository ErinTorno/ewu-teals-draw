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
    using Line = List<WPoint>;

    class Drawing {
        // once the movement is this far in one jump, we will show the fast color fully
        private const double PercentOfWidthToReachMin = 0.15;
        // default color to draw; in future we will change this to whatever the user is holding
        private static readonly MCvScalar DefaultColor = new MCvScalar(0, 0, 0);
        // the minimum and maximum widths, relative to total page width, of the thickness of the line
        private const double MinLineWidthPercent = 0.010, MaxLineWidthPercent = 0.025;

        // after n seconds of not seeing the color, we decide that the next time we see it, it'll be the start of a new line
        private const double SecondsToStartNew = 0.75;

        private Dictionary<MCvScalar, List<Line>> lineMap = new Dictionary<MCvScalar, List<Line>>();
        private Dictionary<MCvScalar, DateTime> lastTick = new Dictionary<MCvScalar, DateTime>();
        private Line LatestLine(MCvScalar color) { return lineMap[color].Last(); }
        private DateTime curTime;

        public int RefreshRate { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Drawing(int width, int height, int refreshRate) {
            Width = width;
            Height = height;
            RefreshRate = refreshRate;
            curTime = DateTime.Now;
        }

        public void AddPoint(ImageBox image, MCvScalar color, int x, int y) {
            curTime = DateTime.Now;
            if (!lineMap.ContainsKey(color)) {
                lineMap[color] = new List<List<WPoint>>();
                lastTick[color] = curTime;
            }

            var lines = lineMap[color];
            // ensure there is a line currently being worked on
            if (lines.Count == 0 || lastTick[color].AddSeconds(SecondsToStartNew) < curTime) {
                lines.Add(new List<WPoint>());
                lastTick[color] = curTime;
            }
            var last = LatestLine(color);

            var xscale = (double)image.Width / (double)Width;
            var yscale = (double)image.Height / (double)Height;
            var nx = (int)(x / xscale);
            var ny = (int)(y / yscale);
            var width = last.Count == 0 ? MinLineWidthPercent : GetWidthBySpeed(last.Last(), nx, ny);

            last.Add(new WPoint(nx, ny, width));
            lastTick[color] = curTime;
        }

        /// <summary>
        /// Draws the current state onto a given ImageBox
        /// </summary>
        /// <param name="imageBox">The canvas to be drawn to</param>
        public void Update(ImageBox imageBox) {
            curTime = DateTime.Now;
            var image = imageBox.Image;
            foreach (var pair in lineMap) {
                var color = pair.Key;
                var lines = pair.Value;
                foreach (var line in lines) {
                    // scales in each direction
                    var xscale = (double)image.Bitmap.Width / (double)Width;
                    var yscale = (double)image.Bitmap.Height / (double)Height;
                    var avgScale = (xscale + yscale) / 2.0;

                    if (line.Count > 0) {
                        // we need a point to start drawing from
                        var lastPoint = line[0].ToRelative(xscale, yscale, avgScale);
                        for (var i = 1; i < line.Count; ++i) {
                            var relativePoint = line[i].ToRelative(xscale, yscale, avgScale);
                            // it will throw an exception if by change width is rounded down to 0
                            var width = (int)relativePoint.Width == 0 ? 1 : (int)relativePoint.Width;
                            CvInvoke.Line(image, lastPoint.ToStandardPoint(), relativePoint.ToStandardPoint(), color, width, LineType.AntiAlias);
                            lastPoint = relativePoint;
                        };
                    }
                }
            }
            imageBox.Refresh();
        }

        // Misc methods

        /// <summary>
        /// Determines the width of the line according to the distance that it moves
        /// </summary>
        /// <param name="start">The last point added</param>
        /// <param name="newX">The new point's X</param>
        /// <param name="newY">The new point's Y</param>
        /// <returns></returns>
        private double GetWidthBySpeed(WPoint start, double newX, double newY) {
            var dx = start.X - newX;
            var dy = start.Y - newY;
            var travelDistance = Math.Sqrt(dx * dx + dy * dy);
            var distToReachThinnest = PercentOfWidthToReachMin * Width;
            if (travelDistance >= distToReachThinnest)
                return MinLineWidthPercent * Width;
            else if (travelDistance == 0.0)
                return MaxLineWidthPercent * Width;
            else {
                var distPerc = (distToReachThinnest - travelDistance) / distToReachThinnest;
                return distPerc * (MaxLineWidthPercent - MinLineWidthPercent) * Width;
            }
        }

        // Interactions

        public void Reset() {
            lineMap = new Dictionary<MCvScalar, List<Line>>();
        }
    }
}
