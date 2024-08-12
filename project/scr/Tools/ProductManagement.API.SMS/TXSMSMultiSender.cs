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
    public class TXSMSMultiSender
    {
        //int sdkappid = 1400013556;
        //string appkey = "ba4604e9bba557e6792c876c5e609df2";
        string url = "https://yun.tim.qq.com/v5/tlssmssvr/sendmultisms2";

        SmsSenderUtil util = new SmsSenderUtil();

        public TXSMSMultiSender() { }

        /**
         * 普通群发短信接口，明确指定内容，如果有多个签名，请在内容中以【】的方式添加到信息内容中，否则系统将使用默认签名
         * 【注意】海外短信无群发功能
         * @param type 短信类型，0 为普通短信，1 营销短信
         * @param nationCode 国家码，如 86 为中国
         * @param phoneNumbers 不带国家码的手机号列表
         * @param msg 信息内容，必须与申请的模板格式一致，否则将返回错误
         * @param extend 扩展码，可填空
         * @param ext 服务端原样返回的参数，可填空
         * @return SmsMultiSenderResult
         */
        public SmsMultiSenderResult Send(
            int sdkappid,
            string appkey,
            int type,
            string nationCode,
            List<string> phoneNumbers,
            string msg,
            string extend,
            string ext)
        {
            /*
            请求包体
            {
                "tel": [
                    {
                        "nationcode": "86", 
                        "mobile": "13788888888"
                    }, 
                    {
                        "nationcode": "86", 
                        "mobile": "13788888889"
                    }
                ], 
                "type": 0, 
                "msg": "你的验证码是1234", 
                "sig": "fdba654e05bc0d15796713a1a1a2318c",
                "time": 1479888540,
                "extend": "", 
                "ext": ""
            }
            应答包体
            {
                "result": 0, 
                "errmsg": "OK", 
                "ext": "", 
                "detail": [
                    {
                        "result": 0, 
                        "errmsg": "OK", 
                        "mobile": "13788888888", 
                        "nationcode": "86", 
                        "sid": "xxxxxxx", 
                        "fee": 1
                    }, 
                    {
                        "result": 0, 
                        "errmsg": "OK", 
                        "mobile": "13788888889", 
                        "nationcode": "86", 
                        "sid": "xxxxxxx", 
                        "fee": 1
                    }
                ]
            }
            */
            if (0 != type && 1 != type)
            {
                throw new Exception("type " + type + " error");
            }
            if (null == extend)
            {
                extend = "";
            }
            if (null == ext)
            {
                ext = "";
            }

            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();
            data.Add("tel", util.PhoneNumbersToJSONArray(nationCode, phoneNumbers));
            data.Add("type", type);
            data.Add("msg", msg);
            data.Add("sig", util.CalculateSig(appkey, random, curTime, phoneNumbers));
            data.Add("time", curTime);
            data.Add("extend", extend);
            data.Add("ext", ext);

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
            SmsMultiSenderResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToMultiSenderResult(responseStr);
            }
            else
            {
                result = new SmsMultiSenderResult();
                result.result = -1;
                result.errmsg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }

        /**
         * 指定模板群发
         * 【注意】海外短信无群发功能
         * @param nationCode 国家码，如 86 为中国
         * @param phoneNumbers 不带国家码的手机号列表
         * @param templId 模板 id
         * @param params 模板参数列表
         * @param sign 签名，如果填空，系统会使用默认签名
         * @param extend 扩展码，可以填空
         * @param ext 服务端原样返回的参数，可以填空
         * @return SmsMultiSenderResult
         */
        public SmsMultiSenderResult SendWithParam(
            int sdkappid,
            string appkey,
            String nationCode,
            List<string> phoneNumbers,
            int templId,
            List<string> templParams,
            string sign,
            string extend,
            string ext)
        {
            /*
            请求包体
            {
                "tel": [
                    {
                        "nationcode": "86", 
                        "mobile": "13788888888"
                    }, 
                    {
                        "nationcode": "86", 
                        "mobile": "13788888889"
                    }
                ], 
                "type": 0, 
                "msg": "你的验证码是1234", 
                "sig": "fdba654e05bc0d15796713a1a1a2318c",
                "time": 1479888540,
                "extend": "", 
                "ext": ""
            }
            应答包体
            {
                "result": 0, 
                "errmsg": "OK", 
                "ext": "", 
                "detail": [
                    {
                        "result": 0, 
                        "errmsg": "OK", 
                        "mobile": "13788888888", 
                        "nationcode": "86", 
                        "sid": "xxxxxxx", 
                        "fee": 1
                    }, 
                    {
                        "result": 0, 
                        "errmsg": "OK", 
                        "mobile": "13788888889", 
                        "nationcode": "86", 
                        "sid": "xxxxxxx", 
                        "fee": 1
                    }
                ]
            }
            */
            if (null == sign)
            {
                sign = "";
            }
            if (null == extend)
            {
                extend = "";
            }
            if (null == ext)
            {
                ext = "";
            }

            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();
            data.Add("tel", util.PhoneNumbersToJSONArray(nationCode, phoneNumbers));
            data.Add("sig", util.CalculateSigForTempl(appkey, random, curTime, phoneNumbers));
            data.Add("tpl_id", templId);
            data.Add("params", util.SmsParamsToJSONArray(templParams));
            data.Add("sign", sign);
            data.Add("time", curTime);
            data.Add("extend", extend);
            data.Add("ext", ext);

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
            SmsMultiSenderResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToMultiSenderResult(responseStr);
            }
            else
            {
                result = new SmsMultiSenderResult();
                result.result = -1;
                result.errmsg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }
    }
}
