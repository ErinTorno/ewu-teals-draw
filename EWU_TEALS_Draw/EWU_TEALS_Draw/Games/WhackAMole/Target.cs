using Emgu.CV.Structure;
using EwuTeals.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.WhackAMole {
    class Target {
        public const double TimeLimit = 3.0;
        public static readonly MCvScalar EmptyColor = new MCvScalar(200, 200, 200);

        public Point Position;
        // we use this color to match up to players; if it is not Nothing, then if a player draws over it with the same color, it will be tallied
        public Maybe<MCvScalar> Color = Maybe<MCvScalar>.Nothing;
        // time left until the target is invalidated
        public double TimeRemaining;
        public MCvScalar InkColor { get => Color.GetOrElse(EmptyColor); }

        public Target(int x, int y) {
            this.Position = new Point(x, y);
            TimeRemaining = TimeLimit;
        }

        /// <summary>
        /// Updates the timing on this target
        /// </summary>
        /// <param name="dT">The change in time for this update, in seconds</param>
        /// <returns>True if the target is expired</returns>
        public bool Update(double dT) {
            TimeRemaining -= dT;
            return TimeRemaining <= 0.0;
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
            new V2<double>(0.10, 0.50),
            new V2<double>(0.50, 0.50),
            new V2<double>(0.90, 0.50)
        };

        /// <summary>
        /// Creates a set of targets from a given list of relative points and a screen's dimensions
        /// </summary>
        /// <param name="w">The width of the screen</param>
        /// <param name="h">The height of the screen</param>
        /// <param name="points">A list of points, where (0.0, 0.0) is top left, and (1.0, 1.0) is bottom right</param>
        /// <returns></returns>
        public static List<Target> PlaceTargets(double w, double h, List<V2<double>> points) {
            return (from p in points select new Target((int)(p.X * w), (int)(p.Y * h))).ToList();
        }
    }
}
