using PMS.Infrastructure.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeChat.Interface;
using WeChat.Model;

namespace PMS.Infrastructure.Application.IService
{
    /// <summary>
    /// 提供对接微信的业务服务。
    /// </summary>
    public interface IWeChatService
    {
        Task<string> GenerateSenceQRCode(string callBackUrl, double expireDays = 1);
        /// <summary>
        /// 【服务号】发送图文消息
        /// </summary>
        /// <param name="toUser">目标用户的openId</param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        /// <param name="picUrl"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<bool> SendNews(string toUser, string title, string desc, string picUrl, string url, string channel = "fwh");

        /// <summary>
        /// 【服务号】 发送文本消息
        /// </summary>
        /// <param name="toUser">目标用户的openId</param>
        /// <param name="content">文本内容</param>
        /// <returns></returns>
        Task<bool> SendText(string toUser, string content, string channel = "fwh");


        /// <summary>
        /// 【服务号】 发送图片消息
        /// </summary>
        /// <param name="toUser">目标用户的openId</param>
        /// <param name="content">文本内容</param>
        /// <returns></returns>
        Task<bool> SendImage(string toUser, string mediaId, string channel = "fwh");

        /// <summary>
        /// 【服务号】 发送小程序卡片消息
        /// </summary>
        /// <param name="toUser"></param>
        /// <param name="appId"></param>
        /// <param name="pagePath"></param>
        /// <param name="thumbMediaId"></param>
        /// <param name="title"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task<bool> SendMiniProgramCard(string toUser, string appId, string pagePath, string thumbMediaId, string title, string channel = "fwh");

        /// <summary>
        /// 【服务号】 新增临时素材
        /// </summary>
        /// <param name="mediaType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> AddTempMedia(MediaType mediaType, byte[] data, string filename, string channel = "fwh");


        /// <summary>
        ///【服务号】  发送模板消息
        /// </summary>
        /// <param name="templateParam"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task SendTemplateMessage(SendTemplateRequest templateParam, string channel = "fwh");

        /// <summary>
        /// 【搜一搜】更新幼儿园/小学/初中/高中大卡
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task<string> UpsertPreUniversityBasic(UpsertPreUniversityBasicRequest request, WeChatAppChannel channel = WeChatAppChannel.fwh);
        Task<string> AddTempMedia(MediaType mediaType, string pageUrl, string filename, string channel = "fwh");

        Task<string> GetUnionId(string openId, WeChatAppChannel channel = WeChatAppChannel.fwh);
    }
}
