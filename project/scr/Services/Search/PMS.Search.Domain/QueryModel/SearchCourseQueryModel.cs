using PMS.Search.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.QueryModel
{
    public class SearchCourseQueryModel : SearchBaseQueryModel
    {
        public SearchCourseQueryModel() : base()
        {
        }

        public SearchCourseQueryModel(string keyword, int pageIndex, int pageSize) : base(keyword, pageIndex, pageSize)
        {
        }

        /// <summary>
        /// 搜索课程type类型 1=课程 2=好物
        /// </summary>
        public int? Type { get; set; }


        public bool SearchTitleOnly { get; set; }
    }
}
