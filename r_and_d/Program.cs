// This is C#9, we have established that fact ...
// #define LEGACY
#nullable enable
using System;
using static dbj.notmacros;
using static dbj.test;
using dbj;

try
{
    // msdoc_delegate_interop_sample.test_enum_windows();

    valstat_dll.descriptor();
    valstat_dll.test_int_charr_pair();
    valstat_dll.test_dll_callback_valstat();
    //valstat_csharp.test_tuple_valstat();
    //valstat_csharp.test_quick_tuple_valstat();
}
catch (System.Exception x)
{
    Log(x);
}

