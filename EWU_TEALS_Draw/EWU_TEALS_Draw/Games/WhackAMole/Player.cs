using Emgu.CV;
using Emgu.CV.UI;
using EwuTeals.Detectables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.WhackAMole {
    public class Player {
        // the percent of the canvas's height to use as the radius of a paddle's dot
        // this is used to display where the paddle is currently at on the canvas
        private const double PaddleMarkerRadiusPercent = 0.025;

        public Detectable PaddleA { get; set; }
        public Detectable PaddleB { get; set; }
        // the player loses if this reaches 0
        // the number of points the player has reached so far
        public int Points { get; set; }

        public Player(Detectable a, Detectable b) {
            PaddleA = a;
            PaddleB = b;
        }

        public void Draw(ImageBox canvas) {
            var paddleRad = (int)(PaddleMarkerRadiusPercent * canvas.Height);
            CvInvoke.Circle(canvas.Image, PaddleA.LastPosition, paddleRad, PaddleA.InkColor, -1);
            CvInvoke.Circle(canvas.Image, PaddleB.LastPosition, paddleRad, PaddleB.InkColor, -1);
        }
    }
}
