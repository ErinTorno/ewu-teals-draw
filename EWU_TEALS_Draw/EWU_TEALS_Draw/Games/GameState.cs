using Emgu.CV;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EwuTeals.Draw.Game {
   abstract class GameState : IDisposable {
        private const double SecondsToAllowToggle = 0.5; // a half second

        /// <summary>
        /// If true, all key inputs will be ignored so that the container for this can address them
        /// </summary>
        public Boolean ShouldYieldKeys { get; private set; }
        public Keys ToggleYieldKey = Keys.Space;

        protected double logicTime = 0;
        protected int logicTicks = 0;
        protected ImageBox canvas;
        protected Form form;
        // this key, when pressed, will toggle ShouldYieldKeys
        private double lastToggleTime = 0.0;

        protected GameState(Form form, ImageBox canvas) {
            this.canvas = canvas;
            ShouldYieldKeys = true;
            this.form = form;
            form.KeyDown += OnKeyPress;
        }

        public virtual void Dispose() {
            form.KeyDown -= OnKeyPress;
        }

        protected virtual void OnKeyPress(object sender, KeyEventArgs e) {
            if (e.KeyCode == ToggleYieldKey && logicTime >= lastToggleTime + SecondsToAllowToggle) {
                lastToggleTime = logicTime;
                ShouldYieldKeys = !ShouldYieldKeys;
            }
        }

        public virtual void Update(double dT, Mat input) {
            ++logicTicks;
            logicTime += dT;
        }
    }
}
