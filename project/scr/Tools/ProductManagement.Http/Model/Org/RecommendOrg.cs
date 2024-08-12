using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.Org
{

    public class RecommendOrg
    {
        public class HttpWrapper
        {
            public IEnumerable<RecommendOrg> Orgs { get; set; }
        }

        public Guid ID { get; set; }
        public string ID_S { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public bool Authentication { get; set; }
        public string Desc { get; set; }
        public string SubDesc { get; set; }
        public int CourceCount { get; set; }
        public int EvaluationCount { get; set; }
    }
}
