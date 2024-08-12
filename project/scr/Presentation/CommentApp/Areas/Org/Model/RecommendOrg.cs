using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.Org.Model
{
    public class RecommendOrg
    {
        public PageItem PageInfo { get; set; }
        public IEnumerable<OrgType> AllOrgTypes { get; set; }
    }

    public class OrgType
    {
        public int Key { get; set; }
        public string Value { get; set; }
        public int Sort { get; set; }
    }
    public class PageItem
    {
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public int TotalPageCount { get; set; }
        public IEnumerable<PageDataObject> CurrentPageItems { get; set; }
    }
    public class PageDataObject
    {
        public Guid ID { get; set; }
        public string ID_S { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public bool Authentication { get; set; }
        public string Desc { get; set; }
        public string SubDesc { get; set; }
        public int CourceCount { get; set; }
        public int EvaluationCount { get; set; }
        public string Url
        {
            get
            {
                return $"/org/orgs/detail/{ID_S}";
            }
        }
    }
}
