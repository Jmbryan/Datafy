using System;

namespace DatafyCore
{
    public abstract class TransactionAction
    {
        public abstract void Undo();
    }

    public sealed class AddClassTransactionAction : TransactionAction
    {
        private readonly IManager m_manager;
        private readonly Transaction m_transaction;
        private readonly Class m_newClass;

        public AddClassTransactionAction(IManager manager, Transaction transaction, Class newClass)
        {
            m_manager = manager;
            m_transaction = transaction;
            m_newClass = newClass;
        }

        public override void Undo()
        {
            m_manager.RemoveClass(m_newClass);
        }
    }
}