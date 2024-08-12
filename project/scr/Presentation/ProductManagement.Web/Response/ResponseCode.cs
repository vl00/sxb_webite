using System;
using System.ComponentModel;

namespace ProductManagement.Web.Response
{
    public enum ResponseCode
    {
        [Description("操作成功")]
        Success = 10000,
        [Description("操作失败")]
        Failed = 10001,
        [Description("没有登录")]
        NoLogin = 10007,
        [Description("游客权限不足")]
        NoAuth = 10263,
        [Description("会员权限不足")]
        UnAuth = 10403,
        [Description("调用方法找不到")]
        NoFound = 10831,
        [Description("系统异常")]
        Error = 10505

    }
}
