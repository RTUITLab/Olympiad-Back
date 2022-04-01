using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Olympiad.Shared.JsonConverters
{
    public class ByteSizeConverter : JsonConverter<ByteSize>
    {
        public override ByteSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(ByteSize));
            return ByteSize.FromBytes(reader.GetDouble());
        }

        public override void Write(Utf8JsonWriter writer, ByteSize value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Bytes);
        }
    }
}
