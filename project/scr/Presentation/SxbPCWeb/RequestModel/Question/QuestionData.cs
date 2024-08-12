using System;
using System.Collections.Generic;

namespace Sxb.Web.RequestModel.Question
{
    public class QuestionData
    {
        public string KeyWords { get; set; }
        public int CityCode { get; set; }
        public List<int> GradeIds { get; set; } = new List<int>();
        public List<int> TypeIds { get; set; } = new List<int>();
        public List<int> Lodging { get; set; } = new List<int>();
        public int Orderby { get; set; }
        public List<int> AreaCodes { get; set; } = new List<int>();
    }
}
