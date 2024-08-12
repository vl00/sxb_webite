using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ProductManagement.Framework.Foundation
{
    public static class EnumExtension
    {
        public static string Description(this Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }

        public static T GetEnum<T>(this string enumc) where T : struct
        {
            T val;

            Enum.TryParse<T>(enumc, out val);

            return val;
        }
        public static string GetName<T>(object obj) where T : struct
        {
            return Enum.GetName(typeof(T), obj);
        }

        /// <summary>
        /// 获取枚举描述性数据
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetDescription(this System.Enum @enum)
        {
            Type type = @enum.GetType();
            FieldInfo field = type.GetField(@enum.ToString());
            if (field == null)
                return default;

            var attr = field.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
            return attr?.Description;
        }

        /// <summary>
        /// 获取枚举携带的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static T GetDefaultValue<T>(this System.Enum @enum)
        {
            Type type = @enum.GetType();
            FieldInfo field = type.GetField(@enum.ToString());
            if (field == null)
                return default(T);

            var attrs = field.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>();

            if (attrs == null)
                return default(T);
            else
                return attrs.Value == null ? default(T) : (T)attrs.Value;
        }

        /// <summary>
        /// 获取枚举携带的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetActionUrlValue(this System.Enum @enum)
        {
            return @enum.GetAttributeValue<ActionUrlAttribute>();
        }

        /// <summary>
        /// 获取枚举携带的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetDefaultImageKeyValue(this System.Enum @enum)
        {
            return @enum.GetAttributeValue<DefaultImageKeyAttribute>();
        }

        /// <summary>
        /// 获取枚举携带的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetAttributeValue<T>(this System.Enum @enum) where T : BaseValueAttribute
        {
            Type type = @enum.GetType();
            FieldInfo field = type.GetField(@enum.ToString());
            if (field == null)
                return default;

            var attrs = field.GetCustomAttribute<T>();

            if (attrs == null)
                return default;
            else
                return attrs.Value ?? default;
        }

        public static IEnumerable<(string name, int value)> GetAllItems(this Enum @enum)
        {
            var enumType = @enum.GetType();
            foreach (object item in Enum.GetValues(enumType))
            {
                var name = item.ToString();// Enum.GetName(enumType, item);
                yield return (name, (int)item);
            }
        }

        public static IEnumerable<(string desc, int value)> GetAllItemsDesc(this Enum @enum)
        {
            var enumType = @enum.GetType();
            foreach (object item in Enum.GetValues(enumType))
            {
                var desc = ((Enum)item).GetDescription();
                yield return (desc, (int)item);
            }
        }
    }
}
