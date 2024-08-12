using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    public static class JsonConvertExtension
    {
        public static T TryDeserializeObject<T>(string value, T def = default)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {
            }
            return def;
        }
    }
}
