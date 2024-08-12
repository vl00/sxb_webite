using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ProcViewEntities
{
    public class SettlementStatusModel
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public Guid PartTimeJobAdminId { get; set; }
    }
}
