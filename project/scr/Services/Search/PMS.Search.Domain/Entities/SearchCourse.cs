using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    /// <summary>
    /// 课程
    /// </summary>
    public class SearchCourse : SearchPUV
    {
        /// <summary>
        /// 课程id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 课程no
        /// </summary>
        public long No { get; set; }

        /// <summary>
        /// 课程标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 课程副标题
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// 课程状态（1:上架；0：下架） 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 课程(最小)价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 上课方式
        /// </summary>
        public List<int> Mode { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public int Subject { get; set; }

        /// <summary>
        /// 最小年龄
        /// </summary>
        public int MinAge { get; set; }

        /// <summary>
        /// 最大年龄
        /// </summary>
        public int MaxAge { get; set; }

        /// <summary>
        /// 机构id
        /// </summary>
        public Guid OrgId { get; set; }

        /// <summary>
        /// 机构no
        /// </summary>
        public long OrgNo { get; set; }

        /// <summary>
        /// 机构名
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 机构是否已认证
        /// </summary>
        public int Authentication { get; set; }

        /// <summary>
        /// 机构描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 机构副描述
        /// </summary>
        public string SubDesc { get; set; }

        /// <summary>
        /// 机构状态
        /// </summary>
        public int OrgStatus { get; set; }

        /// <summary>
        /// 课程修改时间
        /// </summary>
        public DateTime? ModifyDateTime { get; set; }

        /// <summary>
        /// 课程是否有效
        /// </summary>
        public int IsValid { get; set; }

        /// <summary>
        /// 需要整合搜索
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 机构是否有效
        /// </summary>
        public int OrgIsValid { get; set; }

        /// <summary>
        /// 类型 1=课程 2=好物
        /// </summary>
        public int Type { get; set; }
    }
}
