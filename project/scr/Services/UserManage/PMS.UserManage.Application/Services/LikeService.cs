using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.Services
{
    public class LikeService : ILikeService
    {
        public IsHost _IsHost { get; set; }
        public LikeService(IOptions<IsHost> IsHost) 
        {
            _IsHost = IsHost.Value;
        }

        public VoBase GetCommentLike(string cookieStr, int page = 1)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", cookieStr);
            var ret = HttpHelper.HttpGet<VoBase>(
                $"{_IsHost.SiteHost_M}/SchoolComment/GetLikeCommentAndReplies?pageIndex={page}&pageSize=10"
                , headers: headers);
            return ret;
        }
        public VoBase GetQALike(string cookieStr, int page = 1)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", cookieStr);
            var ret = HttpHelper.HttpGet<VoBase>(
                $"{_IsHost.SiteHost_M}/Question/GetLikeAnswerAndAnswerReply?pageIndex={page}&pageSize=10"
                , headers: headers);
            return ret;
        }
    }
}
