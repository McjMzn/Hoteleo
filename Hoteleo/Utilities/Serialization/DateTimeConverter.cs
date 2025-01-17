﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hoteleo.Utilities.Serialization
{
    internal class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _dateTimeFormat;

        public DateTimeConverter(string dateTimeFormat)
        {
            _dateTimeFormat = dateTimeFormat;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), @"yyyyMMdd", null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyyMMdd"));
        }
    }
}
