using System;
namespace Sxb.PCWeb.ViewModels.Search
{
    public class SuggestViewModel
    {
        public int Channel { get; set; }
        public int ChannelName { get; set; }
        public int Type { get; set; }
        public string KeyWord { get; set; }

        public object Show { get; set; }
        public object Value { get; set; }
    }
}
