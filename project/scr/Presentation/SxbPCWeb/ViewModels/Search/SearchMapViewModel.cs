using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.Search
{
    public class SearchMapViewModel
    {
        public int Level { get; set; }

        public int Count { get; set; }
        public int ViewCount { get; set; }

        public List<LocationViewModel> Locations { get; set; }
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

        public int Count { get; set; }

    }
}
