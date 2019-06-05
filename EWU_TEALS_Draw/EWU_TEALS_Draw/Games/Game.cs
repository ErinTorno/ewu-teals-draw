using Emgu.CV;
using Emgu.CV.UI;
using EwuTeals.Detectables;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace EwuTeals.Games {
   public abstract class Game : IDisposable {
        private const double SecondsToAllowToggle = 0.5; // a half second

        /// <summary>
        /// If true, all key inputs will be ignored so that the container for this can address them
        /// </summary>
        public Boolean ShouldYieldKeys { get; protected set; }
        // this key, when pressed, will toggle ShouldYieldKeys
        public Keys ToggleYieldKey = Keys.Space;

        public ObservableCollection<Detectable> Detectables { get; protected set; }
        
        // the time (in seconds) that have passed since the game started
        protected double logicTime = 0;
        // the number of updates performed since the game started
        protected int logicTicks = 0;
        protected ImageBox canvas;
        protected Form form;
        // last time we toggle the controls; if too low, we won't toggle yet to prevent fast back and forth toggling
        private double lastToggleTime = 0.0;

        private TextBox prompt;
        private TableLayoutPanel panel;

        protected Game(Form form, ImageBox canvas, TableLayoutPanel panel) {
            this.canvas = canvas;
            this.panel = panel;
            ShouldYieldKeys = false;
            this.form = form;
            form.KeyDown += OnKeyPress;
            Detectables = new ObservableCollection<Detectable>();

            prompt = new TextBox {
                ReadOnly = true,
                Text = "Empty Prompt",
                Visible = true,
                Dock = DockStyle.Fill,
                AutoSize = true,
                TextAlign = HorizontalAlignment.Center
            };
            prompt.Font = new Font(prompt.Font.FontFamily, 24);
            panel.Controls.Add(prompt);
        }

        public abstract void Reset();

        public virtual void Pause() {
            // we reset each of these to prevent weird line issues when unpausing at far away locations
            foreach (var d in Detectables) d.ResetLastPosition();
        }

        public virtual void Quit() {
            panel.Controls.Clear();
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool callFromDispose) {
            form.KeyDown -= OnKeyPress;
        }

        public virtual void OnKeyPress(object sender, KeyEventArgs e) {
            if (e.KeyCode == ToggleYieldKey && logicTime >= lastToggleTime + SecondsToAllowToggle) {
                lastToggleTime = logicTime;
                ShouldYieldKeys = !ShouldYieldKeys;
            }
        }

        public virtual void Update(double dT, Mat input) {
            ++logicTicks;
            logicTime += dT;
        }

        // if we could work more, this would likely be changed to a Serialize method of some kind, rather than relying on the MainForm to do it for us
        public virtual bool CanSerialize { get => false; }
        
        protected void UpdatePrompt(string msg) {
            prompt.Text = msg;
        }
    }
}
