using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class TopicSuggestDto
    {
        public ChannelIndex Channel { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
