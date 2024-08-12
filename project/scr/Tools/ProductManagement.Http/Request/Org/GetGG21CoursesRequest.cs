using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Request.Org
{
    public class GetGG21CoursesRequest
    {
        /// <summary>
        /// 年龄
        /// </summary>
        public IEnumerable<(int minAge, int maxAge)> ages { get; set; }
        /// <summary>
        /// 学科，多个学科使用英文【,】分割
        /// </summary>
        public string subjs { get; set; }

        /// <summary>
        /// 价格
        /// 0 -> 全部
        /// 1 -> 100元以上
        /// 2 -> 200元以下
        /// </summary>
        public int price { get; set; }

        /// <summary>
        /// 是否需要返回小程序二维码
        /// </summary>
        public bool mp { get; set; } = false;
    }
}
