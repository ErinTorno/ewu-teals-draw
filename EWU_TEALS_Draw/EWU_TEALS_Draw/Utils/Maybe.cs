using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Utils {
    public static class MaybeExtensions {
        /// <summary>
        /// Lifts a value into a maybe, with nulls becoming Nothing, and variables becoming Just
        /// </summary>
        /// <typeparam name="T">The type of variable the maybe should contain</typeparam>
        /// <param name="val">The value to be placed within it</param>
        /// <returns>The value wrapped in a maybe</returns>
        public static Maybe<T> ToMaybe<T>(this T val) {
            if (val == null)
                return Maybe<T>.Empty;
            else
                return Maybe<T>.Of(val);
        }

        public static IEnumerable<T> CatMaybe<T>(this IEnumerable<Maybe<T>> inputs) {
            return from m in inputs where m.IsPresent select m.Value;
        }
    }

    /// <summary>
    /// A class that encapsulates a value that must be either present, or empty
    /// </summary>
    /// <typeparam name="T">The type of object that is held</typeparam>
    public abstract class Maybe<T> {
        // access

        public abstract bool IsPresent { get; }

        public abstract T Value { get; }

        public T FromMaybe() {
            return Value;
        }

        public T GetOrElse(T def) {
            if (IsPresent)
                return Value;
            else
                return def;
        }

        // creation

        public static Maybe<T> Empty = new NothingM<T>();

        public static Maybe<T> Nothing = Empty;

        public static Maybe<T> Of(T val) {
            return new JustM<T>(val);
        }

        public static Maybe<T> Just(T val) {
            return Of(val);
        }

        // modification

        public abstract Maybe<TOutput> Map<TOutput>(Func<T, TOutput> fn);

        // inner classes

        class JustM<TInner> : Maybe<TInner> {
            private readonly TInner _val;

            public JustM(TInner val) {
                _val = val;
            }

            public override bool IsPresent { get => true; }

            public override TInner Value { get => _val; }

            public override Maybe<TOutput> Map<TOutput>(Func<TInner, TOutput> fn) {
                return new JustM<TOutput>(fn(_val));
            }
        }

        class NothingM<TInner> : Maybe<TInner> {
            public NothingM() { }

            public override bool IsPresent { get => false; }

            public override TInner Value { get => throw new InvalidOperationException("Cannot retrieve value from Nothing"); }

            public override Maybe<TOutput> Map<TOutput>(Func<TInner, TOutput> fn) {
                return new NothingM<TOutput>();
            }
        }
    }
}
