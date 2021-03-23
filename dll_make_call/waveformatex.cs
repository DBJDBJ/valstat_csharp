
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
using System.Runtime.InteropServices;

namespace dbj
{
    internal struct win32_make_and_call
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

        [DllImport(@"myfun.dll", EntryPoint = "myFun")]
        static extern int myFun( out WAVEFORMATEX wfx );

        public static void example()
        {
            WAVEFORMATEX wfx;
            try
            {
                int result = myFun(out wfx);
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
                dbj.notmacros.Log(x);
            }
        }
    } // dbj

} // dbj