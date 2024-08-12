using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProductManagement.Tool.HttpRequest.Encrypt;
using Newtonsoft.Json;
using System.Web;

namespace ProductManagement.Tool.HttpRequest.Option
{
    internal class UrlBaseOption : BaseOption
    {
        public override string UrlPath { get; }

        public UrlBaseOption(string url, object data = null)
        {
            UrlPath = url + ToQuery(data);
        }


        public static string ToQuery(object data)
        {
            string str = string.Empty;

            if (data == null)
            {
                return str;
            }


            Type type = data.GetType();
            System.Reflection.PropertyInfo[] propertyInfos = type.GetProperties();

            foreach (var prop in propertyInfos)
            {
                if (prop.CanRead)
                {
                    str += "&" + prop.Name + "=";
                    var value = prop.GetValue(data, null);
                    if (value != null)
                    {
                        str += HttpUtility.UrlEncode(value.ToString());
                    }
                }
            }
            return "?" + str.TrimStart('&');
        }

    }
}
