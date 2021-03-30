using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using static dbj.notmacros;

using System.Runtime.InteropServices;

namespace dbj
{
    public static class valstat_dll_delegate
    {
        // Define a delegate that corresponds to the unmanaged function.
        private delegate bool EnumWC(IntPtr hwnd, IntPtr lParam);

        // Import user32.dll (containing the function we need) and define
        // the method corresponding to the native function.
        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWC lpEnumFunc, IntPtr lParam);

        // Define the implementation of the delegate; here, we simply output the window handle.
        private static bool OutputWindow(IntPtr hwnd, IntPtr lParam)
        {
            Log("{0,4} HWND:{1,16}", count_++, hwnd.ToInt64());
            return true;
        }

        static int count_ = 0;

        public static void test_enum_windows()
        {
            Log("Enumerating windows and showing their HWND's");
            // Invoke the method; note the delegate as a first parameter.
            EnumWindows(OutputWindow, IntPtr.Zero);
        }
    }

}