﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Xml;

namespace Sxb.PCWeb.Common
{
    public static class CookieDataHelper
    {
        private static readonly string latitude = "latitude";
        private static readonly string longitude = "longitude";
        private static readonly string provincecode = "provincecode";
        private static readonly string citycode = "citycode";
        private static readonly string areacode = "areacode";
        private static readonly string uuid = "uuid";
        private static readonly string localcity = "localcity";

        /// <summary>
        /// 获取精度
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetLatitude(this HttpRequest Request, string def = "31.63539")
        {
            return GetData(Request, latitude, def);
        }
        /// <summary>
        /// 获取维度
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetLongitude(this HttpRequest Request, string def = "121.3971")
        {
            return GetData(Request, longitude, def);
        }

        /// <summary>
        /// 获取省份code
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static int GetProvince(this HttpRequest Request, string def = "0")
        {
            var data = GetData(Request, provincecode, def);
            return Convert.ToInt32(data);
        }

        /// <summary>
        /// 获取城市code
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static int GetCity(this HttpRequest Request, string def = "0")
        {
            var data = GetData(Request, citycode, def);
            return Convert.ToInt32(data);
        }

        /// <summary>
        /// 换取用户城市
        /// 根据GetCity, GetLocalCity判断
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static int GetAvaliableCity(this HttpRequest Request, string def = "0")
        {
            var city = Request.GetLocalCity(def);
            if (city.ToString() != def)
            {
                return city;
            }
            return Request.GetCity(def);
        }

        /// <summary>
        /// 获取区域code
        /// </summary>
        /// <param name="request"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static int GetArea(this HttpRequest request, string def = "0")
        {
            var data = GetData(request, areacode, def);
            return Convert.ToInt32(data);
        }

        /// <summary>
        /// 获取设备id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetDevice(this HttpRequest
             request, string def = "")
        {
            return GetData(request, uuid, def);
        }

        /// <summary>
        /// 设置省的值
        /// </summary>
        /// <param name="response"></param>
        /// <param name="value"></param>
        public static void SetProvince(this HttpResponse response, string value)
        {
            response.Cookies.Append(provincecode, value, new CookieOptions { Expires = DateTime.Now.AddDays(7) });
        }
        /// <summary>
        /// 设置城市的值
        /// </summary>
        /// <param name="response"></param>
        /// <param name="value"></param>
        public static void SetCity(this HttpResponse response, string value)
        {
            response.Cookies.Append(citycode, value, new CookieOptions { Expires = DateTime.Now.AddDays(7) });
        }

        /// <summary>
        /// 设置城市的值
        /// </summary>
        /// <param name="response"></param>
        /// <param name="value"></param>
        public static void SetArea(this HttpResponse response, string value)
        {
            response.Cookies.Append(areacode, value, new CookieOptions { Expires = DateTime.Now.AddDays(7) });
        }


        /// <summary>
        /// 获取当前城市code
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static int GetLocalCity(this HttpRequest Request, string def = "440100")
        {
            var data = GetData(Request, localcity, def);
            return Convert.ToInt32(data);
        }


        /// <summary>
        /// 设置城市的值
        /// </summary>
        /// <param name="response"></param>
        /// <param name="value"></param>
        public static void SetLocalCity(this HttpResponse response, string value, string domain = default)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(1)
            };
            if (!string.IsNullOrWhiteSpace(domain))
            {
                var split_Domain = domain.Split('.');
                if (split_Domain.Length > 1)
                {
                    cookieOptions.Domain = $"{split_Domain[split_Domain.Length - 2]}.{split_Domain.Last()}";
                }
            }
            response.Cookies.Append(localcity, value, cookieOptions);
        }


        private static string GetData(HttpRequest httpRequest, string name, string def)
        {
            return string.IsNullOrEmpty(httpRequest.Cookies[name]) ? def : httpRequest.Cookies[name];
        }

        /// <summary>
        /// 获取页面中所有的cookie值
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllCookies(this HttpRequest httpRequest)
        {
            Dictionary<string, string> ky = new Dictionary<string, string>();
            foreach (var item in httpRequest.Cookies)
            {
                ky.Add(item.Key, item.Value);
            }
            return ky;
        }
    }
}
