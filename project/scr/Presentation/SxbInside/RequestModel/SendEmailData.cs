using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Sxb.Inside.RequestModel
{
    public class SendEmailData
    {
        public string[] ToAddr { get; set; }
        public string[] CC{ get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public List<IFormFile> Attachments { get; set; }
    }
}
