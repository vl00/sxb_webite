using System;
using ProductManagement.Tool.HttpRequest.Option;

namespace ProductManagement.API.Http.Option
{
    public class StatisticOption : BaseOption
    {
        public StatisticOption(string JsonString)
        {
            AddValue("Json", new RequestValue(JsonString));
        }

        public override string UrlPath => "/bgstatistic";
    }
}
