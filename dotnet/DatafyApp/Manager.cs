using System;
using System.Collections.Generic;
using DatafyCore;

namespace DatafyApp
{
    class Manager : IManager
    {
        private readonly Dictionary<string, Class> m_classesByName;
        private readonly Dictionary<TypeId, Class> m_classesByTypeId;
        private readonly List<Class> m_classList;

        private Transaction m_activeTransaction = null;

        public Manager()
        {
            m_classesByName = new Dictionary<string, Class>();
            m_classesByTypeId = new Dictionary<TypeId, Class>();
            m_classList = new List<Class>();
        }

        public Class GetClass(string name)
        {
            m_classesByName.TryGetValue(name, out var value);
            return value;
        }

        public Class GetClass(TypeId typeId)
        {
            m_classesByTypeId.TryGetValue(typeId, out var value);
            return value;
        }

        public void StartTransaction(Transaction transaction)
        {
            if (m_activeTransaction != null)
            {
                throw new ArgumentException("A transaction is already active");
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

        public bool TryAddClass(Class newClass, Transaction transaction)
        {
            if (m_activeTransaction != transaction)
            {
                transaction.AddError(new TransactionError($"Failed to add class '{newClass.Name}' - The transaction is not active"));
                return false;
            }

            if (m_classesByName.ContainsKey(newClass.Name))
            {
                transaction.AddError(new TransactionError($"Failed to add class '{newClass.Name}' - A class of that name already exists"));
                return false;
            }

            if (m_classesByTypeId.TryGetValue(newClass.TypeId, out var existingClass))
            {
                transaction.AddError(new TransactionError($"Failed to add class '{newClass.Name}' - The class '{existingClass.Name}' is already using TypeId {newClass.TypeId.Value}"));
                return false;
            }

            AddClass(newClass);
            return true;
        }

        public void AddClass(Class newClass)
        {
            m_classesByName.Add(newClass.Name, newClass);
            m_classesByTypeId.Add(newClass.TypeId, newClass);
            m_classList.Add(newClass);
        }

        public bool TryRemoveClass(Class removeClass, Transaction transaction)
        {
            if (m_activeTransaction != transaction)
            {
                transaction.AddError(new TransactionError($"Failed to remove class '{removeClass.Name}' - The transaction is not active"));
                return false;
            }

            if (!m_classesByName.ContainsKey(removeClass.Name))
            {
                transaction.AddError(new TransactionError($"Failed to remove class '{removeClass.Name}' - No class found"));
                return false;
            }

            if (!m_classesByTypeId.ContainsKey(removeClass.TypeId))
            {
                transaction.AddError(new TransactionError($"Failed to remove class '{removeClass.Name}' - No class found for TypeId {removeClass.TypeId.Value}"));
                return false;
            }

            RemoveClass(removeClass);
            return true;
        }

        public void RemoveClass(Class removeClass)
        {
            m_classesByName.Remove(removeClass.Name);
            m_classesByTypeId.Remove(removeClass.TypeId);
            m_classList.Remove(removeClass);
        }
    }
}
