using System.Collections.Generic;

namespace Datafy.Core
{
    public interface IType
    {
        string Name { get; }
        TypeId TypeId { get; }

        IReadOnlyList<IField> Fields { get; }

        void Copy(IType other);
        IType Clone();
    }
}