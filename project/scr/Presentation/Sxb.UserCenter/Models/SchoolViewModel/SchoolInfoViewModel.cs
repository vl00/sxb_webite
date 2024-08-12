using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.School.Domain.Common;

namespace Sxb.UserCenter.Models.SchoolViewModel
{
    /// <summary>
    /// 学校信息
    /// </summary>
    public class SchoolInfoViewModel
    {
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid Eid { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid Sid { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public int SchoolType { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public int LodgingType { get; set; }
        /// <summary>
        /// 总提问数
        /// </summary>
        public int QuestionTotal { get; set; }
        /// <summary>
        /// 星
        /// </summary>
        public int Star { get; set; }
        /// <summary>
        /// 点评分
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 总点评数
        /// </summary>
        public int CommentTotal { get; set; }
    }   

    /// <summary>
    /// 点评学校卡片
    /// </summary>
    //public class SchoolCdViewModel : SchoolInfoViewModel
    //{
    //    /// <summary>
    //    /// 星
    //    /// </summary>
    //    public int Star { get; set; }
    //    /// <summary>
    //    /// 点评分
    //    /// </summary>
    //    public int Score { get; set; }
    //    /// <summary>
    //    /// 总点评数
    //    /// </summary>
    //    public int CommentTotal { get; set; }
    //}

    ///// <summary>
    ///// 提问学校卡片
    ///// </summary>
    //public class SchoolQViewModel : SchoolInfoViewModel
    //{
    //    /// <summary>
    //    /// 总提问数
    //    /// </summary>
    //    public int QuestionTotal { get; set; }
    //}
}
