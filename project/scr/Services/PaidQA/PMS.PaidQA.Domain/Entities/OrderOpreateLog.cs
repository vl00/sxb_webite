using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table("OrderOpreateLog")]
    public class OrderOpreateLog
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid? OrderId { get; set; }

        public string? Remark { get; set; }

        public DateTime? CreateTime { get; set; }

        public Guid? UserId { get; set; }

        public int? UpdateStatus { get; set; }
        public int? PayStatus { get; set; }
    }
}
