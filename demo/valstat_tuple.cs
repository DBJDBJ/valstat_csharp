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
internal class valstat_tuple
{
    enum valstat_state
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
    public static void decode_valstat((int? val, string? stat) vstat_struct)
    {
        var (val, stat) = vstat_struct;

        if (val is not null)
        {
            DBJLog.debug("\trezult : {0}", val);
        }

        if (stat is not null)
        {
            DBJLog.debug("\tstatus : {0}", stat);
        }
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
