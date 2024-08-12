using System.Text;
using System.Text.RegularExpressions;

namespace iSchool
{
    public static partial class StringHelper
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
        }

        public static string FormatWith(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        //eg: left join MaintenanceTemplates it on it.Id = m.MaintenanceTemplateId
        //    where m.IsDeleted = 0
        //    {" and m.Code = @KeyWord ".If(!string.IsNullOrWhiteSpace(input.KeyWord))}
        //    {" and m.ProjectId = @ProjectId ".If(input.ProjectId.HasValue)}
        //    {" and a.ProductId = @ProductId ".If(input.ProductId.HasValue)}
        public static string If(this string str, bool condition)
        {
            return condition ? str : string.Empty;
        }

        /// <summary>
        /// 忽略大小写对比字符串
        /// </summary>
        public static bool EqualsIgnCase(this string str0, string str1) => string.Equals(str0, str1, System.StringComparison.OrdinalIgnoreCase);
    }
}