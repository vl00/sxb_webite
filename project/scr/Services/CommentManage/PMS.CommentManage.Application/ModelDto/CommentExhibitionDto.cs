using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class CommentExhibitionDto
    {
        /// <summary>
        /// 点评
        /// </summary>
        public SchoolComment schoolComment { get; set; }
        /// <summary>
        /// 点评图片
        /// </summary>
        public List<SchoolImage> CommentImages { get; set; }
        /// <summary>
        /// 点评评分
        /// </summary>
        public SchoolCommentScore SchoolCommentScore { get; set; }
    }
}
