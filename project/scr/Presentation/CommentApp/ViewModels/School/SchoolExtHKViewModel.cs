using System;
namespace Sxb.Web.ViewModels.School
{
    public class SchoolExtHKViewModel
    {
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName{get;set;}

        /// <summary>
        /// 距离
        /// </summary>
        public decimal Distance { get; set; }


        /// <summary>
        /// 口碑评级分
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 星星
        /// </summary>
        public int Star { get; set; }
        /// <summary>
        /// 学校点评总条数
        /// </summary>
        public int CommentTotal { get; set; }
    }
}
