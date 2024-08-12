using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class SchoolRankDetailDto:SchoolRank
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
