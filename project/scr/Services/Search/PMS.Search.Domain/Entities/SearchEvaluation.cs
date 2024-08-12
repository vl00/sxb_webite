using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    /// <summary>
    /// 测评
    /// </summary>
    public class SearchEvaluation : SearchPUV
    {
        /// <summary>
        /// 课程id
        /// </评测id>
        public Guid Id { get; set; }

        /// <summary>
        /// 评测no
        /// </summary>
        public long No { get; set; }

        /// <summary>
        /// 评测标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 评测内容模式
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 是否无图
        /// </summary>
        public int IsPlaintext { get; set; }

        /// <summary>
        /// 是否精华
        /// </summary>
        public int Stick { get; set; }

        /// <summary>
        /// 评测状态（1：上架；2：下架）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否后台添加的评测
        /// </summary>
        public int IsOfficial { get; set; }

        /// <summary>
        /// 专题id
        /// </summary>
        public Guid SpecialId { get; set; }

        /// <summary>
        /// 专题no
        /// </summary>
        public long SpecialNo { get; set; }

        /// <summary>
        /// 专题名
        /// </summary>
        public string SpecialName { get; set; }

        /// <summary>
        /// 专题状态
        /// </summary>
        public int SpecialStatus { get; set; }

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
        public int OrgIsAuthenticated { get; set; }

        /// <summary>
        /// 机构描述
        /// </summary>
        public string OrgDesc { get; set; }

        /// <summary>
        /// 机构副描述
        /// </summary>
        public string OrgSubdesc { get; set; }

        /// <summary>
        /// 机构状态
        /// </summary>
        public int OrgStatus { get; set; }

        /// <summary>
        /// 课程状态
        /// </summary>
        public int CourseStatus { get; set; }

        /// <summary>
        /// 课程id
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// 课程no
        /// </summary>
        public long CourseNo { get; set; }

        /// <summary>
        /// 课程名
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// 课程价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 课程上课方式
        /// </summary>
        public List<int> CourseMode { get; set; }

        /// <summary>
        /// 最小年龄
        /// </summary>
        public int MinAge { get; set; }

        /// <summary>
        /// 最大年龄
        /// </summary>
        public int MaxAge { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public int Subject { get; set; }

        /// <summary>
        /// 评测修改时间
        /// </summary>
        public DateTime? ModifyDateTime { get; set; }

        /// <summary>
        /// 评测是否有效
        /// </summary>
        public int IsValid { get; set; }

        /// <summary>
        /// 需要整合搜索
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 专题是否有效
        /// </summary>
        public int SpecialIsValid { get; set; }

        /// <summary>
        /// 机构是否有效
        /// </summary>
        public int OrgIsValid { get; set; }

        /// <summary>
        /// 课程是否有效
        /// </summary>
        public int CourseIsValid { get; set; }
    }
}
