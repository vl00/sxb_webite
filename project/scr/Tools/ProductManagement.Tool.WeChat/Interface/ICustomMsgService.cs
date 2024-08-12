using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeChat.Model;

namespace WeChat.Interface
{
    public interface ICustomMsgService
    {
        Task<SendCustomMsgResponse> Send(string token, CustomMsg msg);
    }
}
