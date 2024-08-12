using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class TalentInfoResult
    {

        [Description("用户ID")]
        public Guid UserID { get; set; }

        [Description("专家昵称")]
        public string NickName { get; set; }

        [Description("专家头像")]
        public string HeadImgUrl { get; set; }

        [Description("认证称号")]
        public string AuthName { get; set; }

        [Description("专家等级")]
        public string LevelName { get; set; }

        [Description("擅长学段列表")]
        public List<string> GradeNames { get; set; }

        [Description("擅长领域列表")]
        public List<string> RegionTypeNames { get; set; }

        [Description("回复率")]
        public double ReplyPercent { get; set; }

        [Description("评分")]
        public double Score { get; set; }

        [Description("咨询金额")]
        public decimal Price { get; set; }

        [Description("专家简介")]
        public string Introduction { get; set; }

        [Description("当前用户是否关注该专家")]
        public bool IsFollowed { get; set; }
        [Description("专家类型")]
        public int TalentType { get; set; }
    }




    /// <summary>
    /// 携带一条热门回复内容
    /// </summary>
    public class TalentInfoResult_01:TalentInfoResult
    {

        /// <summary>
        /// 热门回复内容
        /// </summary>
        public string Content { get; set; }
    }
}
