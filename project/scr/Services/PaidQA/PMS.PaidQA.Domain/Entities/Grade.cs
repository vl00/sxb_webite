using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// Ñ§¶Î
    /// </summary>
    [Serializable]
    [Table("Grade")]
    public partial class Grade
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
        public int Sort { get; set; }

        /// <summary> 
        /// </summary> 
        public bool IsValid { get; set; }


    }
}