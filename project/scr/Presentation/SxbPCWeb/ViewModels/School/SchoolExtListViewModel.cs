using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.School
{
    public class SchoolExtListViewModel
    {
        public long Total { get; set; }
        public PageNavViewModel PageNav { get; set; }
        public List<SchoolExtListItemViewModel> List { get; set; }
    }
    public class PageNavViewModel
    {
        public long TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public string Url { get; set; }
    }
}
