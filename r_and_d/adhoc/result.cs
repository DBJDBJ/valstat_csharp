// #define LEGACY
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static dbj.notmacros;

namespace dbj
{
    // https://github.com/dotnet/roslyn/blob/main/docs/features/tuples.md
    // https://stackoverflow.com/q/54577827/10870835
    public class Result<V, S>
    {
        public Result(V data) => Ok = (true, data);
        public Result(S data) => Error = (true, data);
        public (bool, V) Ok { get; }
        public (bool, S) Error { get; }

    } // Result

    public record Outcome<V, S>
    {
        private List<V> V_ = new List<V> { };
        private List<S> S_ = new List<S> { };

        public bool val_is_empty() { return V_.Count < 1; }
        public bool stat_is_empty() { return S_.Count < 1; }

        public string val_string()
        {
            if (val_is_empty())
                return "empty";
            else
            {
                if (Object.ReferenceEquals(V_[0], null))
                    return "null";

                return V_[0].ToString();
            }
        }

        public string stat_string()
        {
            if (stat_is_empty())
                return "empty";
            else
            {
                if (Object.ReferenceEquals(S_[0], null))
                    return "null";

                return S_[0].ToString();
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + ": { value: { " + val_string() + " } , status: { " + stat_string() + " }";
        }

        [AllowNull]
        public V Val
        {
            [return: MaybeNull]
            get => (val_is_empty() ? default : V_[0]);
            set { if (val_is_empty()) V_.Add(value); else V_[0] = value; }
        }
        [AllowNull]
        public S Stat
        {
            [return: MaybeNull]
            get => (stat_is_empty() ? default : S_[0]);
            set { if (stat_is_empty()) S_.Add(value); else S_[0] = value; }
        }
        public Outcome(V v_) => Val = (v_);
        public Outcome(S s_) => Stat = (s_);
        public Outcome(V v_, S s_) { Val = (v_); Stat = (s_); }
        public Outcome() { Val = default; Stat = default; }
    } // Result

    internal partial class test
    {
        public static void result_test()
        {
            bool nul_it = true;
            int? result = nul_it ? 42 : null; // nullable value type

            var rez1 = new Outcome<string, int>("X");
            var rez2 = new Outcome<string, int>(13);
            var rez3 = new Outcome<string, int>();

            Log(rez1.ToString());
            Log(rez2.ToString());
            Log(rez3.ToString());
        }
    }

} // dbj