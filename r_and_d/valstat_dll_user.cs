#nullable enable
/*
 Make sure you read this first

https://docs.microsoft.com/en-us/dotnet/standard/native-interop/best-practices

 */
// 
// here is how to allocate and free for C# interop
//
// string smMsg = "whatever" ;
// var len = Marshal.SizeOf(typeof(smMsg));
// IntPtr msg_intptr = Marshal.AllocHGlobal(len);
// try {
//     // call the function, convert output to struct, etc...
// }
// finally {
//     Marshal.FreeHGlobal(msg_intptr);
// }
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using static dbj.notmacros;

namespace dbj
{
    internal partial class test
    {
        // there are better solutions:
        // 1. store the paths in the configuration file
        // 2. dynamiclay load the dll and get the function inside: https://stackoverflow.com/a/8836228/10870835
        // but this is as far as we will go here:
        // const is a compile time thing in C#
#if DEBUG
        const string valstat_dll_location = @"D:\DEVL\GITHUB\DBJDBJ\valstat_dll\x64\Debug\valstat_dll.dll";
#else
        const string valstat_dll_location = @"D:\DEVL\GITHUB\DBJDBJ\valstat_dll\x64\Release\valstat_dll.dll";
#endif
        internal static void test_compiler_string_from_dll()
        {
            // local extern function declaration is C#9 feature

            [DllImport(valstat_dll_location, EntryPoint = "compiler_string", CharSet = CharSet.Unicode, ExactSpelling = true)]
            static extern Int32 compiler_string([Out] char[] string_, Int32 string_length_);

            [DllImport(valstat_dll_location, EntryPoint = "this_name", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            static extern bool this_name([Out] char[] string_, Int32 string_length_);

            char[] buf_ = new char[128];
            compiler_string(buf_, 128);

            char[] dll_name_ = new char[1024];
            if (false == this_name(dll_name_, 1024))
            {
                // the correct message
                Log(new Win32Exception(Marshal.GetLastWin32Error()));
            }

            Log("\nCompiler used to build {0} is: {1}\n", to_string(dll_name_), to_string(buf_));
        }

        // will be passed to/from native DLL
        [StructLayout(LayoutKind.Sequential)/*, Pack = 1)*/]
        unsafe struct int_string_pair
        {
            public int val;
            // By default, .NET marshals a string as a pointer to a null-terminated string.
            [MarshalAs(UnmanagedType.LPStr)]
            public string stat;

            public int stat_len;
        }
        /// <summary>
        /// make emptry int char array pair 
        /// </summary>
        /// <param name="stat_size">charr array size</param>
        /// <returns></returns>
        static int_string_pair make(int stat_size)
        {
            var ret = new int_string_pair();
            ret.val = 0;
            ret.stat = " ".ToLength((uint)stat_size);
            ret.stat_len = stat_size;
            return ret;
        }

        /// <summary>
        ///  from tuple
        /// </summary>
        /// <param name="tup">tuple</param>
        /// <returns></returns>
        static unsafe int_string_pair make((int?, string?) tup)
        {
            var ret = new int_string_pair();
#pragma warning disable CS8629 // Nullable value type may be null.
#pragma warning disable CS8601 // Possible null reference assignment.
            ret.val = (int)(tup.Item1 ?? default);
            ret.stat = tup.Item2 ?? default;
            ret.stat_len = ret.stat.Length;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8629 // Nullable value type may be null.
            return ret;
        }

        /// <summary>
        /// to tuple
        /// </summary>
        /// <param name="isp">int string pair</param>
        /// <returns></returns>
        static (int?, string?) make(int_string_pair isp)
        {
            return (isp.val, isp.stat);
        }
        /*
                internal struct valstat_struct<V, S>
                {
                    public V? val; public S? stat;

                    valstat_struct(V v_, S s_) { val = v_; stat = s_; }
                }

                internal static (VT? val, ST? stat) valstat_to_tuple<VT, ST>(valstat_struct<VT, ST> vstat_)
                {
                    return (vstat_.val, vstat_.stat);
                }

                internal static valstat_struct<VT, ST> tuple_to_valstat<VT, ST>((VT?, ST?) vstat_)
                {
                    var retval = new valstat_struct<VT, ST>();
                    retval.val = vstat_.Item1;
                    retval.stat = vstat_.Item2;
                    return retval;
                }

        */
        internal unsafe static void test_valstat_dll()
        {
            [DllImport(valstat_dll_location, EntryPoint = "safe_division", /*CharSet = CharSet.Unicode,*/ ExactSpelling = true)]
            static extern void safe_division([Out] int_string_pair valstat, int numerator, int denominator);

            var vis = make(0xF);

            safe_division(vis, 42, 12);
            var (val, stat) = make(vis);
            Log(@"{0} : {{ value: {1}, status: {2} }}", whoami(), val ?? default, stat ?? default);
        }

        [StructLayout(LayoutKind.Sequential)/*, Pack = 1)*/]
        unsafe struct int_charr_pair
        {
            public int val;
            // .NET Marshalling defaul is CharSet.Ansi
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            public char[] data;
        }

        internal static void test_int_charr_pair()
        {
            [DllImport(valstat_dll_location/*, EntryPoint = "safe_division_2", CharSet = CharSet.Ansi, ExactSpelling = true*/)]
            static extern void safe_division_2([Out] int_charr_pair valstat, int numerator, int denominator);

            int_charr_pair icp = new int_charr_pair();

            // Marshall does char[255] for the `data` member
            // as per declaration
            safe_division_2(icp, 42, 12);

            Log(@"{0}: {{ value: {1}, status: {2} }}", whoami(), icp.val, to_string(icp.data));
        }

        // declare a delegate 
        private delegate void safe_division_delegate(int_charr_pair icp);

        // implement the delegate
        private static void safe_division_fp(int_charr_pair icp)
        {
            Log(@"{0}: {{ value: {1}, status: {2} }}", whoami(), icp.val, to_string(icp.data));
        }

        internal static void test_dll_callback_valstat()
        {
            [DllImport(valstat_dll_location/*, EntryPoint = "safe_division_2", CharSet = CharSet.Ansi, ExactSpelling = true*/)]
            static extern void safe_division_cb(safe_division_delegate callback_, int numerator, int denominator);

            // Marshall does char[255] for the `data` member
            // as per declaration
            safe_division_cb(safe_division_fp, 42, 12);
        }

    } // test
} // dbj


#region deprecated

// https://stackoverflow.com/a/47648504/10870835
/*
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WAVEFORMATEX
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;

            // C# should generate pretty decent
            // ToString() here, but it did not
            // ditto ...
            public override string ToString()
            {
                return this.GetType().FormattedName()
                        + $"\n{nameof(wFormatTag)} \t: {wFormatTag.ToString()}"
                        + $"\n{nameof(nChannels)} \t: {nChannels.ToString()}"
                        + $"\n{nameof(nSamplesPerSec)} \t: {nSamplesPerSec.ToString()}"
                        + $"\n{nameof(nAvgBytesPerSec)} : {nAvgBytesPerSec.ToString()}"
                        + $"\n{nameof(nBlockAlign)} \t: {nBlockAlign.ToString()}"
                        + $"\n{nameof(wBitsPerSample)} \t: {wBitsPerSample.ToString()}"
                        + $"\n{nameof(cbSize)} \t\t: {cbSize.ToString()}";
            }

        }
        internal static void test_waveformat_from_dll()
        {
#if DEBUG
            [DllImport(@"D:\DEVL\GITHUB\DBJDBJ\valstat_dll\x64\Debug\valstat_dll.dll"
            , EntryPoint = "waveformat", CharSet = CharSet.Unicode, ExactSpelling = true)]
#else
        [DllImport(@"D:\DEVL\GITHUB\DBJDBJ\valstat_dll\x64\Release\valstat_dll.dll"
            , EntryPoint = "waveformat", CharSet = CharSet.Unicode, ExactSpelling = true)]
#endif
            static extern Int32 waveformat(out WAVEFORMATEX wfx);

            WAVEFORMATEX wfx;
            try
            {
                var result = waveformat(out wfx);
                Console.WriteLine(result);
                Console.WriteLine(wfx);
            }
            catch (System.DllNotFoundException x)
            {
                Log(x);
            }
        }
*/

#endregion

