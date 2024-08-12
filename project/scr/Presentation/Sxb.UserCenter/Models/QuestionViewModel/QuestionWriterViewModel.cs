using System;

namespace Sxb.UserCenter.Models.QuestionViewModel
{
    public class QuestionWriterViewModel
    {
        public string Content { get; set; }
        public bool IsAnony { get; set; }
        public Guid Eid { get; set; }
    }
}
