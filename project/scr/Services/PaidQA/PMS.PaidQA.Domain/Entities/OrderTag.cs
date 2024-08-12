using Dapper.Contrib.Extensions;
using PMS.PaidQA.Domain.Enums;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table("OrderTag")]
    public partial class OrderTag
    {
        [ExplicitKey]
        public Guid ID { get; set; }
        public Guid OrderID { get; set; }
        public Guid TagID { get; set; }
        public OrderTagType TagType { get; set; }
    }
}