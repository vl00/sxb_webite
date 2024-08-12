using ProductManagement.API.Http.Model;
using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class ShortUrlOption : BaseOption
    {

        // https://localhost:5001/LinkTransfer/ToShortUrl?originUrl=http://docker.sxkid.com:8080/
        public ShortUrlOption(string originUrl)
        {
            //content = $"/LinkTransfer/ToShortUrl?originUrl={originUrl}";
            PostBody = new { OriginUrl = originUrl };
            AddHeader("contenttype", "application/json");
        }

        public override string UrlPath => "/Api/ToolsApi/ToShortUrl";
    }
}
