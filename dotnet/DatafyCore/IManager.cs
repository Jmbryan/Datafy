
using System.Collections.Generic;

namespace Datafy.Core
{
    public interface IManager
    {
        IReadOnlyList<IType> TypeList { get; }
        IType GetType(TypeId typeId);
        IType GetType(string name);

        void StartTransaction(Transaction transaction);
        void FinishTransaction(Transaction transaction);
        void CommitTransaction(Transaction transaction, TransactionResult result);

        bool TryAddType(IType type, Transaction transaction);
        bool TryUpdateType(IType type, Transaction transaction);
        bool TryRemoveType(IType type, Transaction transaction);

        bool TryAddObject(IObject obj, Transaction transaction);
        bool TryRemoveObject(IObject obj, Transaction transaction);
    }
}