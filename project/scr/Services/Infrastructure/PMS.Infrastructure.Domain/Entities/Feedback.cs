using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Domain.Entities
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public byte type { get; set; }
        public string text { get; set; }
        public DateTime time { get; set; }
    }

    public class Feedback_Img
    {
        public Guid Id { get; set; }
        public Guid Feedback_Id { get; set; }
        public string url { get; set; }
    }
}
