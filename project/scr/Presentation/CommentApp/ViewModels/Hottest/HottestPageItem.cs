using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProductManagement.Framework.Foundation;

namespace Sxb.Web.ViewModels.Hottest
{
    public class HottestPageItem
    {
        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        public HottestPageItemType Type { get; set; }
        /// <summary>
        /// 目标ID
        /// </summary>
        [Description("目标ID")]
        public Guid TargetID { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        public List<string> Images { get; set; } = new List<string>();
        /// <summary>
        /// 标签
        /// </summary>
        [Description("标签")]
        public List<string> Tags { get; set; } = new List<string>();
        /// <summary>
        /// 用户标签
        /// </summary>
        [Description("用户标签")]
        public List<string> UserTags { get; set; } = new List<string>();
        /// <summary>
        /// 用户角色
        /// </summary>
        [Description("用户角色")]
        public List<string> UserRoles { get; set; } = new List<string>();
        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 内容
        /// </summary>
        [Description("内容")]
        public string Content { get; set; } = string.Empty;
        /// <summary>
        /// 话题圈名称
        /// </summary>
        [Description("话题圈名称")]
        public string CircleName { get; set; } = string.Empty;
        /// <summary>
        /// 视频URL
        /// </summary>
        [Description("视频URL")]
        public string VideoUrl { get; set; } = string.Empty;
        /// <summary>
        /// 学校名称
        /// </summary>
        [Description("学校名称")]
        public string SchoolName { get; set; } = string.Empty;
        /// <summary>
        /// 学校分部名称
        /// </summary>
        [Description("学校分部名称")]
        public string SchoolExtName { get; set; } = string.Empty;
        /// <summary>
        /// 用户头像
        /// </summary>
        [Description("用户头像")]
        public string UserHeadImg { get; set; } = string.Empty;
        /// <summary>
        /// 用户名
        /// </summary>
        [Description("用户名")]
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 创建时间(中文)
        /// </summary>
        [Description("创建时间(中文)")]
        public string CreateTimeStr
        {
            get
            {
                return CreateTime.ConciseTime("yyyy年MM月dd日");
            }
        }
        /// <summary>
        /// 短ID
        /// </summary>
        [Description("短ID")]
        public string ShortID { get; set; }
        /// <summary>
        /// 图片数量
        /// </summary>
        [Description("图片数量")]
        public int ImageCount { get; set; }
        /// <summary>
        /// 查看数量(观看数量)
        /// </summary>
        [Description("查看数量(观看数量)")]
        public int ViewCount { get; set; }
        /// <summary>
        /// 回复数量
        /// </summary>
        [Description("回复数量")]
        public int ReplyCount { get; set; }
        /// <summary>
        /// 点赞数量
        /// </summary>
        [Description("点赞数量")]
        public int LikeCount { get; set; }
        /// <summary>
        /// 学校星数
        /// </summary>
        [Description("学校星数")]
        public int StarCount { get; set; }
        /// <summary>
        /// 点评数量
        /// </summary>
        [Description("点评数量")]
        public int CommentCount { get; set; }
        /// <summary>
        /// 文章封面类型
        /// <para>
        /// 1.小图 | 2.大图
        /// </para>
        /// </summary>
        [Description("文章封面类型")]
        public int ArticleLayout { get; set; }
        /// <summary>
        /// 达人类型
        /// </summary>
        [Description("达人类型")]
        public int TalentType { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        [Description("是否辟谣")]
        public bool IsRumor { get; set; }
        /// <summary>
        /// 是否加精
        /// </summary>
        [Description("是否加精")]
        public bool IsHighLight { get; set; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        [Description("是否点赞")]
        public bool IsLike { get; set; }
        /// <summary>
        /// 是否达人
        /// </summary>
        [Description("是否达人")]
        public bool IsTalent { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 站内热门类型
    /// </summary>
    public enum HottestPageItemType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknow = 0,
        /// <summary>
        /// 问答
        /// </summary>
        [Description("问答")]
        Question = 1,
        /// <summary>
        /// 点评
        /// </summary>
        [Description("点评")]
        Comment = 2,
        /// <summary>
        /// 攻略
        /// </summary>
        [Description("攻略")]
        Article = 3,
        /// <summary>
        /// 话题圈
        /// </summary>
        [Description("话题圈")]
        Topic = 4,
        /// <summary>
        /// 直播
        /// </summary>
        [Description("直播")]
        Live = 5
    }
}
