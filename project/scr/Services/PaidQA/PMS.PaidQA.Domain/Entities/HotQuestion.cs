using Dapper.Contrib.Extensions;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table("HotQuestion")]
    public partial class HotQuestion
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }

        /// <summary> 
        /// </summary> 
        public int Sort { get; set; }

        public Guid HotTypeID { get; set; }
        public Guid OrderID { get; set; }
        public long ViewCount { get; set; }
    }
}