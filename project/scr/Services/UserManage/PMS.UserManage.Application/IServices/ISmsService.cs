using PMS.UserManage.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.IServices
{
    public interface ISmsService
    {
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        CommRespon SendCode(string phone, string code);

        /// <summary>
        /// 发送模板短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="templateId"></param>
        /// <param name="templateParams"></param>
        /// <returns></returns>
        CommRespon SendMessage(string phone, string templateId, params string[] templateParams);

        /// <summary>
        /// 发送短信， 失败后在当前上下文多次重试
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="templateId"></param>
        /// <param name="templateParams"></param>
        /// <returns></returns>
        CommRespon SendMessageRetry(string phone, string templateId, params string[] templateParams);

        /// <summary>
        /// 批量发送模板短信
        /// </summary>
        /// <param name="phones"></param>
        /// <param name="templateId"></param>
        /// <param name="templateParams"></param>
        /// <returns></returns>
        CommRespon BatchSendMessage(string[] phones, string templateId, params string[] templateParams);

        /// <summary>
        /// 获取/添加redis中的验证码
        /// </summary>
        /// <param name="key"></param>
        /// <param name="n">add时的验证码位数</param>
        /// <returns></returns>
        Task<string> GetOrAddRedisCode(string key, int n = 6);
        Task<bool> ValidateRedisCode(string key, string inputCode);
    }
}
