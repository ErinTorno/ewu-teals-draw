﻿using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Draw {
    public struct ColorRange : IEquatable<ColorRange> {
        // The color actually drawn on the canvas, in BGR
        public MCvScalar InkColor;
        // These are in HSV
        public MCvScalar MinHsvColor;
        public MCvScalar MaxHsvColor;

        public ColorRange(MCvScalar inkColor, MCvScalar minHsvColor, MCvScalar maxHsvColor) {
            InkColor = inkColor;
            MinHsvColor = minHsvColor;
            MaxHsvColor = maxHsvColor;
        }

        public bool Equals(ColorRange other) {
            return ScalarEq(this.InkColor, other.InkColor)
                && ScalarEq(this.MinHsvColor, other.MinHsvColor)
                && ScalarEq(this.MaxHsvColor, other.MaxHsvColor);
        }

        public override bool Equals(object obj) {
            return obj is ColorRange && this.Equals((ColorRange)obj);
        }

        public override int GetHashCode() {
            return ScalarHash(InkColor) + ScalarHash(MinHsvColor) + ScalarHash(MaxHsvColor);
        }

        public static bool operator ==(ColorRange a, ColorRange b) {
            return a.Equals(b);
        }

        public static bool operator !=(ColorRange a, ColorRange b) {
            return !a.Equals(b);
        }

        // Scalar utility functions

        private static bool ScalarEq(MCvScalar a, MCvScalar b) {
            return a.V0 == b.V0 && a.V1 == b.V1 && a.V2 == b.V2 && a.V3 == b.V3;
        }

        private static int ScalarHash(MCvScalar sc) {
            return sc.V0.GetHashCode() + sc.V1.GetHashCode() + sc.V2.GetHashCode() + sc.V3.GetHashCode();
        }
    }
}