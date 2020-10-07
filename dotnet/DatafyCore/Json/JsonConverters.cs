using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatafyCore.Json
{
    internal abstract class JsonConverterBase<T> : JsonConverter<T>
    {
        #region Readers
        protected void ReadValue(ref Utf8JsonReader reader, Value value, ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Bool:
                    {
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }
                        value.BoolValue = reader.GetBoolean();
                        break;
                    }
                case ValueType.Int:
                    {
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }
                        value.IntValue = reader.GetInt32();
                        break;
                    }
                case ValueType.Float:
                    {
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }
                        value.FloatValue = (float)reader.GetDouble();
                        break;
                    }
                case ValueType.String:
                    {
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }
                        value.StringValue = reader.GetString();
                        break;
                    }
                default:
                    throw new JsonException();
            }
        }

        protected void VerifyStartObject(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
        }

        protected void VerifyEndObject(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }
        }

        protected void VerifyStartArray(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            reader.Read();
        }

        protected void VerifyEndArray(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException();
            }

            reader.Read();
        }

        protected void VerifyPropertyName(ref Utf8JsonReader reader, string expectedPropertyName)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = reader.GetString();
            if (propertyName != expectedPropertyName)
            {
                throw new JsonException();
            }

            reader.Read();
        }

        protected string ReadString(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var result = reader.GetString();
            reader.Read();
            return result;
        }

        protected bool ReadBool(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            var result = reader.GetBoolean();
            reader.Read();
            return result;
        }

        protected ushort ReadUInt16(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            var result = reader.GetUInt16();
            reader.Read();
            return result;
        }

        protected double ReadDouble(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            var result = reader.GetDouble();
            reader.Read();
            return result;
        }
        #endregion

        #region Writers
        protected void WriteValue(Utf8JsonWriter writer, string name, Value value)
        {
            switch (value.ValueType)
            {
                case ValueType.Bool:
                    {
                        writer.WriteBoolean(name, value.BoolValue);
                        break;
                    }
                case ValueType.Int:
                    {
                        writer.WriteNumber(name, value.IntValue);
                        break;
                    }
                case ValueType.Float:
                    {
                        writer.WriteNumber(name, value.FloatValue);
                        break;
                    }
                case ValueType.String:
                    {
                        writer.WriteString(name, value.StringValue);
                        break;
                    }
                default:
                    throw new JsonException();
            }
        }
        #endregion
    }

    internal class JsonClassConverter : JsonConverterBase<Class>
    {
        public override Class Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            VerifyStartObject(ref reader);

            // Name
            VerifyPropertyName(ref reader, "Name");
            var name = ReadString(ref reader);

            // Type ID
            VerifyPropertyName(ref reader, "TypeID");
            var typeId = new TypeId(ReadUInt16(ref reader));

            // Fields array
            VerifyPropertyName(ref reader, "Fields");
            VerifyStartArray(ref reader);
            List<Field> fields = new List<Field>();
            do
            {
                var field = System.Text.Json.JsonSerializer.Deserialize<Field>(ref reader, options);
                if (field != null)
                {
                    fields.Add(field);
                }
            }
            while (reader.Read() && (reader.TokenType != JsonTokenType.EndArray));
            VerifyEndArray(ref reader);

            VerifyEndObject(ref reader);

            return new Class(name, typeId, fields);
        }

        public override void Write(Utf8JsonWriter writer, Class value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Name", value.Name);
            writer.WriteNumber("TypeID", value.TypeId.Value);
            writer.WriteStartArray("Fields");
            foreach (var field in value.Fields)
            {
                System.Text.Json.JsonSerializer.Serialize(writer, field, options);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }

    internal class JsonFieldConverter : JsonConverterBase<Field>
    {
        public override Field Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            VerifyStartObject(ref reader);

            // Name
            VerifyPropertyName(ref reader, "Name");
            var name = ReadString(ref reader);

            // Type
            VerifyPropertyName(ref reader, "Type");
            string valueTypeName = ReadString(ref reader);
            var valueType = ValueTypeUtils.ReadValueType(valueTypeName);

            // Optional properties
            string propertyName;
            Value defaultValue = null;
            do
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "DefaultValue":
                            {
                                defaultValue = new Value(valueType);
                                ReadValue(ref reader, defaultValue, valueType);
                                break;
                            }
                    }
                }
            }
            while (reader.Read() && (reader.TokenType != JsonTokenType.EndObject));

            VerifyEndObject(ref reader);

            return new Field(name, valueType, defaultValue);
        }

        public override void Write(Utf8JsonWriter writer, Field field, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Name", field.Name);
            writer.WriteString("Type", ValueTypeUtils.WriteValueType(field.ValueType));
            WriteValue(writer, "DefaultValue", field.DefaultValue);
            writer.WriteEndObject();
        }
    }
}