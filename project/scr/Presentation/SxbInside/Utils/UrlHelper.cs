using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelper
    {
        //key分隔符
        private static readonly string _keySeparator = "_";
        //value分隔符
        private static readonly string _valueSeparator = "-";

        public static string ShortAction(this IUrlHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }
            //var context = helper.ActionContext.RouteData
            //context.ActionDescriptor
            return helper.ShortAction(
                action: null,
                controller: null,
                values: null,
                protocol: null,
                host: null,
                fragment: null);
        }

        public static string ShortAction(this IUrlHelper helper, string action, string controller, object values)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }
            return helper.ShortAction(
                action: action,
                controller: controller,
                values: values,
                protocol: null,
                host: null,
                fragment: null);
        }

        public static string ShortAction(this IUrlHelper helper, string action, object values)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }
            return helper.ShortAction(
                action: action,
                controller: null,
                values: values,
                protocol: null,
                host: null,
                fragment: null);
        }

        public static string ShortAction(this IUrlHelper helper, string action, string controller)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }
            return helper.ShortAction(action: action,
                controller: controller,
                values: null,
                protocol: null,
                host: null,
                fragment: null);
        }

        public static string ShortAction(this IUrlHelper helper, string action, string controller,
    object values, string protocol, string host, string fragment)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            var urlActionContext = new UrlActionContext()
            {
                Action = action,
                Controller = controller,
                Host = host,
                Values = values,
                Protocol = protocol,
                Fragment = fragment
            };

            var actionContext = helper.ActionContext;
            //获取参数集合
            var valuesDictionary = GetValuesDictionary(urlActionContext.Values);
            NormalizeRouteValuesForAction(urlActionContext.Action, urlActionContext.Controller, valuesDictionary, actionContext.RouteData.Values);
            return GenerateUrl(valuesDictionary);
        }

        /// <summary>
        /// 对请求url进行修改
        /// </summary>
        /// <param name="valuesDictionary"></param>
        /// <returns></returns>
        public static string GenerateUrl(RouteValueDictionary valuesDictionary)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("/");
            builder.Append(valuesDictionary["controller"]);
            valuesDictionary.Remove("controller");
            builder.Append("/");
            builder.Append(valuesDictionary["action"]);
            valuesDictionary.Remove("action");

            if (valuesDictionary.Keys.Count > 0)
            {
                var keys = valuesDictionary.Keys;
                foreach (var key in keys)
                {
                    var value = valuesDictionary[key];

                    if (value == null) continue;
                    if (value is Guid)
                    {
                        builder.Append(_keySeparator);
                        builder.Append(key);
                        builder.Append(_valueSeparator);
                        //guid进行压缩
                        builder.Append(((Guid)value).ToString("N"));
                    }
                    else if (value is Guid?)
                    {
                        builder.Append(_keySeparator);
                        builder.Append(key);
                        builder.Append(_valueSeparator);
                        //guid进行压缩
                        builder.Append(((Guid?)value).Value.ToString("N"));
                    }
                    else if (IsList(value) || value is Array)
                    {
                        builder.Append(GetListLink(key, value as IList));
                    }
                    else
                    {
                        if (value is string)
                        {
                            if (string.IsNullOrEmpty((string)value)) continue;
                        }
                        builder.Append(_keySeparator);
                        builder.Append(key);
                        builder.Append(_valueSeparator);
                        builder.Append(value);
                    }
                    //else
                    //{
                    //    throw new Exception($"暂不支持生成{value.GetType().Name}的url");
                    //}
                }
                builder.Append("~");
            }

            return builder.ToString();

        }


        /// <summary>
        ///判断一个类型是list
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }



        /// <summary>
        /// 生成list的link
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetListLink<T>(string key, T value) where T : IList
        {
            var link = new StringBuilder();

            foreach (var item in value)
            {
                if (item == null)
                    continue;
                else if (item is string)
                {
                    if (string.IsNullOrEmpty((string)item))
                        continue;
                }

                link.Append(_keySeparator);
                link.Append(key);
                link.Append(_valueSeparator);
                if (item is Guid)
                    link.Append(((Guid)item).ToString("N"));
                else if (item is Guid?)
                    link.Append(((Guid?)item).Value.ToString("N"));
                else
                    link.Append(item.ToString());
            }
            return link.ToString();
        }

        internal static void NormalizeRouteValuesForAction(
    string action,
    string controller,
    RouteValueDictionary values,
    RouteValueDictionary ambientValues)
        {
            object obj = null;
            if (action == null)
            {
                if (!values.ContainsKey("action") &&
                    (ambientValues?.TryGetValue("action", out obj) ?? false))
                {
                    values["action"] = obj;
                }
            }
            else
            {
                values["action"] = action;
            }

            if (controller == null)
            {
                if (!values.ContainsKey("controller") &&
                    (ambientValues?.TryGetValue("controller", out obj) ?? false))
                {
                    values["controller"] = obj;
                }
            }
            else
            {
                values["controller"] = controller;
            }
        }

        public static RouteValueDictionary GetValuesDictionary(object values)
        {
            var _routeValueDictionary = new RouteValueDictionary();
            // Perf: RouteValueDictionary can be cast to IDictionary<string, object>, but it is
            // special cased to avoid allocating boxed Enumerator.
            if (values is RouteValueDictionary routeValuesDictionary)
            {
                foreach (var kvp in routeValuesDictionary)
                {
                    _routeValueDictionary.Add(kvp.Key, kvp.Value);
                }

                return _routeValueDictionary;
            }

            if (values is IDictionary<string, object> dictionaryValues)
            {
                foreach (var kvp in dictionaryValues)
                {
                    _routeValueDictionary.Add(kvp.Key, kvp.Value);
                }

                return _routeValueDictionary;
            }
            return new RouteValueDictionary(values);
        }



    }
}
