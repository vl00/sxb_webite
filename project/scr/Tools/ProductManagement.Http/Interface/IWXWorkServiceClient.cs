using ProductManagement.API.Http.Request.WXWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Interface
{
    public interface IWXWorkServiceClient
    {
        Task<string> GetAddCustomerQrCode(GetAddCustomerQrCodeRequest request);


    }
}
