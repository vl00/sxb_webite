using MediatR;
using Microsoft.Extensions.Options;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace PMS.MediatR.Handle.PaidQA
{

    /// <summary>
    /// 处理用户发消息
    /// </summary>
    public class AskWechatTemplateSendHandler : IRequestHandler<AskWechatTemplateSendRequest, bool>
    {

        IHttpClientFactory _httpClientFactory;
        WechatMessageTplSetting _tplCollect;
        internal IWeChatAppClient _weChatAppClient;
        IOrderService _orderService;
        IMessageService _messageService;
        IUserService _userService;
        ITemplateMessageService _templateMessageService;
        public AskWechatTemplateSendHandler(IHttpClientFactory httpClientFactory
            , IOptions<PaidQAOption> options
            , IWeChatAppClient weChatAppClient
            , IOrderService orderService
            , IMessageService messageService
            , IUserService userService
            , ITemplateMessageService templateMessageService)
        {
            _httpClientFactory = httpClientFactory;
            _tplCollect = options.Value.WechatMessageTplSetting;
            _weChatAppClient = weChatAppClient;
            _orderService = orderService;
            this._messageService = messageService;
            _userService = userService;
            _templateMessageService = templateMessageService;
        }
        public async Task<bool> Handle(AskWechatTemplateSendRequest request, CancellationToken cancellationToken)
        {
           
            var message = await GetTplContent(request);
            if (message == null)
            {
                //空内容默认是成功了。
                return true;
            }
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            if (null == accessToken || string.IsNullOrEmpty(accessToken.token)) throw new Exception($"发送模板消息{request.msgtype.ToString()},accessToken获取错误");
            var response = await _templateMessageService.SendAsync(accessToken.token, message);
            if (!string.IsNullOrEmpty(response.errmsg))
                return false;
            return true;
        }
        private async Task<SendTemplateRequest> GetTplContent(AskWechatTemplateSendRequest param)
        {
            var tpl_id = "";
            var list_filed = new List<TemplateDataFiled>();
           
            list_filed.Add(new TemplateDataFiled()
            {
                Filed = "keyword1",
                Value = param.keyword1,
                

            });
            list_filed.Add(new TemplateDataFiled()
            {
                Filed = "keyword2",
                Value = param.keyword2,

            });
            //--未处理风险。各种字段的长度问题
            switch (param.msgtype)
            {
                case WechatMessageType.专家回复问题:
                    if (await IsNotNeedSend(param.openid, param.OrderID)) return null;
                    tpl_id = _tplCollect.DarenAnswerPayAsk.tplid;
                    param.href = _tplCollect.DarenAnswerPayAsk.link;
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "first",
                        Value = $"\n{param.user_nickname}:\n[{param.keyword1}] 已回复了你的提问!马上查看吧！\n",

                    });
                    //内容--可能需要自己截断以防太长
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "keyword3",
                        Value = param.keyword3,
                    });
                    break;
                case WechatMessageType.用户发起追问:
                    if (await IsNotNeedSend(param.openid, param.OrderID)) return null;
                    tpl_id = _tplCollect.AddAsk.tplid;
                    param.href = _tplCollect.AddAsk.link;
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "first",
                        Value = $"\n亲爱的 [{param.daren_nickname}] 您收到了一条新的追问咨询！",

                    });
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "keyword3",
                        Value = param.keyword3,

                    });
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "keyword4",
                        Value = param.keyword4,

                    });
                    break;
                case WechatMessageType.用户发起最后一次追问:
                    if (await IsNotNeedSend(param.openid, param.OrderID)) return null;
                    tpl_id = _tplCollect.LastAddAsk.tplid;
                    param.href = _tplCollect.LastAddAsk.link;
                    list_filed.Clear();//不同颜色独立配置，先清空
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "first",
                        Value = $"\n[{param.daren_nickname}] 您收到了一条新的追问咨询！本次咨询将是该用户的最后一次追问。\n\n请在回复完毕后，点击【结束咨询】邀请用户对本次咨询服务进行评价！\n\n如用户在本次追问后，疑惑仍未解决，或有其他问题需要进一步咨询，您可以引导用户再次向您发起新的咨询订单！\n",

                    });
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "keyword1",
                        Value = param.keyword1,
                        Color = "#0022ff"

                    });
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "keyword2",
                        Value = param.keyword2,
                        Color = "#0022ff"

                    }); 
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "keyword3",
                        Value = param.keyword3,
                        Color = "#0022ff"

                    });
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "keyword4",
                        Value = param.keyword4,

                    });
                    break;
                case WechatMessageType.用户已评价:
                    tpl_id = _tplCollect.UserEvaluate.tplid;
                    param.href = _tplCollect.UserEvaluate.link;
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "first",
                        Value = $"\n收到一条来自用户已结束咨询服务的评价！",

                    });
                    break;
                case WechatMessageType.订单超时转单提醒:
                    tpl_id = _tplCollect.TimeoutOrderChange.tplid;
                    param.href = _tplCollect.TimeoutOrderChange.link;
                    //这里的keyword3用来存放提问的时间了
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "first",
                        Value = $"\n亲爱的 [{param.daren_nickname}]:\n\n     [{param.user_nickname}] 于{param.keyword3}对您发起的咨询已超时，用户已取消当次咨询，当次咨询的收益将进行扣除！\n",

                    });
                    break;
                case WechatMessageType.问答超过时未回复:
                    tpl_id = _tplCollect.TimeoutNotReply.tplid;
                    param.href = _tplCollect.TimeoutNotReply.link;
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "first",
                        Value = $"\n亲爱的 [{ param.daren_nickname }] ：\n\n      您已超过11小时未对用户 [{ param.user_nickname}] 的咨询进行回复。若您超过12小时未回复的咨询订单，用户可选择取消当次咨询，您将因此无法获得当次咨询收益！\n",

                    });
                    break;
                case WechatMessageType.收到提问:
                    tpl_id = _tplCollect.PayAskRecieve.tplid;
                    param.href = _tplCollect.PayAskRecieve.link;
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "first",
                        Value = $"\n亲爱的 [{param.daren_nickname}] 您收到了一条新的用户咨询！\n",

                    });
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "keyword3",
                        Value = param.keyword3,

                    });
                    list_filed.Add(new TemplateDataFiled()
                    {
                        Filed = "keyword4",
                        Value = param.keyword4,

                    });
                    break;

            }
            var message = new SendTemplateRequest(param.openid, tpl_id);
            message.Url = param.href.Replace("{orderId}",param.OrderID.ToString("N"));
            list_filed.Add(new TemplateDataFiled()
            {
                Filed = "remark.DATA",
                Value = param.remark,
            });
            message.SetData(list_filed.ToArray());
            return message;


        }


        public async Task<bool> IsNotNeedSend(string openid,Guid orderID)
        {
            var sendToUser = _userService.GetUserByWeixinOpenId(openid);
            return await _messageService.IsLive(orderID, sendToUser.Id);
        }
    }
}
