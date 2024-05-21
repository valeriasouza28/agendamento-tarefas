using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tarefas.Models
{
  public class NullableDateTimeConverter : JsonConverter<DateTime?>
  {
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (reader.TokenType == JsonTokenType.String)
      {
        var stringValue = reader.GetString();
        if (string.IsNullOrEmpty(stringValue))
        {
          return null;
        }

        if (DateTime.TryParse(stringValue, out DateTime dateValue))
        {
          return dateValue;
        }

        throw new JsonException($"Unable to convert \"{stringValue}\" to DateTime.");
      }

      if (reader.TokenType == JsonTokenType.Null)
      {
        return null;
      }

      throw new JsonException($"Unexpected token parsing DateTime. Expected String or Null, got {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
      if (value.HasValue)
      {
        writer.WriteStringValue(value.Value.ToString("o"));
      }
      else
      {
        writer.WriteNullValue();
      }
    }
  }
}
