using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option.FinanceCenter
{
     

    public class WalletOptions : BaseOption
    {
        public override string UrlPath => "/api/Wallet/Operate";
    }
}
