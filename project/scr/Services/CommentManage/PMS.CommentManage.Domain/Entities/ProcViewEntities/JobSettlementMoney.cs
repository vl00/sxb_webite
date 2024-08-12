using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ProcViewEntities
{
    public class JobSettlementMoney
    {
        public Guid UserId { get; set; }

        public Guid JobId { get; set; }
        
        public float SettlementAmount { get; set; }

        public string openID { get; set; }

        public string appName { get; set; }

        public int SelectTotal { get; set; }
    }
}
