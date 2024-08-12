using System;
namespace Sxb.Web.RequestModel
{
    public class ReplyAdd
    {
        public Guid SchoolCommentId { get; set; }
        public Guid? ReplyId { get; set; }

        /// <summary>
        /// 是否学校发布
        /// </summary>
        public bool IsSchoolPublish { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }
    }
}
