using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductManagement.Framework.Foundation
{
    public class UserCenterCommonHelper
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
            DescriptionAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
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
    }
}
