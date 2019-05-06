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
        private CascadeClassifier FaceDetector;

        public DetectableHand (string name, bool isEnabled, MCvScalar inkColor) : base(name) {
            IsEnabled = isEnabled;
            InkColor = inkColor;
            FaceDetector = new CascadeClassifier(@"../../HaarCascades/haarcascade_lefteye_2splits.xml");
        }

        public override Mat Draw(ImageBox canvas, Mat videoCapture) {
            if (background != null && IsEnabled)
            {
                Mat grayscale = CvInvoke.CvtColor(videoCapture, /*RGB to Gray convertion*/);
                Mat movement = CvInvoke.AbsDiff(grayscale.GetInputArray, background.GetInputArray); //TODO need to add an output Array for some reason
                base.threshMat = CvInvoke.Threshold(/*input*/, /*output*/, 25, 255, /*Thresh binary*/);


            }

            throw new NotImplementedException();
        }

        public override void ResetLastPosition() {
            throw new NotImplementedException();
        }
    }
}
