using ProductManagement.API.Http.Model;
using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ProductManagement.API.Http.Option
{
    public class HttpOption : BaseOption
    {
        private string content = "";

        public HttpOption(string url)
        {
            //AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            AddHeader("ContentType", "text/html;charset=utf-8");
            AddHeader("AcceptEncoding", "gzip, deflate, br");
            AddHeader("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Mobile Safari/537.36");
            
            content = url;
        }

        public override string UrlPath => content;
    }
}
