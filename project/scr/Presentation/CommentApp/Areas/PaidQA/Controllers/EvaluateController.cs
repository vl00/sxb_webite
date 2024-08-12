using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sxb.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.PaidQA.Application.Services;
using Sxb.Web.Response;
using Sxb.Web.Areas.PaidQA.Models.Evaluate;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using AutoMapper;
using PMS.PaidQA.Domain.Entities;
using Sxb.Web.Areas.PaidQA.Models.Talent;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.UserManage.Application.IServices;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Domain.Enums;
using MediatR;
using Sxb.Web.Areas.PaidQA.Models.Order;
using Sxb.Web.Filters;
using ProductManagement.Framework.AspNetCoreHelper.Filters;

namespace Sxb.Web.Areas.PaidQA.Controllers
{
    [Route("PaidQA/[controller]/[action]")]
    [ApiController]
    public class EvaluateController : ApiBaseController
    {
        IEvaluateService _evaluateService;
        IMapper _mapper;
        IOrderService _orderService;
        ITalentSettingService _talentSettingService;
        IEvaluateTagsService _evaluateTagsService;
        IUserService _userService;
        private readonly IMediator _mediator;
        public EvaluateController(IEvaluateService evaluateService, IMapper mapper, IOrderService orderService, ITalentSettingService talentSettingService,
            IEvaluateTagsService evaluateTagsService, IUserService userService, IMediator mediator)
        {
            _evaluateTagsService = evaluateTagsService;
            _evaluateService = evaluateService;
            _mapper = mapper;
            _orderService = orderService;
            _talentSettingService = talentSettingService;
            _userService = userService;
            _mediator = mediator;
        }


        [HttpGet]
        [Authorize]
        [Description("获取进行评价所需的前置信息")]
        public async Task<ResponseResult<GetEvaluatePreInfoResult>> GetEvaluatePreInfo([FromQuery] GetEvaluatePreInfoRequest request)
        {
            Order order = _orderService.Get(request.OrderID);
            if (order.CreatorID != UserIdOrDefault)
            {
                return ResponseResult<GetEvaluatePreInfoResult>.Failed("你没有权限。");
            }
            var talent = await _talentSettingService.GetDetail(order.AnswerID,UserIdOrDefault);
            GetEvaluatePreInfoResult result = new GetEvaluatePreInfoResult();
            result.TalentInfo = _mapper.Map<TalentInfoResult>(talent);
            result.EvaluateTags = _evaluateService.GetEvaluateTags().ToList();
            result.OrderInfo = _mapper.Map<OrderInfoResult>(order);
            return ResponseResult<GetEvaluatePreInfoResult>.Success(result, "");
        }


        [HttpPost]
        [Authorize]
        [Description("创建评价")]
        [ValidateWebContent(ContentParam = "request")]
        public async Task<ResponseResult<EvaluateResult>> CreateEvaluate([FromBody] CreateEvaluateRequest request)
        {
            if (request.Score < 1) {
                return ResponseResult<EvaluateResult>.Failed("请为评价打分");
            }
            Order order = _orderService.Get(request.OrderID);
            if (order.CreatorID != UserIdOrDefault)
            {
                return ResponseResult<EvaluateResult>.Failed("只有订单提问者才能评价");
            }
            Evaluate evaluate = new Evaluate()
            {
                Content = request.Content,
                Score = request.Score,
            };
            if (request.TagIds != null && request.TagIds.Any())
            {
                //添加评价的关系绑定
                evaluate.EvaluateTagRelations = request.TagIds.Select(tag => new EvaluateTagRelation()
                {
                    ID = Guid.NewGuid(),
                    EvaluateID = evaluate.ID,
                    TagID = tag
                }).ToList();
            }

            bool addResult = await _evaluateService.UserCreateEvaluate(order, evaluate);
            if (addResult)
            {
                #region 发送微信模板消息
                string darenOpenId = string.Empty;
                _userService.TryGetOpenId(order.AnswerID, "fwh", out darenOpenId);
                var daren = _userService.GetUserInfo(order.AnswerID);
                var askUser = _userService.GetUserInfo(order.CreatorID);
                if (null!= daren)
                {
                    if (order.IsAnonymous == true) { askUser.NickName = order.AnonyName; }
                    var cmd = new AskWechatTemplateSendRequest()
                    {
                      
                        keyword1 =$"[{askUser.NickName}]的上学问",
                        keyword2 = $"咨询服务已结束",
                        msgtype = WechatMessageType.用户已评价,
                        openid= darenOpenId,
                        OrderID = request.OrderID

                    };
                    var wechatMsgResult = await _mediator.Send(cmd);
                }
                #endregion
                var result = _mapper.Map<EvaluateResult>(evaluate);
                return ResponseResult<EvaluateResult>.Success(result, "创建成功");
            }
            else
            {
                return ResponseResult<EvaluateResult>.Failed("创建失败");
            }

        }



        [HttpGet]
        [Description("分页获取评价")]
        public async Task<ResponseResult> Page([FromQuery] Models.Evaluate.PageRequest request)
        {
            if (request.TalentUserID == Guid.Empty) { return ResponseResult.Failed(); }
            var result = ResponseResult.Success();
            var finds = await _evaluateService.PageByTalentUserID(request.TalentUserID, request.PageIndex, request.PageSize, request.TagID);
            if (finds.Item1?.Any() == true)
            {
                var orders = await _orderService.ListByIDs(finds.Item1.Where(p => p.OrderID.HasValue).Select(p => p.OrderID.Value));
                var userInfos = await _orderService.GetQuestionerInfoByOrderIDs(orders.Select(p => p.ID).Distinct());
                var avgScore = 0.0;
                var tagCounting = new List<EvaluateTagCountingExtend>();
                if (request.PageIndex == 1)
                {
                    avgScore = await _evaluateService.GetTalentAvgScope(request.TalentUserID);
                    var countings = await _evaluateService.GetEvaluateTagCountingByTalentUserID(request.TalentUserID);
                    if (countings?.Any() == true)
                    {
                        tagCounting.AddRange(countings);
                    }
                }

                var evaluates = new List<EvaluateItem>();

                await Task.Run(() =>
                {
                    foreach (var item in finds.Item1)
                    {
                        var entity = new EvaluateItem()
                        {
                            Content = item.Content,
                            CreateTime = item.CreateTime ?? default,
                            OrderID = item.OrderID ?? default,
                            Score = item.Score ?? 1
                        };
                        var order = orders.FirstOrDefault(p => p.ID == item.OrderID);
                        if (order != null)
                        {
                            if (order.IsAnonymous.HasValue && order.IsAnonymous.Value)
                            {
                                entity.NickName = order.AnonyName;
                                entity.HeadImgUrl = "https://cos.sxkid.com/images/head.png";
                            }
                            else
                            {
                                var userinfo = userInfos.FirstOrDefault(p => p.Item2 == order.CreatorID);
                                if (userinfo.Item2 != default)
                                {
                                    entity.NickName = userinfo.Item3;
                                    entity.HeadImgUrl = userinfo.Item4;
                                }
                            }
                        }

                        evaluates.Add(entity);
                    }
                });


                result.Data = new Models.Evaluate.PageResponse()
                {
                    AvgScore = (int)avgScore,
                    Tags = tagCounting,
                    Total = finds.Item2,
                    Evaluates = evaluates
                };
            }

            return result;
        }


        [HttpGet]
        [Description("自动好评")]
        public async Task<ResponseResult> AutoNiceEvaluation()
        {
            var res = await _evaluateService.AutoNiceEvaluation();
            return res ? ResponseResult.Success() : ResponseResult.Failed();
        }

        [HttpPost]
        [Description("根据评价IDs获取标签")]
        public async Task<ResponseResult> GetTagsByEvaluateIDs([FromBody] GetTagsByEvaluateIDsRequest request)
        {
            var result = ResponseResult.Success();
            var finds = await _evaluateTagsService.GetByEvaluateIDs(request.IDs);
            if (finds?.Any() == true)
            {
                result.Data = finds;
            }
            return result;
        }
    }
}
