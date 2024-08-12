using Sxb.UserCenter.Models.SchoolViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.QuestionViewModel
{
    public class QuestionInfoViewModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 问题id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid ExId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid Sid { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 提问内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点赞总数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 回答总数
        /// </summary>
        public int AnswerCount { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 当前用户是否回复
        /// </summary>
        public bool IsAnswer { get; set; }
        /// <summary>
        /// 提问图片
        /// </summary>
        public List<string> Images { get; set; }
        /// <summary>
        /// 提问时间
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 检测是否关注该提问
        /// </summary>
        public bool IsCollection { get; set; }
        /// <summary>
        /// 学校卡片
        /// </summary>
        public SchoolQuestionCardViewModel School { get; set; }
        /// <summary>
        /// 用户实体
        /// </summary>
        public UserViewModel UserInfoVo { get; set; }
        /// <summary>
        /// 回答列表
        /// </summary>
        public List<QuestionAnswerViewModel> Answers { get; set; }
    }
}
