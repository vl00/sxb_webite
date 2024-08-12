using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class StickLecturesOption : BaseOption
    {
        public StickLecturesOption(int city)
        {
            //AddValue("city", new RequestValue(city));
            if(city>0)
                param = "?city=" + city; ;
        }
        private string param = "";
        public override string UrlPath => "/api/Home/StickLectures"+ param;
    }
}
