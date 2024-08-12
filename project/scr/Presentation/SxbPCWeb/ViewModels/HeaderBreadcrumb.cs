using System;
namespace Sxb.PCWeb.ViewModels
{
    /// <summary>
    /// 头部面包屑
    /// </summary>
    public class HeaderBreadcrumb
    {
        public HeaderBreadcrumb(string name, string url)
        {
            Head = this;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        /// <summary>
        /// 是否小边框
        /// </summary>
        public bool IsSmallBorder { get; set; }

        /// <summary>
        /// 面包屑名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 面包屑路由
        /// </summary>
        public string Url { get; set; }

        public HeaderBreadcrumb Head { get; private set; }
        public HeaderBreadcrumb Child { get; set; }

        /// <summary>
        /// 设置子面包屑， 并返回子面包屑的实例
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public HeaderBreadcrumb SetChild(string name, string url)
        {
            Child = new HeaderBreadcrumb(name, url)
            {
                Head = Head
            };
            return Child;
        }

        public static HeaderBreadcrumb Instance
        {
            get
            {
                return new HeaderBreadcrumb("首页", "/");
            }
        }
    }
}
