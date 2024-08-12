using System;
namespace Sxb.UserCenter.Models.CommentViewModel
{
    public class CommentSchoolViewModel
    {
        /// <summary>
        /// 学校分部Id
        /// </summary>
        public Guid ExtId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 是否上学帮认证
        /// </summary>
        public bool Auth { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public int LodgingType { get; set; }
        /// <summary>
        /// 寄宿类型描述
        /// </summary>
        public string LodgingReason { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 是否国际学校
        /// </summary>
        public bool International { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public int? Score { get; set; }

        /// <summary>
        /// 点评评分
        /// </summary>
        public int? Star { get; set; }

        /// <summary>
        /// 学校点评总条数
        /// </summary>
        public int CommentTotal { get; set; }
    }
}
