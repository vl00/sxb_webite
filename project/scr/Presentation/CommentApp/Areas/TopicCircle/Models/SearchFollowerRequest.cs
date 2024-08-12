using Sxb.Web.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class SearchFollowerRequest:PageRequest
    {

        public string SearchContent { get; set; }

    }

}
