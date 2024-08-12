using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductManagement.Framework.Foundation
{
    public class CommonHelper
    {
        public static string GetQueryValue(string QueryString, string QueryKey)
        {
            if (string.IsNullOrEmpty(QueryString))
            {
                return string.Empty;
            }
            int startIndex = QueryString.Substring(0, 1) == "?" ? 1 : 0;
            foreach (string Query in QueryString.Substring(startIndex).Split(new char[] { '&', '#' }))
            {
                if (Query.Split('=')[0] == QueryKey)
                {
                    return Query.Split('=')[1];
                }
            }
            return string.Empty;
        }
        public static bool IsJsonp(string str, out string jsonContent)
        {
            Regex reg = new Regex(@"^\w+\(\s*(\{.*?\})\s*\);*$");
            if (reg.IsMatch(str))
            {
                var groups = reg.Match(str).Groups;
                jsonContent = groups[groups.Count - 1].Value;
                return true;
            }
            jsonContent = null;
            return false;
        }
        public static bool IsJson(string str)
        {
            Regex reg = new Regex(@"^\{.*?\}$");
            return reg.IsMatch(str);
        }
        public static bool isMobile(string InText)
        {
            Regex reg = new Regex(@"^\d{5,11}$");
            return reg.IsMatch(InText) && (InText != "13800138000");
        }
        private const double EARTH_RADIUS = 6378137;//地球半径
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        public static string FormatDistance(double meter)
        {
            meter = Math.Round(meter, 1);
            if (meter == 0)
            {
                return "";
            }
            else if (meter < 1000)
            {
                return meter.ToString() + "米";
            }
            else
            {
                return Math.Round(meter / 100) / 10 + "千米";
            }
        }
        public static string GetDescriptionFromEnumValue(Enum value)
        {
            DescriptionAttribute attribute = value.GetType()?
                .GetField(value.ToString())?
                .GetCustomAttributes(typeof(DescriptionAttribute), false)?
                .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
        public static string HideNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return null;
            }
            else
            {
                int s = number.Length * 3 / 11;
                int e = number.Length * 7 / 11;
                int x = number.Length - s;
                return number.Substring(0, s) + number.Substring(e).PadLeft(x, '*');
            }
        }
        public static string GetTimeString(DateTime time)
        {
            DateTime now = DateTime.Now;
            int totalSecond = (int)(now - time).TotalSeconds;
            if (totalSecond < 60)
            {
                return totalSecond.ToString() + "秒前";
            }
            else if (totalSecond < 3600)
            {
                return Math.Floor((double)totalSecond / 60).ToString() + "分钟前";
            }
            else if (now.ToString("yyyyMMdd") == time.ToString("yyyyMMdd"))
            {
                return "今天 " + time.ToString("HH:mm");
            }
            else if (now.AddDays(-1).ToString("yyyyMMdd") == time.ToString("yyyyMMdd"))
            {
                return "昨天 " + time.ToString("HH:mm");
            }
            else
            {
                return time.ToString("M-d HH:mm");
            }
        }

        /// <summary>
        /// 打乱列表数序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources"></param>
        public static void ListRandom<T>(List<T> sources)
        {
            Random rd = new Random();
            int index = 0;
            T temp;
            for (int i = 0; i < sources.Count; i++)
            {
                index = rd.Next(0, sources.Count - 1);
                if (index != i)
                {
                    temp = sources[i];
                    sources[i] = sources[index];
                    sources[index] = temp;
                }
            }
        }

        public static IEnumerable<T> ListRandom<T>(IEnumerable<T> sources)
        {
            Random rd = new Random();
            int index = 0;
            T temp;
            var result = sources.ToList();
            for (int i = 0; i < result.Count; i++)
            {
                index = rd.Next(0, result.Count - 1);
                if (index != i)
                {
                    temp = result[i];
                    result[i] = result[index];
                    result[index] = temp;
                }
            }
            return result;
        }

        public static List<T> ListRandom<T>(IEnumerable<T> sources, IEnumerable<T> excludes)
        {
            var result = new List<T>();
            excludes = excludes ?? Enumerable.Empty<T>();
            foreach (var item in sources)
            {
                if (excludes.Contains(item))
                {
                    continue;
                }
                result.Add(item);
            }

            ListRandom(result);
            return result;
        }

        public static T TryJsonDeserializeObject<T>(string sources, T def = default)
        {
            if (!string.IsNullOrWhiteSpace(sources))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(sources);
                }
                catch (Exception)
                { }
            }
            return def;
        }

        /// <summary>
        /// mapper
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination MapperProperty<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            if (source == null)
                return default;

            //create instance
            TDestination destination = Activator.CreateInstance<TDestination>();

            //type
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);

            //properties
            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            //mapper
            foreach (var destinationProp in destinationProperties)
            {
                if (!destinationProp.CanWrite)
                    continue;

                var sourceProp = sourceProperties.FirstOrDefault(s => s.Name == destinationProp.Name && s.PropertyType == destinationProp.PropertyType);
                if (sourceProp != null && sourceProp.CanRead)
                {
                    destinationProp.SetValue(destination, sourceProp.GetValue(source));
                }
            }
            return destination;
        }

        public static IEnumerable<TDestination> MapperProperty<TSource, TDestination>(IEnumerable<TSource> sources)
            where TSource : class
            where TDestination : class
        {
            foreach (var item in sources)
            {
                yield return MapperProperty<TSource, TDestination>(item);
            }
        }

        /// <summary>
        /// obj set default string when null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="def"></param>
        public static void PropertyStringDef<T>(T t, string def = "", params string[] keys) where T : class
        {
            if (t == null)
                return;

            //type
            Type type = typeof(T);
            Type stringType = typeof(string);

            //properties
            var properties = type.GetProperties();

            //mapper
            foreach (var prop in properties)
            {
                if (!prop.CanWrite || !prop.CanRead)
                    continue;

                if (prop.PropertyType != stringType)
                    continue;

                var value = prop.GetValue(t);
                //null set def
                if (value == null)
                {
                    prop.SetValue(t, def);
                    continue;
                }

                var stringValue = Convert.ToString(value);
                //empty set def
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    prop.SetValue(t, def);
                    continue;
                }

                //contain key set def
                if (keys != null && keys.Any() && keys.Contains(stringValue))
                {
                    prop.SetValue(t, def);
                }
            }
        }


        public static Dictionary<string, string> ToDictionary<T>(T t) where T : class
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            if (t == null)
                return keyValuePairs;

            //type
            Type type = typeof(T);
            Type stringType = typeof(string);

            //properties
            var properties = type.GetProperties();

            //mapper
            foreach (var prop in properties)
            {
                if (!prop.CanWrite || !prop.CanRead)
                    continue;

                if (prop.PropertyType != stringType)
                    continue;

                var value = prop.GetValue(t);
                //null set def
                if (value == null)
                    continue;

                var stringValue = Convert.ToString(value);
                keyValuePairs.Add(prop.Name, stringValue);
            }

            return keyValuePairs;
        }
    }
}
