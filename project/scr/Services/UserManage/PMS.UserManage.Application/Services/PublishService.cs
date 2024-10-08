﻿using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.Services
{
    public class PublishService : IPublishService
    {
        public IsHost _IsHost;
        public PublishService(IOptions<IsHost> IsHost)
        {
            _IsHost = IsHost.Value;
        }

        public VoBase GetCommentList(string cookieStr, int page)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", cookieStr);
            var ret = HttpHelper.HttpGet<VoBase>($"{_IsHost.SiteHost_M}/SchoolComment/CurrentPublishComment?PageIndex={page}&PageSize=10"
                , headers: headers);
            return ret;
        }
        public VoBase GetQAList(string cookieStr, int page)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", cookieStr);
            var ret = HttpHelper.HttpGet<VoBase>($"{_IsHost.SiteHost_M}/Question/CurrentPublishQuestion?PageIndex={page}&PageSize=10"
                , headers: headers);
            return ret;
        }

    }
}
