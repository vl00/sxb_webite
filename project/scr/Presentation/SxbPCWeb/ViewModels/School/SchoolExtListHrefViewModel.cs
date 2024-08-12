using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.School
{
    public class SchoolExtListHrefViewModel
    {
        public int? SelectCity { get; set; }
        public List<HrefViewModel> SelectCitys { get; set; }

        public List<HrefViewModel> HotCity { get; set; }

        public List<HrefViewModel> Areas { get; set; }
        public List<HrefViewModel> Metros { get; set; }



        public List<HrefViewModel> Tags { get; set; }

        public class HrefViewModel
        {
            public string Href { get; set; }
            public string Id { get; set; }

            public List<HrefViewModel> List { get; set; }
        }
    }
}
