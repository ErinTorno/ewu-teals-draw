using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using EwuTeals.Detectables;
using EwuTeals.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.WhackAMole {
    class Target {
        private static readonly MCvScalar InnerRingColor = new MCvScalar(0, 0, 0);
        private static readonly MCvScalar EmptyColor = new MCvScalar(230, 230, 230);

        private const double TargetRadiusPercent = 0.1, TargetRadiusR2Percent = 0.7, TargetRadiusR3Percent = 0.35; // mult by height to get radius
        public const double TimeLimit = 5.0;
        private const double MaximumDistToHitTarget = 0.1;

        public Point Position;
        // we use this color to match up to players; if it is not Nothing, then if a player draws over it with the same color, it will be tallied
        public Maybe<MCvScalar> Color { get; set; }
        // time left until the target is invalidatedC:\Users\rin\Documents\GitHub\ewu-teals-draw\EWU_TEALS_Draw\EWU_TEALS_Draw\Games\WhackAMole\Target.cs
        public double TimeRemaining;
        public MCvScalar InkColor { get => Color.GetOrElse(EmptyColor); }
        public MCvScalar InnerInkColor { get => InnerRingColor; }

        private int width, height;

        public Target(int x, int y, int width, int height) {
            this.Position = new Point(x, y);
            this.Color = Maybe<MCvScalar>.Nothing;
            this.width = width;
            this.height = height;
            TimeRemaining = TimeLimit;
        }

        private void InvalidateTarget(IList<(MCvScalar color, int count)> colorLegend) {
            int colWithZero = -1;
            for (int i = 0; i < colorLegend.Count(); ++i) {
                var (color, count) = colorLegend[i];
                if (count == 0)
                    colWithZero = i;
                if (color.IsEqualTo(InkColor))
                    colorLegend[i] = (color, count - 1);
            }
            if (colWithZero >= 0) {
                var (color, count) = colorLegend[colWithZero];
                this.Color = color.ToMaybe();
                colorLegend[colWithZero] = (color, count + 1);

                TimeRemaining = TimeLimit;
            }
            else
                this.Color = Maybe<MCvScalar>.Nothing;
        }

        private bool IsValidDetectable(Detectable d) {
            if (d.InkColor.IsEqualTo(this.InkColor)) {
                var dX = this.Position.X - d.LastPosition.X;
                var dY = this.Position.Y - d.LastPosition.Y;
                var distanceSq = dX * dX + dY * dY;
                var maxDist = MaximumDistToHitTarget * height;
                maxDist *= maxDist;
                return maxDist >= distanceSq;
            }
            return false;
        }

        /// <summary>
        /// Updates the timing on this target
        /// </summary>
        /// <param name="dT">The change in time for this update, in seconds</param>
        /// <returns>True if the target is expired</returns>
        public void Update(double dT, IList<Player> players, IList<(MCvScalar color, int count)> colorLegend) {
            TimeRemaining -= dT;
            if (TimeRemaining < 0)
                InvalidateTarget(colorLegend);

            foreach (var p in players) {
                if (IsValidDetectable(p.PaddleA) || IsValidDetectable(p.PaddleB)) {
                    InvalidateTarget(colorLegend);
                    p.Points += 1;
                }
            }
        }

        public void Draw(ImageBox canvas) {
            var tarRadis = (int)(TargetRadiusPercent * canvas.Height);
            var tarRadis2 = (int)(TargetRadiusR2Percent * tarRadis);
            var tarRadis3 = (int)(TargetRadiusR3Percent * tarRadis);
            CvInvoke.Circle(canvas.Image, Position, tarRadis, InkColor, -1);
            CvInvoke.Circle(canvas.Image, Position, tarRadis2, InnerInkColor, -1);
            CvInvoke.Circle(canvas.Image, Position, tarRadis3, InkColor, -1);
        }
    }

    static class TargetSet {
        public static readonly List<V2<double>> Default = new List<V2<double>> {
            new V2<double>(0.25, 0.30),
            new V2<double>(0.50, 0.15),
            new V2<double>(0.75, 0.30),
            new V2<double>(0.25, 0.70),
            new V2<double>(0.50, 0.85),
            new V2<double>(0.75, 0.70),
            new V2<double>(0.15, 0.50),
            new V2<double>(0.50, 0.50),
            new V2<double>(0.85, 0.50)
        };

        /// <summary>
        /// Creates a set of targets from a given list of relative points and a screen's dimensions
        /// </summary>
        /// <param name="w">The width of the screen</param>
        /// <param name="h">The height of the screen</param>
        /// <param name="points">A list of points, where (0.0, 0.0) is top left, and (1.0, 1.0) is bottom right</param>
        /// <returns></returns>
        public static List<Target> PlaceTargets(double w, double h, List<V2<double>> points) {
            return (from p in points select new Target((int)(p.X * w), (int)(p.Y * h), (int)w, (int)h)).ToList();
        }
    }
}
