using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public  class CircleEditRequestDto
    {
        public Guid CircleId { get; set; }

        public string Name { get; set; }

        public string CoverUrl { get; set; }

        public string Intro { get; set; }

        public string BGColor { get; set; }
    }
}
