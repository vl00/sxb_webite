//using NUnit.Framework;

//namespace ProductManagement.Tool.HtmlAnalysis.Test
//{
//    public class Tests
//    {
//        [SetUp]
//        public void Setup()
//        {
//        }

//        static string img3 = "<img src=\"https://3\" />";
//        static string fullHtml = $@"<html>
//<head>
//   <meta property='og:title' content='什么工作？政府工作报告连续5年部署，总理开国常会再出新招' />
//   <meta property='og:url' content='http://mp.weixin.qq.com/s?__biz=MzA4MDA0MzcwMA==&amp;mid=2652577756&amp;idx=1&amp;sn=caa57cdae4461a0693ae9e4163c875b5&amp;chksm=8445f407b3327d1164859f1d54c981ea37288a102264a9da61c6e680006a6783b0580d436134#rd' />
//   <meta property='og:image' content='http://mmbiz.qpic.cn/mmbiz_jpg/k5d4lfEvkExEiaAkCx4FzAnNxcQMPebhsx2OCkIrbZOhbWOKAvUF8T0HTrcoibXtLkxCYYGSJOWuax83pzL68BCw/0?wx_fmt=jpeg' />
//</ head>
//<body>
//<img src='http://1' />
//<img src='https://2' />
//{img3}
//<img src='data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEBLAEsAAD/2' />
//</body>
//</html>";

//        [Test]
//        [TestCase(null)]
//        [TestCase("<img src='http://1' />")]
//        [TestCase("<img src='http://1' >")]
//        [TestCase("<img src='https://2' >")]
//        [TestCase("<img src=\"http://1\" >")]
//        [TestCase("<img src='data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEBLAEsAAD/2' />")]
//        public void TestImages(string html)
//        {
//            html ??= fullHtml;
//            var context = new HtmlAnalysisContext(html).AnalysisImages();

//            Assert.IsNotNull(context.Images);
//        }

//        [Test]
//        [TestCase(null)]
//        [TestCase("<meta property='og:image' content='http://1' />")]
//        [TestCase("<meta property='og : image' content='http://1' />")]
//        [TestCase("<meta property='og:image' content='http://1'>")]
//        [TestCase("<meta property='twitter:image' content='http://1'>")]
//        [TestCase("<meta property='twitter:image:src' content='http://1'>")]
//        [TestCase("<img src='http://1' />")]
//        public void TestTitleImage(string html)
//        {
//            html ??= fullHtml;
//            var context = new HtmlAnalysisContext(html).AnalysisTitleImage();

//            Assert.IsNotNull(context.TitleImage);
//            CollectionAssert.IsNotEmpty(context.TitleImage);
//            CollectionAssert.AllItemsAreNotNull(context.TitleImage);
//        }

//        [Test]
//        [TestCase(null)] //use html
//        [TestCase("<title>标题</title>")]
//        [TestCase("<meta property='og:title' content='标题' />")]
//        [TestCase("<meta property='og:title' content='标题' >")]
//        [TestCase("<meta property=\"twitter: title\" content=\"标题\" />")]
//        [TestCase("<meta property=\"twitter: title\" content=\"标题\">")]
//        public void TestTitle(string html)
//        {
//            html ??= fullHtml;
//            var context = new HtmlAnalysisContext(html).AnalysisTitle();

//            Assert.IsNotNull(context.Title);
//            Assert.AreNotEqual(string.Empty, context.Title);
//        }


//        [Test]
//        public void TestTitleNull()
//        {
//            var context = new HtmlAnalysisContext("").AnalysisTitle();
//            Assert.AreEqual(string.Empty, context.Title);
//        }
//    }
//}