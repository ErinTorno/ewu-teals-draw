using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using EwuTeals.Detectables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EwuTeals.Games {
    class FreeDrawGame : Game {
        private ImageBox video;
        private Queue<Mat> disposableQueue = new Queue<Mat>();

        protected bool ShouldDraw { get; set; }

        public FreeDrawGame(Form form, ImageBox canvas, ImageBox video) : base(form, canvas) {
            this.video = video;
            // we want to yield, since this game does nothing with keys
            ShouldYieldKeys = false;
            // this is a simple drawing mode, with these colors in it by default
            Detectables.Add(new DetectableColor("Red", true, inkColor: new MCvScalar(60, 60, 230), minHsv: new MCvScalar(0, 125, 180), maxHsv: new MCvScalar(6, 255, 255)));
            Detectables.Add(new DetectableColor("Orange", true, inkColor: new MCvScalar(60, 140, 255), minHsv: new MCvScalar(10, 175, 65), maxHsv: new MCvScalar(18, 255, 255)));
            Detectables.Add(new DetectableColor("Yellow", true, inkColor: new MCvScalar(100, 240, 240), minHsv: new MCvScalar(19, 50, 195), maxHsv: new MCvScalar(35, 255, 255)));
            Detectables.Add(new DetectableColor("Green", true, inkColor: new MCvScalar(135, 230, 135), minHsv: new MCvScalar(70, 70, 75), maxHsv: new MCvScalar(95, 255, 255)));
            Detectables.Add(new DetectableColor("Blue", true, inkColor: new MCvScalar(255, 140, 185), minHsv: new MCvScalar(99, 111, 66), maxHsv: new MCvScalar(117, 255, 255)));
            Detectables.Add(new DetectableColor("Purple", true, inkColor: new MCvScalar(255, 135, 135), minHsv: new MCvScalar(125, 100, 100), maxHsv: new MCvScalar(140, 255, 255)));
            Detectables.Add(new DetectableColor("Special", false, inkColor: new MCvScalar(0, 0, 0), minHsv: new MCvScalar(0, 0, 0), maxHsv: new MCvScalar(180, 255, 255)));
        }

        public override void Update(double dT, Mat input) {
            base.Update(dT, input);

            var combinedThreshImage = Mat.Zeros(input.Rows, input.Cols, DepthType.Cv8U, 1);
            disposableQueue.Enqueue(combinedThreshImage);

            foreach (var d in Detectables) {
                if (d.IsEnabled) {
                    Mat curThresh = d.Draw(canvas, input, drawOnCanvas: ShouldDraw);
                    CvInvoke.Add(curThresh, combinedThreshImage, combinedThreshImage);
                }
            }

            video.Image = combinedThreshImage;

            if (disposableQueue.Count > 4)
                disposableQueue.Dequeue().Dispose();
        }

        public override void Reset() {
            var s = canvas.Image.Size;
            canvas.Image.Dispose();
            canvas.Image = new Image<Bgr, byte>(s.Width, canvas.Height, new Bgr(255, 255, 255));
        }

        public override void Quit()
        {
            foreach (Detectable d in Detectables)
            {
                d.IsEnabled = false;
            }
            Reset();
        }
    }
}
