using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Models
{
    public class MongoSetting
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public bool IsSSL { get; set; }
    }
}
