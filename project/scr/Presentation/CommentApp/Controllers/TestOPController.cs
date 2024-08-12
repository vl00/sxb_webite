using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeChat.Interface;
namespace Sxb.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using NSwag.Annotations;
    using PMS.OperationPlateform.Application.IServices;
    using PMS.School.Application.IServices;
    using PMS.School.Infrastructure.Common;
    using ProductManagement.API.Http.Interface;
    using ProductManagement.API.Http.Model;
    using Sxb.Web.Filters;
    using Sxb.Web.RequestModel;
    using Sxb.Web.Response;
    using System.Text;
    using WeChat.Model;

    public class TestSafeActionRequest : SafeActionRequest
    {

        public string DataId { get; set; }
        public override string LockKey(HttpContext context)
        {
         
            return $"TestSafeAction:{context.GetUserInfo()?.UserId}:{DataId}";
        }

        public override string LockValue(HttpContext context)
        {
            return $"TestSafeAction:{context.GetUserInfo()?.UserId}:{DataId}";
        }
    }


    [OpenApiIgnore]
    public class TestOPController : BaseController
    {
        ISchoolRankService schoolRankService;
        IUserServiceClient client;
        IArticleService _articleService;
        ISchService _schService;
        IWeChatQRCodeService _wechatQRCodeService;
        IWeChatAppClient _weChatAppClient;
        public TestOPController(ISchoolRankService schoolRankService, IUserServiceClient client, IArticleService articleService, ISchService schService, IWeChatQRCodeService wechatQRCodeService, IWeChatAppClient weChatAppClient)
        {
            _schService = schService;
            _articleService = articleService;
            this.schoolRankService = schoolRankService;
            this.client = client;
            _wechatQRCodeService = wechatQRCodeService;
            _weChatAppClient = weChatAppClient;
        }

        public IActionResult UAPage()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        [SafeAction( SafeActionRequestParamName = "request")]
        public async Task<IActionResult> TestSafeAction(TestSafeActionRequest request)
        {
            await  Task.Delay(3000);
            return Content($"{request.DataId}");
        }

        public IActionResult Index(string backurl)
        {
            var lst = this.schoolRankService.GetAll();
            return Json(lst);
        }

        public IActionResult TestGetH5SchoolRankInfo(Guid[] schoolIds)
        {
            var lst = this.schoolRankService.GetH5SchoolRankInfoBy(schoolIds);
            return Json(lst);
        }

        public async Task<IActionResult> TestGetMinAppQRCode(string page)
        {
            var accessToken =  await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest()
            {
                App = "app"
            });
            var result = await _wechatQRCodeService.GetWXAcodeUnlimit(accessToken.token, new GetWXAcodeUnlimitRequest()
            {
                Scene = "11",
                Page = page,
            }) ;
            if (result.ErrCode == 0)
            {
                return File(result.Buffer, result.ContentType);

            }
            else {
                return Content(result.ErrMsg);
            }

        }



        public IActionResult ConvertFile()
        {
            var jsonFile = Request.Form.Files.First();

            string jsonString = null;
            using (var fileStream = jsonFile.OpenReadStream())
            {
                var dataBytes = new byte[jsonFile.Length];
                fileStream.Read(dataBytes, 0, dataBytes.Length);
                jsonString = Encoding.UTF8.GetString(dataBytes);
            }
            var dataObject = JsonHelper.JSONToObject<Dictionary<int, testObject>>(jsonString);
            var sb = new StringBuilder();
            var ids = dataObject.Values.Where(p => p.real_url.Length > 36).Select(p => p.real_url.Substring(p.real_url.ToLower().IndexOf("/detail/") + 8).Split('?').FirstOrDefault()).ToList();
            var guids = new List<Guid>();
            foreach (var item in ids)
            {
                if (Guid.TryParse(item, out Guid item1))
                {
                    guids.Add(item1);
                }
            }

            var schools = _schService.GetSchoolextNo(guids.ToArray());
            foreach (var item in schools)
            {
                sb.AppendFormat("https://www.sxkid.com/school/detail/{0} https://www.sxkid.com/school-{1}{2}", item.Item1, ProductManagement.Framework.Foundation.UrlShortIdUtil.Long2Base32(item.Item2), Environment.NewLine);
            }
            //var articles = _articleService.GetArticles(guids);
            //foreach (var item in articles)
            //{
            //    sb.AppendFormat("https://www.sxkid.com/article/detail/{0} https://www.sxkid.com/article/{1}.html{2}", item.id, ProductManagement.Framework.Foundation.UrlShortIdUtil.Long2Base32(item.No), Environment.NewLine);
            //}
            return Content(sb.ToString());
        }

        class testObject
        {
            public int Index { get; set; }
            public string baidu_url { get; set; }
            public string real_url { get; set; }
        }
    }
}