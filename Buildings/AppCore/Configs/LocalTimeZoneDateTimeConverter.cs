namespace AppCore.Configs;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class LocalTimeZoneDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), out var dateTime))
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local);
        }

        return DateTime.MinValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.Local);
        writer.WriteStringValue(localTime.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}
