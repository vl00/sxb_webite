using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ProductManagement.Tool.Amap.Common;
using ProductManagement.Tool.Amap.Model;
using ProductManagement.Tool.HttpRequest.Option;

namespace ProductManagement.Tool.Amap.Option
{
    public class DistrictOption : BaseOption
    {
        public DistrictOption(DistrictModel info, AmapConfig config)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "key", config.ApiKey },
                { "keywords",info.Keywords},
                { "subdistrict",info.Subdistrict.ToString()},
                { "page",info.Page.ToString()},
                { "offset",info.Offset.ToString()},
                { "extensions",info.Extensions},
                { "filter",info.Filter},
                { "callback",info.Callback},
                { "output",info.Output},
            };

            List<string> NotMust = new List<string> { "subdistrict", "page", "offset", "extensions",
                                        "filter","callback","output"};


            List<string> con = new List<string>();
            foreach (var para in parameters)
            {
                string value = HttpUtility.UrlEncode(para.Value, Encoding.UTF8);
                if ((value == "0" || string.IsNullOrWhiteSpace(value)) && NotMust.Contains(para.Key))
                    continue;
                AddValue(para.Key, new RequestValue(value));

                con.Add(para.Key + "=" + value);
            }
            Content = "?" + string.Join("&", con);
        }
        private string Content = "";

        public override string UrlPath => "/v3/config/district" + Content;
    }
}
