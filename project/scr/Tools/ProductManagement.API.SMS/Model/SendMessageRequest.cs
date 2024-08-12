using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.SMS.Model
{
    class SendMessageRequest
    {
        public SendMessageRequest() { }
        public string signature_id { get; set; }
        public string mobile { get; set; }
        public string template_id { get; set; }
        public Dictionary<string, string> context { get; set; }
    }
}
