using System;

namespace ProductManagement.Framework.Foundation
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 文章列表项的时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ArticleListItemTimeFormart(this DateTime dateTime)
        {
            var passBy_H = (int)(DateTime.Now - dateTime).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                //超过24小时,显示日期
                return dateTime.ToString("yyyy年MM月dd日");
            }
            if (passBy_H == 0)
            {
                return dateTime.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }

        /**//// <summary>
        /// 获取随机时间
        /// <remarks>
        /// 由于Random 以当前系统时间做为种值,所以当快速运行多次该方法所得到的结果可能相同,
        /// 这时,你应该在外部初始化 Random 实例并调用 GetRandomTime(DateTime time1, DateTime time2, Random random)
        /// </remarks>
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static DateTime GetRandomTime(DateTime time1, DateTime time2)
        {
            Random random = new Random();
            return GetRandomTime(time1, time2, random);
        }

        /**//// <summary>
        /// 获取随机时间
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static DateTime GetRandomTime(DateTime time1, DateTime time2, Random random)
        {
            DateTime minTime = new DateTime();
            DateTime maxTime = new DateTime();

            System.TimeSpan ts = new System.TimeSpan(time1.Ticks - time2.Ticks);

            // 获取两个时间相隔的秒数
            double dTotalSecontds = ts.TotalSeconds;
            int iTotalSecontds = 0;

            if (dTotalSecontds > System.Int32.MaxValue)
            {
                iTotalSecontds = System.Int32.MaxValue;
            }
            else if (dTotalSecontds < System.Int32.MinValue)
            {
                iTotalSecontds = System.Int32.MinValue;
            }
            else
            {
                iTotalSecontds = (int)dTotalSecontds;
            }


            if (iTotalSecontds > 0)
            {
                minTime = time2;
                maxTime = time1;
            }
            else if (iTotalSecontds < 0)
            {
                minTime = time1;
                maxTime = time2;
            }
            else
            {
                return time1;
            }

            int maxValue = iTotalSecontds;

            if (iTotalSecontds <= System.Int32.MinValue)
                maxValue = System.Int32.MinValue + 1;

            int i = random.Next(System.Math.Abs(maxValue));

            return minTime.AddSeconds(i);
        }

        public static string ArticleListItemTimeFormart(this DateTime dateTime, string format = null)
        {
            var passBy_H = (int)(DateTime.Now - dateTime).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                if (!string.IsNullOrWhiteSpace(format))
                {
                    return dateTime.ToString(format);
                }
                //超过24小时,显示日期
                return dateTime.ToString("yyyy年MM月dd日");
            }
            if (passBy_H == 0)
            {
                return dateTime.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }
    }
}