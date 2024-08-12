using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.PaidQA.Domain.Message;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.PaidQA.Models.HotType;
using Sxb.Web.Areas.PaidQA.Models.Question;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Controllers
{
    [Route("PaidQA/[controller]/[action]")]
    [ApiController]
    public class HotTypeController : ControllerBase
    {
        IHotTypeService _hotTypeService;
        IOrderService _orderService;
        IUserService _userService;
        PMS.PaidQA.Application.Services.IMessageService _messageService;
        IEvaluateService _evaluateService;
        IEasyRedisClient _easyRedisClient;
        ICollectionService _collectionService;

        public HotTypeController(IHotTypeService hotTypeService, IOrderService orderService, IUserService userService, PMS.PaidQA.Application.Services.IMessageService messageService,
            IEvaluateService evaluateService, IEasyRedisClient easyRedisClient, ICollectionService collectionService)
        {
            _collectionService = collectionService;
            _easyRedisClient = easyRedisClient;
            _evaluateService = evaluateService;
            _messageService = messageService;
            _userService = userService;
            _orderService = orderService;
            _hotTypeService = hotTypeService;
        }

        [HttpGet]
        [Description("获取热门咨询分类")]
        public async Task<ResponseResult> GetHotTypes(int count, int type = 1)
        {
            var result = ResponseResult.Success();

            var finds = await _easyRedisClient.GetOrAddAsync($"PaidQA_HotType_Type{type}", () =>
            {
                return _hotTypeService.GetAll(type);
            }, TimeSpan.FromHours(3));
            if (finds?.Any() == true)
            {
                if (count > 0)
                {
                    result.Data = finds.OrderBy(p => p.Sort).Take(count).Select(p => new
                    {
                        ID = p.ID,
                        Name = p.Name,
                        LogoImgUrl = p.LogoImage,
                        Sort = p.Sort
                    });
                }
                else
                {
                    result.Data = finds.OrderBy(p => p.Sort).Select(p => new
                    {
                        ID = p.ID,
                        Name = p.Name,
                        LogoImgUrl = p.LogoImage,
                        Sort = p.Sort
                    });
                }
            }
            return result;
        }

        [HttpGet]
        [Description("分页获取热门咨询")]
        public async Task<ResponseResult> Page([FromQuery] PageRequest request)
        {
            var result = ResponseResult.Success();
            IEnumerable<Guid> ids = new List<Guid>();
            if (request.Random && !request.HotTypeID.HasValue)
            {
                ids = await _hotTypeService.GetRandomOrderIDs(request.PageIndex, request.PageSize, request.OnePageRandom);
            }
            else if (request.HotTypeID.HasValue)
            {
                ids = await _hotTypeService.GetOrderIDByHotTypeID(request.HotTypeID.Value, request.PageIndex, request.PageSize);
            }
            else
            {
                return ResponseResult.Failed("params error");
            }
            var orders = await _orderService.ListByIDs(CommonHelper.ListRandom(ids));
            if (orders?.Any() == true)
            {
                var comments = await _evaluateService.GetByOrderIDs(orders.Where(p => p.IsEvaluate).Select(p => p.ID));
                var userInfos = await Task.Run(() =>
                {
                    return _userService.GetTalentDetails(orders.Select(p => p.AnswerID).Distinct());
                });
                var questionContents = await _messageService.GetByOrders(orders.Select(p => p.ID).Distinct());
                var questions = new List<OrderPageExtend>();



                if (request.HotTypeID.HasValue)
                {
                    var sort_Scores = comments.Select(p => p.Score).Distinct().OrderByDescending(p => p);
                    var sort_OrderIDs = new List<Guid>();
                    foreach (var item in sort_Scores)
                    {
                        var temp_OrderIDs = comments.Where(p => p.Score == item).Select(p => p.OrderID.Value);
                        if (temp_OrderIDs?.Any() == true)
                        {
                            sort_OrderIDs.AddRange(CommonHelper.ListRandom(temp_OrderIDs));
                        }
                    }
                    orders = orders.OrderBy(p => Array.IndexOf(sort_OrderIDs.ToArray(), p.ID));
                }
                foreach (var item in orders)
                {
                    var order = CommonHelper.MapperProperty<Order, OrderPageExtend>(item);
                    if (questionContents.Any(p => p.OrderID == order.ID))
                    {
                        var content = questionContents.Where(p => p.OrderID == order.ID).OrderBy(p => p.CreateTime).First();
                        order.QuestionContent = content.Content;
                    }

                    var answerUserInfo = userInfos.FirstOrDefault(p => p.Id == order.AnswerID);
                    if (answerUserInfo != null)
                    {
                        order.HeadImgUrl = answerUserInfo.HeadImgUrl;
                        order.NickName = answerUserInfo.Nickname;
                    }
                    var comment = comments.FirstOrDefault(p => p.OrderID == item.ID);
                    if (comment != null)
                    {
                        order.Comment = comment;
                    }
                    questions.Add(order);
                }
                result.Data = questions.Select(p => new PageResponse()
                {
                    Content = p.TextContent,
                    HeadImgUrl = p.HeadImgUrl,
                    NickName = p.NickName,
                    Score = p.Comment?.Score ?? 1,
                    TalentUserID = p.AnswerID
                });
            }
            return result;
        }

        [HttpGet]
        [Description("根据热门分类获取案例")]
        public async Task<ResponseResult> PageQuestionByType([FromQuery] PageQuestionByTypeRequest request)
        {
            var result = ResponseResult.Success();
            if (request.HotTypeID == Guid.Empty)
            {
                return ResponseResult.Failed("参数错误");
            }

            var hotType = _hotTypeService.Get(request.HotTypeID);
            if (hotType == null) return ResponseResult.Failed("hot type not found");

            var sortType = request.SortType == 1 ? 2 : 3;
            var ids = await _hotTypeService.GetOrderIDByHotTypeID(request.HotTypeID, request.PageIndex, request.PageSize, sortType);
            var orders = await _orderService.ListByIDs(ids);
            if (orders?.Any() == true)
            {
                var userInfos = await Task.Run(() =>
                {
                    return _userService.GetTalentDetails(orders.Select(p => p.AnswerID).Distinct());
                });
                var questionContents = await _messageService.GetByOrders(orders.Select(p => p.ID).Distinct());
                var questions = new List<OrderPageExtend>();
                var viewCounts = await _hotTypeService.GetViewCountByOrderIDs(orders.Select(p => p.ID).Distinct());
                foreach (var id in ids)
                {
                    if (!orders.Any(p => p.ID == id)) continue;

                    var order = CommonHelper.MapperProperty<Order, OrderPageExtend>(orders.FirstOrDefault(p => p.ID == id));
                    if (questionContents.Any(p => p.OrderID == order.ID))
                    {
                        var content = questionContents.Where(p => p.OrderID == order.ID && p.MsgType == PMS.PaidQA.Domain.Enums.MsgType.Question).OrderBy(p => p.CreateTime).FirstOrDefault();
                        if (content != null)
                        {
                            order.QuestionContent = content.Content;
                        }
                        content = questionContents.Where(p => p.OrderID == order.ID && p.MsgType == PMS.PaidQA.Domain.Enums.MsgType.Answer).OrderBy(p => p.CreateTime).FirstOrDefault();
                        if (content != null)
                        {
                            order.ReplyContent = content.Content;
                        }
                    }

                    var answerUserInfo = userInfos.FirstOrDefault(p => p.Id == order.AnswerID);
                    if (answerUserInfo != null)
                    {
                        order.HeadImgUrl = answerUserInfo.HeadImgUrl;
                        order.NickName = answerUserInfo.Nickname;
                        order.AuthName = answerUserInfo.Certification_title;
                        order.Introduction = answerUserInfo.Introduction;
                        order.TalentUserID = answerUserInfo.Id;
                        order.TalentRole = answerUserInfo.Role ?? -1;
                    }
                    if (viewCounts.Any(p => p.Item1 == order.ID)) order.ViewCount = viewCounts.FirstOrDefault(p => p.Item1 == order.ID).Item2;
                    questions.Add(order);
                }

                result.Data = questions.Select(p => new PageQuestionByTypeResponse()
                {
                    AuthName = p.AuthName,
                    HeadImgUrl = p.HeadImgUrl,
                    HotTypeName = hotType.Name,
                    Introduction = p.Introduction,
                    NickName = p.NickName,
                    Question = p.TextContent,
                    Reply = p.TextContentReply,
                    TalentUserID = p.TalentUserID,
                    ViewCount = p.ViewCount,
                    OrderID = p.ID,
                    CreateTime = p.CreateTime,
                    TalentRole = p.TalentRole
                });
            }
            return result;
        }

        [HttpGet]
        [Description("获取热门案例详细")]
        public async Task<ResponseResult> GetHotQuestionDetail(Guid id)
        {
            var result = ResponseResult.Success();

            if (id == Guid.Empty) return ResponseResult.Failed("param error");

            var order = _orderService.Get(id);
            if (order == null) return ResponseResult.Failed("data not found");

            var isFollowed = false;//是否关注
            if (User.Identity.IsAuthenticated)
            {
                isFollowed = await Task.Run(() =>
                {
                    return _collectionService.IsCollected(User.Identity.GetId(), order.AnswerID);
                });
            }

            var hotQuestionDetail = await _hotTypeService.GetHotQuestionDetail(order.ID);

            await _hotTypeService.ChangeViewCount(order.ID);
            var userInfo = await Task.Run(() =>
            {
                return _userService.GetTalentDetail(order.AnswerID);
            });
            var questionContents = await _messageService.GetByOrders(new Guid[1] { order.ID });
            var questions = new List<OrderPageExtend>();
            var viewCounts = await _hotTypeService.GetViewCountByOrderIDs(new Guid[1] { order.ID });

            var orderEx = CommonHelper.MapperProperty<Order, OrderPageExtend>(order);
            if (questionContents.Any(p => p.OrderID == order.ID))
            {
                var content = questionContents.Where(p => p.OrderID == order.ID && p.MsgType == PMS.PaidQA.Domain.Enums.MsgType.Question).OrderBy(p => p.CreateTime).FirstOrDefault();
                if (content != null)
                {
                    orderEx.QuestionContent = content.Content;
                }
                content = questionContents.Where(p => p.OrderID == order.ID && p.MsgType == PMS.PaidQA.Domain.Enums.MsgType.Answer).OrderBy(p => p.CreateTime).FirstOrDefault();
                if (content != null)
                {
                    orderEx.ReplyContent = content.Content;
                }
            }

            if (userInfo != null)
            {
                orderEx.HeadImgUrl = userInfo.HeadImgUrl;
                orderEx.NickName = userInfo.Nickname;
                orderEx.AuthName = userInfo.Certification_title;
                orderEx.Introduction = userInfo.Introduction;
                orderEx.TalentUserID = userInfo.Id;
            }


            //viewCount++
            if (viewCounts.Any(p => p.Item1 == order.ID)) orderEx.ViewCount = viewCounts.FirstOrDefault(p => p.Item1 == order.ID).Item2;

            result.Data = new
            {
                Question = orderEx.TextContent,
                Reply = orderEx.TextContentReply,
                HeadImgUrl = orderEx.HeadImgUrl,
                NickName = orderEx.NickName,
                AuthName = orderEx.AuthName,
                Introduction = orderEx.Introduction,
                isFollowed,
                ViewCount = orderEx.ViewCount,
                TalentUserID = orderEx.AnswerID,
                TalentRole = userInfo.Role ?? -1,
                TypeName = hotQuestionDetail?.HotTypeName
            };

            return result;

        }
    }
}
