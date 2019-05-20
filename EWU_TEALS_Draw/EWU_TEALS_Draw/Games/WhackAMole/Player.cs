using EwuTeals.Detectables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Games.WhackAMole {
    class Player {
        public Detectable PaddleA, PaddleB;
        // the player loses if this reaches 0
        // the number of points the player has reached so far
        public int Points;

        public Player(Detectable a, Detectable b) {
            PaddleA = a;
            PaddleB = b;
        }
    }
}
