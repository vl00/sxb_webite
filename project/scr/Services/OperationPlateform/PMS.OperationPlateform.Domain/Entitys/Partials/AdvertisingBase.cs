using Dapper.Contrib.Extensions;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.Entitys
{
    public partial class AdvertisingBase
    {
        [Write(false)]
        public int Sort { get; set; }
        public int CityId { get; set; }
        public LocationDataType DataType { get; set; }
        [Write(false)]
        public int BeforeCount { get; set; }
        [Write(false)]
        public bool IsTop { get; set; }
    }
}
