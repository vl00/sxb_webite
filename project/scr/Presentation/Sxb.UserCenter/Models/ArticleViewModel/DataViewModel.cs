using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.ArticleViewModel
{
    public class DataViewModel
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string time { get; set; }
        public int layout { get; set; }
        public string[] covers { get; set; }
        public bool isShow { get; set; }
        public int viewCount { get; set; }
        public string digest { get; set; }
        public string content { get; set; }
    }
}
