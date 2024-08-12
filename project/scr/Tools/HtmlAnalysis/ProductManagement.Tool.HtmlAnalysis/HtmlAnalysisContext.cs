using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProductManagement.Tool.HtmlAnalysis
{
    public class HtmlAnalysisContext
    {
        /// <summary>
        /// html引号 
        /// quotation marks
        /// </summary>
        const string q = "[\"']";

        /// <summary>
        /// html 文字
        /// </summary>
        const string w = "[^>]";

        /// <summary>
        /// 不判断引号中的引号
        /// </summary>
        readonly string vsd = $"{q}(.*?){q}";
        private readonly string ImageUrlDef = "";


        /// <summary>
        /// html value 排除引号中的引号
        /// </summary>
        //const string vd = "\"([^\"]*)\"";
        //const string vs = "'([^']*)'";
        //readonly string vsd = $"(?:{vs}|{vd})";


        private string GetMetaOgValueRegex(string name)
        {
            var prop = GetPropOgValueRegex(name);
            var metaTitlePattern = $"<meta\\b{w}*{prop}{w}*\\s+content={vsd}[^>]*/?>";
            return metaTitlePattern;
        }

        private string GetPropOgValueRegex(string name)
        {
            return $"property={q}\\s*(?:og|twitter)\\s*:\\s*{name}{q}";
        }

        public string Html { get; private set; }
        public string PageUrl { get; set; }
        public string Domain { get; set; }

        internal string GetDomain()
        {
            if (string.IsNullOrWhiteSpace(PageUrl))
            {
                return string.Empty;
            }

            try
            {
                var uri = new Uri(PageUrl);
                
                return $"{uri.Scheme}://{uri.DnsSafeHost}:{uri.Port}";
            }
            catch (Exception)
            {

            }
            return string.Empty;
        }

        internal HtmlAnalysisContext(string html = "", string pageUrl = "")
        {
            Pre();
            LoadHtml(html, pageUrl);
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 标题图片
        /// 1.优先取head中定义的image,
        /// 2.无则取正文第一张
        /// 3.正文也无, 取默认图片
        /// </summary>
        public string TitleImage { get; set; }

        /// <summary>
        /// 正文图片列表
        /// </summary>
        public List<string> Images { get; set; }

        /// <summary>
        /// 是否已经处理分析Images
        /// </summary>
        public bool IsHandledImages { get; set; }


        static bool isLoadPre;
        private void Pre()
        {
            if (!isLoadPre)
            {
                //添加更多编码
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            }
        }

        public void LoadHtml(string html, string pageUrl)
        {
            Reset();
            Html = html;
            PageUrl = pageUrl;
            Domain = GetDomain();
        }

        public void Reset()
        {
            Title = string.Empty;
            TitleImage = string.Empty;
            Images = new List<string>();
            IsHandledImages = false;
        }

        internal HtmlAnalysisContext AnalysisTitle()
        {
            var html = Html;
            var title = string.Empty;

            //从head->title中获取标题
            Match m = Regex.Match(html, "<title(?:[^>]*>)([^<>/]*)</title>");
            if (m.Groups.Count == 2 && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
            {
                title = m.Groups[1].Value;
            }

            //从og->title中获取标题
            //<meta property="og:title" content="几百万！买下了学位房，却迎来了学位慌！" />
            //<meta property="twitter:title" content="几百万！买下了学位房，却迎来了学位慌！" />
            //<meta property="og:title" content="JDK 8 Stream 数据流效率怎么样？" />
            var metaTitlePattern = GetMetaOgValueRegex("title");
            m = Regex.Match(html, metaTitlePattern);
            if (m.Groups.Count >= 2)
            {
                title = m.Groups[1].Value;
            }

            Title = title;
            return this;
        }

        internal HtmlAnalysisContext AnalysisImages()
        {
            if (IsHandledImages)
            {
                return this;
            }

            var html = Html;
            //html = "<img src=\"/pc/_nuxt/img/2bd69d7.png\" alt class=\"talent-card-bg\" data-v-d5ff274e>";
            var images = new List<string>();

            //<img src="" />
            var srcRegex = $"src={vsd}";
            var regex = $"(?<=<img\\b){w}*{srcRegex}{w}*(?=/?>)";
            var matchs = Regex.Matches(html, regex);
            foreach (Match match in matchs)
            {
                if (match.Groups.Count < 2)
                {
                    continue;
                }

                var image = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(image))
                {
                    continue;
                }

                images.Add(GetImageUrl(image));
            }

            IsHandledImages = true;
            Images = images;
            return this;
        }

        /// <summary>
        /// 是否是图片地址
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsImageUrl(string image)
        {
            if (string.IsNullOrWhiteSpace(image))
            {
                return false;
            }

            //is http(s)
            if (image.StartsWith("http://") || image.StartsWith("https://"))
            {
                return true;
            }
            //is base64
            if (image.IndexOf("base64,", StringComparison.OrdinalIgnoreCase) > 0)
            {
                return true;
            }
            return false;
        }

        public string GetImageUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return ImageUrlDef;
            }
            if (IsImageUrl(imageUrl))
            {
                return imageUrl;
            }
            return Domain + imageUrl;
        }

        internal HtmlAnalysisContext AnalysisTitleImage()
        {
            var html = Html;

            //从og->image中获取标题图
            //<meta property="og:title" content="几百万！买下了学位房，却迎来了学位慌！" />
            //<meta property="twitter:title" content="几百万！买下了学位房，却迎来了学位慌！" />
            var metaTitlePattern = GetMetaOgValueRegex("image");
            Match m = Regex.Match(html, metaTitlePattern);
            if (m.Groups.Count >= 2)
            {
                var image = m.Groups[1].Value;
                TitleImage = GetImageUrl(image);
            }
            else
            {
                AnalysisImages();
                TitleImage = Images?.FirstOrDefault();
            }
            return this;
        }
    }
}
