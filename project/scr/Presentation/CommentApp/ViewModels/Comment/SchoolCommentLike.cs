using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Models.Comment
{
    public class SchoolCommentLike
    {
        public SchoolCommentLike(int likeTotal,bool isLike)
        {
            this.LikeTotal = likeTotal;
            this.IsLike = isLike;
        }
        /// <summary>
        /// 该点评总点赞次数
        /// </summary>
        public int LikeTotal { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }
    }
}
