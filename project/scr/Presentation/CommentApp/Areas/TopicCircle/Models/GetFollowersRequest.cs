using PMS.TopicCircle.Application.Dtos;
using Sxb.Web.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{



    public class GetFollowersRequest : PageRequest
    {

        public SortEnum SortType { get; set; }

        public string SearchContent { get; set; }


    }

}
