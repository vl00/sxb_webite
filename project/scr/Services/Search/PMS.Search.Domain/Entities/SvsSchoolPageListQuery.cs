using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Search.Domain.Entities
{
    public class SvsSchoolPageListQuery
    {
        /// <summary>
        /// 第几页.不传默认为1
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 页大小.不传默认为10
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 搜索内容.可null
        /// </summary>
        public string SearchTxt { get; set; }

        /// <summary>
        /// 可null,选中的 省
        /// </summary>
        public string[] Pr { get; set; }

        /// <summary>
        /// 可null,选中的 市
        /// </summary>
        public string[] Ci { get; set; }

        /// <summary>
        /// 可null,选中的 专业
        /// </summary>
        public string[] Ma { get; set; }

        /// <summary>
        /// 可null,选中的 (2级)专业
        /// </summary>
        public string[] Mas { get; set; }

        /// <summary>
        /// 可null,选中的 学校类型
        /// </summary>
        public string[] Ty { get; set; }

        /// <summary>
        /// 可null,选中的 学校性质
        /// </summary>
        public string[] Na { get; set; }

        /// <summary>
        /// 可null,选中的 学校等级
        /// </summary>
        public string[] Lv { get; set; }
    }
}
