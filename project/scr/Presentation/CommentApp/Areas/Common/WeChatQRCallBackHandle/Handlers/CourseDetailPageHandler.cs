using Newtonsoft.Json;
using PMS.Infrastructure.Application.IService;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.Org;
using ProductManagement.API.Http.Request.Org;
using ProductManagement.API.Http.Result.Org;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Web;
using WeChat.Interface;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.DataJson;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class CourseDetailPageHandler : ISubscribeCallBackHandler
    {
        IWeChatService _weChatService;
        IOrgServiceClient _orgServiceClient;
        IWebHostEnvironment _webHostEnvironment;
        public CourseDetailPageHandler(IWeChatService weChatService, IOrgServiceClient orgServiceClient, IWebHostEnvironment webHostEnvironment)
        {
            _weChatService = weChatService;
            _orgServiceClient = orgServiceClient;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            if (qrequest.DataId == null)
            {
                return ResponseResult.Failed("课程ID不可为空。");
            }

            CardListResult<CourseCard> cardListResult = await _orgServiceClient.CoursesLablesByIds(new CoursesLablesByIdsRequest()
            {
                LongIds = new List<string> { qrequest.DataId.Value.ToString() }
            });

            if (cardListResult != null && cardListResult.succeed && cardListResult.data?.Any() == true)
            {
                CourseCard courseCard = cardListResult.data.First();
                // await _weChatService.SendNews(frequest.OpenId
                //, "点击查看以下课程详情"
                //, $"{courseCard.title}"
                //, "https://cdn.sxkid.com/images/logo_share_v4.png"
                //, qrequest.DataUrl);

                if (_webHostEnvironment.IsProduction())
                {
                    await SendMp(qrequest, frequest, courseCard);
                }
                else
                {
                    await SendMpTest(qrequest, frequest, courseCard);
                }

                return ResponseResult.Success("OK");
            }
            else
            {
                return ResponseResult.Failed("找不到该课程。");
            }


        }

        private async Task SendMpTest(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest, CourseCard courseCard)
        {
            var data = JsonConvertExtension.TryDeserializeObject(qrequest.DataJson, new OrgCourseDataJson());
            var pagePath = data.GetCourseMpPagePath(courseCard.id_s);

            await _weChatService.SendText(frequest.OpenId, "发送了网课通小程序课程页面");
            await _weChatService.SendText(frequest.OpenId, pagePath);
        }

        private async Task SendMp(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest, CourseCard courseCard)
        {
            var data = JsonConvertExtension.TryDeserializeObject(qrequest.DataJson, new OrgCourseDataJson());

            //上传二维码到微信的临时素材
            string mediaId = await _weChatService.AddTempMedia(MediaType.image, courseCard.coverUrl, "tempqrcode.jpeg");
            var appid = ConfigHelper.GetWxMpOrgAppId();
            var pagePath = data.GetCourseMpPagePath(courseCard.id_s);

            var ret = await _weChatService.SendMiniProgramCard(frequest.OpenId,
                appid,
                pagePath,
                mediaId ?? "ih5M-3iKDsME3MNG3h5Q3lTqw8mNMydo8P4GqUx3t8Y",
                title: "点击查看以下课程详情");
        }
    }
}
