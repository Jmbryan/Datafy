using System;

namespace Datafy.Core
{
    /// <summary>
    /// A 64 bit id that uniquely identifies an object
    /// </summary>
    public readonly struct ObjectId : IComparable<ObjectId>, IEquatable<ObjectId>
    {
        public ulong Value { get; }
        public ObjectId(ulong value) { Value = value; }
        public static readonly ObjectId Null = new ObjectId(0);
        public bool IsNull => this == Null;
        public bool Equals(ObjectId other) => Value.Equals(other.Value);
        public int CompareTo(ObjectId other) => Value.CompareTo(other.Value);
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is ObjectId other && Equals(other);
        }
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
        public static bool operator ==(ObjectId a, ObjectId b) => a.CompareTo(b) == 0;
        public static bool operator !=(ObjectId a, ObjectId b) => !(a == b);
    }
}