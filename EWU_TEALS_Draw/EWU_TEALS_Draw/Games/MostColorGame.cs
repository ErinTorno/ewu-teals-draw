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
        private const string TextIntro = "Starting Color Game!";
        private const string TextAddPlayer = "Press S to start!";
        // clunky way of doing this, but it works for now
        private const string TextOrder2 = "1st {0} ({1}%), 2nd {2} ({3}%)";
        private const string TextOrder3 = "1st {0} ({1}%), 2nd {2} ({3}%), 3rd {4} ({5}%)";
        private const string TextOrderTie = "It's a tied";
        private enum CGState { AddPlayer, Running, EndGame };

        private List<Player> players = new List<Player>();
        private CGState _curState = CGState.AddPlayer;
        private CGState CurState { get => _curState; set { FinalizeState(); _curState = value; InitState(); } }

        private const Keys KeyStart = Keys.S;

        private TextBox prompt;

        public MostColorGame(Form form, ImageBox canvas, TableLayoutPanel panel) : base(form, canvas) {
            prompt = new TextBox {
                ReadOnly = true,
                Text = TextIntro,
                Visible = true,
                Dock = DockStyle.Fill,
                AutoSize = true,
                TextAlign = HorizontalAlignment.Center
            };
            prompt.Font = new Font(prompt.Font.FontFamily, 24);
            panel.Controls.Add(prompt);

            players.Add(new Player("Jane", Color.FromArgb(135, 250, 250)));
            players.Add(new Player("Tom", Color.FromArgb(250, 250, 120)));
        }

        public override void Dispose() {
            base.Dispose();
            form.Controls.Remove(prompt);
        }

        public override void Update(double dT, Mat input) {
            base.Update(dT, input);
            // we don't update every frame to make it easier on the computer
            if (logicTicks % 2 == 0) {
                switch (CurState) {
                    case CGState.AddPlayer:
                        prompt.Text = TextAddPlayer;
                        break;
                    case CGState.Running:
                        var bmp = input.Bitmap;

                        // we increment by this
                        var incr = 2;
                        foreach (var p in players)
                            p.Pixels = 0;
                        // since we are skipping pixels, we need to divide the total size by the square
                        int totalPixels = (bmp.Width * bmp.Height) / (incr * incr);

                        // we sum each pixel for each player
                        for (int x = 0; x < bmp.Width; x += incr) {
                            for (int y = 0; y < bmp.Height; y += incr) {
                                var pixel = bmp.GetPixel(x, y);
                                foreach (var p in players) {
                                    if (p.InkColor == pixel) {
                                        ++p.Pixels;
                                        break;
                                    }
                                }
                            }
                        }

                        UpdatePlayerOrder(totalPixels);
                        break;
                }
            }
        }

        protected override void OnKeyPress(object sender, KeyEventArgs e) {
            base.OnKeyPress(sender, e);
            if (!ShouldYieldKeys) {
                switch (CurState) {
                    case CGState.AddPlayer:
                        switch (e.KeyCode) {
                            case KeyStart:
                                CurState = CGState.Running;
                                break;
                        }
                        break;
                }
            }
        }

        private void InitState() {
            switch (CurState) {
                case CGState.AddPlayer:

                    break;
            }
        }

        private void FinalizeState() {
            switch (CurState) {
                case CGState.AddPlayer:

                    break;
            }
        }

        private void UpdatePlayerOrder(int totalPixels) {
            Func<Player, double> toPercent = p => p.Pixels / totalPixels * 100.0;
            var order = players.OrderBy(p => p.Pixels).Reverse().ToList();
            var str = "";
            // if both in lead have same pixels, report it as a tie
            if (players.Count >= 2 && order[0].Pixels == order[1].Pixels) {
                str = TextOrderTie;
            }
            else {
                if (players.Count == 2)
                    str = String.Format(TextOrder2, order[0].Name, toPercent(order[0]), order[1].Name, toPercent(order[1]));
                else if (players.Count > 2)
                    str = String.Format(TextOrder3, order[0].Name, toPercent(order[0]), order[1].Name, toPercent(order[1]), order[2].Name, toPercent(order[2]));
            }
            prompt.Text = str;
        }

        class Player {
            public string Name { get; }
            public int Pixels { get; set; }
            public Color InkColor;

            public Player(string name, Color inkColor) {
                Name = name;
                Pixels = 0;
                InkColor = inkColor;
            }
        }
    }
}
