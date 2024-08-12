using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.FinanceCenter
{
    public class CheckPayStatusRequest
    {
        public Guid orderId { get; set; }
        public OrderTypeEnum orderType { get; set; }

    }
}
