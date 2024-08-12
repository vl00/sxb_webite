using PMS.Search.Domain.Entities;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PMS.Search.Domain.Common
{
    public class HighlightHelper
    {
        /// <summary>
        /// 根据ES返回的高亮字段, 返回高亮词
        /// </summary>
        /// <param name="high"></param>
        /// <param name="name"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetHighLightValue(IReadOnlyDictionary<string, IReadOnlyCollection<string>> high, string name, string def = "")
        {
            string esName = name.ToFirstLower();
            string esNamePinyin = $"{esName}.pinyin";
            var namehigh = high.Where(s => s.Key == esName || s.Key == esNamePinyin).FirstOrDefault().Value;
            var value = namehigh?.FirstOrDefault();
            return value ?? def;
        }

        /// <summary>
        /// 根据搜索keyword, 设置结果中的高亮字
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="name"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetHighLightValue(string keyword, string name, string def = "")
        {
            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(name))
            {
                return def;
            }

            string high = string.Empty;
            List<char> chars = new List<char>();
            foreach (var n in name)
            {
                if (keyword.Any(s => s == n))
                {
                    chars.Add('<');
                    chars.Add('e');
                    chars.Add('m');
                    chars.Add('>');
                    chars.Add(n);
                    chars.Add('<');
                    chars.Add('/');
                    chars.Add('e');
                    chars.Add('m');
                    chars.Add('>');
                }
                else
                {
                    chars.Add(n);
                }
            }
            return new string(chars.ToArray());
        }

        public static List<SearchSuggestItem> GetSimpleSuggestItems(List<SearchSuggestOriginItem> source, List<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> highs, string name)
        {
            //Type type = typeof(SearchSuggestOriginItem);
            //var prop = type.GetProperty(name);
            //if (prop == null)
            //{
            //    throw new ArgumentNullException(nameof(name));
            //}
            //return source.Select((q, position) => {
            //    var high = highs.ElementAtOrDefault(position);
            //    var propValue = prop.GetValue(q);
            //    var propStrValue = propValue == null ? string.Empty : propValue.ToString();
            //    return new SearchSuggestItem
            //    {
            //        Name = GetHighLightValue(high, name, propStrValue),
            //        SourceId = q.Id
            //    };
            //}).ToList();

            return GetSimpleSuggestItems(source
                , highs
                , name
                , (originItem, highlightValue) => new SearchSuggestItem
                {
                    Name = highlightValue,
                    SourceId = originItem.Id
                });
        }

        public static List<SearchSuggestItem> GetSimpleSuggestItems(List<SearchSuggestOriginItem> source, string keyword, string name)
        {
            Type type = typeof(SearchSuggestOriginItem);
            var prop = type.GetProperty(name);
            if (prop == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            return source.Select((q, position) =>
            {
                var propValue = prop.GetValue(q);
                var propStrValue = propValue == null ? string.Empty : propValue.ToString();
                return new SearchSuggestItem
                {
                    Name = GetHighLightValue(keyword, propStrValue, propStrValue),
                    SourceId = q.Id
                };
            }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">原始结果集</param>
        /// <param name="highs">高亮结果集</param>
        /// <param name="name">被高亮的字段</param>
        /// <param name="createItemFun"></param>
        /// <returns></returns>
        public static List<T> GetSimpleSuggestItems<T>(List<SearchSuggestOriginItem> source,
            List<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> highs,
            string name,
            Func<SearchSuggestOriginItem, string, T> createItemFun)
        {
            Type type = typeof(SearchSuggestOriginItem);
            var prop = type.GetProperty(name);
            if (prop == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            return source.Select((q, position) =>
            {
                var high = highs.ElementAtOrDefault(position);
                var propValue = prop.GetValue(q);
                var propStrValue = propValue == null ? string.Empty : propValue.ToString();
                var highlightValue = GetHighLightValue(high, name, propStrValue);
                return createItemFun(q, highlightValue);
            }).ToList();
        }

        /// <summary>
        /// <para>added by Labbor on 20201130 设置高亮字段到原数据</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="highs"></param>
        /// <param name="originPropName"></param>
        /// <param name="highlightPropName"></param>
        public static void SetHighlights<T>(ref List<T> source, List<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> highs, string originPropName, string highlightPropName = "")
        {
            Type type = typeof(T);
            var originProp = type.GetProperty(originPropName);
            if (originProp == null)
            {
                throw new ArgumentNullException(nameof(originPropName));
            }

            PropertyInfo highlightProp = null;
            if (string.IsNullOrWhiteSpace(highlightPropName))
            {
                highlightProp = originProp;
            }
            else
            {
                highlightProp = type.GetProperty(highlightPropName);
                if (originProp == null)
                {
                    throw new ArgumentNullException(nameof(highlightPropName));
                }
            }

            var length = source.Count;
            for (int position = 0; position < length; position++)
            {
                var item = source[position];

                var high = highs.ElementAtOrDefault(position);
                var originPropValue = originProp.GetValue(item);
                var originPropStrValue = originPropValue == null ? string.Empty : originPropValue.ToString();

                //把高亮数据赋值到原数据
                var highValue = GetHighLightValue(high, originPropName, def: originPropStrValue);
                highlightProp.SetValue(item, highValue);
            }
        }

        /// <summary>
        /// <para>added by Labbor on 20201130 设置高亮字段到原数据</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="highs"></param>
        /// <param name="name"></param>
        public static void SetHighlights<T>(ref List<T> source, string keyword, string name)
        {
            if (source == null || !source.Any())
            {
                return;
            }

            //var type = typeof(T);
            var type = source.First().GetType();//解决转为object拿不到
            var prop = type.GetProperty(name);
            if (prop == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var length = source.Count;
            for (int position = 0; position < length; position++)
            {
                var item = source[position];

                var propValue = prop.GetValue(item);
                var propStrValue = propValue == null ? string.Empty : propValue.ToString();

                //把高亮数据赋值到原数据
                var highValue = GetHighLightValue(keyword, propStrValue, def: propStrValue);
                prop.SetValue(item, highValue);
            }
        }
    }
}
