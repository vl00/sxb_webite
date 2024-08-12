namespace ProductManagement.Framework.Foundation
{
    public static class DecimalHelper
    {
        /// <summary>
        /// 保留N位小数且不进行四舍五入操作
        /// </summary>
        /// <param name="n">位数</param>
        /// <returns></returns>
        public static decimal CutDecimalWithN(this decimal d, int n)
        {
            string strDecimal = d.ToString();
            int index = strDecimal.IndexOf(".");
            if (index == -1 || strDecimal.Length < index + n + 1)
            {
                strDecimal = string.Format("{0:F" + n + "}", d);
            }
            else
            {
                int length = index;
                if (n != 0)
                {
                    length = index + n + 1;
                }
                strDecimal = strDecimal.Substring(0, length);
            }
            return decimal.Parse(strDecimal);
        }

        public static double CutDoubleWithN(this double d, int n)
        {
            string strDecimal = d.ToString();
            int index = strDecimal.IndexOf(".");
            if (index == -1 || strDecimal.Length < index + n + 1)
            {
                strDecimal = string.Format("{0:F" + n + "}", d);
            }
            else
            {
                int length = index;
                if (n != 0)
                {
                    length = index + n + 1;
                }
                strDecimal = strDecimal.Substring(0, length);
            }
            return double.Parse(strDecimal);
        }
    }
}
