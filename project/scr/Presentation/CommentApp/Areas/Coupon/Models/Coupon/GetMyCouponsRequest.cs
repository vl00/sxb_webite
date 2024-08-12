using PMS.UserManage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Coupon.Models.Coupon
{
    [Flags]
    public enum TakeStatus
    {

        /// <summary>
        /// 待使用
        /// </summary>
        WaitUse=1,
        /// <summary>
        /// 已使用
        /// </summary>
        HasUse =2,
        /// <summary>
        /// 已失效
        /// </summary>
        Failure=3
    }

    public class GetMyCouponsRequest
    {
        public TakeStatus[] TakeStatus { get; set; }
    }
}
