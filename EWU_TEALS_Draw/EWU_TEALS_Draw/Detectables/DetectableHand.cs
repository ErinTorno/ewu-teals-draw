using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace EwuTeals.Draw {
    class DetectableHand : Detectable {
        private Mat background;

        public DetectableHand (string name, bool isEnabled, MCvScalar inkColor) : base(name) {
            IsEnabled = isEnabled;
            InkColor = inkColor;
        }

        public override Mat Draw(ImageBox canvas, Mat videoCapture) {
            if (background != null)
            {

            }

            throw new NotImplementedException();
        }

        public override void ResetLastPosition() {
            throw new NotImplementedException();
        }
    }
}
