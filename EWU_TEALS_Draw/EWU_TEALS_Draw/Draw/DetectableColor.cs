using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using EWU_TEALS_Draw;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Draw {
    public class DetectableColor : Detectable {
        // set to correspond to the "600 pixels" constant in the mainform, so that scaling will work consistently
        private const double PercentOfScreenToDetect = 0.0078125;
        private const int FramesPerSecond = 30;
        private static readonly Point InvalidPoint = new Point(-255, -255);
        // reused to save memory
        private static Mat HsvImageTemp = new Mat();
        private static Mat HsvThreshTemp = new Mat();
        
        public MCvScalar InkColor { get; set; }
        public MCvScalar MinHsv;
        public MCvScalar MaxHsv;
        [JsonIgnore]
        public IInputArray MinHsvRange { get => new ScalarArray(MinHsv); }
        [JsonIgnore]
        public IInputArray MaxHsvRange { get => new ScalarArray(MaxHsv); }

        [JsonIgnore]
        private Mat threshMat = new Mat();
        [JsonIgnore]
        private Point lastPosition = InvalidPoint;

        public DetectableColor(string name, bool isEnabled, MCvScalar inkColor, MCvScalar minHsv, MCvScalar maxHsv) : base(name) {
            IsEnabled = isEnabled;
            InkColor = inkColor;
            MinHsv = minHsv;
            MaxHsv = maxHsv;
        }

        public override void ResetLastPosition() {
            lastPosition = InvalidPoint;
        }

        public override Mat Draw(ImageBox canvas, Mat videoCapture) {
            CvInvoke.CvtColor(videoCapture, HsvImageTemp, ColorConversion.Bgr2Hsv);

            // Convert pixels to white that are in specified color range, black otherwise, save to thresh_image
            CvInvoke.InRange(HsvImageTemp, MinHsvRange, MaxHsvRange, HsvThreshTemp);

            // Get contours of thresh image
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint()) {
                //Mat hierarchy = new Mat(); Use this if need hierarchy parameter in FindContours
                CvInvoke.FindContours(HsvThreshTemp, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                // Find largest contour
                double maxArea = 0;
                int maxContourIndex = -1;
                for (int i = 0; i < contours.Size; i++) {
                    using (VectorOfPoint contour = contours[i]) {
                        double area = CvInvoke.ContourArea(contour);
                        if (area > maxArea) {
                            maxArea = area;
                            maxContourIndex = i;
                        }
                    }
                }

                // If at least one contour was found and its area is at least 300 pixels
                var minAreaToDetect = (int)(PercentOfScreenToDetect * (videoCapture.Width * videoCapture.Height));
                if (maxContourIndex >= 0 && maxArea >= minAreaToDetect) {
                    // Draw the contour in white
                    CvInvoke.DrawContours(videoCapture, contours, maxContourIndex, new MCvScalar(255, 255, 255), 2);

                    // Get the contour center of mass
                    MCvMoments moments = CvInvoke.Moments(contours[maxContourIndex]);
                    Point contourCenter = new Point(
                        (int)(moments.M10 / moments.M00),
                        (int)(moments.M01 / moments.M00));

                    // Draw the contour center on video feed
                    CvInvoke.Circle(videoCapture, contourCenter, 5, new MCvScalar(255, 255, 255), 2);

                    // Draw on canvas using contour center
                    contourCenter = ScaleToFit(contourCenter, videoCapture.Width, videoCapture.Height, canvas.Width, canvas.Height);
                    int width = GetWidthBySpeed(lastPosition, contourCenter);

                    //don't want to draw or change state if this hasn't been enabled
                    if (IsEnabled) {
                        DrawLineTo(canvas, lastPosition, contourCenter, InkColor, width);

                        lastPosition = contourCenter;
                    }
                }
                else if (IsEnabled) {
                    lastPosition = InvalidPoint;
                }
            }

            // warning, this will get reused by the next Draw, so do with it what you wish before calling Draw again
            return HsvThreshTemp;
        }

        private int GetWidthBySpeed(Point colorLastPosition, Point colorDestination) {
            int dx = colorDestination.X - colorLastPosition.X;
            int dy = colorDestination.Y - colorLastPosition.Y;

            double travelDistance = Math.Sqrt(dx * dx + dy * dy);
            double speed = travelDistance / (1000 / FramesPerSecond); // Speed as a ratio of pixels/frame length in ms
            double maxAssumedSpeed = 2; // found this number through testing...
            double speedRatio = speed / maxAssumedSpeed;

            if (speedRatio > 1.0) speedRatio = 1.0;
            int maxWidth = 25;
            int minWidth = 3;
            int strokeWidth = (int)Math.Ceiling(speedRatio * maxWidth);


            // To flip to wider when slower:
            //strokeWidth = maxWidth - strokeWidth;

            if (strokeWidth > maxWidth) strokeWidth = maxWidth;
            if (strokeWidth < minWidth) strokeWidth = minWidth;
            return strokeWidth;
        }
    }
}
