using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class MarkDeviceToken : BaseOption
    {
        private string content = "";

        public MarkDeviceToken(string userAgent, Dictionary<string,string> cookies)
        {
            IEnumerable<string> cks = new List<string>();
            if (cookies != null & cookies.Count > 0)
            {
                cks = cookies.Select(k_v => $"{k_v.Key}={System.Web.HttpUtility.UrlEncode(k_v.Value)}");
            }

            AddHeader("cookie", string.Join(";", cks));
            AddHeader("user-agent", userAgent);
            content = "/home/index";
        }

        public override string UrlPath => content;
    }
}
