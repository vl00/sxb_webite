using System;
using System.ComponentModel;

namespace Sxb.Inside.Response
{
    public enum ResponseCode
    {
        [Description("操作成功")]
        Success = 200,
        [Description("操作失败")]
        Failed = 201,
        [Description("没有登录")]
        NoLogin = 402,
        [Description("权限不足")]
        NoAuth = 403,
        [Description("会员权限不足")]
        UnAuth = 10403,
        [Description("调用方法找不到")]
        NoFound = 10831,
        [Description("系统异常")]
        Error = 500

    }
}
