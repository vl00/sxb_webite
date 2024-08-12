using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    public class SearchLive : SearchPUV
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public List<Guid> TypeIds { get; set; }

        public string City { get; set; }
        public int? CityCode { get; set; }

        public string Area { get; set; }
        public int? AreaCode { get; set; }

        public Guid LectorId { get; set; }
        public string LectorName { get; set; }

        public Guid UserId { get; set; }

        public DateTime? UpdateTime { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
