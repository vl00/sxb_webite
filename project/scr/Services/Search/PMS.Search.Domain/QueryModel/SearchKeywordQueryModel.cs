using PMS.Search.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace PMS.Search.Domain.QueryModel
{
    public class SearchKeywordQueryModel : SearchBaseQueryModel
    {
        public SearchKeywordQueryModel() : base()
        {
        }

        public SearchKeywordQueryModel(string keyword, int pageIndex, int pageSize) : base(keyword, pageIndex, pageSize)
        {
        }


        public ChannelIndex ChannelIndex => GetChannelIndex(Channel ?? 0);

        public int? Channel { get; set; }

        public ChannelIndex GetChannelIndex(int channel)
        {
            if (channel == 0)
            {
                return ChannelIndex.All;
            }
            return (ChannelIndex)channel;
        }
    }
}
