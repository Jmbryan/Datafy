using Datafy.Core;

namespace Datafy.App
{
    public class SimpleObject : IObject
    {
        public ObjectId ObjectId { get; }
        public TypeId TypeId { get; }
        
        public SimpleObject(ObjectId objectId, TypeId typeId)
        {
            ObjectId = objectId;
            TypeId = typeId;
        }
    }
}