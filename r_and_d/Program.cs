// This is C#9, we have established that fact ...
// #define LEGACY
#nullable enable
using System;
using static dbj.notmacros;
using static dbj.test;

try
{
    // not possible before C#9 (2020)
    // int? specimen = true ? 42 : null;

    test_compiler_string_from_dll();
    test_waveformat_from_dll();
    // test_few_os_dlls();

    //test_simple_valstat();
    //result_test();
    // test_valstat_with_fields();
    //test_field();
    test_tuple_valstat();
    //test_runtime_nullable();
}
catch (System.Exception x)
{
    Log(x);
}

