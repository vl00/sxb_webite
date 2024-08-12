using System;
namespace Sxb.PCWeb.ViewModels.Question
{
    public class QuestionSchoolViewModel
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
        public decimal? Score { get; set; }
        /// <summary>
        /// 学校问题总条数
        /// </summary>
        public int QuestionTotal { get; set; }
        /// <summary>
        /// 学校代号
        /// </summary>
        public string ShortSchoolNo { get; set; }
    }
}
