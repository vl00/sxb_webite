using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.SMS.Model
{
    public class SmsTemplateListResult
    {
        public class Data
        {
            public int id { get; set; }
            public string title { get; set; }
            public string text { get; set; }
            public byte status { get; set; }
            public string reply { get; set; }
            public byte type { get; set; }
            public DateTime apply_time { get; set; }
        }
        public int result { get; set; }
        public string msg { get; set; }
        public int total { get; set; }
        public int count { get; set; }
        public IList<Data> data { get; set; }
    }
}
