using Sxb.UserCenter.Models.CommonViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.SchoolViewModel
{
    /// <summary>
    /// 学校总点评分数
    /// </summary>
    public class SchoolCmScoreViewModel : ScoreStarViewModel
    {
        /// <summary>
        /// 学校Id
        /// </summary>
        public Guid Sid { get; set; }
        /// <summary>
        /// 学校分部Id
        /// </summary>
        public Guid ExtId { get; set; }
        /// <summary>
        /// 该学校总点评数
        /// </summary>
        public int CommentTotal { get; set; }
        /// <summary>
        /// 提问总数
        /// </summary>
        public int QuestionTotal { get; set; }
    }
}
