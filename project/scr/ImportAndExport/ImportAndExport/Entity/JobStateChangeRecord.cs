using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Entity
{
    public class JobStateChangeRecord
    {
        /// <summary>
        /// 1：点评，2：回答
        /// </summary>
        public int Type { get; set; }
        public Guid DataSourceId { get; set; }
    }
}
