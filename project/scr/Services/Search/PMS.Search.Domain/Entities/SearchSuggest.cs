using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace PMS.Search.Domain.Entities
{

    public class SearchSuggestResult
    {

        public long Total { get; set; }

        public List<SearchSuggest> Suggests { get; set; }
    }

    public class SearchSuggest
    {
        [Obsolete]
        public int Type { get; set; }
        public ChannelIndex? Index { get; set; }
        public long Total { get; set; }
        public List<SearchSuggestItem> Items { get; set; }
    }

    public class SearchSuggestItem
    {
        public Guid SourceId { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
    }

    public class SearchShortIdSuggestItem
    {
        public int No { get; set; }
        public string ShortId => UrlShortIdUtil.Long2Base32(No);
        public string Name { get; set; }
    }

    public class SearchSuggestOriginItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Title { get; set; }
        public string AuthTitle { get; set; }
        public string Context { get; set; }
        public int No { get; set; }
    }
}