using Emgu.CV;
using Emgu.CV.Structure;
using EWU_TEALS_Draw;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Draw {
    public struct HsvConfig {
        public bool IsEnabled;
        public MCvScalar InkColor;
        public MCvScalar MinHsv;
        public MCvScalar MaxHsv;
        public IInputArray MinHsvRange { get => new ScalarArray(MinHsv); }
        public IInputArray MaxHsvRange { get => new ScalarArray(MaxHsv); }

        public HsvConfig(bool isEnabled, MCvScalar inkColor, MCvScalar minHsv, MCvScalar maxHsv) {
            IsEnabled = isEnabled;
            InkColor = inkColor;
            MinHsv = minHsv;
            MaxHsv = maxHsv;
        }
    }
}
