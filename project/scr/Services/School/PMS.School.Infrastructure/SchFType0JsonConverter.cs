using iSchool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PMS.School.Infrastructure
{
    public class SchFType0JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SchFType0) || objectType == typeof(SchFType0?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var j = JToken.Load(reader);
            if (j.Type == JTokenType.Null) return null;
            if (j.Type == JTokenType.String)
            {
                var str = (j as JValue).ToString();
                return SchFType0.Parse(str);
            }
            else if (j.Type == JTokenType.String)
            {
                return new SchFType0(j.Value<byte>("Grade"), j.Value<byte>("Type"), j.Value<bool>("Discount"), j.Value<bool>("Diglossia"), j.Value<bool>("Chinese"));
            }
            else {
                return null;
            }

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) writer.WriteNull();
            else writer.WriteValue(value.ToString());
        }
    }
}
