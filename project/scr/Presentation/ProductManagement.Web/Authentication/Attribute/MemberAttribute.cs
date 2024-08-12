using System;
using Microsoft.AspNetCore.Authorization;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Common;

namespace ProductManagement.Web.Authentication.Attribute
{
    public class MemberAttribute: AuthorizeAttribute
    {
        //前端登录用户
        public MemberAttribute()
        {
            Roles = $"{(int)UserRole.Member},{(int)UserRole.JobMember}";
        }
        //登录用户角色：普通用户，兼职用户
    }
}
