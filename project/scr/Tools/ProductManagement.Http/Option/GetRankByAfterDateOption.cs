using System;
using ProductManagement.Tool.HttpRequest.Option;

namespace ProductManagement.API.Http.Option
{
    public class GetRankByAfterDateOption : BaseOption
    {
        public GetRankByAfterDateOption(DateTime afterDate, int pageNo=1, int pageSize=20)
        {
            AddValue("afterDate", new RequestValue(afterDate.ToString("yyyy-MM-dd HH:mm:ss")));
            AddValue("offset", new RequestValue((pageNo-1)* pageSize));
            AddValue("limit", new RequestValue(pageSize));
        }

        public override string UrlPath => "/api/RankApi/GetRankByAfterDate";
    }
}
