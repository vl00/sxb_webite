using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using PMS.OperationPlateform.Domain.DTOs;

namespace PMS.OperationPlateform.Domain.Entitys
{
	public partial class SchoolRankBinds
	{
        [Write(false)]
        public SchoolExtDto  SchoolInfo { get; set; }
    }
}