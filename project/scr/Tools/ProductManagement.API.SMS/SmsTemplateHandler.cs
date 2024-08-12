using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductManagement.API.SMS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ProductManagement.API.SMS
{
    public class SmsTemplateHandler
    {
        //int sdkappid = 1400013556;
        //string appkey = "ba4604e9bba557e6792c876c5e609df2";
        SmsSenderUtil util = new SmsSenderUtil();
        public SmsTemplateResult AddTemplate(int sdkappid, string appkey, string title, string text, string remark, byte type)
        {
            string url = "https://yun.tim.qq.com/v5/tlssmssvr/add_template";
            if (0 != type && 1 != type && 2 != type)
            {
                throw new Exception("type " + type + " error");
            }

            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();

            data.Add("title", title);
            data.Add("remark", remark);
            data.Add("text", text);
            data.Add("type", type);
            data.Add("sig", util.StrToHash(String.Format(
                "appkey={0}&random={1}&time={2}",
                appkey, random, curTime)));
            data.Add("time", curTime);

            string wholeUrl = url + "?sdkappid=" + sdkappid + "&random=" + random;
            HttpWebRequest request = util.GetPostHttpConn(wholeUrl);
            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            request.ContentLength = requestData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // 接收返回包
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string responseStr = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            SmsTemplateResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToResult<SmsTemplateResult>(responseStr);
            }
            else
            {
                result = new SmsTemplateResult();
                result.result = -1;
                result.msg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }
        public SmsTemplateResult UpdateTemplate(int sdkappid, string appkey, int tpl_id, string title, string text, string remark, int type)
        {
            string url = "https://yun.tim.qq.com/v5/tlssmssvr/mod_template";
            if (0 != type && 1 != type && 2 != type)
            {
                throw new Exception("type " + type + " error");
            }

            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();

            data.Add("tpl_id", tpl_id);
            data.Add("title", title);
            data.Add("remark", remark);
            data.Add("text", text);
            data.Add("type", type);
            data.Add("sig", util.StrToHash(String.Format(
                "appkey={0}&random={1}&time={2}",
                appkey, random, curTime)));
            data.Add("time", curTime);

            string wholeUrl = url + "?sdkappid=" + sdkappid + "&random=" + random;
            HttpWebRequest request = util.GetPostHttpConn(wholeUrl);
            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            request.ContentLength = requestData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // 接收返回包
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string responseStr = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            SmsTemplateResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToResult<SmsTemplateResult>(responseStr);
            }
            else
            {
                result = new SmsTemplateResult();
                result.result = -1;
                result.msg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }
        public SmsTemplateResult DeleteTemplate(int sdkappid, string appkey, int tpl_id)
        {
            string url = "https://yun.tim.qq.com/v5/tlssmssvr/del_template";
            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();

            data.Add("tpl_id", tpl_id);
            data.Add("sig", util.StrToHash(String.Format(
                "appkey={0}&random={1}&time={2}",
                appkey, random, curTime)));
            data.Add("time", curTime);

            string wholeUrl = url + "?sdkappid=" + sdkappid + "&random=" + random;
            HttpWebRequest request = util.GetPostHttpConn(wholeUrl);
            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            request.ContentLength = requestData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // 接收返回包
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string responseStr = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            SmsTemplateResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToResult<SmsTemplateResult>(responseStr);
            }
            else
            {
                result = new SmsTemplateResult();
                result.result = -1;
                result.msg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }
        public SmsTemplateListResult GetTemplate(int sdkappid, string appkey, int tpl_id)
        {
            string url = "https://yun.tim.qq.com/v5/tlssmssvr/get_template";
            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();
            JArray tpl_id_array = JArray.Parse("[" + tpl_id + "]");
            data.Add("tpl_id", tpl_id_array);
            data.Add("sig", util.StrToHash(String.Format(
                "appkey={0}&random={1}&time={2}",
                appkey, random, curTime)));
            data.Add("time", curTime);

            string wholeUrl = url + "?sdkappid=" + sdkappid + "&random=" + random;
            HttpWebRequest request = util.GetPostHttpConn(wholeUrl);
            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            request.ContentLength = requestData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // 接收返回包
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string responseStr = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            SmsTemplateListResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToResult<SmsTemplateListResult>(responseStr);
            }
            else
            {
                result = new SmsTemplateListResult();
                result.result = -1;
                result.msg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }
        public SmsTemplateListResult GetTemplateList(int sdkappid, string appkey, int page)
        {
            string url = "https://yun.tim.qq.com/v5/tlssmssvr/get_template";
            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();
            JObject tpl_page = new JObject();
            tpl_page.Add("offset", (page - 1) * 50);
            tpl_page.Add("max", 50);

            data.Add("tpl_page", tpl_page);
            data.Add("sig", util.StrToHash(String.Format(
                "appkey={0}&random={1}&time={2}",
                appkey, random, curTime)));
            data.Add("time", curTime);

            string wholeUrl = url + "?sdkappid=" + sdkappid + "&random=" + random;
            HttpWebRequest request = util.GetPostHttpConn(wholeUrl);
            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            request.ContentLength = requestData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // 接收返回包
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string responseStr = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            SmsTemplateListResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToResult<SmsTemplateListResult>(responseStr);
            }
            else
            {
                result = new SmsTemplateListResult();
                result.result = -1;
                result.msg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }
    }
}
