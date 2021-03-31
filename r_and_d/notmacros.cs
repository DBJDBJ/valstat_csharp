using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace dbj
{
    public static class extension
    {
        /// <summary>
        /// https://stackoverflow.com/a/55148881/10870835
        /// </summary>
        ///  [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int index_of<T>(this T[] array, T value)
        {
            notmacros.assert(array != null);
            notmacros.assert(array.Length > 0);
            return Array.IndexOf(array, value);
        }

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
        public const char EOS = (char)0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string whoami([CallerMemberName] string caller_name = null)
        {
            if (string.IsNullOrEmpty(caller_name)) return "unknown";
            if (string.IsNullOrWhiteSpace(caller_name)) return "unknown";
            return caller_name;
        }
        /// <summary>
        /// arg is an array of chars not a string
        /// we need to trim of the cruft right of the EOS char
        /// </summary>
        /// <param name="charray">array of chars</param>
        ///  [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string to_string(char[] charray)
        {
            if (charray == null) return "null";
            if (charray.Length < 1) return "empty ";
            int eospos = charray.index_of(EOS);
            if (eospos < 1) return "empty";
            return new string(charray, 0, eospos);
        }

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

    //internal delegate IntPtr allocator(int size_);
    //internal delegate void deallocator(IntPtr pointer_);

    //internal delegate void marshall_writer<T>(IntPtr handle, T val);
    //internal delegate T marshall_reader<T>(IntPtr handle);

    internal static class dispenser_int_treat
    {
        internal static Func<IntPtr, int> reader = p => Marshal.ReadInt32(p); //  delegate (IntPtr p) { return Marshal.ReadInt32(p); };
        internal static Func<IntPtr, int, bool> writer = delegate (IntPtr p, int val_) { Marshal.WriteInt32(p, val_); return true; };

        internal static Func<IntPtr> alloc = delegate () { return Marshal.AllocHGlobal(sizeof(int)); };
        internal static Func<IntPtr, bool> dealloc = delegate (IntPtr p) { Marshal.FreeHGlobal(p); p = IntPtr.Zero; return true; };
    }

    /// <summary>
    /// usage:
    /// var int_ptr = dispenser(0) ;
    /// send to dll `int * ptr` and then use it
    /// int fty2    = int_ptr.value
    /// </summary>
    internal class dispenser : IDisposable
    {
        internal IntPtr handle { get; private set; }

        internal int value
        {
            /// NOTE: dll code can set it to null
            /// in that case exception happens
            get => dispenser_int_treat.reader(handle);
        }

        private bool disposed = false;

        public dispenser(int val_)
        {
            handle = dispenser_int_treat.alloc();
            dispenser_int_treat.writer(handle, val_);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // component.Dispose();
                }
                dispenser_int_treat.dealloc(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        }

        // will use in different context
        //[System.Runtime.InteropServices.DllImport("Kernal32")]
        // private extern static Boolean CloseHandle(IntPtr handle);
    }

    /// <summary>
    /// https://stackoverflow.com/a/66179657/10870835
    /// </summary>
    public class Value<T> where T : struct
    {
        public static implicit operator T(Value<T> val)
        {
            return val._value;
        }

        private T _value;

        public Value(T value) { _value = value; }

        public Value() : this(default) { }

        public T value
        {
            // get => _value;
            set => _value = value;
        }

        public override string ToString() { return _value.ToString(); }
    }

} // dbj