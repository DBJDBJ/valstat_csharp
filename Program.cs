// This is C#9, we have established that fact ...
// #define LEGACY
#nullable enable
using System;
using static dbj.notmacros;
using static dbj.test;

try
{
    //test_simple_valstat();
    //result_test();
    test_valstat_with_fields();
    //test_field();
    //test_tuple_valstat();
    //test_runtime_nullable();
    //test_dll_call();
}
catch (System.Exception x)
{
    Log(x);
}

