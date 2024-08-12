using System;
using System.Collections.Generic;

namespace Sxb.Web.ViewModels.Search
{
    public class MetroViewModel
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public List<StationViewModel> Child { get; set; }

        public class StationViewModel
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
    }
}
