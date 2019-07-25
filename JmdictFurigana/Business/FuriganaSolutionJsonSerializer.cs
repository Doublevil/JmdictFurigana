using System;
using JmdictFurigana.Models;
using Newtonsoft.Json;

namespace JmdictFurigana.Business
{
    public class FuriganaSolutionJsonSerializer : JsonConverter<FuriganaSolution>
    {
        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, FuriganaSolution value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("text");
            writer.WriteValue(value.Vocab.KanjiReading);
            writer.WritePropertyName("reading");
            writer.WriteValue(value.Vocab.KanaReading);
            
            // Write furigana array
            writer.WritePropertyName("furigana");
            writer.WriteStartArray();
            foreach (var part in value.BreakIntoParts())
            {
                writer.WriteStartObject();
                writer.WritePropertyName("ruby");
                writer.WriteValue(part.Text);
                if (!string.IsNullOrEmpty(part.Furigana))
                {
                    writer.WritePropertyName("rt");
                    writer.WriteValue(part.Furigana);
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
        /// <param name="hasExistingValue">The existing value has a value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override FuriganaSolution ReadJson(JsonReader reader, Type objectType, FuriganaSolution existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
