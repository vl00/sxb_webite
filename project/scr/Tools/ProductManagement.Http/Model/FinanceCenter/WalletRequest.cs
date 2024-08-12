using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.FinanceCenter
{

    public class WalletRequest
    {
        public Guid userId { get; set; }
        public decimal virtualAmount { get; set; }
      
        public decimal amount { get; set; }
        public int statementType { get; set; }
        public int io { get; set; }
        public Guid orderId { get; set; }
        public int orderType { get; set; }
        public string remark { get; set; }
    }

}
