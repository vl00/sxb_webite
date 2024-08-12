using System;
namespace PMS.Search.Domain.Entities
{
    public class SearchHistory
    {
        public string Word { get; set; }
        public string SearchWord { get; set; }
        public Guid? UserId { get; set; }
        public string UUID { get; set; }
        public int CityCode { get; set; }
        public string Application { get; set; }
        public int Channel { get; set; }
        public DateTime DateTime { get; set; }
    }
}
