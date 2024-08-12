using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.Org
{
    public class OrgCard
    {
        public Guid id { get; set; }
        public long no { get; set; }
        public string id_s { get; set; }
        public string orgName { get; set; }
        public string logo { get; set; }
        public string desc { get; set; }
        public string subdesc { get; set; }
        public int evalCount { get; set; }
        public int courseCount { get; set; }
    }
}
