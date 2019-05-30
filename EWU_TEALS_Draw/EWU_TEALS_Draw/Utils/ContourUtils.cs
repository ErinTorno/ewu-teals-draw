using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace EwuTeals.Utils {
    public static class ContourExtension {
        private const int MaxPointsToAnalyze = 50;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetMagnitudeSquare(this Point a, Point b) {
            var distX = a.X - b.X;
            var distY = a.Y - b.Y;
            return distX * distX + distY * distY;
        }

        /// <summary>
        /// Determines how irregular (and not rounded) a shape is from 0...
        /// A perfect circle would be 0
        /// </summary>
        /// <param name="contour"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static int IrregularityRating(this VectorOfPoint contour, Point center) {
            if (contour.Size == 0)
                return Int32.MaxValue;
            // we don't want to measure all points if very complex, as that can get difficult
            int dIndex = contour.Size / MaxPointsToAnalyze;
            if (dIndex <= 0) dIndex = 1;
            int sum = 0;
            int pointsAnalyzed = 0;
            for (int i = 0; i < contour.Size; i += dIndex) {
                ++pointsAnalyzed;
                // don't need to square since we are just comparing with each other
                sum += GetMagnitudeSquare(contour[i], center);
            }

            int averageMagn = sum / pointsAnalyzed;
            int devFromAvg = 0;
            for (int i = 0; i < contour.Size; i += dIndex) {
                ++pointsAnalyzed;
                var magn = GetMagnitudeSquare(contour[i], center);
                // positive diff between average and this
                devFromAvg = magn - averageMagn < 0 ? averageMagn - magn : magn - averageMagn;

            }
            return devFromAvg / pointsAnalyzed;
        }

        /// <summary>
        /// Returns all points that are either the local minimum, or the local maximum
        /// </summary>
        /// <param name="contour"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static (List<Point> min, List<Point> max) GetLocalExtrema(this VectorOfPoint contour, Point center) {
            var min = new List<Point>();
            var max = new List<Point>();
            // if we don't have at least this many points, there can't be any extremum
            if (contour.Size >= 3) {
                for (int i = 1; i < contour.Size; ++i) {
                    var cur = contour[i];
                    var curMag = cur.GetMagnitudeSquare(center);
                    var prevMag = contour[i - 1].GetMagnitudeSquare(center);

                    // so that the next will wrap around if we get to the end of the points
                    var next = i < contour.Size - 1 ? contour[i + 1] : contour[i - contour.Size + 1];
                    var nextMag = next.GetMagnitudeSquare(center);

                    if (curMag > prevMag && curMag > nextMag)
                        // Then this is a maximum
                        max.Add(cur);
                    else if (curMag < prevMag && curMag < nextMag)
                        // This is a minimum
                        min.Add(cur);
                }
            }
            return (min, max);
        }
    }
}
