using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    /// <summary>
    /// 简单唯一串
    /// </summary>
    public class ShakeNoHelper
    {
        public static string Template = "{0}{1}";

        public static long timestamp = 0;
        public static ushort serial = 0;
        public static ushort perSecondMax = 999;

        
        /// <summary>
        /// 一小时内不重复的串
        /// </summary>
        /// <returns></returns>
        public static string Shake()
        {
            DateTime now = DateTime.Now;

            var seconds = now.Minute * 60 + now.Second;
            var rand = VerifyCodeHelper.RndomStr(VerifyCodeHelper.VerifyCodeType.FullWordAndNumber, 6);

            long _timestamp = now.D2ISecond();
            if (timestamp == _timestamp)
            {
                serial++;
                //超过每秒最大值
                if (serial > perSecondMax)
                {
                    throw new ArgumentOutOfRangeException(nameof(serial));
                }
            }
            else
            {
                serial = 1;
                timestamp = _timestamp;
            }

            //9 - 3599 - 001
            //var body = int.Parse("9" + seconds + serial);
            var body = 9 * 10000 * 1000 + seconds * 1000 + serial;
            var bodyStr = DigitBaseConvert.Dec2AZ(body);

            //body - 1ab2
            var code = string.Format(Template, bodyStr, rand);
            return code;
        }

    }
}
