using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ProductManagement.Framework.Foundation
{

    public class RootModel
    {
        public string url { get; set; }
        public string cdnUrl { get; set; }
        public int status { get; set; }
        public string errorDescription { get; set; }
        public string ReturnUrl { get; set; }
        public object data { get; set; }
    }

    public class UploadImager
    {
        /// <summary>
        /// 点评、问题图片上传（200：成功，500：失败）
        /// </summary>
        /// <param name="url">接口路径</param>
        /// <param name="files">文件</param>
        /// <returns></returns>
        public static int UploadFile(string url, IFormFile files)
        {
            int code = 500;
            Stream s = files.OpenReadStream();
            byte[] postData = new byte[s.Length];
            s.Read(postData, 0, postData.Length);
            s.Close();
            s.Dispose();

            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(url);
            Req.Method = "POST";
            Req.Timeout = 30000;
            Req.ContentLength = postData.Length;
            Stream sr_s = Req.GetRequestStream();
            sr_s.Write(postData, 0, postData.Length);
            HttpWebResponse res = (HttpWebResponse)Req.GetResponse();
            if (Req.HaveResponse)
            {
                Stream st = res.GetResponseStream();
                StreamReader re = new StreamReader(st);
                string rez = re.ReadToEnd();
                re.Close();
                code = rez == "0" ? 200 : 500;
            }
            return code;
        }

        /// <summary>
        /// 点评、问题图片上传（200：成功，500：失败）
        /// </summary>
        /// <param name="url">接口路径</param>
        /// <param name="Base64Source">base64数据值</param>
        /// <returns></returns>
        public static int UploadImagerByBase64(string url, byte[] postData)
        {
            int code = 500;
            //string[] sources = Base64Source.Split(',');
            //byte[] postData = Convert.FromBase64String(sources[1]);

            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(url);
            Req.Method = "POST";
            Req.Timeout = 30000;
            Req.ContentLength = postData.Length;
            Stream sr_s = Req.GetRequestStream();
            sr_s.Write(postData, 0, postData.Length);
            HttpWebResponse res = (HttpWebResponse)Req.GetResponse();
            if (Req.HaveResponse)
            {
                Stream st = res.GetResponseStream();
                StreamReader re = new StreamReader(st);
                string rez = re.ReadToEnd();
                re.Close();
                code = rez == "0" ? 200 : 500;
            }
            return code;
        }


        /// <summary>
        /// 意见反馈图片上传（200：成功，500：失败）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Base64Source"></param>
        /// <returns></returns>
        public static RootModel UploadImagerByFeedback(string url, byte[] postData)
        {
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(url);
            Req.Method = "POST";
            Req.Timeout = 30000;
            Req.ContentLength = postData.Length;
            Stream sr_s = Req.GetRequestStream();
            sr_s.Write(postData, 0, postData.Length);
            HttpWebResponse res = (HttpWebResponse)Req.GetResponse();
            if (Req.HaveResponse)
            {
                Stream st = res.GetResponseStream();
                StreamReader re = new StreamReader(st);
                string rez = re.ReadToEnd();
                return JsonConvert.DeserializeObject<RootModel>(rez);
            }
            return new RootModel() {  status = -1 };
        }

    }
}
