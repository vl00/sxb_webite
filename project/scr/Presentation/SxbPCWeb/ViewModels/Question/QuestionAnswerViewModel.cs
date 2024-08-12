using Sxb.PCWeb.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Question
{
    /// <summary>
    /// 问题回答
    /// </summary>
    public class QuestionAnswerViewModel
    {
        /// <summary>
        /// 回答Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 提问Id
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 回答内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeTotal { get; set; }
        /// <summary>
        /// 回答数
        /// </summary>
        public int AnswerTotal { get; set; }
        /// <summary>
        /// 回答日期
        /// </summary>
        public string AddTime { get; set; }
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
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 用户实体
        /// </summary>
        public UserInfoVo UserInfoVo { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学校分部名称
        /// </summary>
        public string SchoolExtName { get; set; }
        /// <summary>
        /// 学校短链接
        /// </summary>
        public string ShortSchoolNo { get; set; }
    }
}
