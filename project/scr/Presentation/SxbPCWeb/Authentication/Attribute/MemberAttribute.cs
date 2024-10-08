﻿using System;
using Microsoft.AspNetCore.Authorization;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Common;

namespace Sxb.PCWeb.Authentication.Attribute
{
    public class MemberAttribute: AuthorizeAttribute
    {
        //前端登录用户
        public MemberAttribute()
        {
            Roles = $"{(int)UserRole.Member},{(int)UserRole.School},{(int)UserRole.OrgTalent},{(int)UserRole.PersonTalent}";
        }
    }
}
