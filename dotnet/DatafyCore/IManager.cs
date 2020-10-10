
using System.Collections.Generic;

namespace Datafy.Core
{
    public interface IManager
    {
        IReadOnlyList<IType> TypeList { get; }

        void StartTransaction(Transaction transaction);
        void FinishTransaction(Transaction transaction);

        bool TryAddType(IType type, Transaction transaction);
        bool TryRemoveType(IType type, Transaction transaction);
        void RemoveType(IType type);

        bool TryAddObject(IObject obj, Transaction transaction);
        bool TryRemoveObject(IObject obj, Transaction transaction);
        void RemoveObject(IObject obj);
    }
}