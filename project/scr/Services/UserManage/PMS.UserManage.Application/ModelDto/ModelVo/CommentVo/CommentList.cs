//using PMS.CommentsManage.Application.ModelDto;
//using PMS.CommentsManage.Domain.Common;
//using PMS.School.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.ModelDto.ModelVo.CommentVo
{
    public class CommentReplyData
    {
        public List<CommentList> commentModels { get; set; }
        public List<ReplayExhibition> commentReplyModels { get; set; }
    }    /// <summary>
         /// 点评
         /// </summary>
    public class CommentList
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 写入点评的用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserHeadImage { get; set; }
        /// <summary>
        /// 点评标签
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public List<string> Images { get; set; }
        /// <summary>
        /// 学校id 
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 点评打分
        /// </summary>
        public CommentScore Score { get; set; }
        /// <summary>
        /// 写入时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        /// <summary>
        /// 点亮
        /// </summary>
        public int StartTotal { get; set; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        public SchoolInfo School { get; set; }
        /// <summary>
        /// 是否为精选
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool IsRumorRefuting { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否为校方身份
        /// </summary>
        public bool IsUserSchool { get; set; }

        public class SchoolInfo
        {
            public Guid SchoolId { get; set; }
            public string SchoolName { get; set; }
            //学校类型
            public SchoolType SchoolType { get; set; }
            //是否寄宿
            public bool IsLodging { get; set; }
            /// <summary>
            /// 是否上学帮认证
            /// </summary>
            public bool IsAuth { get; set; }
            /// <summary>
            /// 学校总平均分
            /// </summary>
            public decimal SchoolAvgScore { get; set; }
            /// <summary>
            /// 前端展示星星
            /// </summary>
            public int SchoolStars { get; set; }
            /// <summary>
            /// 学校总点评数
            /// </summary>
            public int CommentTotal { get; set; }
            /// <summary>
            /// 是否存在点评
            /// </summary>
            public bool IsExists { get; set; }
            public string SchoolBranch { get; set; }
        }

        public class CommentScore
        {
            /// <summary>
            /// 是否就读
            /// </summary>
            public bool IsAttend { get; set; }

            /// <summary>
            /// 总分
            /// </summary>
            public decimal AggScore { get; set; }


            /// <summary>
            /// 师资力量分
            /// </summary>
            public decimal TeachScore { get; set; }

            /// <summary>
            /// 硬件设施分
            /// </summary>
            public decimal HardScore { get; set; }

            /// <summary>
            /// 环境周边分
            /// </summary>
            public decimal EnvirScore { get; set; }

            /// <summary>
            /// 学风管理分
            /// </summary>
            public decimal ManageScore { get; set; }

            /// <summary>
            /// 校园生活分
            /// </summary>
            public decimal LifeScore { get; set; }

        }

        public UserInfo UserInfo { get; set; }
    }
    public class UserInfo
    {
        public Guid ID { get; set; }
        public string NickName { get; set; }
        public string HeadImager { get; set; }
        public bool IsSchool { get; set; }
        public bool IsTalent { get; set; }
    }
}
