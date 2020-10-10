using System;

namespace Datafy.Core
{
    public abstract class TransactionAction
    {
        public abstract void Undo();
    }

    public sealed class AddTypeTransactionAction : TransactionAction
    {
        private readonly IManager m_manager;
        private readonly Transaction m_transaction;
        private readonly IType m_newType;

        public AddTypeTransactionAction(IManager manager, Transaction transaction, IType type)
        {
            m_manager = manager;
            m_transaction = transaction;
            m_newType = type;
        }

        public override void Undo()
        {
            m_manager.RemoveType(m_newType);
        }
    }

    public sealed class AddObjectTransactionAction : TransactionAction
    {
        private readonly IManager m_manager;
        private readonly Transaction m_transaction;
        private readonly IObject m_newObject;

        public AddObjectTransactionAction(IManager manager, Transaction transaction, IObject obj)
        {
            m_manager = manager;
            m_transaction = transaction;
            m_newObject = obj;
        }

        public override void Undo()
        {
            m_manager.RemoveObject(m_newObject);
        }
    }
}