using Datafy.Core;

namespace Datafy.App
{
    public class SimpleField : IField
    {
        public string Name { get; private set; }
        public ValueType ValueType { get; private set; }

        private SimpleValue m_defaultValue = null;
        public IValue DefaultValue => m_defaultValue ?? new SimpleValue(ValueType);
        public bool HasDefaultValue => m_defaultValue != null;
        public void ClearDefaultValue() => m_defaultValue = null;

        private SimpleValue m_minValue = null;
        public IValue MinValue => m_minValue ?? new SimpleValue(ValueType);
        public bool HasMinValue => m_minValue != null;
        public void ClearMinValue() => m_minValue = null;

        private SimpleValue m_maxValue = null;
        public IValue MaxValue => m_maxValue ?? new SimpleValue(ValueType);
        public bool HasMaxValue => m_maxValue != null;
        public void ClearMaxValue() => m_maxValue = null;

        public SimpleField(string name, ValueType valueType, SimpleValue defaultValue = null, SimpleValue minValue = null, SimpleValue maxValue = null)
        {
            Name = name;
            ValueType = valueType;
            SetDefaultValue(defaultValue);
            SetMinValue(minValue);
            SetMaxValue(maxValue);
        }

        public SimpleField(IField other)
        {
            Copy(other);
        }

        public void Copy(IField other)
        {
            Name = other.Name;
            ValueType = other.ValueType;
            SetDefaultValue(other.HasDefaultValue ? new SimpleValue(other.DefaultValue) : null);
            SetMinValue(other.HasMinValue ? new SimpleValue(other.MinValue) : null);
            SetMaxValue(other.HasMaxValue ? new SimpleValue(other.MaxValue) : null);
        }

        public void SetDefaultValue(SimpleValue defaultValue)
        {
            if (defaultValue != null && defaultValue.ValueType != ValueType)
            {
                throw new System.ArgumentException($"Default value ValueType {defaultValue.ValueType} does not match field ValueType {ValueType}");
            }
            m_defaultValue = defaultValue;
        }

        public void SetMinValue(SimpleValue minValue)
        {
            if (minValue != null && minValue.ValueType != ValueType)
            {
                throw new System.ArgumentException($"Min value ValueType {minValue.ValueType} does not match field ValueType {ValueType}");
            }
            m_minValue = minValue;
        }

        public void SetMaxValue(SimpleValue maxValue)
        {
            if (maxValue != null && maxValue.ValueType != ValueType)
            {
                throw new System.ArgumentException($"Max value ValueType {maxValue.ValueType} does not match field ValueType {ValueType}");
            }
            m_maxValue = maxValue;
        }
    }
}