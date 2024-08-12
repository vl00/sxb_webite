using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models.ServerStorage
{
    public class SetRequest
    {

        public string Key { get; set; }

        public dynamic Value { get; set; }

        public int ExpireHours { get; set; } = 6;

    }
}
