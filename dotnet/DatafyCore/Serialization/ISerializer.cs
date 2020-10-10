using System;

namespace Datafy.Core
{
    /// <summary>
    /// Converts objects to and from a textual representation
    /// </summary>
    public interface ISerializer
    {
        TextFormatType TextFormat { get; }

        string SerializeType(IType type);
        IType DeserializeType(string text);

        string SerializeObject(IObject obj);
        IObject DeserializeObject(string text);
    }
}