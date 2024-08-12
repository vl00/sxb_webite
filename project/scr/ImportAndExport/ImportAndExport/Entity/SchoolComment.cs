using ImportAndExport.Models;
using System;
namespace ImportAndExport.Entity
{
    public class SchoolComment
    {
        public SchoolComment() 
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 评论者ID
        /// </summary>
        public Guid CommentUserId { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点评审核状态（0：未阅，1：已阅，2：已加精，-1：已屏蔽） 默认值为：0（可以在前端显示）
        /// </summary>
        public int State { get; set; }

        public int PostUserRole { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        /// <summary>
        /// 是否已结算
        /// </summary>
        public bool IsSettlement { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 是否有上传图片
        /// </summary>
        public bool IsHaveImagers { get; set; }
        /// <summary>
        /// 写入日期
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 导入类型
        /// </summary>
        public ImportType ImportType { get; set; }

        public SchoolCommentScore CommentScore { get; set; }
    }
}
