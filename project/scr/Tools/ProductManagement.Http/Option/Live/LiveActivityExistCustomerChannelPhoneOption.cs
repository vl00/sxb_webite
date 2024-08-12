using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option.Live
{
    public class LiveActivityExistCustomerChannelPhoneOption : BaseOption
    {
        public LiveActivityExistCustomerChannelPhoneOption(int customer, string phone)
        {
            param = $"?customer={customer}&phone={phone}";
        }
        private string param = "";
        public override string UrlPath => "/api/Consumer/ExistCustomerChannelPhone" + param;
    }
}
