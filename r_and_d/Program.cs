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

    // dbj.valstat_dll_delegate.test_enum_windows();
    test_dll_callback_valstat();

    test_int_charr_pair();

    test_valstat_dll();

    test_compiler_string_from_dll();

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

