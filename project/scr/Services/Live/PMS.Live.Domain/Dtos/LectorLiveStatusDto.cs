using PMS.Live.Domain.Enums;
using System;

namespace PMS.Live.Domain.Dtos
{
    public class LectorLiveStatusDto
    {
        public Guid LectureID { get; set; }
        public Guid LectorID { get; set; }
        public LectureStatus Status { get; set; }
        public Guid UserID { get; set; }
    }
}
