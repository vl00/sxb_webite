using ProductManagement.API.SMS.Model;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.SMS
{
    public class SMSHelper
    {
        private IEasyRedisClient _easyRedisClient;
        public SMSHelper(IEasyRedisClient easyRedisClient)
        {
            _easyRedisClient = easyRedisClient;
        }

        public bool SendRndCode(string mobile, string codeType, short nationCode, out string failReason)
        {
            if (!string.IsNullOrEmpty(mobile))
            {
                RndCodeModel CodeCache = _easyRedisClient.GetAsync<RndCodeModel>("login:RNDCode-" + nationCode + mobile + "-" + codeType).Result;
                if (CodeCache != null && CodeCache.CodeTime > DateTime.Now.AddMinutes(-1))
                {
                    failReason = "发送太频繁了，请稍后再试";
                    return false;
                }
                //int sendCount = _easyRedisClient.GetAsync<int>("RNDCodeSendCount-" + nationCode + mobile + "-" + codeType).Result;
                //if (sendCount >= 5)
                //{
                //    failReason = "此号码今天短信验证码发送的次数已超过限制，请改用其他验证方式";
                //    return false;
                //}

                int rndCode = new Random().Next(0, 999999);

                string codeStr = rndCode.ToString().PadLeft(6, '0');
                var sendResult = SendSMS(mobile, 10834, new List<string>() { codeStr }, nationCode);
                if (sendResult.result == 0)
                {
                    _easyRedisClient.AddAsync("login:RNDCode-" + nationCode + mobile + "-" + codeType, new RndCodeModel()
                    {
                        Mobile = nationCode + mobile,
                        Code = codeStr,
                        CodeType = codeType,
                        CodeTime = DateTime.Now
                    }, new TimeSpan(0, 0, 300));

                    //_easyRedisClient.AddAsync("RNDCodeSendCount-" + nationCode + mobile + "-" + codeType, ++sendCount, new TimeSpan(1, 0, 0, 0));
                    failReason = null;
                    return true;
                }
                else
                {
                    failReason = sendResult.errmsg;
                    return false;
                }
            }
            else
            {
                failReason = "请输入有效的手机号码";
                return false;
            }
        }
        public bool CheckRndCode(short nationCode, string mobile, string code, string codeType, bool removeCache = true)
        {
            RndCodeModel CodeCache = _easyRedisClient.GetAsync<RndCodeModel>("login:RNDCode-" + nationCode + mobile + "-" + codeType).Result;

            string insideRndCodeKey = "login:RndCode-Inside";
            string insideRndCode = _easyRedisClient.GetStringAsync(insideRndCodeKey).Result;

            if (!string.IsNullOrWhiteSpace(insideRndCode) && code == insideRndCode)
            {
                return true;
            }
            if (CodeCache != null && CodeCache.Code == code && CodeCache.CodeType == codeType)
            {
                if (removeCache)
                {
                    _easyRedisClient.RemoveAsync("login:RNDCode-" + nationCode + mobile + "-" + codeType);
                }
                return true;
            }
            return false;
        }
        public SmsSingleSenderResult SendSMS(string mobile, int templID, List<string> templ, short nationCode = 86, int sdkappid = 1400013556, string appkey = "ba4604e9bba557e6792c876c5e609df2", string sign = null)
        {
            TXSMSHelper smsHelper = new TXSMSHelper();
            SmsSingleSenderResult res = smsHelper.SendWithParam(sdkappid, appkey, nationCode.ToString(), mobile, templID, templ, sign ?? "", "", "");
            if (res.result == 0)
            {
                //SmsTemplateHandler handler = new SmsTemplateHandler();
                //SmsTemplateListResult templateResult = handler.GetTemplate(sdkappid, appkey, templID);
                //if (templateResult.result == 0)
                //{
                //    templ.Insert(0, "");
                //    string SMSContent = string.Format("{0}" + templateResult.data[0].text, templ.ToArray());
                //    //AddSMSRecord(new Model.Entity.dbo.sms_record()
                //    //{
                //    //    id = Guid.NewGuid(),
                //    //    sid = res.sid,
                //    //    mobile = mobile,
                //    //    status = "发送中",
                //    //    text = SMSContent,
                //    //    time = DateTime.Now,
                //    //    type = Model.Custom.SMSRecordType.系统发送
                //    //});
                //}
            }
            return res;
        }
        //public bool AddSMSRecord(Model.Entity.dbo.sms_record model)
        //{
        //    return SMS_DAL.AddSMSRecord(model);
        //}
    }
}
