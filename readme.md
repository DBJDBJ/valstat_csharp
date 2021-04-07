
 <h1>C# valstat implementation</h1>

 - [valstat protocol](#valstat-protocol)
- [1. valstat protocol](#1-valstat-protocol)
- [2. Motivation](#2-motivation)
- [3. Implementation](#3-implementation)
- [4. Appendix A: valstat protocol implementation](#4-appendix-a-valstat-protocol-implementation)
- [5. valstat field C# synopsis](#5-valstat-field-c-synopsis)
- [6. C# valstat synopsis](#6-c-valstat-synopsis)
- [7. A bit more realistic example](#7-a-bit-more-realistic-example)
- [8. Conclusion](#8-conclusion)
- [9. Appendix A: C# valstat implementation](#9-appendix-a-c-valstat-implementation)

*As soon as we started programming, we found to our surprise that it wasn't as easy to get programs right as we had thought. Debugging had to be discovered. I can remember the exact instant when I realized that a large part of my life from then on was going to be spent in finding mistakes in my own programs.* —Maurice Wilkes discovers bugs, 1949 

## 1. [valstat](https://github.com/DBJDBJ/valstat) protocol

 `valstat` is the language agnostic standard for light but sufficient passing of full response information back to the caller.

 Please read the full details in the valstat ["core document"](https://github.com/DBJDBJ/valstat). 

 ## 2. Motivation
 C# implementation of the valstat protocol should increase interoperability of C# modules. On the code level between C# components aka "assemblies" and on the macro level between remote components. Written in possibly different languages

 On the OS level one can not throw exceptions between processes.

## 3. Implementation

For more details on C#9 using the valstat protocol please look into the Appendix A.

Part of this repository is standard [Visual Studio C#9 project](demo). It is calling into Windows native DLL ["valstat_dll"](https://github.com/DBJDBJ/valstat_dll) written in C. valstat protocol is used to communicate the result of calling "safe division" functions in that dll.

The C structure acting as the valstat structure
```cpp
static enum { icp_data_count = 255 };

typedef struct {
	int* val;
	char data[icp_data_count];
} int_charr_pair;
```
In C# above struct is declared as:
```c#
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
struct int_charr_pair
{
	public int val;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
	public string data;
}
```
There are two C functions in the dll called from C#.
```cpp
// C#9 declaration
//  [DllImport(valstat_dll_location)]
//  static extern void safe_division(out int_charr_pair vst_ptr, int numerator, int denominator);
//
// using valstat protocol to send back full info
// through the pointer to the valstat struct
// sent from the caller
// CAVEAT EMPTOR: in case of MT we would need to enter the critical section here at the entrance
__declspec(dllexport) void safe_division(int_charr_pair* vst_ptr, int numerator, int denominator);
```
The second variant is using the callback.
```cpp
/// <summary>
///  using callbacks seems more malleable as a C# interop mechanism
/// </summary>
typedef void (*safe_division_fp)(int_charr_pair*);

__declspec(dllexport) void safe_division_cb(safe_division_fp callback_, int numerator, int denominator)'
```
DLL code calling from C# is in the file [valstat_dll.cs](demo/valstat_dll.cs). Code is simple C# also extensively commented.

That is correct but perhaps not the most efficient way of calling dll's written in C from C#. The purpose of that code is to show the usefulness of the valstat protocol when crossing the language and component boundaries.
 
 ## 4. Appendix A: valstat protocol implementation

 Current (2021 Q1) C# version is C#9. 

 ## 5. valstat field C# synopsis 

 *Terminology: valstat "field" is similar to but not the same as C# "field". C# class members (not methods) can be "properties" and "fields". C# field is what is in C or C++ structure or class called "member" (not a method).*

 Without going into details, C# type hierarchy is divided on "value types", numbers and booleans and "reference types", classes and subtypes of classes. Reference type can be `null` bu design. Value type can not be `null`. Unless it is declared as a "nullable" type with a question mark on the right of the type name
 ```c#
 // C# value types PRINT, declared as nullable type
 PRINT? n_int = null ;
 // nullable PRINT use
 string sint = nint == null ? "null" : nint.ToString() ;
 ```
valstat field concept will be implemented as C# "nullable type".

## 6. C# valstat synopsis

To implement the valstat in C#, we will use the [C# "tuples"](https://github.com/dotnet/roslyn/blob/main/docs/features/tuples.md).

 C# tuples seems like the most lightweight C# feature available to successfully implement the valstat protocol.

 Following code use is to illustrate C# tuples. It is advisory, not mandatory. 

 ```c#
// not mandatory for valstat protocol implementation 
enum valstat_state { OK, ERROR, INFO, EMPTY }
```
There is no valstat type, structure, class or record required. Valstat structure is C# tuple as function return type. Following is an example of valstat enabled C# method. Note: C# has no free standing functions, just methods, functions inside a class. 
```c#
class example {
// returning a tuple made of two nullable instances of PRINT and string type.
  public static 
       // tuple is declared ad-hoc
       // C# has not typedef in a C/C++ sense
       // type? is a syntax to declare a "nullable" type
       // null can be assigned to any "value type" if it is "nullable"
       (PRINT? val, string? stat) 
     valstat_maker(valstat_state state)
 {
    switch (state)
    {
        case valstat_state.ERROR: return (null, "0xFF");    // ERROR state
        case valstat_state.INFO: return (1234, "0xFF");     // INFO state
        case valstat_state.OK: return (1234, null);         // OK state
    }
    return (null, null);                                    // EMPTY state
 }
}
 ```
 Above is the illustration on how to declare "valstat enabled" function and how to return valstat structure in one of the four states, defined by the valstat protocol. That is just an C# example to show C# syntax and use of C# tuples to implement the valstat protocol. 

 ## 7. A bit more realistic example

 We will implement the almost mandatory, "safe division", in C#. `valstat` will be used for full reporting of the result, and then for call consuming. 

 Ad hoc tuple declaration is acting as `valstat` protocol structure:
 ```c#
 // two "fields"
 // value is nullable PRINT
 // status is nullable string
 // null field is considered "empty"
 // note: field names are optional
 (UInt32? value, string? status)
 ```
Value is "nullable" unsigned 32 bit PRINT. Thus `null` can be also assigned to it. `status` is the same but based on the C# intrinsic `string` type.
 ```c#
// perform safe division of two C# "decimals"
// treated as unsigned 32 bit PRINT internally
// C# UInt32 
// return valstat protocol structure as C# tuple
static public (UInt32? value, string? status)
safe_divide(decimal numerator, decimal divisor)
{
if (numerator > UInt32.MaxValue)
// valstat ERROR state, value is "empty"
    return (null, "Error: numerator > uint32 max");

if (numerator < 0)
    return (null, "Error: numerator < 0");

if (divisor > UInt32.MaxValue)
    return (null, "Error: divisor > uint32 max");

if (divisor < 0)
    return (null, "Error: divisor < 0");

if (divisor == 0)
    return (null, "Error: divisor is 0");

try
{
    // valstat OK state, status is "empty"
    return ((UInt32)(numerator / divisor), null);
}
catch (System.Exception x)
{
    return (null, x.Message);
}
}
 ```
 Caller aka "consumer" performs the classical valstat "two step" response decoding. Step one is using only the states of the fields. Step two is using the content of the occupied field.
 ```c#
static void valstat_caller(long a, long b)
{
    // C# consuming of the tuple returned
    // uses the "tuple deconstructing"
    var (val, stat) = safe_divide(a, b);

    Log("\nsafe_divide({0},{1}) returned ", a, b);

    if (val is not null)
    {
        Log("\trezult : {0}", val);
    }

    if (stat is not null)
    {
        Log("\tstatus : {0}", stat);
    }
}
 ```
 Few use cases
 ```c#
valstat_caller(42, 13);
valstat_caller(42, 0);
valstat_caller(long.MaxValue, 0);
 ```
 ## 8. Conclusion

 What is presented is minimal C# that implement valstat protocol. Adopters might use more elaborate code. As long as it conforms to the valstat protocol, it will be able to interoperate with remote components implemented in any language,as long as everyone is using the valstat protocol.

 &copy; 2021 by dbj@dbj.org , https://dbj.org/license_dbj

 ## 9. Appendix A: C# valstat implementation

 As an valstat protocol field, C# implementation option, one could use the 
class Field defined bellow. It does implement only the behaviour of a valstat field as required by the protocol.

 ```c#
 // https://dotnetfiddle.net/0vweOZ
 #nullable enable
using System ;
using System.Linq;
using static System.Console ;

// https://dbj.org/license_dbj
public static class Field {

/// https://stackoverflow.com/a/66604069/10870835
    public static string FormattedName(this Type type)
    {
        if(type.IsGenericType)
        {
            string genericArguments = type.GetGenericArguments()
                                .Select(x => x.Name)
                                .Aggregate((x1, x2) => $"{x1}, {x2}");
            return $"{type.Name.Substring(0, type.Name.IndexOf("`"))}"
                    + $"<{genericArguments}>";
        }
        return type.Name;
    }

// this is the only behavioural attribute required 
// from a valstat field concept
// field can be in two states: "empty" or "occupied"
// namely is it "empty" or not.
public static bool is_empty<T> (T val_ ) 
{	return val_ is null ; }

// helper
public static string to_string<T> (T val_ ) 
{	return is_empty(val_) ? "empty" : val_.ToString() ;}

// printing the formatted field type and content
public static void describe<T>( T field ) 
{ 
    WriteLine("Field type: {0}, Field content: {1}", 
      typeof(T).FormattedName(), to_string(field) );  
}
	
} // Field

internal class Program
{
	// C#9 'quirk'? Need constraints but can 
	// not overload on constraints only
	static void describe_ref <T> ( T val_ )	where T : class
	{
	  WriteLine("\n");
      T? field = null ; Field.describe(field);          
      field = (T)val_ ; Field.describe(field);  
	}
	
	static void describe_val <T> ( T val_ )	where T : struct
	{
	  WriteLine("\n");
      T? field = null ; Field.describe(field);          
      field = (T)val_ ; Field.describe(field);  
	}
	
     public static void Main () 
     {
	// can not overload on constraints only
		 describe_val(42) ;
		 describe_ref("message") ;
	 }
}
 ```
