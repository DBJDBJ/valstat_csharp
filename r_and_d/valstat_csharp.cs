// #define LEGACY
#nullable enable
// https://github.com/dotnet/roslyn/blob/main/docs/features/tuples.md
using System;
using static dbj.notmacros;

namespace dbj
{

    internal class valstat_csharp
    {
        enum valstat_state
        {
            OK, ERROR, INFO, EMPTY
        }
        // there is no valstat structure, class or record
        // valstat structure is C#7 tuple
        static (int? val, string? stat) valstat_maker(valstat_state state)
        {
            switch (state)
            {
                case valstat_state.ERROR: return (null, "0xFF");    // ERROR state
                case valstat_state.INFO: return (1234, "0xFF");     // INFO state
                case valstat_state.OK: return (1234, null);         // OK state
            }
            return (null, null);                                    // EMPTY state
        }

        // perform safe division of two longs
        // and return the 
        // value narrowed to UInt32
        // return valstat protocol structure as C# tuple
        static (UInt32? value, string? status)
            safe_divide(decimal numerator, decimal divisor)
        {
            if (numerator > UInt32.MaxValue)
                return (null, "Error: numerator > uint32 max");

            if (numerator < 0)
                return (null, "Error: numerator < 0");

            if (divisor > UInt32.MaxValue)
                return (null, "Error: divisor > uint32 max");

            if (divisor < 0)
                return (null, "Error: divisor < 0");

            if (divisor == 0)
                return (null, "Error: divisor is 0");

            try
            {
                return ((UInt32)(numerator / divisor), null);
            }
            catch (System.Exception x)
            {
                return (null, x.Message);
            }
        }

        static void test_driver(long a, long b)
        {
            var (val, stat) = safe_divide(a, b);

            Log("\nsafe_divide({0},{1}) returned ", a, b);

            if (val is not null)
            {
                Log("\trezult : {0}", val);
            }

            if (stat is not null)
            {
                Log("\tstatus : {0}", stat);
            }
        }

        public static void test_tuple_valstat()
        {
            test_driver(42, 13);
            test_driver(42, 0);
            test_driver(long.MaxValue, 0);
        }
        public static void test_quick_tuple_valstat()
        {
            // tuple declaration/creation
            Log((13, 42));
            // valstat consumation variants
            var (val, stat) = valstat_maker(valstat_state.OK);
            var (x, y) = valstat_maker(valstat_state.ERROR);
            var pair = valstat_maker(valstat_state.INFO);
            Log(pair);
        }
    }
}// internal partial class test

#if LEGACY
#region SO inspiration -- LEGACY BEFORE TUPLES -- PLAYGROUND ONLY

        private static void foo()
        {
            var user = AnonCast(
                GetUserTuple(),
                new
                {
                    Name = default(string),
                    Badges = default(int)
                }
            );

            Log("\nName: {0} Badges: {1}", user.Name, user.Badges);
        }

        static object GetUserTuple()
        {
            return new { Name = "dp", Badges = 5 };
        }

        // Using the magic of Type Inference...
        static T AnonCast<T>(object obj, T t)
        {
            return (T)obj;
        }

        public static void test()
        {
            foo();
        }
#endregion
#endif