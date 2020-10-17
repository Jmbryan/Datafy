using System;
using System.Collections.Generic;

namespace Datafy.Core
{
    public interface IDbProvider : IDisposable
    {
        void Connect();
        void Disconnect();

        void OnTypesAdded(IEnumerable<IType> types);
        void OnTypesUpdated(IEnumerable<IType> types);
        void OnTypesRemoved(IEnumerable<IType> types);

        void OnObjectsAdded(IEnumerable<IObject> objs);
        void OnObjectsUpdated(IEnumerable<IObject> objs);
        void OnObjectsRemoved(IEnumerable<IObject> objs);
    }
}