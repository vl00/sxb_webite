using Dapper.Contrib.Extensions;
using System;

namespace PMS.School.Domain.Entities.SEO_International
{
    [Table("SEO_InternationalPage")]
    public class InternationalPage
    {
        [ExplicitKey]
        public Guid PageID { get; set; }
        public string SelectedReading { get; set; }
        public int ID { get; set; }
        public string KeyWords { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
    }
}
