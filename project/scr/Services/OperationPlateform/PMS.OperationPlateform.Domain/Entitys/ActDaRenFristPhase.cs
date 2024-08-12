using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
    [Serializable]
    [Table("ActDaRenFristPhase")]
    public partial class ActDaRenFristPhase
    {
        [Key]
        public int Id { get; set; }

        public Guid? DaRenUserId { get; set; }

        public string DaRenIntro { get; set; }

        public string DaRenQRCode { get; set; }

        public string CommentIDs { get; set; }

        public string ArticleIds { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}