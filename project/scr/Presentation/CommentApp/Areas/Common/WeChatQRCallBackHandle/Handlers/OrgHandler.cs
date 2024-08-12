using PMS.Infrastructure.Application.IService;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.Org;
using ProductManagement.API.Http.Request.Org;
using ProductManagement.API.Http.Result.Org;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.CourseDetailPageHandler;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using WeChat.Interface;
using Sxb.Web.Utils;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.DataJson;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class OrgHandler : ISubscribeCallBackHandler
    {

        IWeChatService _weChatService;
        IOrgServiceClient _orgServiceClient;
        IWebHostEnvironment _webHostEnvironment;

        public OrgHandler(IWeChatService weChatService, IOrgServiceClient orgServiceClient, IWebHostEnvironment webHostEnvironment)
        {
            _weChatService = weChatService;
            _orgServiceClient = orgServiceClient;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            if (qrequest.DataId == null)
            {
                return ResponseResult.Failed("机构ID不能为空");
            }
            CardListResult<OrgCard> orgCardListResult = await _orgServiceClient.OrgsLablesByIds(new OrgsLablesByIdsRequest()
            {
                LongIds = new List<string> { qrequest.DataId.Value.ToString() }
            });

            if (orgCardListResult != null && orgCardListResult.succeed && orgCardListResult.data?.Any() == true)
            {
                OrgCard orgCard = orgCardListResult.data.First();
                // await _weChatService.SendNews(frequest.OpenId
                //, $"查看{orgCard.orgName}详细信息"
                //, $"感谢关注上学帮！点击查看{orgCard.orgName}更多信息"
                //, "https://cdn.sxkid.com/images/logo_share_v4.png"
                //, qrequest.DataUrl);

                if (_webHostEnvironment.IsProduction())
                {
                    await SendMp(qrequest, frequest, orgCard);
                }
                else
                {
                    await SendMpTest(qrequest, frequest, orgCard);
                }

                return ResponseResult.Success("OK");
            }
            else
            {
                return ResponseResult.Failed("找不到对应的机构详情。");
            }

        }

        private async Task SendMpTest(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest, OrgCard orgCard)
        {
            var data = JsonConvertExtension.TryDeserializeObject(qrequest.DataJson, new OrgCourseDataJson());
            var pagePath = data.GetOrgMpPagePath(orgCard.id_s);

            await _weChatService.SendText(frequest.OpenId, "发送了网课通小程序机构页面");
            await _weChatService.SendText(frequest.OpenId, pagePath);
        }

        private async Task SendMp(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest, OrgCard orgCard)
        {
            var data = JsonConvertExtension.TryDeserializeObject(qrequest.DataJson, new OrgCourseDataJson());

            //上传二维码到微信的临时素材
            string mediaId = await _weChatService.AddTempMedia(MediaType.image, orgCard.logo, "tempqrcode.jpeg");
            var appid = ConfigHelper.GetWxMpOrgAppId();
            var pagePath = data.GetOrgMpPagePath(orgCard.id_s);

            var ret = await _weChatService.SendMiniProgramCard(frequest.OpenId,
                appid,
                pagePath,
                mediaId ?? "ih5M-3iKDsME3MNG3h5Q3lTqw8mNMydo8P4GqUx3t8Y",
                title: $"查看{orgCard.orgName}详细信息");
        }
    }
}
