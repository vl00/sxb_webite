using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Domain.Entities
{
    public class Local_V2
    {
        public int id { get; set; }
        public string name { get; set; }
        public int parent { get; set; }
    }
}
