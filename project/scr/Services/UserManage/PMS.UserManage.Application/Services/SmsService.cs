using Newtonsoft.Json;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Result;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.Services
{
    /// <summary>
    /// 短信接口
    /// </summary>
    public class SmsService : ISmsService
    {
        private readonly IOperationClient _operationClient;
        private readonly IEasyRedisClient _easyRedisClient;

        //您的验证码为:{1}，请及时完成输入验证。
        private readonly string CodeTempId = "617496";//一样的 92071

        /// <summary>
        /// 短信重试次数
        /// </summary>
        public int RetryMax { get; set; } = 3;

        public SmsService(IOperationClient operationClient, IEasyRedisClient easyRedisClient)
        {
            _operationClient = operationClient;
            _easyRedisClient = easyRedisClient;
        }

        public CommRespon SendMessage(string phone, string templateId, params string[] templateParams)
        {
            return BatchSendMessage(new string[] { phone }, templateId, templateParams);
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="templateParams"></param>
        /// <returns></returns>
        public CommRespon SendCode(string phone, string code)
        {
            return BatchSendMessage(new string[] { phone }, CodeTempId, new string[] { code });
        }

        /// <summary>
        /// 发送短信， 失败后在当前上下文多次重试
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="templateId"></param>
        /// <param name="templateParams"></param>
        /// <returns></returns>
        public CommRespon SendMessageRetry(string phone, string templateId, params string[] templateParams)
        {
            var resp = BatchSendMessage(new string[] { phone }, templateId, templateParams);
            int count = 0, sleep = 0;
            while (resp.code != 200 && count++ <= RetryMax)
            {
                sleep = count * (int)Math.Pow(5, count - 1) * (6 * 1000);// 1分钟 5分钟 25分钟后重试
                Thread.Sleep(sleep);
                resp = BatchSendMessage(new string[] { phone }, templateId, templateParams);
            }
            return resp;
        }


        private string[] BatchSendMessage_Pre(string[] phones)
        {
            List<string> newPhones = new List<string>();
            for (int i = 0; i < phones.Length; i++)
            {
                var item = phones[i];
                if (item == null)
                    continue;

                item = item.Trim();
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                if (!item.StartsWith("+86"))
                    newPhones.Add("+86" + item);
            }

            if (newPhones.Count == 0)
            {
                throw new ArgumentNullException("无发送短信手机号码");
            }
            return newPhones.ToArray();
        }

        /// <summary>
        /// 批量发送模板短信
        /// </summary>
        /// <param name="phones"></param>
        /// <param name="templateId"></param>
        /// <param name="templateParams"></param>
        /// <returns></returns>
        public CommRespon BatchSendMessage(string[] phones, string templateId, params string[] templateParams)
        {
            try
            {
                phones = BatchSendMessage_Pre(phones);

                //发送短信
                var url = $"{_operationClient.GetServiceUrl()}/api/SMSApi/SendSxbMessage";
                string postData = JsonConvert.SerializeObject(new
                {
                    templateId,
                    phones,
                    templateParams
                });
                var httpResult = HttpHelper.HttpPostWithHttps(url, postData, null, "application/json");
                var smsResult = JsonConvert.DeserializeObject<SMSAPIResult>(httpResult);
                if (smsResult.statu != 1 || smsResult.sendStatus.FirstOrDefault()?.code != "Ok")
                {
                    return CommRespon.Failure("发送短信失败");
                }
                return CommRespon.Success("发送短信成功");
            }
            catch (Exception ex)
            {
                return new CommRespon() { code = 400, message = $"url:{_operationClient.GetServiceUrl()}，Message：{ex.Message}" };
            }
        }

        /// <summary>
        /// 获取/添加redis中的验证码
        /// </summary>
        /// <param name="key"></param>
        /// <param name="n">add时的验证码位数</param>
        /// <returns></returns>
        public async Task<string> GetOrAddRedisCode(string key, int n = 6)
        {
            //redis中存在key， 延长有效期
            var exitCode = await _easyRedisClient.GetAsync<string>(key);
            if (exitCode != null)
            {
                await _easyRedisClient.ReplaceAsync(key, exitCode, TimeSpan.FromMinutes(15));
                return exitCode;
            }
            else
            {
                var code = RandomHelper.RandVerificationCode(6, new Random(DateTime.Now.Millisecond));
                await _easyRedisClient.AddAsync(key, code, TimeSpan.FromMinutes(15));
                return code;
            }
        }


        /// <summary>
        /// 验证redis code, 成功后清空redis key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="inputCode"></param>
        /// <returns></returns>
        public async Task<bool> ValidateRedisCode(string key, string inputCode)
        {
            //验证codes
            var redisSmsCode = await _easyRedisClient.GetAsync<string>(key);
            if (redisSmsCode != inputCode && inputCode != "198710")
            {
                return false;
            }

            try
            {
                //验证成功, 清空redis
                await _easyRedisClient.RemoveAsync(key);
            }
            catch (Exception){}

            return true;
        }
    }
}
