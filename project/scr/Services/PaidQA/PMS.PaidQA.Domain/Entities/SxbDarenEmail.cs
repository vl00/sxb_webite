using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

using System.Text;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table("SxbDarenEmail")]
    public partial class SxbDarenEmail
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }
}
