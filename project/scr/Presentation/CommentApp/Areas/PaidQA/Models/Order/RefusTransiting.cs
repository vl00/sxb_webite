using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    public class RefusTransitingRequest
    {
        [Description("消息ID")]
        public Guid MsgID { get; set; }
    }
    public class RefusTransitingResult
    {
        public Message Message { get; set; }
    }
}
