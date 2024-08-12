using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.Ioc
{
    public class IocServiceContainer
    {
        //注入容器
        public static IIocService Current;

        /// <summary>
        /// 初始化容器
        /// </summary>
        public static void InitUnityContainer()
        {
            Current = new IocService();
        }
    }
}
