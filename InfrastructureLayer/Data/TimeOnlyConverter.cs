using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace INFL.Data
{

    public class TimeOnlyConverter : JsonConverter<TimeOnly?>
    {
        public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var timeString = reader.GetString();
                if (string.IsNullOrWhiteSpace(timeString))
                    return null; // إذا كانت القيمة فارغة

                // محاولة التحليل كـ TimeOnly
                if (TimeOnly.TryParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
                {
                    return time;
                }

                // إذا لم تنجح، تجربة التحليل باستخدام TimeSpan
                if (TimeSpan.TryParse(timeString, out var timeSpan))
                {
                    return new TimeOnly(timeSpan.Hours, timeSpan.Minutes);
                }

                throw new JsonException($"Invalid time format: {timeString}. Expected format: HH:mm.");
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return null; // إذا كانت القيمة Null
            }

            throw new JsonException($"Unexpected token parsing TimeOnly. Expected String or Null, got {reader.TokenType}.");
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


    /*  public class TimeOnlyConverter : JsonConverter<TimeOnly>
      {
          public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
          {
              // التحقق من نوع التوكن الذي تم قراءته من الـ JSON
              if (reader.TokenType == JsonTokenType.StartObject)
              {
                  JsonDocument doc = JsonDocument.ParseValue(ref reader);
                  var root = doc.RootElement;

                  // قراءة الساعة والدقيقة من الـ JSON
                  int hour = root.GetProperty("hour").GetInt32();
                  int minute = root.GetProperty("minute").GetInt32();

                  return new TimeOnly(hour, minute);
              }

              throw new JsonException("Invalid token type for TimeOnly.");
          }

          public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
          {
              // كتابة الساعة والدقيقة ككائن JSON
              writer.WriteStartObject();
              writer.WriteNumber("hour", value.Hour);
              writer.WriteNumber("minute", value.Minute);
              writer.WriteEndObject();
          }
      }*/
}
