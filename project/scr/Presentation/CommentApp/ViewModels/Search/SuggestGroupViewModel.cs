using System;
using System.Collections.Generic;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace Sxb.Web.ViewModels.Search
{
    public class SuggestGroupViewModel
    {
        public int GroupType { get; set; }
        public ChannelIndex Index { get; set; }
        public string TipsText { get; set; }
        public List<SuggestViewModel> Items { get; set; }
    }

    public class SearchAllPage
    {
        public ChannelIndex Index { get; set; }
        public object Items { get; set; }
    }
}
