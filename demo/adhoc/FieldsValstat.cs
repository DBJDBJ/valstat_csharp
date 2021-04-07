#nullable enable
/*
(c) 2021 by dbj@dbj.org -- https://dbj.org/license_dbj

                                               +-------------------------+
                    +------------+             |                         |
                    |            +<------------+     FieldsValstat<VT,ST>      |
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
FieldsValstat: First set changes the val_ or stat_ identity from EmptyField<> to Field<>
`data` property uses the val_ or stat_ data only if not empty

*/
using System;
// using System.Reflection;
// using static System.Console;
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

        public override string ToString() => this.GetType().Name + " {\n\t empty:" + empty + ",\n\t data : empty }";
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
            return thistype.Name + " {\n\t empty:" + empty + ",\n\t data:" + data + " }\n";
        }
    }

    public record FieldsValstat<VT, ST>
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

        public FieldsValstat(VT v_) => val = v_;
        public FieldsValstat(ST s_) => stat = s_;
        public FieldsValstat(VT v_, ST s_) { val = v_; stat = s_; }
        public FieldsValstat() { }

        public override string ToString()
        {
            var thistypename = this.GetType().Name;
            var val_str = val_.ToString();
            var stt_str = stat_.ToString();
            return "\n" + thistypename + " {\n value:" + val_str + ",\n status:" + stt_str + " }";
        }
    } // FieldsValstat<VT,ST>

    internal partial class test
    {
        public static void test_field()
        {
            IField value = new EmptyField<int>();
            Log("value field: {0} ", value);
            value = new Field<int>(42);
            // 
            var data_field = (Field<int>)value;
            // Field<int> occupied =  (Field<int>)value ;
            Log("value field: {0}", data_field);
        }

        static FieldsValstat<V, S> make<V, S>()
        {
            return new FieldsValstat<V, S>();
        }

        static void driver<V, S>(V v_, S s_)
        {
            Log(new FieldsValstat<V, S>());
            Log(new FieldsValstat<V, S>(v_));
            Log(new FieldsValstat<V, S>(s_));
            Log(new FieldsValstat<V, S>(v_, s_));
        }
        public static void test_valstat_with_fields()
        {
            Log("\ntest_valstat_with_fields");
            driver(13, 42);
            driver("Mama", "Tata");
        }

    }
} // dbj
#nullable disable
