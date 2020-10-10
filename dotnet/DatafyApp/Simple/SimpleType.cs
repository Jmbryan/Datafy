using System.Collections.Generic;
using System.Collections.ObjectModel;
using Datafy.Core;

namespace Datafy.App
{
    public class SimpleType : IType
    {
        public string Name { get; }
        public TypeId TypeId { get; }

        private List<SimpleField> m_fields;
        public IReadOnlyList<IField> Fields => new ReadOnlyCollection<SimpleField>(m_fields);

        public SimpleType(string name, TypeId typeId, IEnumerable<SimpleField> fields)
        {
            Name = name;
            TypeId = typeId;
            SetFields(fields);
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