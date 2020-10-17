using System;

namespace Datafy.Core
{
    public interface IValue
    {
        ValueType ValueType { get; }
        bool IsValid { get; }
        void Invalidate();

        bool BoolValue { get; set; }
        int IntValue { get; set; }
        float FloatValue { get; set; }
        string StringValue { get; set; }

        void Copy(IValue other);
    }
}