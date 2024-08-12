using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    [Serializable]
    [Table("CouponOrderPreUseRecord")]
    public class CouponOrderPreUseRecord
    {
        [ExplicitKey]
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid CouponTakeId { get; set; }
    }
}
