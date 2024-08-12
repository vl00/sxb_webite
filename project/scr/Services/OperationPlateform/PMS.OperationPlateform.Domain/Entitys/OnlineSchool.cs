using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.Entitys
{
    public class OnlineSchool
    {
        [Key]
        public Guid id { get; set; }

        public string name { get; set; }

    }

    public class OnlineSchoolExtension
    {
        [Key]
        public Guid  id { get; set; }

        public Guid sid { get; set; }

        public string name { get; set; }



        [Write(false)]
        public OnlineSchool Parent { get; set; }

    }
}
