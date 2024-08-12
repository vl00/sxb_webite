using PMS.School.Domain.Entities;
using System.Collections.Generic;

namespace PMS.School.Domain.Dtos
{
    public class PCNavigationDto
    {
        public PCNavigationInfo Navigation { get; set; }
        public IEnumerable<PCNavigationItemInfo> Items { get; set; }
        public IEnumerable<PCNavigationRecommendInfo> Recommends { get; set; }
    }
}
