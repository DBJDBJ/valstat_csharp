using System;
using System.Runtime.CompilerServices;

namespace dbj
{
    public struct notmacros
    {
        /// <summary>
        /// Prints a message when in debug mode
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Log(object message)
        {
#if DEBUG
            // Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
#endif
        }

        /// <summary>
        /// Prints a formatted message when in debug mode
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An array of objects to write using format</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Log(string format, params object[] args)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(format, args);
            Console.ResetColor();
#endif
        }

        /// <summary>
        /// Prints a formatted exception message when in debug mode
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An array of objects to write using format</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Log(System.Exception x_)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Exception!");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(x_.Message);
            Console.ResetColor();
#endif
        }
        /// <summary>
        /// Computes the square of a number
        /// </summary>
        /// <param name="x">The value</param>
        /// <returns>x * x</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Square(double x)
        {
            return x * x;
        }

        /// <summary>
        /// Wipes a region of memory
        /// </summary>
        /// <param name="buffer">The buffer</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ClearBuffer(ref byte[] buffer)
        {
            ClearBuffer(ref buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Wipes a region of memory
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="offset">Start index</param>
        /// <param name="length">Number of bytes to clear</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ClearBuffer(ref byte[] buffer, int offset, int length)
        {
            fixed (byte* ptrBuffer = &buffer[offset])
            {
                for (int i = 0; i < length; ++i)
                {
                    *(ptrBuffer + i) = 0;
                }
            }
        }
    } // notmacros
} // dbj