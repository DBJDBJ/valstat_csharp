// #define LEGACY
#nullable enable
// https://github.com/dotnet/roslyn/blob/main/docs/features/tuples.md
using System;
//using static dbj.notmacros;

namespace valstatcsharp;

/// this declaration works 
/// but usage with record literals does not
using my_valstat = Tuple<int, string>; //  (int? val, string? stat) ;
/// <summary>
/// tuples are C#9 feature
/// assumption is they will be used extensively
/// </summary>
internal sealed class Valstat
{
    public enum State
    {
        OK, ERROR, INFO, EMPTY
    }

    public static State to_state<V, S>
        ((V? val, S? stat) vstat_struct)
    {
        var (val, stat) = vstat_struct;

        if ((val is null) && (stat is null)) return State.EMPTY;

        if ((val is null) && (stat is not null)) return State.ERROR;

        if ((val is not null) && (stat is not null)) return State.INFO;

        return State.OK;
    }

    /// <summary>
    /// decoding valstat is a two step process
    /// frist step is branching on empty or not state
    /// scond step is using the values that are not empty
    /// </summary>
    /// <param name="vstat_struct">the valstat struct</param>
    public static void Log<V, S>((V? val, S? stat) vstat)
    {
        Valstat.State vs = Valstat.to_state<V, S>(vstat);

        DBJLog.debug($"Valstat (State: {vs})(Val: {vstat.val})(Stat: {vstat.stat})");
    }
}

internal sealed class Sampling { 

    // perform safe division of two longs
    // and return the 
    // value narrowed to UInt32
    // return valstat protocol structure as C# record
    static (int? value, string? status)
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

         return ((int)numerator / (int)divisor, null); // OK ValStat
    }

    public static void test_tuple_valstat()
    {
        DBJLog.debug("safe_divide({0},{1}) returned ", 42, 13);
        Valstat.Log(safe_divide(42, 13));

        DBJLog.debug("safe_divide({0},{1}) returned ", 42, 0);
        Valstat.Log(safe_divide(42, 0));

        DBJLog.debug("safe_divide({0},{1}) returned ", long.MaxValue, 0);
        Valstat.Log(safe_divide(long.MaxValue, 0));
    }
    public static void test_quick_tuple_valstat()
    {
        // record declared defined and printed
        DBJLog.debug($"{(13, 42)}");

        // anonymous record with named fields
        (int ? Val, string ? Stat) = (1234,null); // OK
        (int ? X, string ? Y) = (null, "0xFF"); // ERROR

        // record named and declared with no file names will have them 
        // named: Item1, Item2 ... and so on
        var pair = (1234, "0xFF"); // INFO

        DBJLog.debug($"{pair}");
    }
}
// internal partial class test
