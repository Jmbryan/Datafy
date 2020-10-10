using System.Collections.Generic;

namespace Datafy.Core
{
    public interface IFactory
    {
        IValue CreateValue(ValueType valueType);
        IField CreateField(string name, ValueType valueType, IValue defaultValue, IValue minValue, IValue maxValue);
        IType CreateType(string name, TypeId typeId, IEnumerable<IField> fields);
        IObject CreateObject(ObjectId objectId, TypeId typeId);
    }
}
