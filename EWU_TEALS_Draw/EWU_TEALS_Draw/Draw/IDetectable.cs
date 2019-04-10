using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Draw {
    interface IDetectable {
        bool IsEnabled { get; set; }

        MCvScalar InkColor { get; set; }

        string Name { get; }

        (Mat, Point) Detect(Mat input);
    }
}
