using System;
namespace Nitric.Api.KeyValue
{
    public class Expression
    {
        public string Operand { get; private set; }
        public string Op { get; private set; }
        public string Value { get; private set; }

        public Expression(string operand, string op, string value)
        {
            this.Operand = operand;
            this.Op = op;
            this.Value = value;
        }
        public override string ToString()
        {
            return GetType().Name
                + "[operand=" + this.Operand
                + ", operator=" + this.Op
                + ", value=" + this.Value
                + "]";
        }
    }
}
