using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using PMS.PaidQA.Domain.Message;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.PaidQA.Domain.Enums;
using MediatR;
using PMS.MediatR.Events.PaidQA;
using PMS.MediatR.Request.PaidQA;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;
using ProductManagement.API.Aliyun;
using PMS.School.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Request.Org;
using ProductManagement.API.Http.Model.Org;
using ProductManagement.API.Http.Result.Org;
using AutoMapper;
using PMS.PaidQA.Domain.Dtos;

namespace PMS.PaidQA.Application.Services
{
    public class MessageService : ApplicationService<Message>, IMessageService
    {
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        //离线消息缓存保存时间
        int offlineMsgCacheDay = 7;
        IMessageRepository _messageRepository;
        IEasyRedisClient _easyRedisClient;
        ITalentSettingService _talentSettingService;
        IMediator _mediator;
        ILogger _logger;
        IOrderRepository _orderRepository;
        ISchoolService _schoolService;
        IArticleService _articleService;
        ISchoolRankService _schoolRankService;
        ILiveServiceClient _liveServiceClient;
        IOrgServiceClient _orgServiceClient;
        public MessageService(IMessageRepository messageRepository
            , IEasyRedisClient easyRedisClient
            , ITalentSettingService talentSettingService
            , IMediator mediator
            , ILogger<MessageService> logger
            , IOrderRepository orderRepository
            , ISchoolService schoolService
            , IArticleService articleService
            , ISchoolRankService schoolRankService
            , ILiveServiceClient liveServiceClient
            , IOrgServiceClient orgServiceClient) : base(messageRepository)
        {
            _messageRepository = messageRepository;
            _easyRedisClient = easyRedisClient;
            _talentSettingService = talentSettingService;
            _mediator = mediator;
            _logger = logger;
            _orderRepository = orderRepository;
            _schoolService = schoolService;
            _articleService = articleService;
            _schoolRankService = schoolRankService;
            _liveServiceClient = liveServiceClient;
            _orgServiceClient = orgServiceClient;
        }


        public async Task<bool> IsLive(Guid orderID, Guid userID)
        {
            return await _easyRedisClient.ExistsAsync($"RecordHeart_{orderID}_{userID}");
        }

        public async Task<int> GetAskCount(Guid OrderID)
        {
            return await _messageRepository.GetAskCount(OrderID);
        }

        public async Task<IEnumerable<Message>> GetByOrders(IEnumerable<Guid> orderIDs)
        {
            var str_Where = $"IsValid != 0 AND orderID in @orderIDs";
            IEnumerable<Message> result = new Message[0];
            await Task.Run(() =>
            {
                var finds = _messageRepository.GetBy(str_Where, new { orderIDs }, fileds: new string[1] { "*" });
                if (finds?.Any() == true) result = finds.OrderBy(p => p.OrderID).ThenBy(p => p.CreateTime);
            });
            return result;
        }

        public async Task<bool> CreateQuetion(Order order, List<Message> messages)
        {
            if (messages?.Any() == false)
            {
                return false;
            }

            bool isSuccess = await _messageRepository.CreateQuetion(messages, order);
            if (isSuccess)
            {
                foreach (var message in messages)
                {
                    //把提问消息推到接收者未读消息队列里
                    bool pushResult = await _easyRedisClient.ListPush(UnReadMsgCacheKey(order.ID, message.ReceiveID.GetValueOrDefault()), message, TimeSpan.FromDays(offlineMsgCacheDay));
                }
                //触发创建提问事件
                await _mediator.Publish(new CreateQuestionEvent(order));
                //通知消息发送事件触发。
                await _mediator.Publish(new SendMessageEvent(messages.FirstOrDefault(), order, SendMessageOrigin.User));
                return true;
            }
            else
            {
                return false;
            }



        }



        public async Task<bool> SendMessage(Message message, double latitude = 0, double longitude = 0, SendMessageOrigin sendMessageOrigin = SendMessageOrigin.User)
        {
            switch (message.MediaType)
            {
                case MsgMediaType.Text:
                    TextMessage textMessage = JsonConvert.DeserializeObject<TextMessage>(message.Content);
                    return await SendMessage(message, textMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.Image:
                    ImageMessage imageMessage = JsonConvert.DeserializeObject<ImageMessage>(message.Content);
                    return await SendMessage(message, imageMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.TXT:
                    TXTMessage txtMessage = JsonConvert.DeserializeObject<TXTMessage>(message.Content);
                    return await SendMessage(message, txtMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.SchoolCard:
                    SchoolCardMessage schoolCardMessage = JsonConvert.DeserializeObject<SchoolCardMessage>(message.Content);
                    return await SendMessage(message, schoolCardMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.SchoolRankCard:
                    SchoolRankCardMessage schoolRankCardMessage = JsonConvert.DeserializeObject<SchoolRankCardMessage>(message.Content);
                    return await SendMessage(message, schoolRankCardMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.ArticleCard:
                    ArticleCardMessage articleCardMessage = JsonConvert.DeserializeObject<ArticleCardMessage>(message.Content);
                    return await SendMessage(message, articleCardMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.LiveCard:
                    LiveCardMessage liveCardMessage = JsonConvert.DeserializeObject<LiveCardMessage>(message.Content);
                    return await SendMessage(message, liveCardMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.OrgCard:
                    OrgCardMessage orgCardMessage = JsonConvert.DeserializeObject<OrgCardMessage>(message.Content);
                    return await SendMessage(message, orgCardMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.OrgEvaluationCard:
                    OrgEvaluationCardMessage orgEvaluationCardMessage = JsonConvert.DeserializeObject<OrgEvaluationCardMessage>(message.Content);
                    return await SendMessage(message, orgEvaluationCardMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.CourseCard:
                    CourseCardMessage courseCardMessage = JsonConvert.DeserializeObject<CourseCardMessage>(message.Content);
                    return await SendMessage(message, courseCardMessage, latitude, longitude, sendMessageOrigin);
                case MsgMediaType.Voice:
                case MsgMediaType.RecommandCard:
                case MsgMediaType.RandomTenlentCard:
                case MsgMediaType.SystemStatu:
                case MsgMediaType.Custom:
                case MsgMediaType.System:
                default:
                    throw new Exception("该媒体类型暂时不支持Object方式发送，尝试使用SendMessage<T>泛型方式。");
            }
        }

        public async Task<bool> SendMessage<T>(Message message, T messageType, double latitude = 0, double longitude = 0, SendMessageOrigin sendMessageOrigin = SendMessageOrigin.System) where T : PaidQAMessage
        {
            Order order = this._orderRepository.Get(message.OrderID);
            message = InitialMessage<T>(order, message, messageType);
            try
            {
                bool sendResult = false;
                switch (message.MsgType)
                {
                    case MsgType.Custom:
                    case MsgType.System:
                        sendResult = await _messageRepository.AddAsync(message);
                        break;
                    case MsgType.Question:
                    case MsgType.GoAsk:
                        var userSendMessageResult = await _messageRepository.UserSendMessage(message);
                        sendResult = userSendMessageResult.Success;
                        if (userSendMessageResult.Order.AskRemainCount <= 0)
                        {
                            //触发用户没有提问机会事件
                            await _mediator.Publish(new UserAskRemainNoneEvent(order));
                        }
                        break;
                    case MsgType.Answer:
                        var talentSendMsgResult = await _messageRepository.TalentSendMessage(message);
                        sendResult = talentSendMsgResult.Success;
                        if (talentSendMsgResult.IsFirstReply)
                        {
                            //专家第一次回复
                            await _mediator.Publish(new TalentFirstReplyEvent(order.ID));
                        }
                        break;
                    default:
                        break;
                }
                if (sendResult)
                {
                    //解析msg中的content
                    await MessageContentHandle(new List<Message> { message }, latitude, longitude);
                    //把消息推到接收者未读消息队列里
                    bool pushResult = await PushUnReadMsg(order.ID, message.ReceiveID.Value, message);
                    //通知有消息发送事件触发。
                    await _mediator.Publish(new SendMessageEvent(message, order, sendMessageOrigin));
                }
                return sendResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "上学问消息发送失败。");
                return false;
            }

        }


        public async Task<bool> PushUnReadMsg(Guid orderID, Guid reciverID, Message message)
        {
            return await _easyRedisClient.ListPush(UnReadMsgCacheKey(orderID, reciverID), message, TimeSpan.FromDays(offlineMsgCacheDay));
        }


        public async Task<IEnumerable<Message>> GetChartRecord(Order order, Guid userID, double latitude, double longitude)
        {
            var list = _messageRepository.GetChartRecord(order, userID);
            //拉取聊天记录时，表明用户已读该批次消息
            var unreads = list.Where(l => l.ReceiveID == userID && l.ReadTime == null);
            await _messageRepository.UpdateMessageReadTime(unreads, null);
            //拉消息的时候处理一下消息类型
            await MessageContentHandle(list, latitude, longitude);
            //清空一下当前用户的未读消息缓存队列
            await _easyRedisClient.RemoveAsync(UnReadMsgCacheKey(order.ID, userID));
            return list;
        }


        public async Task<List<Message>> GetUnReadMessage(Order order, Guid userID, double latitude, double longitude, int repeatTime = 15)
        {
            int speed = 1;
            //记录用户提问页面在线心跳
            await _easyRedisClient.AddStringAsync($"RecordHeart_{order.ID}_{userID}", DateTime.Now.ToString(), TimeSpan.FromSeconds(repeatTime * speed));
            return await PollingTool<List<Message>>.StartPolling(async (pool) =>
            {
                //轮询未读消息读的是redis队列，不走数据库。
                List<Message> unreadList = await _easyRedisClient.ListPopAll<Message>(UnReadMsgCacheKey(order.ID, userID));
                if (unreadList != null && unreadList.Any())
                {
                    //更新一下队列里未读消息的读取时间。
                    await _messageRepository.UpdateMessageReadTime(unreadList, null);
                    await MessageContentHandle(unreadList, latitude, longitude);
                    pool.Stop = true;
                    pool.Data = unreadList;
                    return pool;
                }
                if (pool.Count >= repeatTime)
                {
                    pool.Stop = true;
                    pool.Data = new List<Message>();
                    return pool;
                }
                return pool;
            }, speed);


        }

        /// <summary>
        /// 有部分卡片需要提前渲染结构
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        async Task MessageContentHandle(IEnumerable<Message> messages, double latitude, double longitude)
        {
            if (messages == null || !messages.Any())
            {
                return;
            }
            var messageGroups = messages.GroupBy(msg => msg.MediaType);
            foreach (var msggroup in messageGroups)
            {
                switch (msggroup.Key)
                {
                    case MsgMediaType.RecommandCard:
                        //todo:
                        //需要查询卡片信息返回
                        foreach (var msg in msggroup)
                        {
                            var RecommandCardMessage = JsonConvert.DeserializeObject<RecommandCardMessage>(msg.Content);
                            var talentDetail = await _talentSettingService.GetDetail(RecommandCardMessage.TalentUserID);
                            var NewRecommandCardMessage = new
                            {
                                Content = RecommandCardMessage.Content,
                                TalentInfo = talentDetail,
                                Status = RecommandCardMessage.Status,
                                PriceSpread = RecommandCardMessage.PriceSpread,
                                TrantiyOrderID = RecommandCardMessage.TrantiyOrderID
                            };
                            msg.Content = JsonConvert.SerializeObject(NewRecommandCardMessage, jsonSerializerSettings);
                        }
                        break;
                    case MsgMediaType.RandomTenlentCard:
                        foreach (var msg in msggroup)
                        {
                            var RandomTenlentCardMessage = JsonConvert.DeserializeObject<RandomTenlentCardMessage>(msg.Content);
                            List<TalentDetailExtend> talentInfos = new List<TalentDetailExtend>();
                            foreach (var item in RandomTenlentCardMessage.TalentUserIDs)
                            {
                                talentInfos.Add(await _talentSettingService.GetDetail(item));
                            }
                            var NewRandomTenlentCardMessage = new
                            {
                                Content = RandomTenlentCardMessage.Content,
                                TalentInfos = talentInfos
                            };
                            msg.Content = JsonConvert.SerializeObject(NewRandomTenlentCardMessage, jsonSerializerSettings);

                        }
                        break;
                    case MsgMediaType.SchoolCard:
                        IDictionary<Guid, Guid> msg_schoolMap = new Dictionary<Guid, Guid>();
                        foreach (var msg in msggroup)
                        {
                            SchoolCardMessage schoolCardMessage = JsonConvert.DeserializeObject<SchoolCardMessage>(msg.Content);
                            if (Guid.TryParse(schoolCardMessage.Id, out Guid schoolId))
                            {
                                msg_schoolMap[msg.ID] = schoolId;
                            }
                        }
                        var schools = await _schoolService.ListExtSchoolByBranchIds(msg_schoolMap.Select(map => map.Value).ToList(), latitude, longitude);
                        if (schools?.Any() == true)
                        {
                            foreach (var msg in msggroup)
                            {
                                if (msg_schoolMap.TryGetValue(msg.ID, out Guid mapId))
                                {
                                    var school = schools.FirstOrDefault(s => s.ExtId == mapId);
                                    if (school != null)
                                    {
                                        msg.Content = JsonConvert.SerializeObject(school, jsonSerializerSettings);
                                    }
                                }
                            }
                        }
                        break;
                    case MsgMediaType.ArticleCard:
                        IDictionary<Guid, Guid> msg_articleMap = new Dictionary<Guid, Guid>();
                        foreach (var msg in msggroup)
                        {
                            ArticleCardMessage articleCardMessage = JsonConvert.DeserializeObject<ArticleCardMessage>(msg.Content);
                            if (Guid.TryParse(articleCardMessage.Id, out Guid articleId))
                            {
                                msg_articleMap[msg.ID] = articleId;
                            }
                        }
                        var articles = _articleService.GetArticles(msg_articleMap.Select(map => map.Value), false);
                        if (articles?.Any() == true)
                        {
                            foreach (var msg in msggroup)
                            {
                                if (msg_articleMap.TryGetValue(msg.ID, out Guid mapId))
                                {
                                    var article = articles.FirstOrDefault(a => a.Id == mapId);
                                    if (article != null)
                                    {
                                        msg.Content = JsonConvert.SerializeObject(article, jsonSerializerSettings);
                                    }
                                }
                            }
                        }
                        break;
                    case MsgMediaType.OrgEvaluationCard:
                        IDictionary<Guid, Guid> msg_evalutionMap = new Dictionary<Guid, Guid>();
                        foreach (var msg in msggroup)
                        {
                            OrgEvaluationCardMessage orgEvaluationCardMessage = JsonConvert.DeserializeObject<OrgEvaluationCardMessage>(msg.Content);
                            if (Guid.TryParse(orgEvaluationCardMessage.Id, out Guid evalutionId))
                            {
                                msg_evalutionMap[msg.ID] = evalutionId;
                            }
                        }
                        CardListResult<OrgEvalutionCard> orgEvalutionCardListResult = await _orgServiceClient.EvalsLablesByIds(new EvalsLablesByIdsRequest()
                        {
                            ids = msg_evalutionMap.Select(map => map.Value.ToString()).ToList()
                        });
                        if (orgEvalutionCardListResult != null && orgEvalutionCardListResult.succeed && orgEvalutionCardListResult.data?.Any() == true)
                        {
                            foreach (var msg in msggroup)
                            {
                                if (msg_evalutionMap.TryGetValue(msg.ID, out Guid mapId))
                                {

                                    var orgEvalution = orgEvalutionCardListResult.data.FirstOrDefault(evalution => evalution.id == mapId);
                                    if (orgEvalution != null)
                                    {
                                        msg.Content = JsonConvert.SerializeObject(orgEvalution, jsonSerializerSettings);
                                    }
                                }
                            }
                        }
                        break;
                    case MsgMediaType.OrgCard:
                        IDictionary<Guid, Guid> msg_orgMap = new Dictionary<Guid, Guid>();
                        foreach (var msg in msggroup)
                        {
                            OrgCardMessage orgCardMessage = JsonConvert.DeserializeObject<OrgCardMessage>(msg.Content);
                            if (Guid.TryParse(orgCardMessage.Id, out Guid orgId))
                            {
                                msg_orgMap[msg.ID] = orgId;
                            }
                        }
                        CardListResult<OrgCard> orgCardListResult = await _orgServiceClient.OrgsLablesByIds(new OrgsLablesByIdsRequest()
                        {
                            LongIds = msg_orgMap.Select(map => map.Value.ToString()).ToList()
                        });
                        if (orgCardListResult != null && orgCardListResult.succeed && orgCardListResult.data?.Any() == true)
                        {
                            foreach (var msg in msggroup)
                            {
                                if (msg_orgMap.TryGetValue(msg.ID, out Guid mapId))
                                {
                                    var org = orgCardListResult.data.FirstOrDefault(org => org.id == mapId);
                                    if (org != null)
                                    {
                                        msg.Content = JsonConvert.SerializeObject(org, jsonSerializerSettings);
                                    }
                                }
                            }
                        }
                        break;
                    case MsgMediaType.CourseCard:
                        IDictionary<Guid, Guid> msg_courseMap = new Dictionary<Guid, Guid>();
                        foreach (var msg in msggroup)
                        {
                            CourseCardMessage courseCardMessage = JsonConvert.DeserializeObject<CourseCardMessage>(msg.Content);
                            if (Guid.TryParse(courseCardMessage.Id, out Guid courseId))
                            {
                                msg_courseMap[msg.ID] = courseId;
                            }
                        }
                        CardListResult<CourseCard> courseCardListResult = await _orgServiceClient.CoursesLablesByIds(new CoursesLablesByIdsRequest()
                        {
                            LongIds = msg_courseMap.Select(map => map.Value.ToString()).ToList()
                        });
                        if (courseCardListResult != null && courseCardListResult.succeed && courseCardListResult.data?.Any() == true)
                        {
                            foreach (var msg in msggroup)
                            {
                                if (msg_courseMap.TryGetValue(msg.ID, out Guid mapId))
                                {
                                    var course = courseCardListResult.data.FirstOrDefault(course => course.id == mapId);
                                    if (course != null)
                                    {
                                        msg.Content = JsonConvert.SerializeObject(course, jsonSerializerSettings);
                                    }
                                }
                            }
                        }
                        break;
                    case MsgMediaType.LiveCard:
                        IDictionary<Guid, Guid> msg_LectureMap = new Dictionary<Guid, Guid>();
                        foreach (var msg in msggroup)
                        {
                            LiveCardMessage liveCardMessage = JsonConvert.DeserializeObject<LiveCardMessage>(msg.Content);
                            if (Guid.TryParse(liveCardMessage.Id, out Guid lectureId))
                            {
                                msg_LectureMap[msg.ID] = lectureId;
                            }
                        }
                        var lectureResult = await _liveServiceClient.QueryLectures(msg_LectureMap.Select(map => map.Value).ToList(), null);
                        if (lectureResult.Status == 0 && lectureResult.Items?.Any() == true)
                        {
                            foreach (var msg in msggroup)
                            {
                                if (msg_LectureMap.TryGetValue(msg.ID, out Guid mapId))
                                {
                                    var lecture = lectureResult.Items.FirstOrDefault(lecture => lecture.Id == mapId);
                                    if (lecture != null)
                                    {
                                        msg.Content = JsonConvert.SerializeObject(lecture, jsonSerializerSettings);
                                    }
                                }
                            }
                        }
                        break;
                    case MsgMediaType.SchoolRankCard:
                        IDictionary<Guid, Guid> msg_SchoolRankMap = new Dictionary<Guid, Guid>();
                        foreach (var msg in msggroup)
                        {
                            SchoolRankCardMessage schoolRankCardMessage = JsonConvert.DeserializeObject<SchoolRankCardMessage>(msg.Content);
                            if (Guid.TryParse(schoolRankCardMessage.Id, out Guid schoolRankId))
                            {
                                msg_SchoolRankMap[msg.ID] = schoolRankId;
                            }
                        }
                        var schoolRanks = _schoolRankService.GetSchoolRanks(msg_SchoolRankMap.Select(map => map.Value));
                        if (schoolRanks?.Any() == true)
                        {
                            foreach (var msg in msggroup)
                            {
                                if (msg_SchoolRankMap.TryGetValue(msg.ID, out Guid mapId))
                                {
                                    var schoolRank = schoolRanks.FirstOrDefault(sr => sr.Id == mapId);
                                    if (schoolRank != null)
                                    {
                                        msg.Content = JsonConvert.SerializeObject(schoolRank, jsonSerializerSettings);
                                    }
                                }

                            }
                        }

                        break;
                    default:
                        break;
                }

            }
        }


        /// <summary>
        /// 根据订单初始化消息
        /// </summary>
        /// <param name="order"></param>
        /// <param name="message"></param>
        Message InitialMessage<T>(Order order, Message message, T msgContentType) where T : PaidQAMessage
        {

            message.ID = Guid.NewGuid();
            message.IsValid = true;
            message.OrderID = order.ID;
            message.CreateTime = DateTime.Now;
            message.Content = JsonConvert.SerializeObject(msgContentType, jsonSerializerSettings);
            //系统和客服消息不能识别到接收者。
            switch (message.MsgType)
            {
                case MsgType.System:
                    message.SenderID = default(Guid);
                    break;
                case MsgType.Custom:
                    message.SenderID = default(Guid);
                    break;
                case MsgType.Question:
                case MsgType.GoAsk:
                    message.SenderID = order.CreatorID;
                    message.ReceiveID = order.AnswerID;
                    break;
                case MsgType.Answer:
                    message.SenderID = order.AnswerID;
                    message.ReceiveID = order.CreatorID;
                    break;
                default:
                    break;
            }
            return message;
        }



        public async Task UpdateTrantingMsgToHasTranstiy(Order oldOrder, Order newOrder)
        {
            try
            {
                string where = @"OrderID=@OrderID
AND MediaType=@MediaType";
                var msgs = _messageRepository.GetBy(where, new { OrderID = oldOrder.ID, MediaType = MsgMediaType.RecommandCard });
                foreach (var msg in msgs)
                {
                    var rcmsg = PaidQAMessage.Create<RecommandCardMessage>(msg);
                    rcmsg.TrantiyOrderID = newOrder.ID;
                    rcmsg.Status = RecommandCardMessageStatu.HasTransity;
                    msg.Content = rcmsg.Serialize();
                    await _messageRepository.UpdateAsync(msg, null, new[] { "Content" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新转单推送卡片状态异常。");
            }
        }



        /// <summary>
        /// 生成AnonyName
        /// </summary>
        /// <returns></returns>
        string GenerateAnonyName()
        {
            StringBuilder sb = new StringBuilder();
            char[] alphabet = new char[] {
            'a','b','c','d','e','f','g','h','i','j','k','l','n','m','o','p','q','x','w','u','v'
            };
            Random random = new Random();
            for (int i = 0; i < 2; i++)
            {
                sb.Append(alphabet[random.Next(0, alphabet.Length)]);
            }
            for (int i = 0; i < 5; i++)
            {
                sb.Append(random.Next(0, 9));
            }
            return sb.ToString();
        }


        /// <summary>
        /// 未读消息的缓存Key
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        string UnReadMsgCacheKey(Guid orderID, Guid userID)
        {
            return DesTool.Md5($"paidqaureadmsg_{orderID}_{userID}");
        }

        public async Task<Message> GetOrderQuetion(Guid orderID)
        {
            return await _messageRepository.GetOrderQuetion(orderID);
        }

        public async Task<IEnumerable<Message>> GetOrdersQuetion(IEnumerable<Guid> orderIDs)
        {
            return await _messageRepository.GetOrdersQuetion(orderIDs);
        }

        public async Task<bool> Online(Guid orderId, Guid userId)
        {
            return await _easyRedisClient.AddStringAsync($"RecordHeart_{orderId}_{userId}", DateTime.Now.ToString(), TimeSpan.FromSeconds(15));
        }

        public async Task<bool> Offline(Guid orderId, Guid userId)
        {
            return await _easyRedisClient.RemoveAsync($"RecordHeart_{orderId}_{userId}");
        }

        public async Task<bool> IsOnline(Guid orderId, Guid userId)
        {
            return await _easyRedisClient.ExistsAsync($"RecordHeart_{orderId}_{userId}");
        }

        public async Task<bool> UpdateMessageReadTime(Guid id)
        {
            return await _messageRepository.UpdateAsync(new Message()
            {
                ID = id,
                ReadTime = DateTime.Now
            }, null, new[] { "ReadTime" });
        }
    }



}
