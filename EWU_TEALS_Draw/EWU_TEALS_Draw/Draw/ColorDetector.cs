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
    class ColorDetector {
        private static readonly ColorRange NullColorRange = new ColorRange();
        private static readonly Point IncorrectPoint = new Point(-255, -255);
        private const double PointDistPercent = 0.19, MinPointDistPercent = 0.14;
        private const int WhitePixelsToMatch = 100;

        private List<ColorRange> detectedColors = new List<ColorRange>();
        private List<ColorRange> surroundingColors = new List<ColorRange>();
        private Mat hsvImage, points, nearbyImage;
        public Mat ThreshImage { get; private set; }
        public ImageBox VideoSource { get; set; }
        public ImageBox CanvasBox { get; set; }

        public ColorDetector(ImageBox source, ImageBox canvas, Size size, params ColorRange[] defColors) {
            VideoSource = source;
            CanvasBox = canvas;
            foreach (var c in defColors) detectedColors.Add(c);
            hsvImage = new Mat(size, DepthType.Cv8U, 3);
            points = new Mat(size, DepthType.Cv8U, 1);
            nearbyImage = new Mat(size, DepthType.Cv8U, 1);
            ThreshImage = new Mat(size, DepthType.Cv8U, 1);
        }

        public void AddRange(ColorRange c) {
            detectedColors.Add(c);
        }

        public void RemoveRange(ColorRange c) {
            detectedColors.Where(val => val != c);
        }

        private MCvScalar MatchNearby(Mat input, Point innerColorCenter) {
            var wasFound = false;
            ColorRange matching = NullColorRange;
            foreach (var c in surroundingColors) {
                var center = GetCenter(input, c, nearbyImage);
                if (center != IncorrectPoint) {
                    matching = c;
                    wasFound = true;
                }
            }

            // if we couldn't find a match, we should create a new one
            if (!wasFound) {
                var avgC = GetColorFromSurrounding(input, innerColorCenter);
                matching = ColorRange.GenerateRange(avgC);
                wasFound = true;
                surroundingColors.Add(matching);
            }
            
            return matching.InkColor;
        }
        
        public void UpdateDrawing(Mat input, Drawing drawing) {
            Mat allPoints = null;

            foreach (var c in detectedColors) {
                CvInvoke.CvtColor(input, hsvImage, ColorConversion.Bgr2Hsv);

                // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
                CvInvoke.InRange(hsvImage, new ScalarArray(c.MinHsvColor), new ScalarArray(c.MaxHsvColor), ThreshImage);

                // Find average of white pixels

                // These two matrices are the next things to be looked at for optimization
                // Is there a way to reuse them? The size doesn't change, maybe we can just clear them
                CvInvoke.FindNonZero(ThreshImage, points);
                if (allPoints == null)
                    allPoints = ThreshImage;
                else
                    CvInvoke.Add(allPoints, ThreshImage, allPoints);

                // An alternative approach to averaging would be to use the K-means 
                // algorithm to find clusters, since average is significantly influenced by outliers.
                MCvScalar avg = CvInvoke.Mean(points);
                Point avgPoint = new Point((int)avg.V0, (int)avg.V1);

                int sumWhitePixels = CvInvoke.CountNonZero(ThreshImage);
                if (sumWhitePixels > WhitePixelsToMatch) {
                    var inkColor = MatchNearby(input, avgPoint);

                    // Draw circle on camera feed
                    CvInvoke.Circle(input, avgPoint, 5, new MCvScalar(0, 10, 220), 2);

                    // Draw on canvas
                    drawing.AddPoint(VideoSource, inkColor, avgPoint.X, avgPoint.Y);
                }
            }

            ThreshImage = allPoints;
            drawing.Update(CanvasBox);
        }


        private Point GetCenter(Mat input, ColorRange c, Mat threshImage) {
            CvInvoke.CvtColor(input, hsvImage, ColorConversion.Bgr2Hsv);

            // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
            CvInvoke.InRange(hsvImage, new ScalarArray(c.MinHsvColor), new ScalarArray(c.MaxHsvColor), threshImage);

            // Find average of white pixels
            CvInvoke.FindNonZero(threshImage, points);

            // An alternative approach to averaging would be to use the K-means 
            // algorithm to find clusters, since average is significantly influenced by outliers.
            MCvScalar avg = CvInvoke.Mean(points);
            var p = new Point((int)avg.V0, (int)avg.V1);

            int sumWhitePixels = CvInvoke.CountNonZero(threshImage);
            if (sumWhitePixels > WhitePixelsToMatch)
                return p;
            else
                return IncorrectPoint;
        }

        private MCvScalar GetColorFromSurrounding(Mat input, Point center) {
            int dist = (int)(PointDistPercent * ThreshImage.Width);
            int minDist = (int)(MinPointDistPercent * ThreshImage.Width);
            var bitmap = input.Bitmap;
            var minX = center.X - dist < 0 ? 0 : center.X - dist;
            var maxX = center.X + dist >= input.Size.Width ? input.Size.Width  - 1 : center.X + dist;
            var minY = center.Y - dist < 0 ? 0 : center.Y - dist;
            var maxY = center.Y + dist >= input.Size.Height ? input.Size.Height - 1 : center.Y + dist;
            int points = 0;
            var b = 0.0; var g = 0.0; var r = 0.0;
            for (int x = minX; x < maxX; x += 2) {
                if (x > minX + minDist && x < maxX - minDist)
                    continue;
                for (int y = minY; y < maxY; y += 2) {
                    if (y > minY + minDist && y < maxY - minDist)
                        continue;
                    var col = bitmap.GetPixel(x, y);
                    b += col.B;
                    g += col.G;
                    r += col.R;
                    ++points;
                }
            }
            return new MCvScalar(b / points, g / points, r / points);
        }
    }
}
