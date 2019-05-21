using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using EwuTeals.Detectables;
using EwuTeals.Utils;
using static EwuTeals.Utils.AutoColor;

namespace EwuTeals.Games {
    class MostColorGame : Game {
        private const int MinPlayers = 2;
        private const string TextIntro = "Starting Color Game!";
        private const string TextAddPlayerNone      = "Fill the box with a color; press P to add as a player";
        private const string TextAddPlayerLessThan2 = "Press P to add another player!";
        private const string TextAddPlayerReady     = "Added player! Press P to add another, or S to start";
        private enum CGState { AddPlayer, Running, EndGame };

        private List<Player> players = new List<Player>();
        private CGState _curState = CGState.AddPlayer;
        private CGState CurState { get => _curState; set { FinalizeState(); _curState = value; InitState(); } }

        private const Keys KeyStart = Keys.S;
        private const Keys KeyAddPlayer = Keys.P;

        private TableLayoutPanel table;
        private TextBox prompt;
        private AutoColor autoColor;

        public MostColorGame(Form form, ImageBox canvas, ImageBox video, TableLayoutPanel panel) : base(form, canvas) {
            this.autoColor = new AutoColor(video);

            prompt = new TextBox {
                ReadOnly = true,
                Text = TextIntro,
                Visible = true,
                Dock = DockStyle.Fill,
                AutoSize = true,
                TextAlign = HorizontalAlignment.Center
            };
            prompt.Font = new Font(prompt.Font.FontFamily, 24);

            table = new TableLayoutPanel {
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            table.Controls.Add(prompt, 0, 1);
            panel.Controls.Add(table);

            CurState = CGState.AddPlayer;
        }

        public override void Dispose() {
            base.Dispose();
            form.Controls.Remove(prompt);
        }

        public override void Update(double dT, Mat input) {
            base.Update(dT, input);
            autoColor.Update(input);

            // we don't update every frame to make it easier on the computer
            if (logicTicks % 2 == 0) {
                switch (CurState) {
                    case CGState.AddPlayer:
                        switch (players.Count) {
                            case 0:
                                UpdatePrompt(TextAddPlayerNone); break;
                            case int n when n < MinPlayers:
                                UpdatePrompt(TextAddPlayerLessThan2); break;
                            default:
                                UpdatePrompt(TextAddPlayerReady); break;
                        }

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

        private void UpdatePlayerOrder(int totalPixels) {
            var percents = from p in players select p.Pixels / (double)totalPixels;
            var text = percents.Aggregate("", (acc, cur) => cur + " " + acc);
            UpdatePrompt(text);
        }

        protected override void OnKeyPress(object sender, KeyEventArgs e) {
            base.OnKeyPress(sender, e);
            if (!ShouldYieldKeys) {
                switch (CurState) {
                    case CGState.AddPlayer:
                        switch (e.KeyCode) {
                            case KeyStart:
                                // we only let it change if we have enough players
                                if (players.Count >= MinPlayers)
                                    CurState = CGState.Running;
                                break;

                            case KeyAddPlayer:
                                autoColor.CaptureNextUpdate = true;
                                break;
                        }
                        break;
                }
            }
        }

        private void InitState() {
            // we always will wish to clear the prompt
            UpdatePrompt(String.Empty);
            switch (CurState) {
                case CGState.AddPlayer:
                    // we clear the detectables, as we need fresh ones from the new players
                    Detectables.Clear();
                    autoColor.IsActive = true;
                    autoColor.OnColorCapture += OnColorCapture;

                    break;
            }
        }

        private void FinalizeState() {
            switch (CurState) {
                case CGState.AddPlayer:
                    autoColor.IsActive = false;
                    autoColor.OnColorCapture -= OnColorCapture;
                    break;
            }
        }

        /// <summary>
        /// Called whenever we are adding players and the AutoColor captures a color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnColorCapture(object sender, ColorCaptureArgs args) {
            var detect = args.Color;
            var ink = detect.InkColor.ToColor();
            players.Add(new Player(ink));
            Detectables.Add(detect);
            
            var content = new TableLayoutPanel {
                RowCount = players.Count,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };
            for (int i = 0; i < players.Count; ++i) {
                var player = players[i];
                var bitmap = new Bitmap(256, 256);
                using (var g = Graphics.FromImage(bitmap)) {
                    g.Clear(player.InkColor);
                }

                // for some reason none of these are displaying, but they are still adjusting the spacing of other elements
                var picture = new PictureBox {
                    InitialImage = bitmap,
                    Dock = DockStyle.Fill,
                    Visible = true
                };
            }
            SetGuiContent(content);
        }

        private void UpdatePrompt(String t) {
            prompt.Text = t;
        }
        
        private void SetGuiContent(Control c) {
            table.Controls.Clear();
            table.Controls.Add(prompt, 0, 1);
            table.Controls.Add(c, 0, 0);
        }

        public override void DisplayIntro()
        {
            throw new NotImplementedException();
        }

        public override void DetectColor()
        {
            throw new NotImplementedException();
        }

        public override void PromptAddPlayer()
        {
            throw new NotImplementedException();
        }

        public override void DisplayResults()
        {
            throw new NotImplementedException();
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }

        class Player {
            public int Pixels { get; set; }
            public Color InkColor;

            public Player(Color inkColor) {
                Pixels = 0;
                InkColor = inkColor;
            }
        }
    }
}
