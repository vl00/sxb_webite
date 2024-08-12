using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.ModelVo
{
    public class TestVo
    {
        public Guid Id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public DateTime AddTime { get; set; }
    }
}
