using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.RequestModel.Question
{
    public class QuestionWriterViewModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 学校Id
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 问题写入者
        /// </summary>
        public Guid UserId { get; set; }
        public string Content { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否有上传图片
        /// </summary>
        public bool IsHaveImagers { get; set; }
    }
}
