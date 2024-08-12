using System;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace Sxb.Web.ViewModels.Search
{
    public class SuggestViewModel
    {
        public int Channel { get; set; }
        public int ChannelName { get; set; }
        public int Type { get; set; }
        public ChannelIndex Index { get; set; }
        public string KeyWord { get; set; }
        public string KeyWord2 { get; set; }

        public object Show { get; set; }
        public object Value { get; set; }

        /// <summary>
        /// 搜索建议, 点击跳转url
        /// </summary>
        public string ActionUrl { get; set; }
    }

}
