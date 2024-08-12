using System;
using System.Collections.Generic;

namespace Sxb.Web.RequestModel.Comment
{
    public class CommentData
    {
        public string KeyWords { get; set; }
        public int CityCode{get;set;}
        public List<int> GradeIds{get;set; } = new List<int>();
        public List<int> TypeIds{get;set; } = new List<int>();
       // public List<int> Lodging{get;set; } = new List<int>();
       //定义可null值的bool type
        public bool? Lodging { get; set; }

        public int Orderby {get;set;}
        public List<int> AreaCodes { get; set; } = new List<int>();
    }
}
