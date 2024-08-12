using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// ��֤�ȼ�
    /// </summary>
    [Serializable]
    [Table(nameof(LevelType))]
    public partial class LevelType
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }

        /// <summary> 
        /// </summary> 
        public string Name { get; set; }
        public int Sort { get; set; }
    }
}