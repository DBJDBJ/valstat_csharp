#nullable enable
/*
(c) 2021 by dbj@dbj.org -- https://dbj.org/license_dbj

                    +------------+
                    |            |
                    |   IField   |
                    |            |
                    +------+-----+
                           ^
                           |
          +----------------+---------------+
          |                                |
          |                                |
+---------+---------+           +----------+--------+
|                   |           |                   |
|  EmptyField<T>    |           |     Field<T>      |
|                   |           |                   |
+-------------------+           +-------------------+

*/
using System;
namespace dbj
{
    public interface IField
    {
        public bool empty { get; }
    }

    public struct EmptyField<T> : IField
    {
        public bool empty { get => true; }
    }

    public record Field<T> : IField
    {
        private bool empty_ = true;
        public bool empty { get => false; internal set => empty_ = value; }

        private T? data_;

        public T? data { get; internal set; }

        public IField field
        {
            get
            {
                if (empty) return new EmptyField<T>();
                return this;
            }
        }

        // record can have default ctors
        // struct can not
        public Field()
        {
            empty = true;
            data_ = default;
            data = default;
        }
        public Field(T new_data_)
        {
            empty = false;
            data_ = default;
            data = new_data_;
        }
        public override string ToString() => "{ empty:" + empty + ", data:" + data + " }";
    }
    public struct field_user
    {
        record Valstat<VT, ST>
        // where VT : class   
        // where ST : class   
        {
            public VT? value { get; set; }
            public ST? status { get; set; }

            public Valstat() { }
            public Valstat(VT? v_, ST? s_)
            {
                value = v_;
                status = s_;
            }

            public bool value_empty() { return value == null; }
            public bool status_empty() { return status == null; }

        }

        static void driver<VT, ST>(VT v_, ST s_)
        // where VT : class   
        // where ST : class  
        {
            Valstat<VT, ST> vst = new Valstat<VT, ST>();

            Console.WriteLine("\n Valstat< {0}, {1} > : {2}\n",
            typeof(VT), typeof(ST), vst
            );
            Console.WriteLine("\n value_empty : {0}, status_empty : {1}\n",
            vst.value_empty(), vst.status_empty(), vst
            );

            Valstat<VT, ST> vst2 = new Valstat<VT, ST>(v_, s_);

            Console.WriteLine("\n Valstat< {0}, {1} > : {2}\n",
            typeof(VT), typeof(ST), vst2
            );
            Console.WriteLine("\n value_empty : {0}, status_empty : {1}\n",
            vst.value_empty(), vst.status_empty(), vst2
            );
        }
        public static void use()
        {
            driver((object)1, (object)2);
            driver("Bimbi", "Bumbi");
        }
    }
} // dbj
#nullable disable
