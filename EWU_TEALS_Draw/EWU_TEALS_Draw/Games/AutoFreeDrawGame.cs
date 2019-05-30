using Emgu.CV;
using Emgu.CV.UI;
using EwuTeals.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EwuTeals.Utils.AutoColor;

namespace EwuTeals.Games {
    class AutoFreeDrawGame : FreeDrawGame {
        private const string TextHowTo = "Put an object in the square and press Enter to draw with it";
        private const Keys KeyCaptureColor = Keys.Enter;

        private AutoColor autoColor;

        public AutoFreeDrawGame(Form form, ImageBox canvas, ImageBox video, ImageBox videoGrey, TableLayoutPanel panel) : base(form, canvas, videoGrey, panel) {
            autoColor = new AutoColor(video);
            autoColor.IsActive = true;
            autoColor.OnColorCapture += this.OnColorCapture;
            Detectables.Clear();
            UpdatePrompt(TextHowTo);
        }

        public override void Update(double dT, Mat input) {
            base.Update(dT, input);
            autoColor.Update(input);
        }

        public override void OnKeyPress(object sender, KeyEventArgs e) {
            base.OnKeyPress(sender, e);
            var kc = e.KeyCode;
            if (!ShouldYieldKeys && kc == KeyCaptureColor) {
                autoColor.CaptureNextUpdate = true;
            }
        }

        public override void Reset() {
            base.Reset();
            autoColor.Reset();
        }

        /// <summary>
        /// Called whenever we are adding players and the AutoColor captures a color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnColorCapture(object sender, ColorCaptureArgs args) {
            var detect = args.Color;
            Detectables.Add(detect);
        }
    }
}
