using System;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace PMS.Search.Domain.Entities
{
    public class SearchAll
    {
        public int Type { get; set; }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Title { get; set; }
        public string Context { get; set; }

        public DateTime? UpdateTime { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class SearchId
    {
        public ChannelIndex Channel { get; set; }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}
