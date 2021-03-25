using System;
using static dbj.notmacros;

namespace dbj
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                valstat_tuple.doesit();
                valstat_tuple.test();
                whatever.runtime_nullable();
                actual_valstat_user.does();
                field_user.test();
                valstat_user.use();
                win32_make_and_call.example();

            }
            catch (System.Exception x)
            {
                Log(x);
            }
        }
    }
} // dbj