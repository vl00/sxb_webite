using System;
using System.ComponentModel;

namespace PMS.UserManage.Domain.Common
{
    public enum RegisterClientEnum : int
    {
        /// <summary>
        /// PC
        /// </summary>
        [Description("PC")]
        PC = 1,
        /// <summary>
        /// H5
        /// </summary>
        [Description("H5")]
        H5 = 2,

        /// <summary>
        /// APP
        /// </summary>
        [Description("APP")]
        APP = 3,


        /// <summary>
        /// 小程序
        /// </summary>
        [Description("小程序")]
        MiniProgramma = 4,


        /// <summary>
        /// 公众号
        /// </summary>
        [Description("公众号")]
        Official = 5
    }
}
