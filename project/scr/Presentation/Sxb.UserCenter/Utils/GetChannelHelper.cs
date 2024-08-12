using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Utils
{
    public static class GetChannelHelper
    {
        /// <summary>
        /// 获取url中的fw 渠道标识
        /// </summary>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static string GetChannelByRequestReferer(string refererUrl)
        {
            if (refererUrl.IndexOf("?") > -1) 
            {
                string channel = "";
                string parameter = refererUrl.Split('?')[1];
                string[] paraArr = parameter.Split("&");
                for (int i = 0; i < paraArr.Length; i++)
                {
                    string[] temp = paraArr[i].Split("=");
                    if (temp[0] == "fw") 
                    {
                        return temp[1];
                    }
                }
                return channel;
            }
            else 
            {
                return "";
            }
        }
    }
}
