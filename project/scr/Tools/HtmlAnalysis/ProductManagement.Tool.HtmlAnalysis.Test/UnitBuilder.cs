using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Net.Http;

namespace ProductManagement.Tool.HtmlAnalysis.Test
{
    public class UnitBuilder :  BaseUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        [TestCase("https://www.cnblogs.com/admans/p/11955614.html")]
        [TestCase("https://mp.weixin.qq.com/s/Drqj3j-lKlGo8Zo7i9D1zw")]
        [TestCase("https://www4.sxkid.com/article/im37l.html")]
        [TestCase("https://www3.sxkid.com/org/evaluation/detail/m6")]
        [TestCase("https://www.baidu.com")]
        public void TestUrl(string url)
        {
            var logger = NullLogger.Instance;
            var httpClient = new HttpClient();
            var builder = new HtmlAnalysisBuilder(logger, httpClient);

            var context = builder.GetHtmlAnalysisContext(url).GetAwaiter().GetResult();

            WriteLine(context.Title);
            WriteLine(context.TitleImage);
            WriteLine(context.Images);

            Assert.IsNotNull(context);
            StringAssert.AreNotEqualIgnoringCase(context.Title, string.Empty);
        }
    }
}