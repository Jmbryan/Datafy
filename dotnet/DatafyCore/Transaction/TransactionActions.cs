using System;

namespace Datafy.Core
{
    public abstract class TransactionAction
    {
        protected IManager Manager { get; }
        protected Transaction Transaction { get; }

        public TransactionAction(IManager manager, Transaction transaction)
        {
            Manager = manager;
            Transaction = transaction;
        }

        public abstract void Do();
        public abstract void Undo();
        public abstract void Commit(TransactionResult result);
    }

    public sealed class AddTypeTransactionAction : TransactionAction
    {
        private readonly IType m_newType;

        public AddTypeTransactionAction(IManager manager, Transaction transaction, IType newType)
            : base(manager, transaction)
        {
            m_newType = newType;
        }

        public override void Do()
        {
            Manager.TryAddType(m_newType, Transaction);
        }

        public override void Undo()
        {
            Manager.TryRemoveType(m_newType, Transaction);
        }

        public override void Commit(TransactionResult result)
        {
            result.AddedTypes.Add(m_newType);
        }
    }

    public sealed class UpdateTypeTransactionAction : TransactionAction
    {
        private readonly IType m_newType;
        private readonly IType m_oldType;

        public UpdateTypeTransactionAction(IManager manager, Transaction transaction, IType updateType)
            : base(manager, transaction)
        {
            m_newType = updateType;

            var type = Manager.GetType(updateType.TypeId);
            if (type != null)
            {
                m_oldType = type.Clone();
            }
        }

        public override void Do()
        {
            Manager.TryUpdateType(m_newType, Transaction);
        }

        public override void Undo()
        {
            Manager.TryUpdateType(m_oldType, Transaction);
        }

        public override void Commit(TransactionResult result)
        {
            result.UpdatedTypes.Add(m_newType);
        }
    }

    public sealed class AddObjectTransactionAction : TransactionAction
    {
        private readonly IObject m_newObject;

        public AddObjectTransactionAction(IManager manager, Transaction transaction, IObject newObject)
            : base(manager, transaction)
        {
            m_newObject = newObject;
        }

        public override void Do()
        {
            Manager.TryAddObject(m_newObject, Transaction);
        }

        public override void Undo()
        {
            Manager.TryRemoveObject(m_newObject, Transaction);
        }

        public override void Commit(TransactionResult result)
        {
            result.AddedObjects.Add(m_newObject);
        }
    }
}