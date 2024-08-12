using ProductManagement.API.Http.Model;
using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ProductManagement.API.Http.Option
{
    public class Message : BaseOption
    {
        private string content = "";

        public Message(AddMessage Message)
        {
            AddHeader("cookie", $"iSchoolAuth={Message.iSchoolAuth}");
            //var title = HttpUtility.UrlEncode(Message.title, Encoding.UTF8);
            //var text = HttpUtility.UrlEncode(Message.content, Encoding.UTF8);

            AddValue("userID", new RequestValue(Message.userId));
            AddValue("type", new RequestValue((int)Message.type));
            AddValue("title", new RequestValue(Message.title));
            AddValue("content", new RequestValue(Message.content));
            AddValue("dataID", new RequestValue(Message.dataID));
            AddValue("dataType", new RequestValue((int)Message.dataType));
            AddValue("eID", new RequestValue(Message.eID)); 
            AddValue("IsAnony", new RequestValue(Message.IsAnony));

            content = $"/apimessage/add";
        }

        public override string UrlPath => content;
    }
}
