using ProductManagement.API.Http.Model;
using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductManagement.API.Http.Option
{
    public class HistoryAdd : BaseOption
    {
        private string content = "";

        public HistoryAdd(AddHistory addHistory)
        {
            //string cookies = string.Join(";",addHistory.cookies.Select((q, index) => { q+":"+q[index]}));
            string cookies = "";
            foreach (var item in addHistory.cookies)
            {
                cookies += $"{item.Key}="+Uri.EscapeDataString($"{item.Value}")+";";
            }
            AddHeader("cookie", cookies);
            content = $"/apihistory/addhistory?dataID={addHistory.dataID}&dataType={(int)addHistory.dataType}";
        }

        public override string UrlPath => content;
    }
}
