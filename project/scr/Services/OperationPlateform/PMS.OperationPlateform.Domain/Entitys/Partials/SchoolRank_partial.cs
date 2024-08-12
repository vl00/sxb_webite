using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using ProductManagement.Framework.Foundation;

namespace PMS.OperationPlateform.Domain.Entitys
{
    public partial class SchoolRank
    {
        [Write(false)]
        public List<SchoolRankBinds> SchoolRankBinds { get; set; }

        [Computed]
        public string ShortId
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(this.No).ToLower(); ;
            }
        }
    }
}