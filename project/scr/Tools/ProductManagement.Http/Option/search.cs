using ProductManagement.API.Http.Result;
using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class search : BaseOption
    {
        private string Content = "";
        public search(GetSchoolByNameAndAdCode para)
        {
            //Content = $"?name={para.name}&curpage={para.curpage}&countperpage={para.countperpage}";
            Content = $"?name={para.name}&citycode={para.citycode}&curpage={para.curpage}&countperpage={para.countperpage}";
        }
        public override string UrlPath => "/search" + Content;
    }
}
