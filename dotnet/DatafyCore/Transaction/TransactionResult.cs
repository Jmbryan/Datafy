using System.Collections.Generic;

namespace Datafy.Core
{
    public class TransactionResult
    {
        public bool Success { get; set; }

        public List<IType> AddedTypes { get; }
        public List<IType> UpdatedTypes { get; }
        public List<IType> RemovedTypes { get; }

        public List<IObject> AddedObjects { get; }
        public List<IObject> UpdatedObjects { get; }
        public List<IObject> RemovedObjects { get; }

        public TransactionResult()
        {
            Success = false;

            AddedTypes = new List<IType>();
            UpdatedTypes = new List<IType>();
            RemovedTypes = new List<IType>();

            AddedObjects = new List<IObject>();
            UpdatedObjects = new List<IObject>();
            RemovedObjects = new List<IObject>();
        }
    }
}
