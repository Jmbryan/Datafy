using System;

namespace Datafy.Core
{
    public interface IObject
    {
        ObjectId ObjectId { get; }
        TypeId TypeId { get; }
    }
}