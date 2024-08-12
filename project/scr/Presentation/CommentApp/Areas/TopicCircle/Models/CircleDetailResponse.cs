using PMS.TopicCircle.Application.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class CircleDetailResponse
    {




        [Description("话题圈ID")]
        public Guid Id { get; set; }
        [Description("话题圈名称")]
        public string Name { get; set; }
        [Description("话题圈简介")]
        public string Intro { get; set; }
        [Description("话题圈背景图链接")]
        public string Cover { get; set; }
        [Description("话题圈启用状态（是否允许人加入）")]
        public bool IsDisable { get; set; }
        [Description("话题圈圈主ID")]
        public Guid UserId { get; set; }
        [Description("话题圈圈主昵称")]
        public string Nickname { get; set; }
        [Description("话题圈圈主达人认证信息")]
        public string AuthInfo { get; set; }
        [Description("圈主介绍")]
        public string MasterIntroduce { get; set; }
        [Description("话题圈圈主头像链接")]
        public string HeadImgUrl { get; set; }
        [Description("粉丝数")]
        public int FollowerCount { get; set; }
        [Description("新增加的粉丝数")]
        public int NewFollower { get; set; }
        [Description("话题数")]
        public long TopicCount { get; set; }
        [Description("是否是圈主")]
        public bool IsCircleMaster { get; set; }
        [Description("是否登录")]
        public bool IsLogin { get; set; }
        [Description("登录用户是否加入圈子")]
        public bool IsFollow { get; set; }
        [Description("昨日新增粉丝数")]
        public int YesterdayFollowerCount { get; set; }
        [Description("昨日新增话题数")]
        public int YesterdayTopicCount { get; set; }

        [Description("背景颜色")]
        public string BgColor { get; set; }




    }
}
