// #define LEGACY
#nullable enable
// https://github.com/dotnet/roslyn/blob/main/docs/features/tuples.md
using System;
//using static dbj.notmacros;

namespace valstatcsharp;

/// this declaration works 
/// but usage with tuple literals does not
using my_valstat = Tuple<int, string>; //  (int? val, string? stat) ;
/// <summary>
/// tuples are C#9 feature
/// assumption is they will be used extensively
/// </summary>
internal sealed class Valstat
{
    public enum valstat_state
    {
        OK, ERROR, INFO, EMPTY
    }
    // there is no valstat structure, class or record
    // valstat structure is C#7 tuple
    static (int? val, string? stat)
        valstat_maker(valstat_state state)
    {
        switch (state)
        {
            case valstat_state.ERROR: return (null, "0xFF");    // ERROR state
            case valstat_state.INFO: return (1234, "0xFF");     // INFO state
            case valstat_state.OK: return (1234, null);         // OK state
        }
        return (null, null);                                    // EMPTY state
    }

    public static valstat_state to_state<V,S>
        ((V? val, S? stat) vstat_struct )
    {
        var (val, stat) = vstat_struct;

        if ((val is null) && (stat is null)) return valstat_state.EMPTY ;

        if ((val is null) && (stat is not null)) return valstat_state.ERROR;

        if ((val is not null) && (stat is not null)) return valstat_state.INFO;

        return valstat_state.OK;
    }

    // perform safe division of two longs
    // and return the 
    // value narrowed to UInt32
    // return valstat protocol structure as C# tuple
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

        try
        {
            return ((int)numerator / (int)divisor, null);
        }
        catch (System.Exception x)
        {
            return (null, x.Message);
        }
    }

    /// <summary>
    /// decoding valstat is a two step process
    /// frist step is branching on empty or not state
    /// scond step is using the values that are not empty
    /// </summary>
    /// <param name="vstat_struct">the valstat struct</param>
    public static void decode_valstat<V,S>((V? val, S? stat) vstat)
    {
        Valstat.valstat_state vs = Valstat.to_state<V,S>(vstat);

        DBJLog.debug("Valstat");
        DBJLog.debug($"State : {vs}");
        DBJLog.debug($"Val   : {vstat.val}");
        DBJLog.debug($"Stat  : {vstat.stat}");
    }

    public static void test_tuple_valstat()
    {
        DBJLog.debug("\nsafe_divide({0},{1}) returned ", 42, 13);
        decode_valstat(safe_divide(42, 13));

        DBJLog.debug("\nsafe_divide({0},{1}) returned ", 42, 0);
        decode_valstat(safe_divide(42, 0));

        DBJLog.debug("\nsafe_divide({0},{1}) returned ", long.MaxValue, 0);
        decode_valstat(safe_divide(long.MaxValue, 0));
    }
    public static void test_quick_tuple_valstat()
    {
        // tuple declaration/creation
        DBJLog.debug($"{(13, 42)}");
        // valstat consumation variants
        var (val, stat) = valstat_maker(valstat_state.OK);
        var (x, y) = valstat_maker(valstat_state.ERROR);
        var pair = valstat_maker(valstat_state.INFO);
        DBJLog.debug($"{pair}");
    }
}
// internal partial class test
