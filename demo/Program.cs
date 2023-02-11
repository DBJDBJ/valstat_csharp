// This is C#9, we have established that fact ...
// #define LEGACY
#nullable enable
using System;
using static dbj.notmacros;
using static dbj.test;
using dbj;

try
{
    // straight into the deep
    // use C:\Users\<your user name>\.valstat\valstat_dll.dll
    // folow through debugger 
    valstat_dll.descriptor();
    valstat_tuple.decode_valstat(valstat_dll.safe_division(42, 12));
    valstat_tuple.decode_valstat(valstat_dll.safe_division(42, 0));

    valstat_tuple.decode_valstat(valstat_dll.safe_division_2(42, 12));
    valstat_tuple.decode_valstat(valstat_dll.safe_division_2(42, 0));
    #region adhoc
    // msdoc_delegate_interop_sample.test_enum_windows();
    //valstat_csharp.test_tuple_valstat();
    //valstat_csharp.test_quick_tuple_valstat();
    #endregion
}
catch (System.Exception x)
{
    Log(x);
}

