using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    public static class IEnumerableExtension
    {

        /// <summary>
        /// 对照指定有序列表排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources"></param>
        /// <param name="sorts"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static IEnumerable<T> SortTo<TKey, T>(this IEnumerable<T> sources, IEnumerable<TKey> sorts, string propName = "Id")
            where T : class
            where TKey : IEquatable<TKey>
        {
            var type = typeof(T);
            var prop = type.GetProperty(propName);
            if (prop == null)
            {
                throw new KeyNotFoundException($"没有此属性:{propName}");
            }

            if (!prop.CanRead)
            {
                throw new AccessViolationException($"属性:{propName}不可读");
            }

            foreach (var item in sorts)
            {
                var source = sources.FirstOrDefault(s => prop.GetValue(s)?.ToString() == item.ToString());
                if (source != null)
                {
                    yield return source;
                }
            }

        }


        public static (IEnumerable<T> data, bool succeed) TrySortTo<TKey, T>(this IEnumerable<T> sources, IEnumerable<TKey> sorts, string propName = "Id")
            where T : class
            where TKey : IEquatable<TKey>
        {

            try
            {
                return (sources.SortTo(sorts, propName), true);
            }
            catch (Exception)
            {
                return (Enumerable.Empty<T>(), false);
            }
        }
    }
}
