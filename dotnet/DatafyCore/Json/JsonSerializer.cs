using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatafyCore.Json
{
    /// <summary>
    /// Converts objects to and from the JSON text format
    /// </summary>
    public sealed class JsonSerializer : ISerializer
    {
        public TextFormatType FormatType => TextFormatType.JSON;
        private readonly JsonSerializerOptions m_options;

        public JsonSerializer(bool writeIndented)
        {
            m_options = new JsonSerializerOptions
            {
                WriteIndented = writeIndented // pretty printed
            };
            m_options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            m_options.Converters.Add(new JsonClassConverter());
            m_options.Converters.Add(new JsonFieldConverter());
        }

        public string SerializeClass(Class value)
        {
            return System.Text.Json.JsonSerializer.Serialize<Class>(value, m_options);
        }
        public Class DeserializeClass(string text)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Class>(text, m_options);
        }
    }
}
