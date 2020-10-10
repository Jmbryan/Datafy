using System.Collections.Generic;
using Datafy.Core;

namespace Datafy.App
{
    public class SimpleFactory : IFactory
    {
        public IValue CreateValue(ValueType valueType)
        {
            return new SimpleValue(valueType);
        }

        public IField CreateField(string name, ValueType valueType, IValue defaultValue, IValue minValue, IValue maxValue)
        {
            return new SimpleField(name, valueType, defaultValue as SimpleValue, minValue as SimpleValue, maxValue as SimpleValue);
        }

        public IType CreateType(string name, TypeId typeId, IEnumerable<IField> fields)
        {
            var simpleFields = new List<SimpleField>();
            foreach (var field in fields)
            {
                if (field is SimpleField simpleField)
                {
                    simpleFields.Add(simpleField);
                }
            }
            return new SimpleType(name, typeId, simpleFields);
        }

        public IObject CreateObject(ObjectId objectId, TypeId typeId)
        {
            return new SimpleObject(objectId, typeId);
        }
    }
}