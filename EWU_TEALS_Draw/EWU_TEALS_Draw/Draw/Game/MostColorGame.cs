using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace EwuTeals.Draw.Game {

    class MostColorGame : GameState {
        private List<Player> players = new List<Player>();

        public MostColorGame(Form form, ImageBox canvas, ImageBox videoBox) : base(form, canvas, videoBox) {
        }

        public override void Update(int dT, Mat input) {
            base.Update(dT, input);
            var bmp = input.Bitmap;
            foreach (var p in players)
                p.Pixels = 0;

            // we sum each pixel for each player
            for (int x = 0; x < bmp.Width; ++x) {
                for (int y = 0; y < bmp.Height; ++y) {
                    var pixel = bmp.GetPixel(x, y);
                    foreach (var p in players) {
                        if (p.InkColor == pixel) {
                            ++p.Pixels;
                            break;
                        }
                    }
                }
            }
        }

        public override void Render(int dT) {
            base.Render(dT);
        }

        class Player {
            public string Name { get; }
            public int Pixels { get; set; }
            public Color InkColor;

            public Player(string name, int pixels, Color inkColor) {
                Name = name;
                Pixels = pixels;
                InkColor = inkColor;
            }
        }
    }
}
