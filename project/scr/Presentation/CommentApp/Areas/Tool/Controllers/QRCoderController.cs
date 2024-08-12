using Microsoft.AspNetCore.Mvc;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Tool
{
    using ProductManagement.API.Http.Interface;
    using ProductManagement.API.Http.Model;
    using ProductManagement.Tool.QRCoder;
    using Sxb.Web.Areas.Tool.Models.QRCoder;
    using System.Drawing.Imaging;
    using System.IO;
    using WeChat.Interface;
    using WeChat.Model;

    [Route("Tool/[controller]/[action]")]
    public class QRCoderController : ControllerBase
    {
        IWeChatQRCodeService _weChatQRCodeService;
        IWeChatAppClient _weChatAppClient;

        public QRCoderController(IWeChatQRCodeService weChatQRCodeService, IWeChatAppClient weChatAppClient)
        {
            _weChatQRCodeService = weChatQRCodeService;
            _weChatAppClient = weChatAppClient;
        }

        [HttpGet]
        [Description("生成上学帮二维码")]
        public IActionResult Generate(string url)
        {
            LinkCoder coder = new LinkCoder();
            var bitmap = coder.Create(url, 20);
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), "image/png");
            }
        }

        [HttpGet]
        [Description("生成小程序码")]
        public async Task<IActionResult>  GetMinAppQRCode([FromQuery]GetMinAppQRCodeRequest request)
        {
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest()
            {
                App = request.AppName
            });
            var result = await _weChatQRCodeService.GetWXAcodeUnlimit(accessToken.token, new GetWXAcodeUnlimitRequest()
            {
                Scene = request.Scene,
                Page = request.Page,
                 Width = request.Width
            });
            if (result.ErrCode == 0)
            {
                return File(result.Buffer, result.ContentType);

            }
            else
            {
                return Content(result.ErrMsg);
            }

        }
    }
}
