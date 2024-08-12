using System;
using System.Collections.Generic;
using System.Text;

namespace WeChat.Model
{
   public  class SendCustomMsgResponse: StatusResponse
    {
        public bool Success => this.errcode == 0;
    }
}
