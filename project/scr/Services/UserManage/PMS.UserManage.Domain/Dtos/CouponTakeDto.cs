using Dapper.Contrib.Extensions;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Dtos
{
    public class CouponTakeDto:CouponTake
    {

        [Write(false)]
        public CouponInfo  CouponInfo { get; set; }
    }
}
