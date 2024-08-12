using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Sxb.Web.Common
{
    public static class Extensions
    {
        public static string GetDescription<T>(this T value)
        {
            var type = typeof(T);
            var memberInfo = type.GetMember(value.ToString()).FirstOrDefault();
            var descriptionAttribute =
                memberInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute == null)
                return string.Empty;
            return descriptionAttribute.Description;
        }
    }
}
