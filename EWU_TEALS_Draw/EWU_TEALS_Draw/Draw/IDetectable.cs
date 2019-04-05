using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Draw {
    interface IDetectable {
        bool IsEnabled { get; set; }

        MCvScalar InkColor { get; set; }
    }
}
