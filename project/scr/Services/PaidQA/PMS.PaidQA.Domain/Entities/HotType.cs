using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table("HotType")]
    public partial class HotType
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }

        /// <summary> 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        /// </summary> 
        public int? Sort { get; set; }

        /// <summary> 
        /// </summary> 
        public string LogoImage { get; set; }

        public bool IsValid { get; set; }
    }
}