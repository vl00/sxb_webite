using Dapper.Contrib.Extensions;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.DTOs
{
    public class FixedAdDto
    {
        public int AdId { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string PicUrl { get; set; }

        public Guid? DataId { get; set; }

        public Guid? RefId { get; set; }

        /// <summary>
        /// 广告固定的位置  1 前缀  2 后缀
        /// </summary>
        public int PositionType { get; set; }

        public LocationDataType DataType { get; set; }
    }
}
