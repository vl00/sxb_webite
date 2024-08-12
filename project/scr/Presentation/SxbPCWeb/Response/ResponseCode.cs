using System;
using System.ComponentModel;

namespace Sxb.PCWeb.Response
{
    public enum ResponseCode
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功")]
        Success = 200,
        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败")]
        Failed = 201,
        /// <summary>
        /// 没有登录
        /// </summary>
        [Description("没有登录")]
        NoLogin = 402,
        /// <summary>
        /// 权限不足
        /// </summary>
        [Description("权限不足")]
        NoAuth = 403,
        /// <summary>
        /// 会员权限不足
        /// </summary>
        [Description("会员权限不足")]
        UnAuth = 10403,
        /// <summary>
        /// 调用方法找不到
        /// </summary>
        [Description("调用方法找不到")]
        NoFound = 10831,
        /// <summary>
        /// 系统异常
        /// </summary>
        [Description("系统异常")]
        Error = 500,

        /// <summary>
        /// 学校下架
        /// </summary>
        [Description("学校下架")]
        SchoolOffline = 406,
        /// <summary>
        /// 学校不存在
        /// </summary>
        [Description("学校不存在")]
        NoSchool = 406,
    }
}
