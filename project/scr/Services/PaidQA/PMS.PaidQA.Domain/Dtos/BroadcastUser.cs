using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Domain.Dtos
{

    /// <summary>
    /// 优惠券广播轮播的用户
    /// </summary>
    public class BroadcastUser
    {
        public string Minute { get; set; }

        public string Second { get; set; }

        public string NickName { get; set; }
    }
}
