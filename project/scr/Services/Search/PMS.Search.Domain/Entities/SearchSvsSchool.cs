using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    public class PageData
    {
        public long Total { get; set; }
        public List<SearchSvsSchool> PageList{get;set;}
    }

    public class SearchSvsSchool
    {
        /// <summary>
        /// guid
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid? SId { get; set; }

        /// <summary>
        /// 学校短id
        /// </summary>
        public long No { get; set; }

        /// <summary>
        /// 学校name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 学校logo
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 学校联系电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public int? Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public int? City { get; set; }

        /// <summary>
        /// 学校类型
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 学校等级
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// 学校性质
        /// </summary>
        public int? Nature { get; set; }

        /// <summary>
        /// 学校地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 学校热门专业
        /// </summary>
        public string HotMajors { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 二级专业
        /// </summary>
        public List<long> MajorSubNos { get; set; }

        /// <summary>
        /// 一级专业
        /// </summary>
        public List<long> MajorNos { get; set; }
    }
}
