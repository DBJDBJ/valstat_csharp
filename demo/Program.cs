// This is C#9, we have established that fact ...
// #define LEGACY
#nullable enable
using System;
using static DBJLog;
//using static dbj.test;
//using dbj;

namespace valstatcsharp;

public sealed class Program {

    public static void Main(string[] args)
    {
            // straight into the deep
            // use C:\Users\<your user name>\.valstat\valstat_dll.dll
            // folow through debugger and you will see ;)
            Valstat.Log( valstat_dll.descriptor());

            Valstat.Log(valstat_dll.safe_division(42, 12));
            Valstat.Log(valstat_dll.safe_division(42, 0));

            Valstat.Log(valstat_dll.safe_division_2(42, 12));
            Valstat.Log(valstat_dll.safe_division_2(42, 0));

        Sampling.test_quick_tuple_valstat();
        Sampling.test_tuple_valstat();

        #region adhoc
        // msdoc_delegate_interop_sample.test_enum_windows();
        //valstat_csharp.test_tuple_valstat();
        //valstat_csharp.test_quick_tuple_valstat();
        #endregion
    } // Main

} // Program