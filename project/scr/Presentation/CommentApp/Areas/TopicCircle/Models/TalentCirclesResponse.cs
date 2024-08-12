using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Entities;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class TalentCirclesResponse 
    {
        public IEnumerable<CircleItemDto> circles { get; set; }


    }
}
