using PMS.Live.Domain.Enums;
using System;

namespace PMS.Live.Domain.Dtos
{
    public class LectureStatusDto
    {
        public Guid ID { get; set; }
        public LectureStatus Status { get; set; }
    }
}
