using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
   public  class CheckIsFollowRequestDto
    {
        public Guid CircleId { get; set; }

        public Guid UserId { get; set; }

    }

}
