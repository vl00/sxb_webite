using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Web.Areas.PaidQA.Models.Question;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Sxb.Web.Areas.PaidQA.Models.Order;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Application.Services;
using ProductManagement.Framework.Foundation;
using PMS.UserManage.Application.IServices;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using AutoMapper;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.Areas.PaidQA.Models.Talent;
using Sxb.Web.Utils;
using Sxb.Web.Areas.PaidQA.Models.Evaluate;
using PMS.UserManage.Application.ModelDto;
using PMS.PaidQA.Domain.Enums;
using PMS.MediatR.Request.PaidQA;
using MediatR;
using ProductManagement.API.Aliyun;
using Sxb.Web.Filters;
using static PMS.Search.Domain.Common.GlobalEnum;
using PMS.Search.Application.IServices;
using PMS.School.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Areas.PaidQA.Models;
using PMS.OperationPlateform.Domain.DTOs;
using ProductManagement.API.Http.Request.Org;
using ProductManagement.API.Http.Result.Org;
using ProductManagement.API.Http.Model.Org;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using PMS.SignalR.Hubs.PaidQAHub;
using PMS.SignalR.Clients.PaidQAClient;
using PMS.SignalR;
using ProductManagement.Framework.AspNetCoreHelper.Filters;

namespace Sxb.Web.Areas.PaidQA.Controllers
{

    [Route("PaidQA/[controller]/[action]")]
    public class QuestionController : ApiBaseController
    {
        string defaultHeadImg = "https://cos.sxkid.com/images/head.png";

        ISearchService _searchService;
        ISchService _schService;
        IOrderService _OrderService;
        IMessageService _messageService;
        IUserService _userService;
        IMapper _mapper;
        IEvaluateService _evaluateService;
        ITalentSettingService _talentSettingService;
        IEvaluateTagsService _evaluateTagsService;
        IEasyRedisClient _easyRedisClient;
        IText _text;
        private readonly IMediator _mediator;
        ISchoolService _schoolService;
        ISchoolRankService _schoolRankService;
        IArticleService _articleService;
        ILiveServiceClient _liveServiceClient;
        IOrgServiceClient _orgServiceClient;

        public QuestionController(IOrderService orderService,
            IMessageService messageService,
            IUserService userService,
            IMapper mapper,
            IEvaluateService evaluateService,
            ITalentSettingService talentSettingService,
            IEvaluateTagsService evaluateTagsService,
            IMediator mediator,
            IText text, IEasyRedisClient easyRedisClient
            , ISearchService searchService
            , ISchoolService schoolService
            , ISchService schService
            , ISchoolRankService schoolRankService
            , IArticleService articleService
            , ILiveServiceClient liveServiceClient
            , IOrgServiceClient orgServiceClient)
        {
            _OrderService = orderService;
            _messageService = messageService;
            _userService = userService;
            _mapper = mapper;
            _evaluateService = evaluateService;
            _talentSettingService = talentSettingService;
            _evaluateTagsService = evaluateTagsService;
            _mediator = mediator;
            _text = text;
            _easyRedisClient = easyRedisClient;
            _searchService = searchService;
            _schoolService = schoolService;
            _schService = schService;
            _schoolRankService = schoolRankService;
            _articleService = articleService;
            _liveServiceClient = liveServiceClient;
            _orgServiceClient = orgServiceClient;
        }



        [HttpGet]
        [Authorize]
        [Description("获取转单的原订单提问信息")]
        public async Task<ResponseResult<GetOrderOrginAskInfoResult>> GetOrderOrginAskInfo([FromQuery] GetOrderOrginAskInfoRequest request)
        {
            var order = _OrderService.Get(request.OrderID);
            if (order.OrginAskID != null)
            {
                var question = await _messageService.GetOrderQuetion(order.OrginAskID.Value);
                if (question != null)
                {
                    GetOrderOrginAskInfoResult result = new GetOrderOrginAskInfoResult();
                    result.IsPublic = order.IsPublic;
                    result.IsAnonymity = order.IsAnonymous.GetValueOrDefault();
                    result.MessageResult = _mapper.Map<MessageResult>(question);
                    return ResponseResult<GetOrderOrginAskInfoResult>.Success(result, "原订单提问信息");
                }
            }
            return ResponseResult<GetOrderOrginAskInfoResult>.Success(null, "找不到原订单提问信息");
        }


        [HttpPost]
        [Authorize]
        [Description("创建提问")]
        [ValidateWebContent(ContentParam = "request")]
        public async Task<ResponseResult<CreateQuestionResult>> CreateQuestion(CreateQuestionRequest request)
        {
            Order order = _OrderService.Get(request.OrderID);
            if (!PermisionCheck(order))
            {
                return ResponseResult<CreateQuestionResult>.Failed("无该订单操作权限");
            }
            if (!_OrderService.CheckOrderCanOperate(order))
            {
                return ResponseResult<CreateQuestionResult>.Failed("订单已禁止操作。");
            }
            if (order.Status != OrderStatus.WaitingAsk)
            {
                //处于待提问的状态才能创建提问
                return ResponseResult<CreateQuestionResult>.Failed("只有待提问状态的订单才能创建提问");
            }
            //订单信息
            order.IsAnonymous = request.IsAnonymity;
            order.IsPublic = request.IsPublic;
            //发送消息
            DateTime now = DateTime.Now;
            foreach (var message in request.Messages)
            {
                message.ID = Guid.NewGuid();
                message.SenderID = UserIdOrDefault;
                message.ReceiveID = order.AnswerID;
                message.OrderID = order.ID;
                message.MsgType = MsgType.Question;
                message.CreateTime = now;
                message.IsValid = true;
                now = now.AddMilliseconds(10); //拉开时间间距，保证消息的顺序性
            }
            var sendResult = await _messageService.CreateQuetion(order, request.Messages);

            if (request.Messages.Any(msg => msg.MediaType == MsgMediaType.Attachment))
            {
                //包含附件消息类型的提问关闭掉转单功能
                order.EnableTransiting = false;
                order.UpdateTime = DateTime.Now;
                await _OrderService.UpdateAsync(order, new[] { nameof(order.EnableTransiting),nameof(order.UpdateTime) });
            }

            if (sendResult)
            {
                //发送成功           
                CreateQuestionResult result = new CreateQuestionResult()
                {
                    //MsgID = message.ID,
                    OrderID = order.ID
                };
                return ResponseResult<CreateQuestionResult>.Success(result, "");

            }
            else
            {
                //发送失败
                return ResponseResult<CreateQuestionResult>.Failed("提问创建失败");
            }

        }



        [Description("发送消息（追问）")]
        [HttpPost]
        [Authorize]
        [ValidateWebContent(ContentParam = "request")]
        public async Task<ResponseResult<SendMsgResult>> SendMsg(SendMsgRequest request)
        {
            var order = _OrderService.Get(request.OrderID); 
            if (!PermisionCheck(order))
            {
                return ResponseResult<SendMsgResult>.Failed("无该订单操作权限");
            }
            if (!_OrderService.CheckOrderCanOperate(order))
            {
                return ResponseResult<SendMsgResult>.Failed("订单已禁止操作。");
            }
            Message message = new Message()
            {
                Content = request.Content,
                OrderID = request.OrderID,
                MediaType = request.MediaType,
            };
            if (UserIdOrDefault == order.CreatorID)
            {
                message.MsgType = PMS.PaidQA.Domain.Enums.MsgType.GoAsk;
            }
            else if (UserIdOrDefault == order.AnswerID)
            {
                message.MsgType = PMS.PaidQA.Domain.Enums.MsgType.Answer;
            }

            var addResult = await _messageService.SendMessage(message,Latitude,Longitude, SendMessageOrigin.User) ;
            order =  _OrderService.Get(order.ID);//更新订单信息
            if (addResult)
            {
                var userInfo = _userService.GetUserInfo(message.SenderID.GetValueOrDefault());
                SendMsgResult result = new SendMsgResult()
                {
                    Message = _mapper.Map<MessageResult>(message),
                    OrderInfo = _mapper.Map<OrderInfoResult>(order)
                };
                result.Message.SenderInfo = _mapper.Map<UserInfoResult>(userInfo);
                result.Message.IsBelongSelf = result.Message.SenderID == UserIdOrDefault;
                return ResponseResult<SendMsgResult>.Success(result, "");
            }
            else
            {
                return ResponseResult<SendMsgResult>.Failed("消息发送失败");
            }
        }


        [HttpGet]
        [Description("提问详情")]
        [Authorize]
        public async Task<ResponseResult<QuestionDetailResult>> QuestionDetail([FromQuery] QuestionDetailRequest request)
        {
            //获取订单信息
            Order order = _OrderService.Get(request.OrderID);

            if (!PermisionCheck(order))
            {
                return ResponseResult<QuestionDetailResult>.Failed("无该订单操作权限");
            }

            //获取订单中专家的信息
            var talent = await _talentSettingService.GetDetail(order.AnswerID,UserIdOrDefault);
            //获取当前用户的聊天记录
            var charts = await _messageService.GetChartRecord(order, UserIdOrDefault,Latitude,Longitude);
            var messageResults = _mapper.Map<List<MessageResult>>(charts);
            MessageAddSenderInfo(messageResults, order);
            UserInfoResult otherUser = GetOtherInfo(order);

            QuestionDetailResult result = new QuestionDetailResult()
            {
                Talent = _mapper.Map<TalentInfoResult>(talent),
                OrderInfo = _mapper.Map<OrderInfoResult>(order),
                ChatRecords = messageResults,
                IsReply = order.IsReply,
                IsAnswer = UserIdOrDefault == order.AnswerID,
                Other = otherUser
            };
            if (order.IsEvaluate)
            {
                //获取评价信息
                var evalute = (await _evaluateService.GetByOrderIDs(new List<Guid>() { order.ID })).FirstOrDefault();
                EvaluateResult evaluateResult = _mapper.Map<EvaluateResult>(evalute);
                var evaluteTags = await _evaluateTagsService.GetByEvaluateIDs(new List<Guid> { evalute.ID });
                if (evaluteTags != null && evaluteTags.Any())
                {
                    evaluateResult.Tags = evaluteTags.First().Value?.Select(t => t.Name).ToList();
                }
                result.Evaluate = evaluateResult;
            }

            return ResponseResult<QuestionDetailResult>.Success(result, "");
        }


        [HttpGet]
        [Description("获取消息列表(未读消息)")]
        [Authorize]
        public async Task<ResponseResult<GetMessagesResult>> GetMessages([FromQuery] GetMessagesRequest request)
        {
            Order order = _OrderService.Get(request.OrderID);
            if (!PermisionCheck(order))
            {
                return ResponseResult<GetMessagesResult>.Failed("无该订单操作权限");
            }
            GetMessagesResult result = new GetMessagesResult();
            List<Message> unreadList = await _messageService.GetUnReadMessage(order, UserIdOrDefault,Latitude,Longitude, request.RepeatTime);
            var messageResults = _mapper.Map<List<MessageResult>>(unreadList);
            MessageAddSenderInfo(messageResults,order);
            result.ChatRecords = messageResults;
            return ResponseResult<GetMessagesResult>.Success(result, "未读消息列表");
        }


        /// <summary>
        /// 最新回复
        /// </summary>
        /// <returns></returns>
        public ResponseResult PageNewReplyQuestions(PageNewReplyQuestionsRequest request)
        {
            var result = ResponseResult.Success();

            return result;
        }


        [HttpGet]
        [Description("搜索引用")]
        public async Task<ResponsePageResult> SearchRender(string keyWords,string id_s, ChannelIndex? channel,int pageIndex=1,int pageSize=20)
        {
            List<object> datas = new List<object>();
            if (!string.IsNullOrEmpty(id_s))
            {
                Regex base32regex = new Regex("[^A-Za-z0-9]+");
                if (!Guid.TryParse(id_s, out Guid Id) && base32regex.IsMatch(id_s))
                {
                    return ResponsePageResult.Failed("检测到ID中包含非正常字符。");
                }

                if (channel == null) {
                    return ResponsePageResult.Failed("ID query must set the arg `channel`.");
                }
                switch (channel.Value)
                {
                    case ChannelIndex.School:
                        //id查询
                        if (!Guid.TryParse(id_s, out Guid extId))
                        {
                            var schExt = await _schService.GetSchextSimpleInfoViaShortNo(id_s);
                            extId = schExt.Eid;
                        }
                        var schools = await _schoolService.ListExtSchoolByBranchIds( new List<Guid>() { extId },Latitude,Longitude);
                        if (schools?.Any() == true)
                        {
                            datas.AddRange(schools.Select(s => new
                            {
                                Channel = ChannelIndex.School,
                                Item = _mapper.Map<SearchRenderSchoolResult>(s)
                            }));
                        }
                        break;
                    case ChannelIndex.SchoolRank:
                        //id查询
                        Guid rankId;
                        long? rankId_no = null;
                        if (!Guid.TryParse(id_s, out rankId))
                        {
                            rankId_no = UrlShortIdUtil.Base322Long(id_s);
                        }
                        SchoolRank rank = _schoolRankService.GetSchoolRank(rankId, rankId_no);
                        if (rank != null)
                        {
                            datas.Add(new
                            {
                                Channel = ChannelIndex.SchoolRank,
                                Item = _mapper.Map<SearchRenderSchoolRanklResult>(rank)
                            });
                        }
                        break;
                    case ChannelIndex.Article:
                        //id查询
                        Guid articleId;
                        long? articleId_no = null;
                        if (!Guid.TryParse(id_s, out articleId))
                        {
                            articleId_no = UrlShortIdUtil.Base322Long(id_s);
                        }
                        ArticleDto article  = _articleService.GetArticle(articleId,articleId_no, false);
                        if (article != null)
                        {
                            datas.Add(new
                            {
                                Channel = ChannelIndex.Article,
                                Item = _mapper.Map<SearchRenderArticleResult>(article)
                            });
                        }
                        break;
                    case ChannelIndex.Live:
                        if (Guid.TryParse(id_s, out Guid letureId))
                        {
                          var lectureResult =  await  _liveServiceClient.QueryLectures(new List<Guid>() { letureId }, null);
                            if (lectureResult.Status == 0 && lectureResult.Items?.Any() == true)
                            {
                                datas.AddRange(lectureResult.Items.Select(lecture => new {
                                    Channel = ChannelIndex.Live,
                                    Item = lecture
                                }));
                            }
                        }
                        break;
                    case ChannelIndex.Org:
                        CardListResult<OrgCard> orgCardListResult = null;
                        if (Guid.TryParse(id_s, out Guid orgId))
                        {
                            orgCardListResult = await _orgServiceClient.OrgsLablesByIds(new OrgsLablesByIdsRequest() { 
                             LongIds = new List<string>{ orgId.ToString() }
                            });

                        }
                        else {
                            orgCardListResult = await _orgServiceClient.OrgsLablesById_ss(new  OrgsLablesById_SSRequest()
                            {
                                 id_ss = new List<string> { id_s }
                            });
                        }
                        if (orgCardListResult != null && orgCardListResult.succeed && orgCardListResult.data?.Any() == true)
                        {
                            datas.AddRange(orgCardListResult.data.Select(org => new
                            {
                                Channel = ChannelIndex.Org,
                                Item = org
                            }));
                        }
                        break;
                    case ChannelIndex.OrgEvaluation:
                        CardListResult<OrgEvalutionCard> orgEvalutionCardListResult = null;
                        if (Guid.TryParse(id_s, out Guid orgEvalutionId))
                        {
                            orgEvalutionCardListResult = await _orgServiceClient.EvalsLablesByIds(new  EvalsLablesByIdsRequest()
                            {
                                 ids = new List<string> { orgEvalutionId.ToString() }
                            });

                        }
                        else
                        {
                            orgEvalutionCardListResult = await _orgServiceClient.EvalsLablesById_ss(new  EvalsLablesById_SSRequest()
                            {
                                 id_ss = new List<string> { id_s }
                            });
                        }
                        if (orgEvalutionCardListResult != null && orgEvalutionCardListResult.succeed && orgEvalutionCardListResult.data?.Any() == true)
                        {
                            datas.AddRange(orgEvalutionCardListResult.data.Select(evalution => new
                            {
                                Channel = ChannelIndex.OrgEvaluation,
                                Item = evalution
                            }));
                        }
                        break;
                    case ChannelIndex.Course:
                        CardListResult<CourseCard> courseCardListResult = null;
                        if (Guid.TryParse(id_s, out Guid courseId))
                        {
                            courseCardListResult = await _orgServiceClient.CoursesLablesByIds(new  CoursesLablesByIdsRequest()
                            {
                                 LongIds = new List<string> { courseId.ToString() }
                            });
                        }
                        else
                        {
                            courseCardListResult = await _orgServiceClient.CoursesLablesById_ss(new  CoursesLablesById_SSRequest()
                            {
                                 id_ss = new List<string> { id_s }
                            });
                        }
                        if (courseCardListResult != null && courseCardListResult.succeed && courseCardListResult.data?.Any() == true)
                        {
                            datas.AddRange(courseCardListResult.data.Select(course => new
                            {
                                Channel = ChannelIndex.Course,
                                Item = course
                            }));
                        }
                        break;
                }
                return  ResponsePageResult.Success(datas, 1, "id query:OK");
            }
            else {

                //搜索引擎搜索
                var _channel = channel == null ?  ChannelIndex.School | ChannelIndex.Article | ChannelIndex.Live | ChannelIndex.SchoolRank:channel.Value; 
                var paginationResult = _searchService.SearchIds(keyWords, _channel,null, pageIndex, pageSize);
               var groupResult =  paginationResult.List.GroupBy(l => l.Channel);
                foreach (var group in groupResult)
                {
                    switch (group.Key)
                    {
                        case ChannelIndex.School:
                            var schools = await _schoolService.ListExtSchoolByBranchIds(group.Select(g=>g.Id).ToList(), Latitude, Longitude);
                            if (schools?.Any() == true)
                            {
                                datas.AddRange(schools.Select(s => new
                                {
                                    Channel = ChannelIndex.School,
                                    Item = _mapper.Map<SearchRenderSchoolResult>(s)
                                }));
                            
                           }
                            break;
                        case ChannelIndex.Article:
                            IEnumerable<ArticleDto> articles = _articleService.GetArticles(group.Select(g => g.Id), false);
                            if (articles?.Any() == true)
                            {
                                datas.AddRange(articles.Select(a => new
                                {
                                    Channel = ChannelIndex.Article,
                                    Item = _mapper.Map<SearchRenderArticleResult>(a)
                                }));

                            }
                            break;
                        case ChannelIndex.Live:
                            var lectureResult = await _liveServiceClient.QueryLectures(group.Select(g => g.Id).ToList(), null);
                            if (lectureResult.Status == 0 && lectureResult.Items?.Any() == true)
                            {
                                datas.AddRange(lectureResult.Items.Select(lecture => new {
                                    Channel = ChannelIndex.Live,
                                    Item = lecture
                                }));
                            }
                            break;
                        case ChannelIndex.SchoolRank:
                            IEnumerable<SchoolRank> schoolRanks = _schoolRankService.GetSchoolRanks(group.Select(g => g.Id));
                            if (schoolRanks?.Any() == true)
                            {
                                datas.AddRange(schoolRanks.Select(rank => new
                                {
                                    Channel = ChannelIndex.SchoolRank,
                                    Item = _mapper.Map<SearchRenderSchoolRanklResult>(rank)
                                }));
                            }
                            break;
                        case ChannelIndex.Org:
                            CardListResult<OrgCard> orgCardListResult = await _orgServiceClient.OrgsLablesByIds(new OrgsLablesByIdsRequest(){
                                LongIds = group.Select(g => g.Id.ToString()).ToList()
                            });
                            if (orgCardListResult != null && orgCardListResult.succeed && orgCardListResult.data?.Any() == true)
                            {
                                datas.AddRange(orgCardListResult.data.Select(org => new
                                {
                                    Channel = ChannelIndex.Org,
                                    Item = org
                                }));
                            }
                            break;
                        case ChannelIndex.OrgEvaluation:
                            CardListResult<OrgEvalutionCard> orgEvalutionCardListResult = await _orgServiceClient.EvalsLablesByIds(new EvalsLablesByIdsRequest(){
                                ids = group.Select(g => g.Id.ToString()).ToList()
                            });
                            if (orgEvalutionCardListResult != null && orgEvalutionCardListResult.succeed && orgEvalutionCardListResult.data?.Any() == true)
                            {
                                datas.AddRange(orgEvalutionCardListResult.data.Select(evalution => new
                                {
                                    Channel = ChannelIndex.OrgEvaluation,
                                    Item = evalution
                                }));
                            }
                            break;
                        case ChannelIndex.Course:
                            CardListResult<CourseCard> courseCardListResult   = await _orgServiceClient.CoursesLablesByIds(new CoursesLablesByIdsRequest(){
                                LongIds = group.Select(g => g.Id.ToString()).ToList()
                            });
                            if (courseCardListResult != null && courseCardListResult.succeed && courseCardListResult.data?.Any() == true)
                            {
                                datas.AddRange(courseCardListResult.data.Select(course => new
                                {
                                    Channel = ChannelIndex.Course,
                                    Item = course
                                }));
                            }
                            break;


                    }
                }
                return  ResponsePageResult.Success(datas, paginationResult.Total, "keyword search:OK");

            }
        }


        /// <summary>
        /// 订单的一些业务权限校验
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool PermisionCheck(Order order)
        {

            if (UserIdOrDefault != order.CreatorID && UserIdOrDefault != order.AnswerID)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取对话对方信息
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        UserInfoResult GetOtherInfo(Order order)
        {
            UserInfoDto otherInfo = null;
            if (UserIdOrDefault == order.CreatorID)
            {
                otherInfo = _userService.GetUserInfo(order.AnswerID);

            }
            else if (UserIdOrDefault == order.AnswerID)
            {
                otherInfo = _userService.GetUserInfo(order.CreatorID);
                if (order.IsAnonymous == true)
                {
                    otherInfo.NickName = order.AnonyName;
                    otherInfo.HeadImgUrl = defaultHeadImg;
                }
            }
            return _mapper.Map<UserInfoResult>(otherInfo);

        }


        /// <summary>
        /// 赋值消息的SenderInfo字段
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="order"></param>
        void MessageAddSenderInfo(List<MessageResult> messages, Order order)
        {
            messages.ForEach(msg =>
            {
                msg.IsBelongSelf = msg.SenderID == UserIdOrDefault;
                var userInfo = _userService.GetUserInfo(msg.SenderID.Value);
                msg.SenderInfo = _mapper.Map<UserInfoResult>(userInfo);
                 if (UserIdOrDefault == order.AnswerID
                    && order.IsAnonymous == true 
                    && msg.SenderID == order.CreatorID)
                {
                    //只有当前登录者是专家身份，并且消息是用户发的才需要屏蔽，专家消息不处理
                    msg.SenderInfo.NickName = order.AnonyName;
                    msg.SenderInfo.HeadImgUrl = defaultHeadImg;
                }
            
            });
        }



    }
}
