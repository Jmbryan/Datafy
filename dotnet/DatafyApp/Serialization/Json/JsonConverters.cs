using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Datafy.Core;

namespace Datafy.App.Json
{
    internal abstract class JsonConverterBase<T> : JsonConverter<T>
    {
        public IFactory Factory { get; }

        protected JsonConverterBase(IFactory factory)
        {
            Factory = factory;
        }

        #region Readers
        protected void ReadValue(ref Utf8JsonReader reader, IValue value, ValueType valueType)
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

        protected ulong ReadUInt64(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            var result = reader.GetUInt64();
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
        protected void WriteValue(Utf8JsonWriter writer, string name, IValue value)
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

    internal class JsonObjectConverter : JsonConverterBase<IObject>
    {
        public JsonObjectConverter(IFactory factory) : base(factory) { }

        public override IObject Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            VerifyStartObject(ref reader);

            // Object ID
            VerifyPropertyName(ref reader, "ObjectID");
            var objectId = new ObjectId(ReadUInt64(ref reader));

            // Type ID
            VerifyPropertyName(ref reader, "TypeID");
            var typeId = new TypeId(ReadUInt64(ref reader));

            VerifyEndObject(ref reader);

            return Factory.CreateObject(objectId, typeId);
        }

        public override void Write(Utf8JsonWriter writer, IObject value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("ObjectID", value.ObjectId.Value);
            writer.WriteNumber("TypeID", value.TypeId.Value);
            writer.WriteEndObject();
        }
    }

    internal class JsonTypeConverter : JsonConverterBase<IType>
    {
        public JsonTypeConverter(IFactory factory) : base(factory) { }

        public override IType Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            VerifyStartObject(ref reader);

            // Name
            VerifyPropertyName(ref reader, "Name");
            var name = ReadString(ref reader);

            // Type ID
            VerifyPropertyName(ref reader, "TypeID");
            var typeId = new TypeId(ReadUInt64(ref reader));

            // Fields array
            VerifyPropertyName(ref reader, "Fields");
            VerifyStartArray(ref reader);
            List<IField> fields = new List<IField>();
            do
            {
                var field = System.Text.Json.JsonSerializer.Deserialize<IField>(ref reader, options);
                if (field != null)
                {
                    fields.Add(field);
                }
            }
            while (reader.Read() && (reader.TokenType != JsonTokenType.EndArray));
            VerifyEndArray(ref reader);

            VerifyEndObject(ref reader);

            return Factory.CreateType(name, typeId, fields);
        }

        public override void Write(Utf8JsonWriter writer, IType value, JsonSerializerOptions options)
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

    internal class JsonFieldConverter : JsonConverterBase<IField>
    {
        public JsonFieldConverter(IFactory factory) : base(factory) { }

        public override IField Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
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
            IValue defaultValue = default, minValue = default, maxValue = default;
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
                                defaultValue = Factory.CreateValue(valueType);
                                ReadValue(ref reader, defaultValue, valueType);
                                break;
                            }
                        case "MinValue":
                            {
                                minValue = Factory.CreateValue(valueType);
                                ReadValue(ref reader, minValue, valueType);
                                break;
                            }
                        case "MaxValue":
                            {
                                maxValue = Factory.CreateValue(valueType);
                                ReadValue(ref reader, maxValue, valueType);
                                break;
                            }
                    }
                }
            }
            while (reader.Read() && (reader.TokenType != JsonTokenType.EndObject));

            VerifyEndObject(ref reader);

            return Factory.CreateField(name, valueType, defaultValue, minValue, maxValue);
        }

        public override void Write(Utf8JsonWriter writer, IField field, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Name", field.Name);
            writer.WriteString("Type", ValueTypeUtils.WriteValueType(field.ValueType));
            WriteValue(writer, "DefaultValue", field.DefaultValue);
            writer.WriteEndObject();
        }
    }
}