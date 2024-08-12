using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    /// <summary>
    /// 机构
    /// </summary>
    public class SearchOrganization
    {
        /// <summary>
        /// 机构id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 机构no
        /// </summary>
        public long No { get; set; }

        /// <summary>
        /// 机构名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 机构是否已认证
        /// </summary>
        public int Authentication { get; set; }

        /// <summary>
        /// 机构状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 机构描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 机构副描述
        /// </summary>
        public string SubDesc { get; set; }

        /// <summary>
        /// 最小年龄
        /// </summary>
        public int MinAge { get; set; }

        /// <summary>
        /// 最大年龄
        /// </summary>
        public int MaxAge { get; set; }

        /// <summary>
        /// 机构类型
        /// </summary>
        public string Types { get; set; }

        /// <summary>
        /// 教学模式
        /// </summary>
        public string Modes { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subjects { get; set; }

        /// <summary>
        /// 机构修改时间
        /// </summary>
        public DateTime? ModifyDateTime { get; set; }

        /// <summary>
        /// 机构是否有效
        /// </summary>
        public int IsValid { get; set; }

    }
}
