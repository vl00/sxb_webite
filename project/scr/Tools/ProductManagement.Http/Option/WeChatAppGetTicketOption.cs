using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class WeChatAppGetTicketOption : BaseOption
    {
        protected string app;
        /// <summary>
        /// app的名称，上学帮内部定义，如果fwh表示上学帮服务号，cd表示上学帮成都订阅号
        /// </summary>
        /// <param name="app"></param>
        public WeChatAppGetTicketOption(string app = "fwh")
        {
            this.app = app;
        }
        public override string UrlPath => "/api/JSAPITicket/getticket?app=" + app;
    }
}
