using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.SMS.Model
{
    public class SmsTemplateResult
    {
        public class Data
        {
            public int id { get; set; }
            public string text { get; set; }
            public byte status { get; set; }
            public byte type { get; set; }
        }
        public int result { get; set; }
        public string msg { get; set; }
        public Data data { get; set; }
    }
}
