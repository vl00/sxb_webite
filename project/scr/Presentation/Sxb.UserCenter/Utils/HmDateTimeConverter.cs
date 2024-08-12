using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sxb.UserCenter.Utils
{
    public class HmDateTimeConverter : IsoDateTimeConverter
    {
        public const string DefaultFormat = "yyyy-MM-dd HH:mm:ss";

        public const string MillisecondFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public const string SlashFormat = "yyyy/MM/dd HH:mm:ss";

        public HmDateTimeConverter() : this(DefaultFormat)
        {
        }

        public HmDateTimeConverter(string dateTimeFormat)
        {
            DateTimeFormat = dateTimeFormat;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is string)
            {
                if (DateTime.TryParse(reader.Value.ToString(), out DateTime dateTime))
                {
                    return dateTime;
                }

                return DateTime.MinValue;
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is string)
            {
                if (DateTime.TryParse(value.ToString(), out DateTime dateTime))
                {
                    base.WriteJson(writer, dateTime, serializer);
                    return;
                }

                base.WriteJson(writer, DateTime.MinValue, serializer);
                return;
            }
            base.WriteJson(writer, value, serializer);
        }

    }

}
