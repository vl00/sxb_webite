using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Common
{
    public class RandomHelper
    {
        /// <summary>
        /// 随机邀请码（数字和字母随机数）
        /// </summary>
        /// <param name="n">生成长度</param>
        /// <returns></returns>
        public static string RandInviteCode(int n, Random random)
        {
            char[] arrChar = new char[]{
                'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
                '0','1','2','3','4','5','6','7','8','9',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
            };
            char[] numChar = new char[]{
                '0','1','2','3','4','5','6','7','8','9'
            };
            StringBuilder codeSB = new StringBuilder();

            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    codeSB.Append(numChar[random.Next(0, numChar.Length)].ToString());
                }
                else
                {
                    codeSB.Append(arrChar[random.Next(0, arrChar.Length)].ToString());
                }
            }
            return codeSB.ToString();
        }

        /// <summary>
        /// 获取用验证码(随机数字)
        /// </summary>
        /// <param name="n">生成长度</param>
        /// <returns></returns>
        public static string RandVerificationCode(int n, Random random)
        {
            char[] numChar = new char[]{
                '0','1','2','3','4','5','6','7','8','9'
            };
            StringBuilder codeSB = new StringBuilder();

            for (int i = 0; i < n; i++)
            {
                codeSB.Append(numChar[random.Next(0, numChar.Length)].ToString());
            }
            return codeSB.ToString();
        }
    }
}
