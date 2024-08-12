using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace PMS.Search.Domain.Common
{
    public class GlobalEnum
    {
        /// <summary>
        ///  1 学校 2 点评 4 问题 8 学校排名 16 文章  32 直播  64 话题圈 128 达人
        /// </summary>
        [Flags]
        public enum ChannelIndex
        {
            [DefaultValue(0)]
            //避免直接使用All的值. 可以用All本身或Def. 因当新增枚举时, 可能导致All的值改变, 以其为key的es/redis存储失效
            All = School | Comment | Question | SchoolRank | Article | Live | Circle | Talent | Course | OrgEvaluation,

            /// <summary>
            /// DefaultValue 兼容旧版本值
            /// </summary>
            [DefaultValue(1)]
            [ActionUrl("school-{0}")]
            School = 1,

            [DefaultValue(2)]
            [ActionUrl("/comment/{0}.html")]
            Comment = 2,

            [DefaultValue(3)]
            [ActionUrl("/question/{0}.html")]
            Question = 4,

            [DefaultValue(4)]
            [ActionUrl("SchoolRank/Detail?id={0}")]
            SchoolRank = 8,

            [DefaultValue(5)]
            [ActionUrl("article/{0}.html")]
            Article = 16,

            [DefaultValue(6)]
            [ActionUrl("live/client/livedetail.html?showtype=1&contentid={0}")]
            Live = 32,

            [DefaultValue(7)]
            [ActionUrl("topic/subject-details.html?circleId={0}")]
            Circle = 64,

            /// <summary>
            /// id是userId, 跳转到用户详情页
            /// </summary>
            [DefaultValue(8)]
            [ActionUrl("mine/mine-msg/?id={0}")]
            Talent = 128,

            /// <summary>
            /// 学校详情
            /// </summary>
            [DefaultValue(9)]
            [ActionUrl("svs/school/detail/{0}.html")]
            SvsSchool = 256,

            /// <summary>
            /// 结构
            /// </summary>
            [DefaultValue(10)]
            //[ActionUrl("svs/school/detail/{0}.html")]
            Org = 512,

            /// <summary>
            /// 机构测评
            /// </summary>
            [DefaultValue(11)]
            //[ActionUrl("svs/school/detail/{0}.html")]
            OrgEvaluation = 1024,

            /// <summary>
            /// 课程
            /// </summary>
            [DefaultValue(12)]
            //[ActionUrl("svs/school/detail/{0}.html")]
            Course = 2048,


            [DefaultValue(1000)]
            SchoolCommentQuestionArticle = School | Comment | Question | Article,
        }

    }

}
