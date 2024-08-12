using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.UserManage.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace PMS.UserManage.Application.Common
{
    public class ApiService : IApiService
    {
        public IsHost _IsHost;

        public ApiService(IOptions<IsHost> IsHost) 
        {
            _IsHost = IsHost.Value;
        }

        public ArticleModel GetArticlesByIds(List<Guid> iDList)
        {
            return HttpHelper.HttpPost<ArticleModel>(
                $"{_IsHost.ConsoleHost_Operation}/api/ArticleApi/GetArticlesByIds",
                Newtonsoft.Json.JsonConvert.SerializeObject(iDList), "application/json");
        }

        public VoBase GetQuestionOrAnswer(List<object> requestObj, string cookieStr)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", cookieStr);
            return HttpHelper.HttpPost<VoBase>(
                $"{_IsHost.SiteHost_M}/Question/GetQuestionOrAnswer",
                Newtonsoft.Json.JsonConvert.SerializeObject(requestObj), contentType: "application/json", headers: headers);
        }
        public VoBase GetSchooCommentOrReply(List<object> requestObj, string cookieStr)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", cookieStr);
            return HttpHelper.HttpPost<VoBase>(
                $"{_IsHost.SiteHost_M}/SchoolComment/GetSchooCommentOrReply",
                Newtonsoft.Json.JsonConvert.SerializeObject(requestObj), contentType: "application/json", headers: headers);
        }
        public List<SchoolModel> GetCollectionExtAsync(List<Guid> iDList)
        {
            return HttpHelper.HttpPost<List<SchoolModel>>(
                $"{_IsHost.SiteHost_M}/School/GetCollectionExt", Newtonsoft.Json.JsonConvert.SerializeObject(iDList), "application/json");
        }
        public bool CheckIsLogOut(string cookieStr)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", cookieStr);
            return HttpHelper.HttpGet<bool>($"{_IsHost.SiteHost_M}/SchoolComment/GetSchooCommentOrReply", headers: headers);
        }
    }
}
