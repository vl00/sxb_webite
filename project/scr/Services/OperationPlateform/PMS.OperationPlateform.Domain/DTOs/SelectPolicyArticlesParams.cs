using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.DTOs
{

    public class SchoolType {

        /// <summary>
        /// 办学类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 学校年纪
        /// </summary>
        public int SchoolGrade { get; set; }


        public bool Discount { get; set; }

        public bool Diglossia { get; set; }

        public bool Chinese { get; set; }

    }

    public class Local {

        /// <summary>
        /// 省ID
        /// </summary>
        public int? ProvinceId { get; set; }

        /// <summary>
        /// 城市ID
        /// </summary>
        public int? CityId { get; set; }


        /// <summary>
        /// 区ID
        /// </summary>
        public int? AreaId { get; set; }

    }

  
}
