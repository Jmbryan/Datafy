using System;

namespace DatafyCore
{
    /// <summary>
    /// Converts objects to and from a textual representation
    /// </summary>
    public interface ISerializer
    {
        TextFormatType FormatType { get; }

        string SerializeClass(Class value);
        Class DeserializeClass(string text);
    }
}