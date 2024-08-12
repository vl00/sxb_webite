using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.Common
{
    public class SchoolListNavViewModel
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public List<SchoolNavViewModel> List { get; set; }
    }
    public class SchoolNavViewModel
    {
        public string Title { get; set; }
        public string Src { get; set; }
    }

}
