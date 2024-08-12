using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Entities
{
    public partial class Order
    {
        [Computed]
        public long OverRemainTime
        {
            get
            {
                var now = DateTime.Now;
                if (OverTime <= now)
                {
                    return 0;
                }
                else
                {
                    return (long)(OverTime - now).TotalMilliseconds;
                }
            }
        }

        [Computed]
        public DateTime OverTime
        {
            get
            {
                return this.CreateTime.AddHours(24); //结束时间;
            }
        }

    }
}
