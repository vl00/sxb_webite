using ProductManagement.Tool.Amap.Common;
using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Tool.Amap.Option
{
    public class CurrentLocationOption : BaseOption
    {
        public CurrentLocationOption(string ip,AmapConfig config)
        {
            //IDictionary<string, string> parameter = new Dictionary<string, string>
            //{
            //    { "key",config.ApiKey }
            //};
            Content = "?key="+config.ApiKey+"&ip="+ip;
        }

        private string Content = "";
        public override string UrlPath => "/v3/ip" + Content;
    }
}
