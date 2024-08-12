using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace PMS.Search.Domain.Common
{
    public class ChannelUrlHelper
    {
        public string SiteDomain { get; set; }
        public string UserDomain { get; set; }

        public ChannelUrlHelper(string domain, string userDomain)
        {
            SiteDomain = domain;
            UserDomain = userDomain;
        }

        public void CompleteUrl<T>(ref List<T> data, ChannelIndex channel, string keyPropName = "Id", string urlPropName = "ActionUrl")
            where T : class
        {
            if (data == null || !data.Any())
            {
                return;
            }

            //var type = typeof(T);
            var type = data.First().GetType();//解决转为object拿不到
            var keyProp = type.GetProperty(keyPropName);
            var urlProp = type.GetProperty(urlPropName);
            if (keyProp == null || !keyProp.CanRead)
            {
                return;
            }
            if (urlProp == null || !urlProp.CanWrite)
            {
                return;
            }

            var templateUrl = channel.GetActionUrlValue().ToWebUrl(SiteDomain);
            var templateUrlUser = channel.GetActionUrlValue().ToWebUrl(UserDomain);
            foreach (var item in data)
            {
                var key = keyProp.GetValue(item);
                if (key != null)
                {
                    //学校,点评,问题,文章 短链接
                    if (channel == ChannelIndex.School || channel == ChannelIndex.Comment || channel == ChannelIndex.Question || channel == ChannelIndex.Article)
                    {
                        if (keyProp.PropertyType == typeof(int))
                        {
                            key = UrlShortIdUtil.Long2Base32((int)key).ToLower();
                        }
                        else if (keyProp.PropertyType == typeof(long))
                        {
                            key = UrlShortIdUtil.Long2Base32((long)key).ToLower();
                        }
                    }
                    var template = channel == ChannelIndex.Talent ? templateUrlUser : templateUrl;
                    var url = string.Format(template, key);
                    urlProp.SetValue(item, url);
                }
            }
        }
    }
}
