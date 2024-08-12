using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Result;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Interface
{
    public interface IHtmlServiceClient
    {
        Task<Image> GetImage(string url);
        Task<string> GetTitle(string url);
    }
}
