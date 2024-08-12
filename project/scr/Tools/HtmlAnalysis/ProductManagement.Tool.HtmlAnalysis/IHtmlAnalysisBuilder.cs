using ProductManagement.Tool.HtmlAnalysis.Helper;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProductManagement.Tool.HtmlAnalysis
{
    public interface IHtmlAnalysisBuilder
    {
        /// <summary>
        /// 获取正确编码的html
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        Task<string> GetHtml(HttpContent httpContent);
        /// <summary>
        /// 获取正确编码的html
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        Task<string> GetHtml(string url);

        /// <summary>
        /// 获取html解析对象
        /// </summary>
        /// <param name="httpContent"></param>
        /// <param name="analysisType"></param>
        /// <returns></returns>
        Task<HtmlAnalysisContext> GetHtmlAnalysisContext(string html, string pageUrl, AnalysisType analysisType = AnalysisType.All);
        /// <summary>
        /// 获取html解析对象
        /// </summary>
        /// <param name="httpContent"></param>
        /// <param name="analysisType"></param>
        /// <returns></returns>
        Task<HtmlAnalysisContext> GetHtmlAnalysisContext(string url, AnalysisType analysisType = AnalysisType.All);
    }
}
