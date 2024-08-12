using System;
using System.Threading.Tasks;
using ProductManagement.UserCenter.BaiduCommon.Model;

namespace ProductManagement.UserCenter.BaiduCommon
{
    public interface IBaiduOAuthClient
    {
        Task<BDAccessKeyResult> GetBaiduAccessKey(string code);
    }
}
