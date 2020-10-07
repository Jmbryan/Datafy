using System;

namespace DatafyCore
{
    public interface IManager
    {
        void StartTransaction(Transaction transaction);
        void FinishTransaction(Transaction transaction);

        bool TryAddClass(Class newClass, Transaction transaction);
        bool TryRemoveClass(Class removeClass, Transaction transaction);
        void RemoveClass(Class removeClass);
    }
}