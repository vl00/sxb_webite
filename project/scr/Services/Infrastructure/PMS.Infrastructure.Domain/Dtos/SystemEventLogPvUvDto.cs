using Microsoft.EntityFrameworkCore;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PMS.Infrastructure.Domain.Dtos
{
    public class SystemEventLogPvUvDto
    {
        public string PagePath { get; set; }
        public string Title { get; set; }

        public DateTime Date { get; set; }

        public int Pv { get; set; }

        public int Uv { get; set; }
    }

    public class SystemEventLogWeChatPvUvDto : SystemEventLogPvUvDto
    {
        const string _noPathKey = "no";
        const string _eidPathKey = "eid";
        const string _typePathKey = "type";

        private Guid? extId;
        public Guid? ExtId { get => extId ?? GetExtId(); set => extId = value; }
        public long SchoolShortNo {
            get
            {
                try
                {
                    //https://m.sxkid.com/school/wechat/hpegfrom=groupmessage
                    return UrlShortIdUtil.Base322Long(Query.Get(_noPathKey));
                }
                catch (Exception)
                {}
                return 0;
            }
        }

        public string SchoolName { get; set; }

        /// <summary>
        /// <see cref="iSchool.SchFType0"/>
        /// <see cref="SchTypeUtils"/>
        /// </summary>
        public string SchFType0Desc { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }

        /// <summary>
        /// 锚点名称
        /// </summary>
        public string AnchorName => GetAnchorName();

        /// <summary>
        /// 点击品牌/好物标题关注或进入服务号人数
        /// </summary>
        public int WeixinPv { get; set; }

        private NameValueCollection _query;
        private NameValueCollection Query => GetPathQuery();


        private Guid? GetExtId()
        {
            if (Guid.TryParse(Query.Get(_eidPathKey), out Guid id))
            {
                return id;
            }
            return null;
        }

        private string GetAnchorName()
        {
            var typeStr = Query.Get(_typePathKey);
            if (int.TryParse(typeStr, out int type)
                && type < AnchorNames.Length)
            {
                return AnchorNames[type];
            }
            return string.Empty;
        }

        /// <summary>
        /// 1 学校概况 2 招生信息 3 小升初安排 4 指标分配 5 分数线 6 升学成绩 7 学校图册 8 相关推荐
        /// </summary>
        public static string[] AnchorNames = new string[] {
            "",
            "学校概况", "招生信息", "小升初安排", "指标分配", "分数线",
            "升学成绩", "学校图册", "相关推荐" };

        private NameValueCollection GetPathQuery()
        {
            if (_query != null)
            {
                return _query;
            }
            if (string.IsNullOrWhiteSpace(PagePath))
            {
                return new NameValueCollection();
            }
            var pathPath = PagePath.ToLower();

            //pathPath = "https://m.sxkid.com/school/wechat/hpeg?from=groupmessage";
            //pathPath = "https://m.sxkid.com/school/wechat/detail/vnzfa_type=1";
            //https://m.sxkid.com/school/wechat/vnzfa
            //https://m.sxkid.com/school/wechat/detail/vnzfa_type=1

            //https://m3.sxkid.com/school_detail_wechat/eid=092D223A-5D02-48FF-AE04-FD1C9683EB5D
            //https://m3.sxkid.com/school_detail_wechat/data/eid=092D223A-5D02-48FF-AE04-FD1C9683EB5D_type=1

            string keyStr;
            //新版链接
            if (pathPath.IndexOf("/school/wechat/detail/") > 0)
            {
                keyStr = "/school/wechat/detail/";
                _query = GetPathQuery(pathPath, keyStr, isNew: true);
            }
            else if (pathPath.IndexOf("/school/wechat/") > 0)
            {
                keyStr = "/school/wechat/";
                _query = GetPathQuery(pathPath, keyStr, isNew: true);
            }
            //旧版链接
            else if (pathPath.IndexOf("/school_detail_wechat/data/") > 0)
            {
                keyStr = "/school_detail_wechat/data/";
                _query = GetPathQuery(pathPath, keyStr, isNew: false);
            }
            else
            {
                keyStr = "/school_detail_wechat/";
                _query = GetPathQuery(pathPath, keyStr, isNew: false);
            }

            return _query;
        }

        private static NameValueCollection GetPathQuery(string pagePath, string keyStr, bool isNew)
        {
            var path = pagePath.ToLower();
            var index = path.IndexOf(keyStr);

            if (index == -1)
            {
                return new NameValueCollection();
            }

            index += keyStr.Length;
            var queryStr = path.Substring(index);

            if (isNew) queryStr = $"{_noPathKey}={queryStr}".Replace("?", "&");
            var query = HttpUtility.ParseQueryString(queryStr.Replace('_', '&'));

            return query;
        }
    }


    public class SystemEventLogBigKPvUvDto : SystemEventLogPvUvDto
    {
        /// <summary>
        /// 关注服务号人数（含取关再关）
        /// </summary>
        public int WeixinSubscribeUv { get; set; }

        /// <summary>
        /// 新关注服务号的人数
        /// </summary>
        public int WeixinNewSubscribeUv { get; set; }

        /// <summary>
        /// 关注或进入服务号次数
        /// </summary>
        public int WeixinScanPv { get; set; }


    }
}
