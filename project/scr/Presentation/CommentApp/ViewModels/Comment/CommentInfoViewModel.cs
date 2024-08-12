using PMS.CommentsManage.Application.ModelDto;
using Sxb.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.School.Domain.Common;

namespace Sxb.Web.ViewModels.Comment
{

    /// <summary>
    /// 点评信息
    /// </summary>
    public class CommentInfoViewModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 点评id
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
        /// 是否为精华点评
        /// </summary>
        public bool IsEssence { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeTotal { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyTotal { get; set; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 点评图片
        /// </summary>
        public List<string> Images { get; set; }
        /// <summary>
        /// 写入日期
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 检测是否关注该点评
        /// </summary>
        public bool IsCollection { get; set; }
        /// <summary>
        /// 学校卡片
        /// </summary>
        public SchoolCommentCardViewModel School { get; set; }
        /// <summary>
        /// 写入用户
        /// </summary>
        public UserInfoVo UserInfo { get; set; }
        /// <summary>
        /// 用户评分
        /// </summary>
        public SchoolCmScoreDto CommentScore { get; set; }
    }

    /// <summary>
    /// 学校点评卡片
    /// </summary>
    public class SchoolCommentCardViewModel
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
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 点评条数
        /// </summary>
        public int CommentTotal { get; set; }
        /// <summary>
        /// 星
        /// </summary>
        public int Stars { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public SchoolType SchoolType { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public int LodgingType { get; set; }
        /// <summary>
        /// 寄宿类型描述
        /// </summary>
        public string LodgingReason { get; set; }
        /// <summary>
        /// 是否上学帮认证
        /// </summary>
        public bool IsAuth { get; set; }

        public bool International { get; set; }
        /// <summary>
        /// 学校代号
        /// </summary>
        public string ShortSchoolNo { get; set; }
    }
}
