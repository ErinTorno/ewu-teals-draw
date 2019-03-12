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
        private List<ColorRange> detectedColors = new List<ColorRange>();
        private Mat hsvImage;
        public Mat ThreshImage { get; private set; }
        public ImageBox VideoSource { get; set; }
        public ImageBox CanvasBox { get; set; }

        public ColorDetector(ImageBox source, ImageBox canvas, Size size, params ColorRange[] defColors) {
            VideoSource = source;
            CanvasBox = canvas;
            foreach (var c in defColors) detectedColors.Add(c);
            hsvImage = new Mat(size, DepthType.Cv8U, 3);
            ThreshImage = new Mat(size, DepthType.Cv8U, 1);
        }

        public void AddRange(ColorRange c) {
            detectedColors.Add(c);
        }

        public void RemoveRange(ColorRange c) {
            detectedColors.Where(val => val != c);
        }

        public void UpdateDrawing(Mat input, Drawing drawing) {
            Mat allPoints = null; // = new Mat(input.Size, DepthType.Cv8U, 1);

            foreach (var c in detectedColors) {
                CvInvoke.CvtColor(input, hsvImage, ColorConversion.Bgr2Hsv);

                // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
                CvInvoke.InRange(hsvImage, new ScalarArray(c.MinHsvColor), new ScalarArray(c.MaxHsvColor), ThreshImage);

                // Find average of white pixels
                Mat points = new Mat(input.Size, DepthType.Cv8U, 1);
                Mat combined = new Mat();
                CvInvoke.FindNonZero(ThreshImage, points);
                if (allPoints == null)
                    allPoints = ThreshImage;
                else {
                    CvInvoke.Add(allPoints, ThreshImage, combined);
                    allPoints = combined;
                }

                // An alternative approach to averaging would be to use the K-means 
                // algorithm to find clusters, since average is significantly influenced by outliers.
                MCvScalar avg = CvInvoke.Mean(points);
                Point avgPoint = new Point((int)avg.V0, (int)avg.V1);

                int sumWhitePixels = CvInvoke.CountNonZero(ThreshImage);
                if (sumWhitePixels > 100) {
                    // Draw circle on camera feed
                    CvInvoke.Circle(input, avgPoint, 5, new MCvScalar(0, 10, 220), 2);

                    // Draw on canvas
                    drawing.AddPoint(VideoSource, c.InkColor, avgPoint.X, avgPoint.Y);
                }

                drawing.Update(CanvasBox);
            }

            ThreshImage = allPoints;
        }
    }
}
