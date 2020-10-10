using System.Collections.Generic;
using System.Collections.ObjectModel;
using Datafy.Core;

namespace Datafy.App
{
    class Manager : IManager
    {
        private readonly Dictionary<string, IType> m_typesByName;
        private readonly Dictionary<TypeId, IType> m_typesById;
        
        private readonly List<IType> m_typeList;
        public IReadOnlyList<IType> TypeList => new ReadOnlyCollection<IType>(m_typeList);

        private Transaction m_activeTransaction = null;

        public Manager()
        {
            m_typesByName = new Dictionary<string, IType>();
            m_typesById = new Dictionary<TypeId, IType>();
            m_typeList = new List<IType>();
        }

        public IType GetType(string name)
        {
            m_typesByName.TryGetValue(name, out var value);
            return value;
        }

        public IType GetType(TypeId typeId)
        {
            m_typesById.TryGetValue(typeId, out var value);
            return value;
        }

        public void StartTransaction(Transaction transaction)
        {
            if (m_activeTransaction != null)
            {
                throw new System.ArgumentException("A transaction is already active");
            }
            m_activeTransaction = transaction;
        }

        public void FinishTransaction(Transaction transaction)
        {
            if (m_activeTransaction != transaction)
            {
                return;
            }
            m_activeTransaction = null;
        }

        public bool TryAddType(IType type, Transaction transaction)
        {
            if (m_activeTransaction != transaction)
            {
                transaction.AddError(new TransactionError($"Failed to add type '{type.Name}' - The transaction is not active"));
                return false;
            }

            if (m_typesByName.ContainsKey(type.Name))
            {
                transaction.AddError(new TransactionError($"Failed to add type '{type.Name}' - A type of that name already exists"));
                return false;
            }

            if (m_typesById.TryGetValue(type.TypeId, out var existingType))
            {
                transaction.AddError(new TransactionError($"Failed to add type '{type.Name}' - The type '{existingType.Name}' is already using TypeId {type.TypeId.Value}"));
                return false;
            }

            AddType(type);
            return true;
        }

        public void AddType(IType type)
        {
            m_typesByName.Add(type.Name, type);
            m_typesById.Add(type.TypeId, type);
            m_typeList.Add(type);
        }

        public bool TryRemoveType(IType type, Transaction transaction)
        {
            if (m_activeTransaction != transaction)
            {
                transaction.AddError(new TransactionError($"Failed to remove type '{type.Name}' - The transaction is not active"));
                return false;
            }

            if (!m_typesByName.ContainsKey(type.Name))
            {
                transaction.AddError(new TransactionError($"Failed to remove type '{type.Name}' - No type found"));
                return false;
            }

            if (!m_typesById.ContainsKey(type.TypeId))
            {
                transaction.AddError(new TransactionError($"Failed to remove type '{type.Name}' - No type found for TypeId {type.TypeId.Value}"));
                return false;
            }

            RemoveType(type);
            return true;
        }

        public void RemoveType(IType type)
        {
            m_typesByName.Remove(type.Name);
            m_typesById.Remove(type.TypeId);
            m_typeList.Remove(type);
        }

        public bool TryAddObject(IObject obj, Transaction transaction)
        {
            AddObject(obj);
            return true;
        }

        public void AddObject(IObject obj)
        {
        }

        public bool TryRemoveObject(IObject obj, Transaction transaction)
        {
            RemoveObject(obj);
            return true;
        }

        public void RemoveObject(IObject obj)
        {
        }
    }
}
