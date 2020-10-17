using System;

namespace Datafy.Core
{
    public interface IField
    {
        string Name { get; }
        ValueType ValueType { get; }

        bool HasDefaultValue { get; }
        IValue DefaultValue { get; }
        void ClearDefaultValue();

        bool HasMinValue { get; }
        IValue MinValue { get; }
        void ClearMinValue();

        bool HasMaxValue { get; }
        IValue MaxValue { get; }
        void ClearMaxValue();

        void Copy(IField other);
    }
}
