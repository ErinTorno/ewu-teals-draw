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
        private const double PaddleMarkerRadiusPercent = 0.025;

        public Detectable PaddleA, PaddleB;
        // the player loses if this reaches 0
        // the number of points the player has reached so far
        public int Points;

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
