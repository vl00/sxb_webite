using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityTest
{
    using ProductManagement.API.Aliyun;

    [TestClass]
    public class TestAliyun
    {
        [TestMethod]
        public void TestGarbageCheck()
        {
            //using (var httpClient = new System.Net.Http.HttpClient())
            //{
            //    IText text = new Text(httpClient, "", "", null);
            //    var response = text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
            //    {
            //        scenes = new[] { "antispam" },
            //        tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
            //          new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
            //           content="澳门赌博 在线裸聊 德瑞哦额委屈你,程序i就得死就发绿卡的杀戮困难俄日九二六九的身份数据库里的说法看就看",
            //             dataId = Guid.NewGuid().ToString()
            //          }
            //          }
            //    }).Result;
            //    Assert.IsNotNull(response);
            //}
        }
    }
}