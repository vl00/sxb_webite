using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.Org
{
    public class CourseCard
    {
        public Guid id { get; set; }
        public string id_s { get; set; }
        public string title { get; set; }
        public decimal price { get; set; }
        public string coverUrl { get; set; }

    }
}
