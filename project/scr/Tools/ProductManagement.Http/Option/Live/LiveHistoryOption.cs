using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class LiveHistoryOption : BaseOption
    {
        public LiveHistoryOption(Dictionary<string, string> cookie, int page)
        {
            string cookies = "";
            foreach (var item in cookie)
            {
                cookies += $"{item.Key}=" + Uri.EscapeDataString($"{item.Value}") + ";";
            }

            AddHeader("cookie", cookies);
            param = "?CurrentPage=" + page;
        }
        private string param = "";
        public override string UrlPath => "/api/Consumer/MyHistory";
    }
}
