using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class QueryLecturesOption : BaseOption
    {
        public QueryLecturesOption(List<Guid> ids,Dictionary<string, string> cookie)
        {
            string cookies = "";
            if (cookie != null)
            {
                foreach (var item in cookie)
                {
                    cookies += $"{item.Key}=" + Uri.EscapeDataString($"{item.Value}") + ";";
                }
            }
            AddHeader("contenttype", "application/json");
            AddHeader("cookie", cookies);
            AddValue("Ids", new RequestValue(ids));
        }

        public override string UrlPath => "/api/Home/QueryLectures";
    }
}
