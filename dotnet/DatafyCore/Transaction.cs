using System;
using System.Collections.Generic;

namespace DatafyCore
{
    public class Transaction : IDisposable
    {
        private readonly IManager m_manager;
        private readonly List<TransactionAction> m_actions;
        private readonly List<TransactionError> m_errors;
        
        public Transaction(IManager manager)
        {
            m_manager = manager;
            m_actions = new List<TransactionAction>();
            m_errors = new List<TransactionError>();

            m_manager?.StartTransaction(this);
        }

        public void Dispose()
        {
            m_manager?.FinishTransaction(this);
        }

        public void Commit()
        {
            int errorCount = m_errors.Count;
            if (errorCount > 0)
            {
                Rollback();
            }
        }

        public void Rollback()
        {
            for (int i = m_actions.Count - 1; i >= 0; --i)
            {
                m_actions[i].Undo();
            }
        }

        public bool AddClass(Class newClass)
        {
            if (m_manager.TryAddClass(newClass, this))
            {
                m_actions.Add(new AddClassTransactionAction(m_manager, this, newClass));
                return true;
            }

            return false;
        }

        public void AddError(TransactionError error)
        {
            m_errors.Add(error);
        }
    }
}
