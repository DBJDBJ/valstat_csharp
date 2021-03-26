// #define LEGACY
#nullable enable
// https://github.com/dotnet/roslyn/blob/main/docs/features/tuples.md
using System;
using static dbj.notmacros;

namespace dbj
{

    internal partial class test
    {
        // cast object arg1 to type of arg2
        // usage:
        // IShape s_ = cast<IShape>(square);
        // or much more significantly
        // declare and define  the anonymous type
        // var data_ = cast(new { data = 42 }, new { data = default(int); });
        public static T cast<T>(object obj, T t = default)
        {
            // can throw System.InvalidCastException
            return (T)obj;
        }

        // we will consider value 'empty' if 
        // it is equal to default value of its type
        public static bool is_empty<T>(T val_)
        {
            return val_.Equals(default(T));
        }

        // valstat structure is C#7 tuple
        static (int? val, string? stat) valstat_api(int state)
        {
            switch (state)
            {
                case 0: return (null, "0xFF");
                case 1: return (1234, "0xFF");
                case 2: return (1234, null);
            }
            return (null, null);
        }
        public static void test_tuple_valstat()
        {
            // declaration/creation
            var p1 = (13, 42);
            // consumation variants
            var (val, stat) = valstat_api(0);
            var (x, y) = valstat_api(1);
            var pair = valstat_api(2);
            Log("pair: {0}", pair);
            var v_ = pair.val;
            var s_ = pair.stat;
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