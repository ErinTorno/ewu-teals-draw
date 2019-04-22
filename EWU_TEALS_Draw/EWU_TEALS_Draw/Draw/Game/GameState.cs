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
        private const int TicksToAllowToggle = 15; // a half second, as we run at 30 fps right now

        /// <summary>
        /// If true, all key inputs will be ignored so that the container for this can address them
        /// </summary>
        public Boolean ShouldYieldKeys { get; set; }

        protected int logicTime = 0, renderTime = 0;
        protected ImageBox canvas, videoBox;
        protected Form form;
        // this key, when pressed, will toggle ShouldYieldKeys
        protected Keys yieldKeysKey = Keys.Tab;
        private int lastToggleTick = 0;

        protected GameState(Form form, ImageBox canvas, ImageBox videoBox) {
            this.canvas = canvas;
            this.videoBox = videoBox;
            ShouldYieldKeys = true;
            this.form = form;
            form.KeyDown += OnKeyPress;
        }

        public void Dispose() {
            form.KeyDown -= OnKeyPress;
        }

        protected virtual void OnKeyPress(object sender, KeyEventArgs e) {
            if (e.KeyCode == yieldKeysKey && logicTime >= lastToggleTick + TicksToAllowToggle) {
                lastToggleTick = logicTime;
                ShouldYieldKeys = !ShouldYieldKeys;
            }
        }

        public virtual void Update(int dT, Mat input) {
            logicTime += dT;
        }

        public virtual void Render(int dT) {
            renderTime += dT;
        }
    }
}
