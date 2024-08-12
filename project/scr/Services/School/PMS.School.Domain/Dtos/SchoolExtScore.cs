using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    public class SchoolExtScore
    {
        public int Id { get; set; }
        public int IndexId { get; set; }
        public double? Score { get; set; }
        public Guid Eid { get; set; }


    }
    public class SchoolExtScoreIndex
    {
        public int Id { get; set; }
        public string Index_Name { get; set; }
        public byte Level { get; set; }
        public int ParentId { get; set; }
    }

    public class AmbientScore
    {
        public Guid Eid { get; set; }

        /// <summary>
        /// 博物馆
        /// </summary>
        public int Museum { get; set; }

        /// <summary>
        /// 地铁
        /// </summary>
        public int Metro { get; set; }

        /// <summary>
        /// 商场
        /// </summary>
        public int Market { get; set; }

        /// <summary>
        /// 图书馆
        /// </summary>
        public int Library { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ShoppingInfo { get; set; }

        public int Police { get; set; }

        public int BookMarket { get; set; }

        public int River { get; set; }

        public int Bus { get; set; }

        public int Rubbish { get; set; }

        public int Hospital { get; set; }

        public int Subway { get; set; }

        public int View { get; set; }

        public double Buildingprice { get; set; }

        public int Traininfo { get; set; }

        public int Poiinfo { get; set; } = 0;

        public int Play { get; set; }
    }
}
