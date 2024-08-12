using PMS.Search.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace PMS.Search.Domain.QueryModel
{
    public class SearchAllQueryModel : SearchSuggestionBase
    {
        public SearchAllQueryModel(string keyword)
        {
            Keyword = keyword;
        }

        public SearchAllQueryModel()
        {
        }

        /// <summary>
        /// 输入的关键词
        /// </summary>
        public string Keyword { get; set; }

        public int CityCode { get; set; }

        public int Size { get; set; } = 2;

        public int? Channel { get; set; }

        public ChannelIndex ChannelIndex => GetChannelIndex(Channel ?? 0);

        public bool MustCityCode { get; set; }

        public ChannelIndex GetChannelIndex(int channel)
        {
            if (channel == 0)
            {
                return ChannelIndex.All;
            }
            return (ChannelIndex)channel;
        }

        /// <summary>
        /// 所有字都必须命中
        /// </summary>
        public bool IsMatchAllWord { get; set; }
    }
}
