using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace dbj
{
    public static class extension
    {
        /// <summary>
        /// https://stackoverflow.com/a/66604069/10870835
        /// </summary>
        ///  [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormattedName(this Type type)
        {
            if (type.IsGenericType)
            {
                string genericArguments = type.GetGenericArguments()
                                    .Select(x => x.Name)
                                    .Aggregate((x1, x2) => $"{x1}, {x2}");
                return $"{type.Name.Substring(0, type.Name.IndexOf("`"))}"
                        + $"<{genericArguments}>";
            }
            return type.Name;
        }
        /// <summary>
        ///https://dbj.org/license_dbj
        /// </summary>
        ///  [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MAX_STRING_SIZE(this string self)
        {
            return 0xFFFF; // aka 65535
        }

        /// <summary>
        ///https://stackoverflow.com/a/50746368/10870835
        /// </summary>
        ///  [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToLength(this string self, UInt32 length)
        {
            if (self == null) return null;

            if (length > self.MAX_STRING_SIZE())
                throw new System.ArgumentOutOfRangeException("length > MAX_STRING_SIZE");

            return self.Length > length ? self.Substring(0, (int)length) : self.PadRight((int)length);
        }
    } // extension

    public struct notmacros
    {

        /// <summary>
        /// the all small letters assert ;)
        /// </summary>
        /// <param name="condition">result of boolean expression </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void assert(bool condition_)
        {
            // has effect only in debug builds
            Debug.Assert(condition_);
        }

        /// <summary>
        /// make string bufer of required size
        /// </summary>
        /// <param name="size_">required size, default is 1024, max is MAX_STRING_SIZE as defined in here. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string make_buffer(UInt32 size_ = 1024)
        {
            return " ".ToLength(size_);
        }
        /// <summary>
        /// Prints a message when in debug mode
        /// </summary>
        /// <param name="message">object whose ToStrng() will be used</param>
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
            Console.WriteLine("\nException!\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("{0}", x_.GetType().FormattedName());
            Console.ResetColor();
            Console.WriteLine("\nStack trace:\t");
            Console.Write(x_.StackTrace);
            Console.WriteLine("\nMessage:\t");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(x_.Message);
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