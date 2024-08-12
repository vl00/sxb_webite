using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
    [Serializable]
    [Table("view_articlejoinuv")]
    public partial class view_articlejoinuv : article
    {
    }
}