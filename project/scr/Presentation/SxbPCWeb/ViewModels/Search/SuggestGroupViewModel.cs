using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.Search
{
    public class SuggestGroupViewModel
    {
        public int GroupType { get; set; }
        public string TipsText { get; set; }
        public List<SuggestViewModel> Items { get; set; }
    }
}
