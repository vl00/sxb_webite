using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class CircleCreateReturnDto
    {
        public Guid CircleId { get; set; }


        public static implicit operator CircleCreateReturnDto(Circle circle)
        {
            return new CircleCreateReturnDto()
            {
                CircleId = circle.Id
            };
        }
    }
}
