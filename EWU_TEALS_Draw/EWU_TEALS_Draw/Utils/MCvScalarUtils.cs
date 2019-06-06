using Emgu.CV.Structure;
using EwuTeals.Detectables;
using System;
using System.Drawing;

namespace EwuTeals.Utils {
    public static class MCvScalarUtils {
        private static double Max3(double a, double b, double c) {
            return Math.Max(a, Math.Max(b, c));
        }

        private static double Min3(double a, double b, double c) {
            return Math.Min(a, Math.Min(b, c));
        }

        public static bool IsEqualTo(this MCvScalar a, MCvScalar b) {
            return a.V0 == b.V0 && a.V1 == b.V1 && a.V2 == b.V2 && a.V3 == b.V3;
        }

        public static MCvScalar RestrictHsvRanges(this MCvScalar s) {
            var hue = s.V0 < 0.0 ? 0.0 : s.V0 > 179.0 ? 179.0 : s.V0;
            var sat = s.V1 < 0.0 ? 0.0 : s.V1 > 255.0 ? 255.0 : s.V1;
            var val = s.V2 < 0.0 ? 0.0 : s.V2 > 255.0 ? 255.0 : s.V2;
            return new MCvScalar(hue, sat, val);
        }

        public static Color ToColor(this MCvScalar bgr) {
            // for MCvScalar, 0 alpha is opaque; for color, 0 is fully transparent, and 255 is opaque
            return Color.FromArgb(255 - (int)bgr.V3, (int)bgr.V2, (int)bgr.V1, (int)bgr.V0);
        }
        
        public static DetectableColor GenerateDetectable(this MCvScalar bgrColor, string name) {
            var hsv = bgrColor.ToHsv();
            // we increase the saturation, since it appears duller on camera
            var ink = new MCvScalar(hsv.V0, Math.Min(255.0, hsv.V1 * 1.75), Math.Min(hsv.V2 * 1.75, 255.0)).ToBgr();
            var minHsv = new MCvScalar(hsv.V0 - 6.0, hsv.V1 * 0.5, 80.0);
            var maxHsv = new MCvScalar(hsv.V0 + 6.0, 255.0, 255.0);
            return new DetectableColor(name, true, ink, minHsv.RestrictHsvRanges(), maxHsv.RestrictHsvRanges());
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
