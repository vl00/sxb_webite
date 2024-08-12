using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.Foundation
{
    public static class TimeUtil
    {

        /// <summary>
        /// UNIX 最小时间
        /// </summary>
        /// <returns>C#格式时间</returns>
        public static DateTime UnixMiniTime()
        {
            return new DateTime(1970, 01, 01).ToLocalTime();
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime I2D(this string timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1).ToLocalTime();
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime I2D(this long timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1).ToLocalTime();
            return dtStart.AddMilliseconds(timeStamp);
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static long D2I(this DateTime time)
        {
            DateTime startTime = new DateTime(1970, 1, 1).ToLocalTime();
            return (long)(time - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳(10位)格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int D2ISecond(this DateTime time)
        {
            DateTime startTime = new DateTime(1970, 1, 1).ToLocalTime();
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        ///  DateTime时间格式转成短时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToInt64(DateTime time)
        {
            return (long)(time.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// 时间转换为简明时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static string ConciseTime(this long timeStamp)
        {
            return ConciseTime(I2D(timeStamp));
        }

        /// <summary>
        /// 根据生日计算年龄
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public static int CalculateAgeCorrect(this DateTime birthDate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) age--;
            return age;
        }

        /// <summary>
        /// 使用C#把发表的时间改为几个月,几天前,几小时前,几分钟前,或几秒前
        /// 2008年03月15日 星期六 02:35
        ///  ref： http://www.cnblogs.com/summers/p/3225716.html
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConciseTime(this DateTime datetime)
        {
            string format = "yyyy年MM月dd日 HH:mm";
            return datetime.ConciseTime(format);
        }

        public static string ConciseTime(this DateTime datetime, string format)
        {
            if (datetime.Date == DateTime.Now.Date)
            {
                TimeSpan span = DateTime.Now - datetime;

                if (span.TotalHours > 1)
                {
                    //return $"{datetime:HH:mm}";
                    return $"{(int)span.TotalHours}小时前";
                }
                else
                {
                    if (span.TotalMinutes <= 1)
                        return "刚刚";
                    else
                        return $"{(int)span.TotalMinutes}分钟前";
                }
            }
            else
            {
                return datetime.ToString(format);
                //double Time = (DateTime.Now.Date - datetime.Date).TotalDays;
                //if (Time == 1)
                //{
                //    //return $"昨天{datetime:HH:mm}";
                //    return $"昨天";
                //}
                //else if (Time > 1)
                //{
                //    return "几天前";
                //}
                //else if (Time > 7)
                //{
                //    return "一周前";
                //}
                //else
                //{
                //    return "一个月前";
                //}
            }
        }

    }
}