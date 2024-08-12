using PMS.PaidQA.Domain.Entities;
using System.Collections.Generic;

namespace PMS.PaidQA.Domain.EntityExtend
{
    public class RegionTypeExtend : RegionType
    {
        public IEnumerable<RegionType> SubItems { get; set; }
    }
}
