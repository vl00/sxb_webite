using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.Foundation
{

    public static class DigitBaseConvert
    {
        /// <summary>
        /// GUID 36xRadix
        /// </summary>
        public static readonly char[] BASE36_CHARS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        /// <summary>
        /// Sence
        /// </summary>
        public static readonly char[] BASE_CHARS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public static int ReverseGetIndex(char[] chars, char c)
        {
            int len = chars.Length;

            for (int i = len - 1; i >= 0; i--)
            {
                if (chars[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetBaseIndex(char[] baseChars, char c)
        {
            //97 = a
            int index = 0, asc = c;
            // 0-9
            if (asc >= 48 && asc <= 57)
            {
                index = asc - 48;
            }
            //a-z
            else if (asc >= 97 && asc <= 122)
            {
                index = asc - 87;
            }
            //A-Z
            else if (asc >= 65 && asc <= 90)
            {
                index = asc - 29;
            }
            else
            {
                index = ReverseGetIndex(baseChars, c);
            }
            return index;
        }

        public static string Dec2AZ(long decim)
        { 
            return DecimalToBase(BASE_CHARS, decim);
        }

        public static long AZ2Dec(string az)
        {
            return BaseToDecimal(BASE_CHARS, az);
        }

        public static string DecimalToBase(char[] baseChars, long decim)
        {
            int digit = baseChars.Length;
            List<char> target = new List<char>();

            //转为指定进制
            while (true)
            {
                var value = decim % digit;
                decim = (decim - value) / digit;

                var c = baseChars[value];
                target.Add(c);

                if (decim <= 0) break;
            }

            target.Reverse();
            return new string(target.ToArray());
        }

        public static long BaseToDecimal(char[] baseChars, string str)
        {
            if (str == null)
            {
                return 0;
            }

            //转为十进制
            long decim = 0;
            int digit = baseChars.Length;
            int power = str.Length - 1;
            for (int i = 0; i < str.Length; i++)
            {
                var index = GetBaseIndex(baseChars, str[i]);
                decim += index * (int)Math.Pow(digit, power - i);
            }
            return decim;
        }

    }
}
