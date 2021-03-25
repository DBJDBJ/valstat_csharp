#nullable enable
/*
(c) 2021 by dbj@dbj.org -- https://dbj.org/license_dbj

                                               +-------------------------+
                    +------------+             |                         |
                    |            +<------------+     Valstat<VT,ST>      |
                    |   IField   |             |                         |
                    |   empty    |             |  IField val_            |
                    +------+-----+             |  IField stat_           |
                           ^                   |  VT? { get; set; }      |
                           |                   |  ST? { get; set; }      |
          +----------------+---------------+   |                         |
          |                                |   +-------------------------+
          |                                |
+---------+---------+           +----------+--------+
|                   |           |                   |
|  EmptyField<T>    |           |     Field<T>      |
|                   |           |                   |
|  empty : true     |           |  empty : false    |
|                   |           |  data { get; set;}|
+-------------------+           +-------------------+

IField : property `empty` reveals the identity of the implementation
Valstat: First set changes the val_ or stat_ identity from EmptyField<> to Field<>
`data` property uses the val_ or stat_ data only if not empty

*/
using System;
using System.Reflection;
using static System.Console;
using static dbj.notmacros;

namespace dbj
{
    public interface IField
    {
        public bool empty { get; }
    }

    public struct EmptyField<T> : IField
    {
        public bool empty { get => true; }

        public override string ToString() => this.GetType().Name + " { empty:" + empty + ", data : empty }";
    }

    public record Field<T> : IField
    {
        public bool empty { get => false; }

        public T data { get; set; }

        // record can have default ctors
        // struct can not
        public Field() { data = default; }
        public Field(T new_data_)
        {
            data = new_data_;
        }

        public override string ToString()
        {
            Type thistype = this.GetType();
            return thistype.Name + " { empty:" + empty + ", data:" + data + " }";
        }
    }

    struct field_user
    {
        public static void test()
        {
            IField value = new EmptyField<int>();
            Log("value field: {0} ", value);
            value = new Field<int>(42);
            // 
            var data_field = (Field<int>)value;
            // Field<int> occupied =  (Field<int>)value ;
            Log("value field: {0}", data_field);
        }
    } // field_user

    public record Valstat<VT, ST>
    {
        private IField val_ = new EmptyField<VT>();
        public VT? val
        {
            get => (val_.empty ? default : ((Field<VT>)val_).data);
            set
            {
                if (val_.empty)
                {
                    val_ = new Field<VT>();
                }
                ((Field<VT>)val_).data = value ?? default;
            }
        }
        private IField stat_ = new EmptyField<ST>();
        public ST? stat
        {
            get => (stat_.empty ? default : ((Field<ST>)stat_).data);
            set
            {
                if (stat_.empty)
                {
                    stat_ = new Field<ST>();
                }
            ((Field<ST>)stat_).data = value ?? default;
            }
        }

        public override string ToString()
        {
            var thistypename = this.GetType().Name;
            var val_str = val_.ToString();
            var stt_str = stat_.ToString();
            return thistypename + " { value:" + val_str + ", status:" + stt_str + " }";
        }
    } // Valstat<VT,ST>

    public struct actual_valstat_user
    {
        static Valstat<V, S> make<V, S>()
        {
            return new Valstat<V, S>();
        }

        static void driver<V, S>(V v_, S s_)
        {
            var vs = make<V, S>();
            Log("vs: {0}\n", vs);
            vs.val = v_;
            vs.stat = s_;
            Log("vs: {0}\n", vs);
        }
        public static void does()
        {
            driver(13, 42);
            driver("Mama", "Tata");
        }
    } // actual_valstat_user {}

    public struct valstat_user
    {
        record Valstat<VT, ST>
        where VT : class
        where ST : class
        {
            public VT? value { get; set; }
            public ST? status { get; set; }

            public Valstat()
            {
                // value =  default(VT) ;
                // status = default(ST) ;
            }
            public Valstat(VT? v_, ST? s_)
            {
                value = v_;
                status = s_;
            }
            public bool value_empty() { return value is null; }
            public bool status_empty() { return status is null; }

        }

        static void driver<VT, ST>(VT v_, ST s_)
        where VT : class
        where ST : class
        {
            Valstat<VT, ST> vst = new Valstat<VT, ST>();

            Log("\n Valstat< {0}, {1} > : {2}\n",
            typeof(VT), typeof(ST), vst
            );
            Log("\n value_empty : {0}, status_empty : {1}\n",
            vst.value_empty(), vst.status_empty(), vst
            );

            Valstat<VT, ST> vst2 = new Valstat<VT, ST>(v_, s_);

            Log("\n Valstat< {0}, {1} > : {2}\n",
            typeof(VT), typeof(ST), vst2
            );
            Log("\n value_empty : {0}, status_empty : {1}\n",
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
