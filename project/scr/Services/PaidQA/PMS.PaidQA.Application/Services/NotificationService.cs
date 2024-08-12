using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.PaidQA.Domain.Entities;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.SMS;
using ProductManagement.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace PMS.PaidQA.Application.Services
{
    public class NotificationService : INotificationService
    {
        IUserService _userService;
        ITencentSmsService _tencentSmsService;
        PaidQAOption _paidQAOption;
        ITemplateMessageService _templateMessageService;
        IWeChatAppClient _weChatAppClient;
        ILogger _logger;
        IOrderService _orderService;
        ICouponTakeService _couponTakeService;
        ICustomMsgService _customMsgService;
        public NotificationService(ILogger<NotificationService> logger
            , IUserService userService
            , IOptions<PaidQAOption> paidQAOption
            , ITencentSmsService tencentSmsService
            , ITemplateMessageService templateMessageService
            , IWeChatAppClient weChatAppClient
            , IOrderService orderService
            , ICouponTakeService couponTakeService
            , ICustomMsgService customMsgService)
        {
            _userService = userService;
            _paidQAOption = paidQAOption.Value;
            _tencentSmsService = tencentSmsService;
            _templateMessageService = templateMessageService;
            _weChatAppClient = weChatAppClient;
            _logger = logger;
            _orderService = orderService;
            _couponTakeService = couponTakeService;
            _customMsgService = customMsgService;
        }


        public async Task NotifiUserCreateQuestion(Guid orderId)
        {
            try
            {
                var order = await _orderService.GetAsync(orderId);
                if (order == null)
                {
                    return;
                }
                var createUserinfo = _userService.GetUserInfo(order.CreatorID);
                var talentUserinfo = _userService.GetUserInfo(order.AnswerID);
                if (createUserinfo != null)
                {
                    if (!_userService.TryGetOpenId(createUserinfo.Id, "fwh", out string openId))
                    {
                        //短信触达
                        var tplParams = _paidQAOption.MobileMsgTplSetting.CreateQuestionNotify.tplParams?.Select(tplp => tplp.Replace("{nickName}", talentUserinfo.NickName)).ToArray();
                        await _tencentSmsService.SendSmsAsync($"+{createUserinfo.NationCode ?? 86}{createUserinfo.Mobile}", _paidQAOption.MobileMsgTplSetting.CreateQuestionNotify.tplid, tplParams, App.Push);
                    }
                    else
                    {
                        var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
                        if (accessToken != null)
                        {
                            if (_paidQAOption.WechatMessageTplSetting.CreateQuestion == null)
                            {
                                throw new ArgumentNullException("找不到\"CreateQuestion\"的微信模板配置");
                            }
                            var tplmsg = new WeChat.Model.SendTemplateRequest(openId, _paidQAOption.WechatMessageTplSetting.CreateQuestion.TemplateId);
                            tplmsg.Url = _paidQAOption.WechatMessageTplSetting.CreateQuestion.Url.Replace("{orderId}", order.ID.ToString());
                            tplmsg.SetData(
                                 _paidQAOption.WechatMessageTplSetting.CreateQuestion.Fields.Select(s =>
                                 {
                                     return new WeChat.Model.TemplateDataFiled()
                                     {
                                         Filed = s.FieldName,
                                         Color = s.Color,
                                         Value = s.Value?.Replace("{AskNickName}", createUserinfo.NickName).Replace("{TalentNickName}", talentUserinfo.NickName)
                                     };
                                 }).ToArray()
                                );
                            await _templateMessageService.SendAsync(accessToken.token, tplmsg);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "给用户发提问通知失败。");
            }


        }

        public async Task NotifiUserEvalute(Guid orderId)
        {
            var order = await _orderService.GetAsync(orderId);

            try
            {
                if (_userService.TryGetOpenId(order.CreatorID, "fwh", out string openID))
                {
                    //发送服务号消息
                    var talentUserInfo = _userService.GetUserInfo(order.AnswerID);
                    TextCustomMsg msg = new TextCustomMsg()
                    {
                        ToUser = openID,
                        content = _paidQAOption.CustomMsgSetting.EvaluteTips.Content
                        .Replace("{NickName}", talentUserInfo.NickName)
                        .Replace("{orderId}", order.ID.ToString("N"))
                    };
                    var accessToken = _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
                    var result = await _customMsgService.Send(accessToken.token, msg);
                }
                else
                {
                    //发送短信
                    var creatorInfo = _userService.GetUserInfo(order.CreatorID);
                    var answerInfo = _userService.GetUserInfo(order.AnswerID);
                    var tplparams = _paidQAOption.MobileMsgTplSetting.EvaluteTipsNotify.tplParams
                        .Select(p =>
                            p
                            .Replace("{nickName}", answerInfo.NickName)
                            )
                        .ToArray();
                    await _tencentSmsService.SendSmsAsync($"+{creatorInfo.NationCode ?? 86}{creatorInfo.Mobile}"
                       , _paidQAOption.MobileMsgTplSetting.EvaluteTipsNotify.tplid
                       , tplparams
                       , App.Push);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "给用户提醒评价通知失败。");
            }
        }


        public async Task NotifiTransting(Guid orderId)
        {
            var order = await _orderService.GetAsync(orderId);
            //发送服务号消息
            if (_userService.TryGetOpenId(order.CreatorID, "fwh", out string openID))
            {
                TextCustomMsg msg = new TextCustomMsg()
                {
                    ToUser = openID,
                    content = _paidQAOption.CustomMsgSetting.TranstingTips.Content.Replace("{orderId}", order.ID.ToString("N"))
                };
                var accessToken = _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
                var result = await _customMsgService.Send(accessToken.token, msg);
            }
            else
            {
                //发送短信
                var creatorInfo = _userService.GetUserInfo(order.CreatorID);
                var answerInfo = _userService.GetUserInfo(order.AnswerID);
                var tplparams = _paidQAOption.MobileMsgTplSetting.TranstingTipsNotify.tplParams
                    .Select(p =>
                        p
                        .Replace("{nickName}", answerInfo.NickName)
                        )
                    .ToArray();
                await _tencentSmsService.SendSmsAsync($"+{creatorInfo.NationCode ?? 86}{creatorInfo.Mobile}"
                   , _paidQAOption.MobileMsgTplSetting.TranstingTipsNotify.tplid
                   , tplparams
                   , App.Push);
            }
        }



        public async Task NotifiUserOrderTimeOut(Guid orderId)
        {
            var order = await _orderService.GetAsync(orderId);
            //通知提问者
            if (_userService.TryGetOpenId(order.CreatorID, "fwh", out string openID))
            {
                //发送服务号消息
                TextCustomMsg msg = new TextCustomMsg()
                {
                    ToUser = openID,
                    content = _paidQAOption.CustomMsgSetting.TalentUnReplyTips.Content.Replace("{orderId}", order.ID.ToString("N"))
                };
                var accessToken = _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
                var result = await _customMsgService.Send(accessToken.token, msg);

            }
            else
            {
                //发送短信
                var creatorInfo = _userService.GetUserInfo(order.CreatorID);
                var answerInfo = _userService.GetUserInfo(order.AnswerID);
                var tplparams = _paidQAOption.MobileMsgTplSetting.TalentUnReplyTipsNotify.tplParams
                    .Select(p =>
                        p
                        .Replace("{nickName}", answerInfo.NickName)
                        )
                    .ToArray();
                await _tencentSmsService.SendSmsAsync($"+{creatorInfo.NationCode ?? 86}{creatorInfo.Mobile}"
                   , _paidQAOption.MobileMsgTplSetting.TalentUnReplyTipsNotify.tplid
                   , tplparams
                   , App.Push);
            }

        }


        public async Task NotifiTalentOrderTimeOut(Guid orderId)
        {
            var order = await _orderService.GetAsync(orderId);
            //发送公众号模板消息
            var answerInfo = _userService.GetUserInfo(order.AnswerID);
            if (_userService.TryGetOpenId(answerInfo.Id, "fwh", out string openId))
            {
                var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
                if (accessToken != null)
                {
                    if (_paidQAOption.WechatMessageTplSetting.OrderTimeOut == null)
                    {
                        throw new ArgumentNullException("找不到\"OrderTimeOut\"的微信模板配置");
                    }
                    var creatorInfo = _userService.GetUserInfo(order.CreatorID);
                    var tplmsg = new WeChat.Model.SendTemplateRequest(openId, _paidQAOption.WechatMessageTplSetting.OrderTimeOut.TemplateId);
                    tplmsg.Url = _paidQAOption.WechatMessageTplSetting.OrderTimeOut.Url.Replace("{orderId}", order.ID.ToString());
                    tplmsg.SetData(
                         _paidQAOption.WechatMessageTplSetting.OrderTimeOut.Fields.Select(s =>
                         {
                             return new WeChat.Model.TemplateDataFiled()
                             {
                                 Filed = s.FieldName,
                                 Color = s.Color,
                                 Value = s.Value?
                                 .Replace("{answerNickName}", answerInfo.NickName)
                                 .Replace("{askerNickName}", creatorInfo.NickName)
                                 .Replace("{orderCreateTime}", order.CreateTime.ToString("yyyy年MM月dd日 HH:mm:ss"))
                             };
                         }).ToArray()
                        );
                    await _templateMessageService.SendAsync(accessToken.token, tplmsg);
                }


            }


        }


        public async Task NotifiUserRefusOrder(Guid orderId)
        {
            var order = await _orderService.GetAsync(orderId);
            //通知提问者
            if (_userService.TryGetOpenId(order.CreatorID, "fwh", out string openID))
            {
                //发送服务号消息
                var talentUserInfo = _userService.GetUserInfo(order.AnswerID);
                TextCustomMsg msg = new TextCustomMsg()
                {
                    ToUser = openID,
                    content = _paidQAOption.CustomMsgSetting.TalentRefusOrderTips.Content
                    .Replace("{NickName}", talentUserInfo.NickName)
                    .Replace("{orderId}", order.ID.ToString("N"))
                };
                var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
                var result = await _customMsgService.Send(accessToken.token, msg);

            }
            else
            {
                //发短信
                var creatorInfo = _userService.GetUserInfo(order.CreatorID);
                var answerInfo = _userService.GetUserInfo(order.AnswerID);
                var tplparams = _paidQAOption.MobileMsgTplSetting.TalentRefusOrderTipsNotify.tplParams
                    .Select(p =>
                        p
                        .Replace("{nickName}", answerInfo.NickName)
                        )
                    .ToArray();
                await _tencentSmsService.SendSmsAsync($"+{creatorInfo.NationCode ?? 86}{creatorInfo.Mobile}"
                   , _paidQAOption.MobileMsgTplSetting.TalentRefusOrderTipsNotify.tplid
                   , tplparams
                   , App.Push);
            }


        }


        public async Task NotifiUserRefundAmount(Guid orderId)
        {
            var order = await _orderService.GetAsync(orderId);
            //通知提问者
            //发短信
            var creatorInfo = _userService.GetUserInfo(order.CreatorID);
            var tplparams = _paidQAOption.MobileMsgTplSetting.RefundNotify
                .tplParams
                .ToArray();
            await _tencentSmsService.SendSmsAsync($"+{creatorInfo.NationCode ?? 86}{creatorInfo.Mobile}"
               , _paidQAOption.MobileMsgTplSetting.RefundNotify.tplid
               , tplparams
               , App.Push);

        }





        public async Task NotifiExpireCoupon(Guid takeId)
        {
            try
            {
              var couponTake =   await  _couponTakeService.GetCouponTake(takeId);

                var takeUser = _userService.GetUserInfo(couponTake.UserId.Value);
                if (takeUser != null)
                {
                    if (!_userService.TryGetOpenId(takeUser.Id, "fwh", out string openId))
                    {
                        //短信触达
                        var tplParams = _paidQAOption.MobileMsgTplSetting.CouponExpireTips.tplParams?.Select(tplp => tplp.Replace("{nickName}", takeUser.NickName)).ToArray();
                        await _tencentSmsService.SendSmsAsync($"+{takeUser.NationCode ?? 86}{takeUser.Mobile}", _paidQAOption.MobileMsgTplSetting.CouponExpireTips.tplid, tplParams, App.Push);
                    }
                    else
                    {
                        var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
                        if (accessToken != null)
                        {
                            if (_paidQAOption.WechatMessageTplSetting.CouponExpireTips == null)
                            {
                                throw new ArgumentNullException("找不到\"CouponExpireTips\"的微信模板配置");
                            }
                            var tplmsg = new WeChat.Model.SendTemplateRequest(openId, _paidQAOption.WechatMessageTplSetting.CouponExpireTips.TemplateId);
                            tplmsg.Url = _paidQAOption.WechatMessageTplSetting.CouponExpireTips.Url;
                            tplmsg.SetData(
                                 _paidQAOption.WechatMessageTplSetting.CouponExpireTips.Fields.Select(s =>
                                 {
                                     return new WeChat.Model.TemplateDataFiled()
                                     {
                                         Filed = s.FieldName,
                                         Color = s.Color,
                                         Value = s.Value?.Replace("{userNickName}", takeUser.NickName)
                                     };
                                 }).ToArray()
                                );
                            await _templateMessageService.SendAsync(accessToken.token, tplmsg);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "通知优惠券过期失败。");
            }


        }

        public async Task NotifiUserAsk(Guid orderId)
        {
            try
            {
                var order = await _orderService.GetAsync(orderId);
                if (order == null)
                {
                    return;
                }
                var createUserinfo = _userService.GetUserInfo(order.CreatorID);
                var talentUserinfo = _userService.GetUserInfo(order.AnswerID);
                if (createUserinfo != null)
                {
                    if (!_userService.TryGetOpenId(createUserinfo.Id, "fwh", out string openId))
                    {
                        //短信触达
                        var tplParams = _paidQAOption.MobileMsgTplSetting.UnAskTips.tplParams?.Select(tplp => tplp.Replace("{answerNickName}", talentUserinfo.NickName)).ToArray();
                        await _tencentSmsService.SendSmsAsync($"+{createUserinfo.NationCode ?? 86}{createUserinfo.Mobile}", _paidQAOption.MobileMsgTplSetting.UnAskTips.tplid, tplParams, App.Push);
                    }
                    else
                    {
                        var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
                        if (accessToken != null)
                        {
                            if (_paidQAOption.WechatMessageTplSetting.UnAskTips == null)
                            {
                                throw new ArgumentNullException("找不到\"UnAskTips\"的微信模板配置");
                            }
                            var tplmsg = new WeChat.Model.SendTemplateRequest(openId, _paidQAOption.WechatMessageTplSetting.UnAskTips.TemplateId);
                            tplmsg.Url = _paidQAOption.WechatMessageTplSetting.UnAskTips.Url.Replace("{orderId}", order.ID.ToString());
                            tplmsg.SetData(
                                 _paidQAOption.WechatMessageTplSetting.UnAskTips.Fields.Select(s =>
                                 {
                                     return new WeChat.Model.TemplateDataFiled()
                                     {
                                         Filed = s.FieldName,
                                         Color = s.Color,
                                         Value = s.Value?.Replace("{askerNickName}", createUserinfo.NickName).Replace("{answerNickName}", talentUserinfo.NickName)
                                     };
                                 }).ToArray()
                                );
                            await _templateMessageService.SendAsync(accessToken.token, tplmsg);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "给用户发未提问通知。");
            }

        }

        public async Task NotifiReply(Guid orderId)
        {
            try
            {
                var order = await _orderService.GetAsync(orderId);
                if (order == null)
                {
                    return;
                }
                var talentUserinfo = _userService.GetUserInfo(order.AnswerID);
                var createUserinfo = _userService.GetUserInfo(order.CreatorID);
                if (talentUserinfo != null)
                {
                    //短信触达
                    var tplParams = _paidQAOption.MobileMsgTplSetting.ReplyTips.tplParams?.Select(tplp => tplp.Replace("{askerNickName}", createUserinfo.NickName)).ToArray();
                    await _tencentSmsService.SendSmsAsync($"+{talentUserinfo.NationCode ?? 86}{talentUserinfo.Mobile}", _paidQAOption.MobileMsgTplSetting.ReplyTips.tplid, tplParams, App.Push);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "提醒达人回复。");
            }
        }
    }
}
