#nullable enable
using System;
// using System.Reflection;
// using static System.Console;
using static dbj.notmacros;

namespace dbj
{

    partial class test
    {

        internal record SimplValstat<VT, ST>
        {
            public VT? value { get; }
            public ST? status { get; }

            public bool value_empty() { return value is null; }
            public bool status_empty() { return status is null; }

            public string value_string() { return value is null ? "empty" : value.ToString(); }
            public string status_string() { return status is null ? "empty" : status.ToString(); }

            public override string ToString()
            {
                return this.GetType().Name + " { value:" + value_string() + ", status:" + status_string() + " }";
            }

            public SimplValstat(VT? v_) => value = v_;
            public SimplValstat(ST? s_) => status = s_;
            public SimplValstat(VT? v_, ST? s_) { value = v_; status = s_; }
            public SimplValstat() { }
        }

        static void simple_valstat_test_driver<VT, ST>(VT v_, ST s_)
        {
            Log(new SimplValstat<VT, ST>());
            Log(new SimplValstat<VT, ST>(v_));
            Log(new SimplValstat<VT, ST>(s_));
            Log(new SimplValstat<VT, ST>(v_, s_));
        }
        public static void test_simple_valstat()
        {
            simple_valstat_test_driver(13, 42);
            simple_valstat_test_driver("Bimbi", "Bumbi");
            simple_valstat_test_driver(13, "Bumbi");
        }
    }
} // dbj