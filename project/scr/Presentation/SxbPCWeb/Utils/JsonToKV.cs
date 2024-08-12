using Newtonsoft.Json;
using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Utils
{
    public static class JsonToKV
    {
        public static List<KeyValue> ConvertObject(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<KeyValue>();
            return JsonConvert.DeserializeObject<List<KeyValue>>(json).ToList();
        }
    }
}
