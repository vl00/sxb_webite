using System;

namespace Sxb.PCWeb.ViewModels.Article
{
    public class PaginationViewModel
    {
        public Func<PaginationViewModel, object> OnCreatePrePageItemParams;

        public Func<int, PaginationViewModel, object> OnCreatePageItemParams;

        public Func<PaginationViewModel, object> OnCreateNextPageItemParams;

        public PaginationViewModel(
           string action,
           string controller,
           int totalPage,
           int pageCount,
           int currentPage,
           Func<PaginationViewModel, object> createPrePageItemParams,
            Func<int, PaginationViewModel, object> createPageItemParams,
            Func<PaginationViewModel, object> createNextPageItemParams
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

        public PaginationViewModel(
           string action,
           string controller,
           int totalPage,
           int pageCount,
           int currentPage,
           int cityCode,
           Func<PaginationViewModel, object> createPrePageItemParams,
            Func<int, PaginationViewModel, object> createPageItemParams,
            Func<PaginationViewModel, object> createNextPageItemParams
           )
        {
            this.Action = action;
            this.Controller = controller;
            this.TotalPage = totalPage;
            this.PageCount = pageCount;
            this.CurrentPage = currentPage;
            this.CityCode = cityCode;
            this.OnCreatePrePageItemParams = createPrePageItemParams;
            this.OnCreatePageItemParams = createPageItemParams;
            this.OnCreateNextPageItemParams = createNextPageItemParams;
        }

        public PaginationViewModel(
           string action,
           string controller,
           int totalPage,
           int pageCount,
           int currentPage,
           int cityCode,
           string tagName,
           Func<PaginationViewModel, object> createPrePageItemParams,
            Func<int, PaginationViewModel, object> createPageItemParams,
            Func<PaginationViewModel, object> createNextPageItemParams
           )
        {
            this.Action = action;
            this.Controller = controller;
            this.TotalPage = totalPage;
            this.PageCount = pageCount;
            this.CurrentPage = currentPage;
            this.CityCode = cityCode;
            this.OnCreatePrePageItemParams = createPrePageItemParams;
            this.OnCreatePageItemParams = createPageItemParams;
            this.OnCreateNextPageItemParams = createNextPageItemParams;
            TagName = tagName;
        }

        public int CurrentPage { get; set; } = 1;

        public int PageCount { get; set; } = 10;

        public int TotalPage { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }

        public int CityCode { get; set; }
        public string TagName { get; set; }
    }
}
