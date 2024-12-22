using System.Text.Json;
using System.Text.Json.Serialization;

namespace APPL.Services
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly?>
    {
        public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var timeString = reader.GetString();

                if (TimeOnly.TryParseExact(timeString, "HH:mm", null, System.Globalization.DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }

            return null; // إذا كانت القيمة فارغة أو غير قابلة للتحويل
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("HH:mm"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

}
