using ProductManagement.API.Http.Model.FinanceCenter;
using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option.FinanceCenter
{
    public class AddPayOrderOption : BaseOption
    {

        public AddPayOrderOption()
        {
        }

        public override string UrlPath => "/api/PayOrder/Add";
    }
}
