using System;
using System.Collections.Generic;

namespace Sxb.Web.ViewModels.Search
{
    public class SearchMapViewModel
    {
        public int Level { get; set; }

        public long Count { get; set; }
        public long ViewCount { get; set; }

        public List<LocationViewModel> Locations { get; set; }

        public List<string> SelectType { get; set; }
    }

    public class LocationViewModel
    {
        public string Name { get; set; }
        /// <summary>
        /// 维度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        public long Count { get; set; }

        public object Ext { get; set; }

    }
}
