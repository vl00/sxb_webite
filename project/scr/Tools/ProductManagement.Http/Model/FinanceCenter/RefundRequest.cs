using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.FinanceCenter
{
    public class RefundRequest
    {

        public Guid OrderId { get; set; }

        public decimal RefundAmount { get; set; }

        public string Remark { get; set; }

        public int System { get; set; } = 1;
    }
}
