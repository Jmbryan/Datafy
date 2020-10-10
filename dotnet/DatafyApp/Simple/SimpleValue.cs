using Datafy.Core;

namespace Datafy.App
{
    public class SimpleValue : IValue
    {
        public ValueType ValueType { get; private set; } = ValueType.Invalid;
        private double m_numericValue = 0.0;
        private string m_stringValue = string.Empty;
        private object m_objectValue = null;

        public bool IsValid => ValueType > ValueType.Invalid && ValueType < ValueType.Count;
        public void Invalidate()
        {
            ValueType = ValueType.Invalid;
            ResetValue();
        }

        public SimpleValue(bool value)
        {
            BoolValue = value;
        }

        public SimpleValue(int value)
        {
            IntValue = value;
        }

        public SimpleValue(float value)
        {
            FloatValue = value;
        }

        public SimpleValue(string value)
        {
            StringValue = value;
        }

        public SimpleValue(SimpleValue other)
        {
            ValueType = other.ValueType;
            m_numericValue = other.m_numericValue;
            m_stringValue = other.m_stringValue;
            m_objectValue = other.m_objectValue;
        }

        public SimpleValue(ValueType valueType)
        {
            ValueType = valueType;
        }

        public bool BoolValue
        {
            get
            {
                if (ValueType != ValueType.Bool)
                {
                    throw new System.ArgumentException("Value is not type Bool");
                }
                return (m_numericValue > 0);
            }
            set
            {
                ValueType = ValueType.Bool;
                ResetValue();
                m_numericValue = (value ? 1 : 0);
            }
        }

        public int IntValue
        {
            get
            {
                if (ValueType != ValueType.Int)
                {
                    throw new System.ArgumentException("Value is not type Int");
                }
                return (int)m_numericValue;
            }
            set
            {
                ValueType = ValueType.Int;
                ResetValue();
                m_numericValue = value;
            }
        }

        public float FloatValue
        {
            get
            {
                if (ValueType != ValueType.Float)
                {
                    throw new System.ArgumentException("Value is not type Float");
                }
                return (float)m_numericValue;
            }
            set
            {
                ValueType = ValueType.Float;
                ResetValue();
                m_numericValue = value;
            }
        }

        public string StringValue
        {
            get
            {
                if (ValueType != ValueType.String)
                {
                    throw new System.ArgumentException("Value is not type String");
                }
                return m_stringValue;
            }
            set
            {
                ValueType = ValueType.String;
                ResetValue();
                m_stringValue = value;
            }
        }

        private void ResetValue()
        {
            m_numericValue = 0.0;
            m_stringValue = string.Empty;
            m_objectValue = null;
        }
    }
}