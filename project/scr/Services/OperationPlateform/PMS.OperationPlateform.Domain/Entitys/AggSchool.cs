using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
    [Serializable]
    [Table("AggSchool")]
    public partial class AggSchool
    {
        [Key]
        public Guid ExtId { get; set; }
    }
}