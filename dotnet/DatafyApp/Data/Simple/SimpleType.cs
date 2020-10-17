using System.Collections.Generic;
using System.Collections.ObjectModel;
using Datafy.Core;

namespace Datafy.App
{
    public class SimpleType : IType
    {
        public TypeId TypeId { get; }
        public string Name { get; private set; }
        
        private List<SimpleField> m_fields;
        public IReadOnlyList<IField> Fields => new ReadOnlyCollection<SimpleField>(m_fields);

        public SimpleType(string name, TypeId typeId, IEnumerable<SimpleField> fields)
        {
            TypeId = typeId;
            Name = name;
            SetFields(fields);
        }

        public SimpleType(SimpleType other)
        {
            TypeId = other.TypeId;
            Copy(other);
        }

        public void Copy(IType other)
        {
            Name = other.Name;

            m_fields.Clear();
            foreach (var otherField in other.Fields)
            {
                m_fields.Add(new SimpleField(otherField));
            }
        }

        public IType Clone()
        {
            return new SimpleType(this);
        }

        public void SetFields(IEnumerable<SimpleField> fields)
        {
            m_fields = new List<SimpleField>(fields);
        }

        public void AddField(SimpleField field)
        {
            m_fields.Add(field);
        }
    }
}