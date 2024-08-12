using System;
using System.Linq.Expressions;
using PMS.School.Domain.Common;
using PMS.School.Domain.Entities;

namespace PMS.School.Application.ModelDto
{
    public class ExtraSchCorrect0Dto
    {
        //public Guid Id { get; set; }
        public Guid Eid { get; set; }
        public SchCorrectErrType Type { get; set; }
        //public SchCorrectStatus Status { get; set; }
        public string[] Img { get; set; }
        public string Remark { get; set; }

        public Guid Creator { get; set; }
        public string CreatorMobile { get; set; }

        public string Address { get; set; }
        public double? Lng { get; set; }
        public double? Lat { get; set; }
        public string SchName { get; set; }
        public string SchType { get; set; }

    }
}
