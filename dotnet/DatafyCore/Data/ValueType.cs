using System;

namespace Datafy.Core
{
    public enum ValueType
    {
        Invalid = -1,

        Bool,
        Int,
        Float,
        String,

        Count
    }

    public static class ValueTypeUtils
    {
        public static string WriteValueType(ValueType valueType)
        {
            return valueType.ToString();
        }

        public static ValueType ReadValueType(string valueTypeName)
        {
            return (ValueType)Enum.Parse(typeof(ValueType), valueTypeName, true);
        }
    }
}