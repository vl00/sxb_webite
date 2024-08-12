using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WxworkInviteAvitityController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IOAuthWeixinService _authWeixinService;
        public WxworkInviteAvitityController(HttpClient httpClient, IOAuthWeixinService authWeixinService)
        {
            _authWeixinService = authWeixinService;
            _httpClient = httpClient;
        }

        [Authorize]
        [HttpGet]
        public async Task<ResponseResult> GetInvitePointData()
        {
            var userId = User.Identity.GetId();
            var unionId = _authWeixinService.GetBindWxUnionId(userId);
            //unionId = "oBY8iuPi6ZYB6o3lLQT8q2HrzXEQ";
            string url = "/api/ShopActivity/GetInvitePointData?unionId="+ unionId;

            var resp = await _httpClient.GetAsync(url); 

            if(resp.StatusCode == HttpStatusCode.OK)
            {
                var resultString = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseResult<InviteStatisticalViewModel>>(resultString);
                if (result.Succeed)
                {
                    return ResponseResult.Success(result.Data);
                }
            }
            return ResponseResult.Failed("获取信息失败，请稍后再试");
        }


        [Authorize]
        [HttpGet]
        public async Task<ResponseResult> ListInvitePointData(int pageNo = 1,int pageSize = 10)
        {
            var userId = User.Identity.GetId();
            var unionId = _authWeixinService.GetBindWxUnionId(userId);
            //unionId = "oBY8iuPi6ZYB6o3lLQT8q2HrzXEQ";
            string url = "/api/ShopActivity/GetInvitePointData2?unionId=" + unionId + "&pageNo="+pageNo+ "&pageSize=" + pageSize;

            var resp = await _httpClient.GetAsync(url);

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var resultString = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseResult<InviteStatisticalViewModel>>(resultString);
                if (result.Succeed)
                {
                    return ResponseResult.Success(result.Data);
                }
            }
            return ResponseResult.Failed("获取信息失败，请稍后再试");
        }



        class InviteStatisticalViewModel
        {

            public string UnionId { get; set; }
            /// <summary>
            /// 总数
            /// </summary>
            public int Total { get; set; }
            /// <summary>
            /// 总数成员
            /// </summary>
            public string TotalUser { get; set; }
            /// <summary>
            /// 有效好友数
            /// </summary>
            public int ValidTotal { get; set; }
            /// <summary>
            /// 有效好友
            /// </summary>
            public string ValidUser { get; set; }
            /// <summary>
            /// 非有效好友数
            /// </summary>
            public int UnvalidTotal { get; set; }
            /// <summary>
            /// 非加群好友数
            /// </summary>
            public int NotJoinTotal { get; set; }
            /// <summary>
            /// 非加群好友
            /// </summary>
            public string NotJoinUser { get; set; }
            /// <summary>
            /// 非女性好友数
            /// </summary>
            public int NotladyTotal { get; set; }
            /// <summary>
            /// 非女性好友
            /// </summary>
            public string NotladyUser { get; set; }
            /// <summary>
            /// 之前已经加过群好友数
            /// </summary>
            public int BeforeJoinTotal { get; set; }
            /// <summary>
            /// 之前已经加过群好友
            /// </summary>
            public string BeforeJoinUser { get; set; }

        }
    }
}
