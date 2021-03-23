// https://stackoverflow.com/a/19831653/10870835
using System;

public record runtime_nullable<T>
{
    public T data { get; set; }

    public runtime_nullable()    { is_nullable(); }
    public runtime_nullable( T item_)    {  is_nullable();  data = item_;  }
    static public void is_nullable()
    {
        var type = typeof(T);

        if (Nullable.GetUnderlyingType(type) != null)
            return;

        if (type.IsClass)
            return;

        throw new InvalidOperationException(
            string.Format(
            "Type {0} is not nullable or reference type.", typeof(T).Name
            )
        );
    }

    public bool IsNull()
    {
        return data == null;
    }
}

struct whatever
{
    public static void runtime_nullable()
    {
        try
        {
            var foo1 = new runtime_nullable<int?>();
            Console.WriteLine(foo1.IsNull());

            var foo2 = new runtime_nullable<string>();
            Console.WriteLine(foo2.IsNull());

            var foo3 = new runtime_nullable<int>();  // THROWS
            Console.WriteLine(foo3.IsNull());
        }
        catch (InvalidOperationException x)
        {
            dbj.notmacros.Log("Exception: {0} ", x);
        }
    }
}