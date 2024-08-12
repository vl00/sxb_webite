using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.SchoolRank
{
    public class PageVersionViewModel
    {
        public Func<PageVersionViewModel, object> OnCreatePrePageItemParams;

        public Func<int, PageVersionViewModel, object> OnCreatePageItemParams;

        public Func<PageVersionViewModel, object> OnCreateNextPageItemParams;

        public PageVersionViewModel(
           string action,
           string controller,
           int totalPage,
           int pageCount,
           int currentPage,
           Func<PageVersionViewModel, object> createPrePageItemParams,
            Func< int, PageVersionViewModel, object> createPageItemParams,
            Func<PageVersionViewModel, object> createNextPageItemParams
           )
        {
            this.Action = action;
            this.Controller = controller;
            this.TotalPage = totalPage;
            this.PageCount = pageCount;
            this.CurrentPage = currentPage;
            this.OnCreatePrePageItemParams = createPrePageItemParams;
            this.OnCreatePageItemParams = createPageItemParams;
            this.OnCreateNextPageItemParams = createNextPageItemParams;
        }
        public int CurrentPage { get; set; } = 1;

        public int PageCount { get; set; } = 30;

        public int TotalPage { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }
    }
}
