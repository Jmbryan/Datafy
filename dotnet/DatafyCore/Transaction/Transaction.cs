using System;
using System.Collections.Generic;

namespace Datafy.Core
{
    public class Transaction : IDisposable
    {
        private readonly IManager m_manager;
        private readonly List<TransactionAction> m_actions;
        private readonly List<TransactionError> m_errors;

        public bool IsRollingBack { get; private set; }
        
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

        public TransactionResult Commit()
        {
            var result = new TransactionResult();

            int errorCount = m_errors.Count;
            if (errorCount > 0)
            {
                result.Success = false;
                Rollback();
            }
            else
            {
                result.Success = true;
                foreach (var action in m_actions)
                {
                    action.Commit(result);
                }
            }

            m_manager.CommitTransaction(this, result);

            return result;
        }

        public void Rollback()
        {
            IsRollingBack = true;
            for (int i = m_actions.Count - 1; i >= 0; --i)
            {
                m_actions[i].Undo();
            }
        }

        public void AddType(IType type)
        {
            AddAction(new AddTypeTransactionAction(m_manager, this, type));
        }

        public void UpdateType(IType type)
        {
            AddAction(new UpdateTypeTransactionAction(m_manager, this, type));
        }

        public void AddObject(IObject obj)
        {
            AddAction(new AddObjectTransactionAction(m_manager, this, obj));
        }

        private void AddAction(TransactionAction action)
        {
            action.Do();
            m_actions.Add(action);
        }

        public void AddError(TransactionError error)
        {
            m_errors.Add(error);
        }
    }
}
