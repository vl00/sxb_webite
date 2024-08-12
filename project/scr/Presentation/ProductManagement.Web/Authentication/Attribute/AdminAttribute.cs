using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using PMS.CommentsManage.Domain.Common;

namespace ProductManagement.Web.Authentication.Attribute
{
    public class AdminAttribute: AuthorizeAttribute
    {
        //后台管理员用户
        public AdminAttribute()
        {
            Roles = $"{(int)AdminUserRole.Admin},{(int)AdminUserRole.Assessor}," +
                    $"{(int)AdminUserRole.Supplier},{(int)AdminUserRole.JobLeader}";
        }
        //登录用户角色：兼职领队，审核员， 供应商， 管理员


    }
}
