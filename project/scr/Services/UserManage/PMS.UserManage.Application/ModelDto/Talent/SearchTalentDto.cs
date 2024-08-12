using PMS.Search.Domain.Entities;
using PMS.UserManage.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.ModelDto.Talent
{
    public class SearchTalentDto : TalentAbout
    {
        /// <summary>
        /// 点击跳转的链接
        /// </summary>
        public string ActionUrl { get; set; }

        /// <summary>
        /// 登录人是否关注
        /// </summary>
        public bool IsFollow { get; set; }
    }

    public class TalentFollowDto : TalentAbout
    {
        /// <summary>
        /// 登录人是否关注
        /// </summary>
        public bool IsFollow { get; set; }

        /// <summary>
        /// 用户简介
        /// </summary>
        public string Introduction { get; set; }
    }
}
