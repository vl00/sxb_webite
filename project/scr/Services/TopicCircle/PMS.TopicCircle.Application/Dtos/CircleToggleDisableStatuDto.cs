using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class CircleToggleDisableStatuDto
    {
        public Guid CircleId { get; set; }

        public  bool IsDisable { get; set; }
    }
}
