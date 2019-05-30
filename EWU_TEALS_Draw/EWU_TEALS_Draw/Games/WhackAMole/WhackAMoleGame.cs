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
        // these describe how the state of the game flows, and controls what behaviors the game is doing
        // Intro -> (AddFirstDetect <-> AddSecondDetect) -> Ready -> Playing -> Results
        private enum State { Intro, AddFirstDetect, AddSecondDetect, Ready, Playing, Results }

        private const string TextIntro = "Welcome to Whack-a-mole!";
        private const string TextAddFirst = "Player {0}, put first object in center of box and press Enter";
        private const string TextAddSecond = "Player {0}, put second object in center of box and press Enter";
        private const string TextReady = "Press Enter to start the game";
        private const string TextTimeRemaining = "Time Remaining {0:0.00} seconds";
        private const string TextRestart = "Press Enter to play again!";
        private const string TextPointsSP = "{0} point(s)";
        private const string TextPoints = "P{0}: {1} point(s)";

        private const double TimeToShowIntro = 2.0; // in seconds
        private const double ResultsCycleTime = 4.0; // in seconds
        private const double NormalMatchLength = 20.0; // in seconds
        private const int PlayerCount = 2;

        private static readonly MCvScalar CanvasColor = new MCvScalar(255, 255, 255);

        private const Keys KeyCaptureColor = Keys.Enter;

        private State _curState = State.Intro;
        private State CurState { get => _curState; set { FinalizeState(); _curState = value; InitState(); } }

        private ImageBox video;
        private AutoColor autoColor;
        private List<Player> players = new List<Player>();
        private List<(MCvScalar color, int count)> colorCounts = new List<(MCvScalar color, int count)>();
        private List<Target> targets;
        private Player unfinishedPlayer;
        private TextBox prompt;
        private Label score;
        private TableLayoutPanel panel;
        private double lastSwitchTime = 0;
        private double timeRemaining = 0;

        public WhackAMoleGame(Form form, ImageBox canvas, ImageBox video, ImageBox videoGrey, TableLayoutPanel panel) : base(form, canvas, videoGrey) {
            this.video = video;
            this.panel = panel;
            autoColor = new AutoColor(video);
            autoColor.IsActive = false;
            autoColor.OnColorCapture += this.OnColorCapture;
            Detectables.Clear();
            ShouldYieldKeys = false;
            ShouldDraw = false;

            targets = TargetSet.PlaceTargets(canvas.Image.Bitmap.Width, canvas.Image.Bitmap.Height, TargetSet.Default);

            prompt = new TextBox {
                ReadOnly = true,
                Text = TextIntro,
                Visible = true,
                Dock = DockStyle.Fill,
                AutoSize = true,
                TextAlign = HorizontalAlignment.Center
            };
            prompt.Font = new Font(prompt.Font.FontFamily, 24);
            score = new Label {
                BackColor = Color.White,
                ForeColor = Color.Black,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top
            };
            score.Font = new Font(prompt.Font.FontFamily, 24);
            panel.Controls.Add(prompt);
            panel.Controls.Add(score);
        }

        public override void Update(double dT, Mat input) {
            base.Update(dT, input);
            autoColor.Update(input);

            // For some unknowable reason, calling .Rectangle does nothing, so we draw a huge circle instead
            // alternatives tried: calling SetPixel through a for loop, which takes 30 seconds to run a single frame
            // CvInvoke.Rectangle(canvas.Image, new Rectangle(0, 0, canvas.Width, canvas.Height), CanvasColor);
            CvInvoke.Circle(canvas.Image, new Point(canvas.Width / 2, canvas.Height / 2), canvas.Width, CanvasColor, -1);

            // we draw targets every state, so during set up people can prepare for the game
            foreach (var t in targets)
                t.Draw(canvas);
            foreach (var p in players)
                p.Draw(canvas);

            switch (CurState) {
                case State.Intro:
                    // once timer has ended, we switch to adding players
                    if (lastSwitchTime + TimeToShowIntro < logicTime)
                        CurState = State.AddFirstDetect;
                    break;
                case State.Playing:
                    if (timeRemaining > 0) {
                        timeRemaining -= dT;
                        foreach (var t in targets) {
                            t.Update(dT, players, colorCounts);
                        }
                        UpdatePrompt(String.Format(TextTimeRemaining, timeRemaining));
                    }
                    else {
                        CurState = State.Results;
                    }
                    UpdateScores();
                    break;
            }
            canvas.Refresh();
        }

        public override void OnKeyPress(object sender, KeyEventArgs e) {
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
                    case State.Results:
                        if (kc == KeyCaptureColor)
                            Reset();
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
                case State.Playing:
                    timeRemaining = NormalMatchLength;
                    colorCounts = new List<(MCvScalar, int)>(players.Count * 2);
                    foreach (var t in targets) t.TimeRemaining = 0;
                    foreach (var p in players) {
                        colorCounts.Add((p.PaddleA.InkColor, 0));
                        colorCounts.Add((p.PaddleB.InkColor, 0));
                    }
                    UpdateScores();
                    break;
                case State.Results:
                    UpdatePrompt(TextRestart);
                    break;
            }
        }

        private void FinalizeState() {
            autoColor.IsActive = false;
            UpdatePrompt(String.Empty);
            score.Text = String.Empty;
        }

        public override void Reset() {
            base.Reset();
            players.Clear();
            colorCounts.Clear();
            Detectables.Clear();
            unfinishedPlayer = null;
            autoColor.Reset();
            foreach (var t in targets) {
                t.TimeRemaining = 0;
                t.Color = Maybe<MCvScalar>.Nothing;
            }

            CurState = State.Intro;
            ShouldYieldKeys = false;
        }

        public override void Quit() {
            base.Quit();
            players.Clear();
            colorCounts.Clear();
            Detectables.Clear();
            unfinishedPlayer = null;
            autoColor.Reset();
            panel.Controls.Clear();
        }

        private void UpdatePrompt(string msg) {
            prompt.Text = msg;
        }

        public void UpdateScores() {
            if (players.Count == 1) {
                score.Text = String.Format(TextPointsSP, players[0].Points);
            }
            else if (players.Count > 0) {
                var sortedPlayers = players.OrderBy(p => p.Points).Reverse().ToList();
                var sb = new StringBuilder();
                for (int i = 0; i < sortedPlayers.Count(); ++i) {
                    var p = sortedPlayers[i];
                    // we append a bar between for more players
                    if (i > 0)
                        sb.Append(" | ");
                    sb.Append(String.Format(TextPoints, i, p.Points));
                }

                score.Text = sb.ToString();
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
    }
}
