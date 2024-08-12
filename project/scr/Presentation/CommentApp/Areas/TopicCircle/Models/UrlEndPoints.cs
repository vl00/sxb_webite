using Nest;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class UrlEndPoints
    {
        /// <summary>
        /// 是否是可被解析的上学帮内链
        /// </summary>
        public bool IsSxkidUrl { get; set; }

        /// <summary>
        /// url信息
        /// </summary>
        public string Url { get; set; }
        public string ControllerName { get; set; }
        public string ActionHtmlName { get; set; }
        public string ActionName { get; set; }
        public string Query { get; set; }
        public NameValueCollection NameValueCollection { get; set; }

        /// <summary>
        /// 解析结果
        /// </summary>
        public long No { get; set; }
        public Guid? Id { get; set; }
        public TopicType Type { get; set; }
        public string Content { get; set; }

        public static UrlEndPoints Build(string url)
        {
            //去空格
            var realUrl = url.Trim();
            if (string.IsNullOrWhiteSpace(realUrl))
                return null;

            var host = "http://";
            if (realUrl.StartsWith("https://"))
                host = "https://";

            //去url重复
            realUrl = realUrl.Replace(host, "").Replace("//", "/");
            realUrl = host + realUrl;

            return Analysis(realUrl);
        }

        /// <summary>
        /// 分析链接
        /// </summary>
        /// <param name="url"></param>
        public static UrlEndPoints Analysis(string url)
        {
            Uri uri = GetUri(url);
            try
            {
                if (uri.Host.Contains(".sxkid.com"))
                {
                    //内部url
                    var result = AnalysisInsideUrl(uri);
                    if (result != null)
                        return result;
                }
            }
            catch (Exception)
            {
            }

            //外链
            return AnalysisOutsideUrl(uri);
        }

        public static Uri GetUri(string url)
        {
            try
            {
                return new Uri(url);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 可能是Id的传值
        /// </summary>
        public static string[] Keys = new string[] { "id", "Id", "circleId", "lectureid", "contentid" };
        /// <summary>
        /// 会被解析的sxkid Controller
        /// </summary>
        public static string[] sxkidControllerKeys = new string[] { "school", "comment", "question", "article", "live" };

        /// <summary>
        /// 内链
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static UrlEndPoints AnalysisInsideUrl(Uri uri)
        {
            //内部url
            var rawUrl = uri.AbsolutePath.Trim('/');
            var parts = rawUrl.Split('/');

            //特殊的学校详情 https://m3.sxkid.com/school-SQ7F/
            if (rawUrl.Contains("school-"))
            {
                parts = rawUrl.Split('-');
            }

            if (parts.Length >= 2)
            {
                var controllerName = parts[0].ToLower();
                var htmlName = parts[1].ToLower();
                var urlId = "";
                ///school/detail/968efc8f33d3437d86248a04758aa78f
                if (parts.Length >= 3)
                    urlId = parts[2];

                var isSxkidUrl = sxkidControllerKeys.Contains(controllerName);
                if (!isSxkidUrl) return null;

                //针对 school-SQ7F
                var actionName = htmlName;

                var pos = htmlName.LastIndexOf(".html");
                if (pos > 0)
                    actionName = htmlName.Substring(0, pos);
                var query = uri.Query;

                var nameValueCollection = GetNameValueCollection(query);

                //url页面中的主键
                Guid? id = GetId(nameValueCollection);

                //尝试从url中获取id
                if (id == null && Guid.TryParse(urlId, out Guid guid))
                    id = guid;

                long no = id == null ? GetNo(actionName) : default;
                if (id == null && no <= 0)
                {
                    return null;
                }
                return new UrlEndPoints()
                {
                    Type = TopicType.None,
                    IsSxkidUrl = isSxkidUrl,
                    Url = uri.AbsoluteUri,
                    ControllerName = controllerName,
                    ActionHtmlName = htmlName,
                    ActionName = actionName,
                    Query = query,
                    NameValueCollection = nameValueCollection,
                    Id = id,
                    No = no
                };
            }
            return null;
        }

        /// <summary>
        /// 外链
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static UrlEndPoints AnalysisOutsideUrl(Uri uri)
        {
            //解析外链
            return new UrlEndPoints()
            {
                Type = TopicType.OuterUrl,
                Url = uri.AbsoluteUri
            };
        }

        /// <summary>
        /// query转成NameValueCollection
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static NameValueCollection GetNameValueCollection(string query)
        {
            var nvc = new NameValueCollection();
            if (string.IsNullOrWhiteSpace(query))
                return nvc;

            // 开始分析参数对    
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(query);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2"), m.Result("$3"));
            }
            return nvc;
        }

        /// <summary>
        /// 从可能的key值中寻找Id
        /// </summary>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        public static Guid? GetId(NameValueCollection nameValueCollection)
        {
            Guid? Id = null;
            if (nameValueCollection != null)
            {
                foreach (var key in Keys)
                {
                    var id = nameValueCollection.Get(key);
                    if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out Guid guid))
                    {
                        Id = guid;
                        break;
                    }
                }
            }
            return Id;
        }

        public static long GetNo(string actionName)
        {
            var no = UrlShortIdUtil.Base322Long(actionName);
            return no;
        }
    }
}
