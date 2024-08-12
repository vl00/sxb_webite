using Newtonsoft.Json;
using ProductManagement.API.Http.Model.Org;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.DataJson
{
    public class OrgCourseDataJson
    {
        public string pageUrl { get; set; }
        public string eid { get; set; }
        public string fw { get; set; }

        public static OrgCourseDataJson TryGet<OrgCourseDataJson>(string dataJson)
        {
            try
            {
                return JsonConvert.DeserializeObject<OrgCourseDataJson>(dataJson);
            }
            catch (Exception)
            {
            }
            return default;
        }

        public string GetCourseMpPagePath(string shortId)
        {
            var encodePageUrl = HttpUtility.UrlEncode(pageUrl);
            var pagePath = $"pagesA/pages/course_detail/course_detail?id={shortId}&fw={fw}&surl={encodePageUrl}&eid={eid}";
            return pagePath;
        }

        public string GetOrgMpPagePath(string shortId)
        {
            var encodePageUrl = HttpUtility.UrlEncode(pageUrl);
            var pagePath = $"pagesA/pages/org_detail/org_detail?orgnoid={shortId}&fw={fw}&surl={encodePageUrl}&eid={eid}";
            return pagePath;
        }
    }
}
