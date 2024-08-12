using System;
using ProductManagement.Tool.HttpRequest;

namespace ProductManagement.API.Http.Common
{
    public class StaticInsideConfig : RequestConfig
    {
        public override int TimeOut { get; set; } = 120;
    }
}
