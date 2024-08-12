using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class CircleExitRequestDto
    {
        public Guid UserId { get; set; }

        public Guid CircleId { get; set; }
    }
}
