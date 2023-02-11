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

namespace valstatcsharp;

internal class valstat_dll
{

    // This is as far as we will go here: const is a compile time thing in C#

    const string valstat_dll_location = @"C:\Users\dusan\.valstat\valstat_dll.dll";

    // during the development we need the full path to the dll, so we hard code it here.
    // there might be better solutions:
    // 1. store the paths in the configuration file
    // 2. dynamiclay load the dll and get the function inside: https://stackoverflow.com/a/8836228/10870835
    // that might be very slow

    internal static void descriptor()
    {
        // local extern function declaration is C#9 feature
        // CharSet attribute parameter is CharSet.Unicode
        // CharSet.Ansi is default and that is unfortunate

        [DllImport(valstat_dll_location, EntryPoint = "compiler_string", CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern Int32 compiler_string([Out] char[] string_, Int32 string_length_);

        [DllImport(valstat_dll_location, EntryPoint = "this_name", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        static extern bool this_name([MarshalAs(UnmanagedType.LPWStr)] string name_, out Int32 string_length_);

        char[] buf_ = new char[128];
        compiler_string(buf_, 128);

        // first ask how much space is required for the string rezult
        // if the target dll is built in debug mode
        // follow through debugger and you will find yourself inside
        // the dll written in C
        int size_ = 0;
        if (false == this_name("", out size_))
            throw (new Win32Exception(Marshal.GetLastWin32Error()));

        // second obtain the string of reported size
        var dll_name_ = new string(' ', size_);
        if (false == this_name(dll_name_, out size_))
            throw (new Win32Exception(Marshal.GetLastWin32Error()));

        DBJLog.debug(
            "\n{2} : Compiler used to build {0} is: {1}\n", 
            dll_name_, 
            buf_.ToString() ?? "NULL" , 
            DBJcore.Whoami()
            );
    }

    /// <summary>
    ///  VALSTAT structure C version:
    ///  
    ///  enum { icp_data_count = 255 };
    ///  typedef struct {
    ///    int val;
    ///   char data[icp_data_count];
    ///    }
    ///  int_charr_pair;
    ///  
    ///  VALSTAT structure C# version
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct int_charr_pair
    {
        public IntPtr val;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string data;
    }

    /// <summary>
    /// make tuple valstat aka: ( int ? , string )
    /// we need to "marshall" to tuples since tuples are so called "generics"
    /// and generics can not be marshalled in C#9:
    /// 
    /// MarshalDirectiveException
    /// Cannot marshal 'parameter #1': Non-blittable generic types cannot be marshaled.
    /// 
    ///</summary>
    /// <param name="icp">C# struct version</param>
    /// <param name="caller_">the name of the caller</param>
    static (int? val, string? stat)
        int_charr_pair_to_tuple
          (ref int_charr_pair icp)
    {
        // field names are optional
        // start in valstat EMPTY state
        (int? val, string? stat) vstat = (null, null);

        if (icp.val != IntPtr.Zero)
            vstat.val = Marshal.ReadInt32(icp.val);

        if (!string.IsNullOrEmpty(icp.data))
        {
            if (!string.IsNullOrWhiteSpace(icp.data))
            {
                vstat.stat = icp.data;
            }
        }
        return vstat;
    }

    // log it and return it
    // notice the ad-hoc tuple declaration
    static (int? val, string? stat)
        log_the_tuple((int? val, string? stat) vstat, [CallerMemberName] string caller_ = " ")
    {
        DBJLog.debug(@"{0}: {{ value: {1}, status: {2} }}",
            caller_,
            vstat.val.ToString() ?? "empty",
            vstat.stat ?? "empty" );
        return vstat;
    }

    internal static (int? val, string? stat) safe_division
        (int nominator, int denominator)
    {
        [DllImport(valstat_dll_location)]
        static extern void safe_division(out int_charr_pair vst_ptr, int numerator, int denominator);

        // allocate the valstat struct here
        int_charr_pair icp = new int_charr_pair();
        // call the dll
        safe_division(out icp, nominator, denominator);
        // log and marhsall to tuple
        // return tuple
        return log_the_tuple(int_charr_pair_to_tuple(ref icp));
    }

    /// <summary>
    /// this is the tuple variant of the struct above
    /// general C#9 code will use tuples
    /// but we can not use non-blitable tuples C# interop function arguments
    /// and this is non-blitable besause it used string
    /// this is static variable not a tuple declaration
    /// </summary>
    static (int?, string?) vstat_tuple;

    ////////////////////////////////////////////////////////////////////////////////////////
    // declare a C# funcion pointer aka `delegate` 
    // C version:
    //  typedef void (*safe_division_fp)(int_charr_pair*);
    private delegate void safe_division_delegate(int_charr_pair icp);

    // implement the `delegate`
    // this is a callback used from inside the DLL
    // we need to take care not to spend too 
    // much time in here, because dll will wait 
    // for this function to return
    private static void safe_division_fp(int_charr_pair icp)
    {
        // saving the result
        vstat_tuple = log_the_tuple(int_charr_pair_to_tuple(ref icp));
    }

    internal static (int? val, string? stat)
        safe_division_2(int nominator, int denominator)
    {
        [DllImport(valstat_dll_location)]
        static extern void safe_division_cb(safe_division_delegate callback_, int numerator, int denominator);

        // call into dkk
        safe_division_cb(safe_division_fp, nominator, denominator);

        // return the result of log_the_tuple() obtained
        // from inside safe_division_fp()
        return vstat_tuple;
    }

} // valstat_dll
// dbj


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
            DBJLog.debug(x);
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
DBJLog.debug(@"{0} : {{ value: {1}, status: {2} }}", whoami(), val ?? default, stat ?? default);
}
*/


#endregion

