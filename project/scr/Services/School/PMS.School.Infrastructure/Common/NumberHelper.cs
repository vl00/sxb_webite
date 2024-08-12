using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Infrastructure.Common
{
    public static class NumberHelper
    {
        /// <summary>
        /// 将小数点转换成分数
        /// </summary>
        /// <param name="fNum"></param>
        /// <returns></returns>
        public static (double, double) ConvertToFraction(int num1, int num2)
        {

            if (num1 == 0)
            {
                return (0, 0);
            }

            //var result = CommonDivisor(num1, num2);
            //return (num1 / result, num2 / result);
            var next = (int)(Math.Round(((decimal)num2 / num1), 1));
            return (1, next);
        }

        public static int CommonDivisor(int num1, int num2)
        {
            int tmp;
            if (num1 < num2)
            {
                tmp = num1; num1 = num2; num2 = tmp;
            }
            int a = num1; int b = num2;
            while (b != 0)
            {
                tmp = a % b;
                a = b;
                b = tmp;
            }
            return a;
        }


        public static int getGongYueShu(int a, int b)
        {
            //求两个数的最大公约数
            int t = 0;
            if (a < b)
            {
                t = a;
                a = b;
                b = t;
            }
            int c = a % b;
            if (c == 0)
            {
                return b;
            }
            else
            {
                return getGongYueShu(b, c);
            }
        }



        /// <summary>
        /// 将分子 分母 转换成小数点
        /// </summary>
        /// <param name="iNum1"></param>
        /// <param name="iNum2"></param>
        /// <returns></returns>
        public static int MaxCommonDivisor(int iNum1, int iNum2)
        {
            int a = iNum1;
            int b = iNum2;
            int temp;

            while (b != 0)/*利用辗除法，直到b为0为止*/
            {
                temp = a % b;
                a = b;
                b = temp;
            }

            return a;
        }
    }
}
