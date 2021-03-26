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
            Console.ForegroundColor = ConsoleColor.White;
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
            if (args.Length < 1)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(format);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(format, args);
            }
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
            Console.WriteLine("\nException!");
            Console.WriteLine(x_.StackTrace);
            Console.ResetColor();
            Console.WriteLine("\n");
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

        /// <summary>
        /// cast object arg1 to type of arg2
        /// usage:
        /// give type explicitly
        /// IShape s_ = cast<IShape>(square);
        /// or
        /// declare and define to the type of the secon argument
        /// var data_ = cast( 42 , new decimal() );
        /// </summary>
        /// <param name="obj">object to cast</param>
        /// <param name="offset">optional cast to this arg type</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T cast<T>(object obj, T t = default)
        {
            // can throw System.InvalidCastException
            return (T)obj;
        }

        /// <summary>
        /// consider value 'empty' if it is null
        /// </summary>
        /// <param name="val_">value to be tested</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool is_empty<T>(T val_)
        {
            if (val_ is null) return true;
            return false;
        }
    } // notmacros
} // dbj