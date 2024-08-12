using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table("Evaluate")]
    public partial class Evaluate
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }

        /// <summary> 
        /// </summary> 
        public string Content { get; set; }

        /// <summary> 
        /// </summary> 
        public Guid? OrderID { get; set; }

        /// <summary> 
        /// 10分值 
        /// </summary> 
        public int? Score { get; set; }

        /// <summary> 
        /// 评价时间 
        /// </summary> 
        public DateTime? CreateTime { get; set; }

        /// <summary> 
        /// 是否系统自动评价 
        /// </summary> 
        public bool? IsAuto { get; set; }

        /// <summary> 
        /// </summary> 
        public bool? IsValid { get; set; }


    }
}