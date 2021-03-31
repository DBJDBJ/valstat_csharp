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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static dbj.notmacros;

namespace dbj
{
    internal class valstat_dll
    {
        // there might be better solutions:
        // 1. store the paths in the configuration file
        // 2. dynamiclay load the dll and get the function inside: https://stackoverflow.com/a/8836228/10870835
        // that might be very slow
        //
        // This is as far as we will go here: const is a compile time thing in C#
#if DEBUG
        const string valstat_dll_location = @"D:\DEVL\GITHUB\DBJDBJ\valstat_dll\x64\Debug\valstat_dll.dll";
#else
        const string valstat_dll_location = @"D:\DEVL\GITHUB\DBJDBJ\valstat_dll\x64\Release\valstat_dll.dll";
#endif
        internal static void descriptor()
        {
            // local extern function declaration is C#9 feature

            [DllImport(valstat_dll_location, EntryPoint = "compiler_string", CharSet = CharSet.Unicode, ExactSpelling = true)]
            static extern Int32 compiler_string([Out] char[] string_, Int32 string_length_);

            [DllImport(valstat_dll_location, EntryPoint = "this_name", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            static extern bool this_name([MarshalAs(UnmanagedType.LPWStr)] string name_, out Int32 string_length_);

            char[] buf_ = new char[128];
            compiler_string(buf_, 128);

            // first ask how much space is required for the string rezult
            int size_ = 0;
            if (false == this_name(null, out size_))
                throw (new Win32Exception(Marshal.GetLastWin32Error()));

            // second obtain the string of reported size
            var dll_name_ = new string(' ', size_);
            if (false == this_name(dll_name_, out size_))
                throw (new Win32Exception(Marshal.GetLastWin32Error()));

            Log("\n{2} : Compiler used to build {0} is: {1}\n", dll_name_, to_string(buf_), whoami());
        }

        /// <summary>
        ///  C version:
        ///  
        ///  enum { icp_data_count = 255 };
        ///  typedef struct {
        ///    int val;
        ///   char data[icp_data_count];
        ///    }
        ///  int_charr_pair;
        ///  
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        unsafe struct int_charr_pair
        {
            public IntPtr val;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string data;
        }

        // https://stackoverflow.com/a/6093621/10870835
        static IntPtr make_ptr(int val_)
        {
            // Allocating memory for int
            IntPtr intPointer = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(intPointer, val_);
            return intPointer;

            // sending intPointer to unmanaged code , then
            // Test reading of IntPtr object
            // int test2 = Marshal.ReadInt32(intPointer); // test2 would be equal 55
            // Free memory
            // Marshal.FreeHGlobal(intPointer);
        }
        static void int_charr_pair_log(ref int_charr_pair icp, [CallerMemberName] string caller_ = " ")
        {
            if (icp.val == IntPtr.Zero)
                Log(@"{0}: {{ value: {1}, status: {2} }}", caller_, "empty", (icp.data));
            else
                Log(@"{0}: {{ value: {1}, status: {2} }}", caller_, Marshal.ReadInt32(icp.val), (icp.data));
        }

        internal static unsafe void test_int_charr_pair()
        {
            [DllImport(valstat_dll_location)]
            static extern void safe_division_2(out int_charr_pair vst_ptr, int numerator, int denominator);

            int_charr_pair icp = new int_charr_pair();

            // Int32 safe_location = 13;
            // icp.val = make_ptr(13);

            // Marshall does: char[255], for the `data` member, as per declaration
            safe_division_2(out icp, 42, 12);

            int_charr_pair_log(ref icp, whoami());

            // 'out' means the dll has made it
            // and in that dll we did not allocated from the heap
            // Marshal.FreeHGlobal(icp.val);
        }
        ////////////////////////////////////////////////////////////////////////////////////////
        // declare a C# funcion pointer aka `delegate` 
        // C version: typedef void (*safe_division_fp)(int_charr_pair*);
        private delegate void safe_division_delegate(int_charr_pair icp);

        // implement the `delegate`
        private static void safe_division_fp(int_charr_pair icp)
        {
            // Marshall transforms string to char[255]
            // for the `data` member
            // as per declaration
            int_charr_pair_log(ref icp, whoami());
        }

        internal static void test_dll_callback_valstat()
        {
            [DllImport(valstat_dll_location)]
            static extern void safe_division_cb(safe_division_delegate callback_, int numerator, int denominator);

            safe_division_cb(safe_division_fp, 42, 12);
        }

    } // valstat_dll
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

/* 

[StructLayout(LayoutKind.Sequential)]
unsafe struct int_string_pair
{
    public int val;
    public string stat;
    public int stat_len;
}
/// make emptry int char array pair 
static int_string_pair make(int stat_size)
{
    var ret = new int_string_pair();
    ret.val = 0;
    ret.stat = " ".ToLength((uint)stat_size);
    ret.stat_len = stat_size;
    return ret;
}

///  from tuple
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

/// to tuple
static (int?, string?) make(int_string_pair isp)
{
    return (isp.val, isp.stat);
}
internal unsafe static void test_valstat_dll()
{
    [DllImport(valstat_dll_location)]
    static extern void safe_division([Out] int_string_pair valstat, int numerator, int denominator);

    var vis = make(0xF);

    safe_division(vis, 42, 12);
    var (val, stat) = make(vis);
    Log(@"{0} : {{ value: {1}, status: {2} }}", whoami(), val ?? default, stat ?? default);
}
*/


#endregion

