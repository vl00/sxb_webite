using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.ModelDto.ModelVo
{
    public class ArticleModel
    {
        public int errCode { get; set; }
        public string msg { get; set; }
        public List<Data> data { get; set; }
    }

    public class Data
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public DateTime time { get; set; }
        public int layout { get; set; }
        public string[] covers { get; set; }
        public bool isShow { get; set; }
        public int viewCount { get; set; }
        public string digest { get; set; }
    }
}
