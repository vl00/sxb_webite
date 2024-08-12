using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto.Query
{
    public class SearchQuestionQuery
    {
        public string Keyword{get;set;}
        public List<string> Keywords{get;set; }
        public Guid? Eid { get; set; }
        public Guid? UserId { get; set; }

        public List<int> CityIds {get;set; } = new List<int>();
        public List<int> AreaIds{get;set; } = new List<int>();
        public List<int> GradeIds{get;set; } = new List<int>();
        public List<int> TypeIds{get;set; } = new List<int>();
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public bool? Lodging { get; set; }

        /// <summary>
        /// 排序 0 智能排序 1 时间优先 3 评论数优先
        /// </summary>
        public int Orderby { get; set; }
    }
}
