
// https://stackoverflow.com/a/20597406/10870835
// that was VERY WRONG so I had to re-act (laugh)

// 
// here is how to allocare and free from C#
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
using static dbj.notmacros;

namespace dbj
{
    internal partial class test
    {
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
        }

        [DllImport(@"myfun.dll", EntryPoint = "waveformat")]
        static extern int waveformat(out WAVEFORMATEX wfx);

        public static void test_dll_call()
        {
            WAVEFORMATEX wfx;
            try
            {
                int result = waveformat(out wfx);
                Console.WriteLine(result);
                Console.WriteLine(wfx.cbSize);
                Console.WriteLine(wfx.nAvgBytesPerSec);
                Console.WriteLine(wfx.nBlockAlign);
                Console.WriteLine(wfx.nChannels);
                Console.WriteLine(wfx.nSamplesPerSec);
                Console.WriteLine(wfx.wBitsPerSample);
                Console.WriteLine(wfx.wFormatTag);

                // Console.ReadLine();
            }
            catch (System.DllNotFoundException x)
            {
                Log(x);
            }
        }
        // https://stackoverflow.com/a/47648504/10870835
        // note: use a struct, not a class, so that
        // you can call Marshal.PtrToStructure.
        // in C this is declared as:
        //
        // (winsvc.h)
        //         typedef struct _QUERY_SERVICE_CONFIGW {
        //   DWORD  dwServiceType;
        //   DWORD  dwStartType;
        //   DWORD  dwErrorControl;
        //   LPWSTR lpBinaryPathName;
        //   LPWSTR lpLoadOrderGroup;
        //   DWORD  dwTagId;
        //   LPWSTR lpDependencies;
        //   LPWSTR lpServiceStartName;
        //   LPWSTR lpDisplayName;
        // } QUERY_SERVICE_CONFIGW, *LPQUERY_SERVICE_CONFIGW;
        //
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct QUERY_SERVICE_CONFIG
        {
            public int dwServiceType;
            public int dwStartType;
            public int dwErrorControl;
            public string lpBinaryPathName;
            public string lpLoadOrderGroup;
            public int dwTagID;
            public string lpDependencies;
            public string lpServiceStartName;
            public string lpDisplayName;
        };

        // return type is a bool, no need for an int here
        // user SetLastError when the doc says so
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool QueryServiceConfig(
            IntPtr hService,
            IntPtr lpServiceConfig,
            int cbBufSize,
            out int pcbBytesNeeded
            );

        static string GetServicePath(IntPtr service)
        {
            QueryServiceConfig(service, IntPtr.Zero, 0, out int size);
            if (size == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                if (!QueryServiceConfig(service, ptr, size, out size))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                var config = Marshal.PtrToStructure<QUERY_SERVICE_CONFIG>(ptr);
                return config.lpBinaryPathName;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        // This is a platform invoke prototype. SetLastError is true, which allows
        // the GetLastWin32Error method of the Marshal class to work correctly.
        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]
        static extern Boolean CloseHandle(IntPtr h);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandleW(string lpModuleName);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern Int32 GetModuleFileNameW(IntPtr hModule, out IntPtr lpFilename, Int32 nSize);

        static public unsafe void test_few_os_dlls()
        {
            IntPtr handle = IntPtr.Zero;
            IntPtr bufy = IntPtr.Zero;
            try
            {
                handle = GetModuleHandleW("Kernel32.dll");

                //string fullpath = make_buffer();
                //assert(fullpath.Length == 1024);
                // bufy = Marshal.StringToHGlobalUni(fullpath);

                bufy = Marshal.AllocHGlobal(1024);

                if (0 != GetModuleFileNameW(handle, out bufy, 1024))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                string? rezult = Marshal.PtrToStringAuto(bufy);

            }
            catch (Win32Exception x)
            {
                Log(x);
            }
            finally
            {
                if (handle != IntPtr.Zero) CloseHandle(handle);
                if (bufy != IntPtr.Zero) Marshal.FreeHGlobal(bufy);
            }
        }


    } //test class

} // dbj