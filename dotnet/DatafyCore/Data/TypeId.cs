using System;

namespace Datafy.Core
{
    /// <summary>
    /// A 64 bit id that uniquely identifies an object type
    /// </summary>
    public readonly struct TypeId : IComparable<TypeId>, IEquatable<TypeId>
    {
        public ulong Value { get; }
        public TypeId(ulong value) { Value = value; }
        public static readonly TypeId Null = new TypeId(0);
        public bool IsNull => this == Null;
        public bool Equals(TypeId other) => Value.Equals(other.Value);
        public int CompareTo(TypeId other) => Value.CompareTo(other.Value);
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is TypeId other && Equals(other);
        }
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
        public static bool operator ==(TypeId a, TypeId b) => a.CompareTo(b) == 0;
        public static bool operator !=(TypeId a, TypeId b) => !(a == b);
    }
}