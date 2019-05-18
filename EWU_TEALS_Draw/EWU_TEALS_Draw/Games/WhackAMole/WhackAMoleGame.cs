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
using System.Windows.Forms;
using static EwuTeals.Utils.AutoColor;

namespace EwuTeals.Games.WhackAMole {
    /// <summary>
    /// A game where in two players will move their paddle between different points on the field
    /// </summary>
    class WhackAMoleGame : FreeDrawGame {
        private const string TextIntro = "Welcome to Whack-a-mole!";
        private const string TextAddFirst = "Player {0}, put first object in center of box and press Enter";
        private const string TextAddSecond = "Player {0}, put second object in center of box and press Enter";
        private const string TextReady = "Press Enter to start the game";

        private const double TimeToShowIntro = 2.0; // in seconds
        private const double TargetRadiusPercent = 0.1; // mult by height to get radius
        private const int PlayerCount = 2;

        private const Keys KeyCaptureColor = Keys.Enter;
        private enum State { Intro, AddFirstDetect, AddSecondDetect, Ready, Playing, Results }

        private State _curState = State.Intro;
        private State CurState { get => _curState; set { FinalizeState(); _curState = value; InitState(); } }

        private AutoColor autoColor;
        private List<Player> players = new List<Player>();
        private List<Target> targets;
        private Player unfinishedPlayer;
        private TextBox prompt;
        private double lastSwitchTime = 0;

        public WhackAMoleGame(Form form, ImageBox canvas, ImageBox video, ImageBox videoGrey, TableLayoutPanel panel) : base(form, canvas, videoGrey) {
            autoColor = new AutoColor(video);
            autoColor.IsActive = false;
            autoColor.OnColorCapture += this.OnColorCapture;
            Detectables.Clear();
            ShouldYieldKeys = false;

            targets = TargetSet.PlaceTargets(canvas.Width, canvas.Height, TargetSet.Default);

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
        }

        public override void Update(double dT, Mat input) {
            base.Update(dT, input);
            autoColor.Update(input);

            // we draw targets every state, so during set up people can prepare for the game
            var tarRadis = (int)(TargetRadiusPercent * canvas.Height);
            foreach (var t in targets) {
                CvInvoke.Circle(canvas.Image, t.Position, tarRadis, t.InkColor, -1);
            }

            switch (CurState) {
                case State.Intro:
                    // once timer has ended, we switch to adding players
                    if (lastSwitchTime + TimeToShowIntro < logicTime)
                        CurState = State.AddFirstDetect;
                    break;
                case State.Playing:
                    foreach (var t in targets) {
                        var isExpired = t.Update(dT);
                        if (isExpired)
                            t.Color = GetNewTargetColor().ToMaybe();
                    }
                    break;
            }
            canvas.Refresh();
        }

        private MCvScalar GetNewTargetColor() {
            throw new NotImplementedException();
        }

        protected override void OnKeyPress(object sender, KeyEventArgs e) {
            base.OnKeyPress(sender, e);
            var kc = e.KeyCode;
            if (!ShouldYieldKeys) {
                switch (CurState) {
                    case State.AddFirstDetect:
                    case State.AddSecondDetect:
                        if (kc == KeyCaptureColor)
                            autoColor.CaptureNextUpdate = true;
                        break;
                    case State.Ready:
                        if (kc == KeyCaptureColor)
                            CurState = State.Playing;
                        break;
                }
            }
        }

        private void InitState() {
            switch (CurState) {
                case State.Intro:
                    UpdatePrompt(TextIntro);
                    lastSwitchTime = logicTime;
                    break;
                case State.AddFirstDetect:
                    autoColor.IsActive = true;
                    UpdatePrompt(String.Format(TextAddFirst, players.Count + 1));
                    unfinishedPlayer = new Player(null, null);
                    break;
                case State.AddSecondDetect:
                    autoColor.IsActive = true;
                    // assumes you must have just been in AddFirstDetected State
                    UpdatePrompt(String.Format(TextAddSecond, players.Count + 1));
                    break;
                case State.Ready:
                    UpdatePrompt(TextReady);
                    break;
            }
        }

        private void FinalizeState() {
            autoColor.IsActive = false;
            UpdatePrompt(String.Empty);
            switch (CurState) {
                case State.Intro:
                    break;
            }
        }

        private void UpdatePrompt(string msg) {
            prompt.Text = msg;
        }


        /// <summary>
        /// Called whenever we are adding players and the AutoColor captures a color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnColorCapture(object sender, ColorCaptureArgs args) {
            var detect = args.Color;
            var ink = detect.InkColor.ToColor();
            if (unfinishedPlayer != null) {
                if (unfinishedPlayer.PaddleA == null) {
                    unfinishedPlayer.PaddleA = detect;
                    // we now want to add the second paddle
                    CurState = State.AddSecondDetect;
                }
                else if (unfinishedPlayer.PaddleB == null) {
                    // we either move on to the next player, or go to ready
                    unfinishedPlayer.PaddleB = detect;
                    players.Add(unfinishedPlayer);
                    Detectables.Add(unfinishedPlayer.PaddleA);
                    Detectables.Add(unfinishedPlayer.PaddleB);
                    unfinishedPlayer = null;
                    if (players.Count == PlayerCount)
                        CurState = State.Ready;
                    else
                        CurState = State.AddFirstDetect;
                }
            }
        }

        private class Player {
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
}
