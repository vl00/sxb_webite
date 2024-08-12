using NUnit.Framework;

namespace ProductManagement.Tool.HtmlAnalysis.Test
{
    public class UnitNodeCompile
    {
        [SetUp]
        public void Setup()
        {
        }

        static string img3 = "<img src=\"https://3\" />";
        static string fullHtml = $@"<html>
<head>
   <meta property='og:title' content='什么工作？政府工作报告连续5年部署，总理开国常会再出新招' />
   <meta property='og:url' content='http://mp.weixin.qq.com/s?__biz=MzA4MDA0MzcwMA==&amp;mid=2652577756&amp;idx=1&amp;sn=caa57cdae4461a0693ae9e4163c875b5&amp;chksm=8445f407b3327d1164859f1d54c981ea37288a102264a9da61c6e680006a6783b0580d436134#rd' />
   <meta property='og:image' content='http://mmbiz.qpic.cn/mmbiz_jpg/k5d4lfEvkExEiaAkCx4FzAnNxcQMPebhsx2OCkIrbZOhbWOKAvUF8T0HTrcoibXtLkxCYYGSJOWuax83pzL68BCw/0?wx_fmt=jpeg' />
</ head>
<body>
<img src='http://1' />
<img src='https://2' />
{img3}
<img src='data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEBLAEsAAD/2' />
</body>
</html>";

        [Test]
        [TestCase("src=\"1 2\"  alt='测 \" 试'")]
        public void Test(string nodeString)
        { 

            for (int i = 0; i< 111; i++)
			{
                nodeString = "<div src=\"1 2\"  alt='测 \" 试'></div>";
                var compile = new NodeCompile() { NodeString = nodeString };

                compile.Compile();
            }

            //Assert.IsNotNull(compile.Node);
        }

    }
}