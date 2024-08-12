using Sxb.UserCenter.Models.CommonViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.CommentViewModel
{
    /// <summary>
    ///点评评分
    /// </summary>
    public class CommentScoreViewModel : ScoreStarViewModel
    {
        /// <summary>
        /// 点评Id
        /// </summary>
        public Guid CommentId { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
    }
}
