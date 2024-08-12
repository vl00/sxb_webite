using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class LectorLecturesOption : BaseOption
    {
        public LectorLecturesOption(Guid userId,int? Status,int Page = 1)
        {
            //AddValue("city", new RequestValue(city));
            param = "?ContentId=" + userId + "&CurrentPage="+ Page;
            if (Status !=null)
                param += "&Status=" + Status; ;
        }
        private string param = "";
        public override string UrlPath => "/api/Lector/MyLectures" + param;
    }
}
