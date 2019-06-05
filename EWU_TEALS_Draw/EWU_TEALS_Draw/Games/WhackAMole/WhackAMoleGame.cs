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
    public class WhackAMoleGame : AutoFreeDrawGame {
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
        private const string TextWinnerName = "P{0}";
        private const string TextWinnerNameCombo = " and ";
        private const string TextWinnerDesc = " won with {0} points!";

        private const double TimeToShowIntro = 2.0; // in seconds
        private const double ResultsCycleTime = 4.0; // in seconds
        private const double NormalMatchLength = 20.0; // in seconds
        private int PlayerCount { get; set; }

        private static readonly MCvScalar CanvasColor = new MCvScalar(255, 255, 255);

        private const Keys KeyAccept = Keys.Enter;

        private State _curState = State.Intro;
        private State CurState { get => _curState; set { FinalizeState(); _curState = value; InitState(); } }

        // since we hold more State than our parent, we want to prevent it from serializing improperly
        public override bool CanSerialize => false;

        private ImageBox video;
        private List<Player> players = new List<Player>();
        private List<(MCvScalar color, int count)> colorCounts = new List<(MCvScalar color, int count)>();
        private List<Target> targets;
        private Player unfinishedPlayer;
        private Label score;
        private double lastSwitchTime = 0;
        private double timeRemaining = 0;

        public WhackAMoleGame(Form form, ImageBox canvas, ImageBox video, ImageBox videoGrey, TableLayoutPanel panel, int playercount) : base(form, canvas, video, videoGrey, panel) {
            this.video = video;
            this.PlayerCount = playercount;
            AutoColor.IsActive = false;
            Detectables.Clear();
            ShouldYieldKeys = false;
            ShouldDraw = false;

            var ran = new Random();
            var targetset = TargetSet.AllSets[ran.Next(TargetSet.AllSets.Count)];
            targets = TargetSet.PlaceTargets(canvas.Image.Bitmap.Width, canvas.Image.Bitmap.Height, targetset);

            score = new Label {
                BackColor = Color.White,
                ForeColor = Color.Black,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top
            };
            score.Font = new Font(score.Font.FontFamily, 24);
            panel.Controls.Add(score);
            CurState = State.Intro;
        }

        public override void Update(double dT, Mat input) {
            base.Update(dT, input);

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
                case State.Results:
                    timeRemaining -= dT;
                    if (timeRemaining <= 0)
                        timeRemaining = ResultsCycleTime;
                    UpdateResultsPrompt();
                    break;
            }
            canvas.Refresh();
        }

        public override void OnKeyPress(object sender, KeyEventArgs e) {
            base.OnKeyPress(sender, e);
            var kc = e.KeyCode;
            if (!ShouldYieldKeys) {
                switch (CurState) {
                    // color capture key times not needed, since our subclass will call it for us
                    case State.Ready:
                        if (kc == KeyAccept)
                            CurState = State.Playing;
                        break;
                    case State.Results:
                        if (kc == KeyAccept)
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
                    AutoColor.IsActive = true;
                    UpdatePrompt(String.Format(TextAddFirst, players.Count + 1));
                    unfinishedPlayer = new Player(null, null);
                    break;
                case State.AddSecondDetect:
                    AutoColor.IsActive = true;
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
            AutoColor.IsActive = false;
            UpdatePrompt(String.Empty);
            score.Text = String.Empty;
        }

        public override void Reset() {
            base.Reset();
            players.Clear();
            colorCounts.Clear();
            Detectables.Clear();
            unfinishedPlayer = null;
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
        }

        public void UpdateResultsPrompt() {
            // we only do this updating if there are more than one players; otherwise it doesn't matter to show the winner
            if (PlayerCount > 1) {
                // second half of cycle: show the winner
                if (timeRemaining > 0.5 * ResultsCycleTime) {
                    int winnerPoints = -1;
                    var winnerIs = new List<int>();
                    for (int i = 0; i < players.Count; ++i) {
                        if (players[i].Points > winnerPoints) {
                            // all in the list previously are less than this, and can be dropped
                            winnerIs.Clear();
                            winnerIs.Add(i);
                            winnerPoints = players[i].Points;
                        } else if (players[i].Points == winnerPoints) {
                            winnerIs.Add(i);
                        }
                    }
                    var str = winnerIs.Aggregate(new StringBuilder(), (acc, cur) => acc.Append(String.Format((acc.Length == 0 ? TextWinnerName : TextWinnerNameCombo + TextWinnerName), cur + 1))).ToString();
                    UpdatePrompt(String.Format(str + TextWinnerDesc, winnerPoints));
                }
                else {
                    UpdatePrompt(TextRestart);
                }
            }
            else {
                // only one player, no need to inform of winner
                UpdatePrompt(TextRestart);
            }
        }

        public void UpdateScores() {
            if (players.Count == 1) {
                score.Text = String.Format(TextPointsSP, players[0].Points);
            }
            else if (players.Count > 0) {
                var inplayers = new List<(int, Player)>(players.Count);
                for (int i = 0; i < players.Count; ++i)
                    inplayers.Add((i, players[i]));

                score.Text = inplayers
                    .OrderBy(v => v.Item2.Points)
                    .Reverse()
                    .Aggregate(new StringBuilder(), (acc, cur) => (acc.Length == 0 ? acc : acc.Append(" | ")).Append(String.Format(TextPoints, cur.Item1 + 1, cur.Item2.Points)))
                    .ToString();
            }
        }

        /// <summary>
        /// Called whenever we are adding players and the AutoColor captures a color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnColorCapture(object sender, ColorCaptureArgs args) {
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
