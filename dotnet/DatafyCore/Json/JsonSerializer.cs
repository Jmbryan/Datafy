using System.Text.Json;
using System.Text.Json.Serialization;

namespace Datafy.Core.Json
{
    /// <summary>
    /// Converts objects to and from the JSON text format
    /// </summary>
    public sealed class JsonSerializer : ISerializer
    {
        public TextFormatType FormatType => TextFormatType.JSON;
        private readonly JsonSerializerOptions m_options;

        public JsonSerializer(IFactory factory, bool writeIndented)
        {
            m_options = new JsonSerializerOptions
            {
                WriteIndented = writeIndented // pretty printed
            };
            m_options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            m_options.Converters.Add(new JsonFieldConverter(factory));
            m_options.Converters.Add(new JsonTypeConverter(factory));
            m_options.Converters.Add(new JsonObjectConverter(factory));
        }

        public string SerializeType(IType type)
        {
            return System.Text.Json.JsonSerializer.Serialize<IType>(type, m_options);
        }
        public IType DeserializeType(string text)
        {
            return System.Text.Json.JsonSerializer.Deserialize<IType>(text, m_options);
        }

        public string SerializeObject(IObject obj)
        {
            return System.Text.Json.JsonSerializer.Serialize<IObject>(obj, m_options);
        }

        public IObject DeserializeObject(string text)
        {
            return System.Text.Json.JsonSerializer.Deserialize<IObject>(text, m_options);
        }
    }
}