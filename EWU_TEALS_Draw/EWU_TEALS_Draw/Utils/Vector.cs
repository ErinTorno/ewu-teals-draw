using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Utils {
    public struct V2<A> {
        public A X;
        public A Y;

        public V2(A x, A y) {
            X = x;
            Y = y;
        }
    }

    public struct V3<A> {
        public A X;
        public A Y;
        public A Z;

        public V3(A x, A y, A z) {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
