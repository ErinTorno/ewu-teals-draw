using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Draw {
    static class MCvScalarUtils {
        private static double Max3(double a, double b, double c) {
            return Math.Max(a, Math.Max(b, c));
        }

        private static double Min3(double a, double b, double c) {
            return Math.Min(a, Math.Min(b, c));
        }

        public static MCvScalar RestrictHsvRanges(this MCvScalar s) {
            var hue = s.V0 < 0.0 ? 0.0 : s.V0 > 179.0 ? 179.0 : s.V0;
            var sat = s.V1 < 0.0 ? 0.0 : s.V1 > 255.0 ? 255.0 : s.V1;
            var val = s.V2 < 0.0 ? 0.0 : s.V2 > 255.0 ? 255.0 : s.V2;
            return new MCvScalar(hue, sat, val);
        }

        public static MCvScalar ToHsv(this MCvScalar bgr) {
            var blue = bgr.V0;
            var green = bgr.V1;
            var red = bgr.V2;

            var max = Max3(red, green, blue);
            var min = Min3(red, green, blue);
            var nblue = blue / 255.0;
            var ngreen = green / 255.0;
            var nred = red / 255.0;
            var delta = Max3(nred, ngreen, nblue) - Min3(nred, ngreen, nblue);

            var hue = 0.0;
            if (delta == 0.0)
                hue = 0.0;
            else if (max == red)
                hue = (ngreen - nblue) / delta;
            else if (max == green)
                hue = 2.0 + (nblue - nred) / delta;
            else
                hue = 4.0 + (nred - ngreen) / delta;
            hue *= 30.0; // not 60.0, since Emgu CV uses a range of 0 - 180
            hue = hue < 0.0 ? hue + 180.0 : hue;
            var saturation = max == 0.0 ? 0.0 : 1.0 - (min / max);
            return new MCvScalar(hue, saturation * 255.0, max);
        }

        public static MCvScalar ToBgr(this MCvScalar hsv) {
            var hue = hsv.V0;
            var saturation = hsv.V1 / 255.0;
            var value = hsv.V2 / 255.0;

            var hi = (int)(hue / 30.0) % 6;
            var f = hue / 30.0 - (int)(hue / 30.0);

            var v = value * 255.0;
            var p = value * (1.0 - saturation) * 255.0;
            var q = value * (1.0 - f * saturation) * 255.0;
            var t = value * (1.0 - (1.0 - f) * saturation) * 255.0;
            switch (hi) {
                case 0: return new MCvScalar(p, t, v);
                case 1: return new MCvScalar(p, v, q);
                case 2: return new MCvScalar(t, v, p);
                case 3: return new MCvScalar(v, q, p);
                case 4: return new MCvScalar(v, p, t);
                default: return new MCvScalar(q, p, v);
            }
        }
    }
}
