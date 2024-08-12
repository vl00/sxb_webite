using AutoMapper;
using PMS.RabbitMQ.Message;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.API.Aliyun;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Infrastructure.AppService;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.TopicCircle.Application.Services
{
    public class TopicService : ApplicationService<Topic>, ITopicService
    {
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataSearch _dataSearch;
        private readonly IDataImport _dataImport;
        private readonly IEasyRedisClient _easyRedisClient;
        public static string _redisTagKey = "iSchoolTopicCircle:Tag";

        private readonly ITopicRepository _topicRepository;
        private readonly ITopicReplyRepository _topicReplyRepository;
        private readonly ITopicReplyLikeRepository _topicReplyLikeRepository;
        private readonly ITopicReplyImageRepository _topicReplyImageRepository;
        private readonly ITopicReplyAttachmentRepository _topicReplyAttachmentRepository;
        private readonly ICircleRepository _circleRepository;
        private readonly IToppingRepository _toppingRepository;
        private readonly ICircleFollowerRepository _circleFollowerRepository;
        private readonly ITopicTagRepository _topicTagRepository;

        private readonly ICollectionRepository _collectionRepository;
        private readonly IUserRepository _userRepository;


        public TopicService(TopicCircleDBContext unitOfWork, ITopicRepository topicRepository, ITopicReplyRepository topicReplyRepository, ICircleRepository circleRepository, ITopicReplyImageRepository topicReplyImageRepository, ITopicReplyAttachmentRepository topicReplyAttachmentRepository, IMapper mapper, ITopicReplyLikeRepository topicReplyLikeRepository, ICollectionRepository collectionRepository, IToppingRepository toppingRepository, IUserRepository userRepository, ICircleFollowerRepository circleFollowerRepository, ITopicTagRepository topicTagRepository, IEasyRedisClient easyRedisClient, IDataSearch dataSearch, IDataImport dataImport, IEventBus eventBus) : base(topicRepository)
        {
            _unitOfWork = unitOfWork;
            _topicRepository = topicRepository;
            _topicReplyRepository = topicReplyRepository;
            _circleRepository = circleRepository;
            _topicReplyImageRepository = topicReplyImageRepository;
            _topicReplyAttachmentRepository = topicReplyAttachmentRepository;
            _mapper = mapper;
            _topicReplyLikeRepository = topicReplyLikeRepository;
            _collectionRepository = collectionRepository;
            _toppingRepository = toppingRepository;
            _userRepository = userRepository;
            _circleFollowerRepository = circleFollowerRepository;
            _topicTagRepository = topicTagRepository;
            _easyRedisClient = easyRedisClient;
            _dataSearch = dataSearch;
            _dataImport = dataImport;
            _eventBus = eventBus;
        }

        public List<TopicDto> GetByIds(IEnumerable<Guid> ids, Guid? loginUserId)
        {
            //var topics = _topicRepository.GetBy(" Id in @ids ", new { ids });
            var topics = _topicRepository.GetList(ids);
            var data = _mapper.Map<List<TopicDto>>(topics);
            var topicIds = data.Select(s => s.Id).ToList();

            //图片
            IEnumerable<TopicReplyImage> images = _topicReplyImageRepository.GetList(topicIds);
            data.AsParallel().ForAll(topic =>
            {
                topic.Images = images.Where(s => s.TopicReplyId == topic.Id).OrderBy(s => s.Sort).ToList();
            });

            //登录用户的点赞关注情况
            AssembleLikeFollow(loginUserId, topicIds, data);
            return data;
        }
        public List<TopicDto> GetByIds(IEnumerable<Guid> ids)
        {
            var topics = _topicRepository.GetByIDs(ids);
            var data = _mapper.Map<List<TopicDto>>(topics);
            var topicIds = data.Select(s => s.Id).ToList();

            //图片
            IEnumerable<TopicReplyImage> images = _topicReplyImageRepository.GetList(topicIds);
            data.AsParallel().ForAll(topic =>
            {
                topic.Images = images.Where(s => s.TopicReplyId == topic.Id).OrderBy(s => s.Sort).ToList();
            });
            return data;
        }


        public async Task<IEnumerable<SimpleTagDto>> GetTags()
        {
            var tags = await _easyRedisClient.GetAsync<IEnumerable<SimpleTagDto>>(_redisTagKey);
            if (tags == null || !tags.Any())
            {
                tags = _topicTagRepository.GetTags();

                _ = _easyRedisClient.AddAsync(_redisTagKey, tags, TimeSpan.FromDays(1));
            }
            return tags;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="topicDto"></param>
        /// <returns></returns>
        public AppServiceResultDto<object> Add(TopicAddDto topicDto)
        {
            var circle = _circleRepository.Get(topicDto.CircleId);
            if (circle == null)
                return AppServiceResultDto.Failure<object>("话题圈已关闭");
            if (topicDto.Tags != null && topicDto.Tags.Count > 3)
                return AppServiceResultDto.Failure<object>("最多选择3个标签");
            if (topicDto.Images != null && topicDto.Images.Count > 9)
                return AppServiceResultDto.Failure<object>("最多选择9张图片");

            //发帖人是圈主, 并且非同步话题
            if (circle.UserId == topicDto.UserId && !topicDto.IsAutoSync)
            {
                if (topicDto.Tags == null || topicDto.Tags.Count == 0)
                    return AppServiceResultDto.Failure<object>("请您选择标签");

                //圈主发帖页无“只给圈主看”“标为问题”选项
                topicDto.IsQA = false;
                topicDto.IsOpen = true;
            }

            //对于attachemnt无内容. 或者类型错误,  置为空
            if (topicDto.Attachment != null && (string.IsNullOrWhiteSpace(topicDto.Attachment.Content) || topicDto.Attachment.Type == TopicType.None))
                topicDto.Attachment = null;


            _unitOfWork.BeginTransaction();

            Guid replyId = Guid.NewGuid();
            Guid topicId = replyId;

            //添加话题
            var topic = new Topic(topicDto.IsOpen, topicId, topicDto.Content, topicDto.IsQA, topicDto.UserId, circle)
            {
                IsAutoSync = topicDto.IsAutoSync
            };

            //添加评论, 第一条评论即话题
            var reply = new TopicReply(topicId, topicDto.Content, topicDto.UserId);
            _topicReplyRepository.Add(reply);

            if (topicDto.Tags != null && topicDto.Tags.Count > 0)
            {
                _topicTagRepository.AddOrUpdate(topicDto.Tags.Select(s => new TopicTag() { TopicId = topicId, TagId = s }));
            }

            if (topicDto.Images != null && topicDto.Images.Count > 0)
            {
                var images = topicDto.Images.Select(s => new TopicReplyImage()
                {
                    Id = Guid.NewGuid(),
                    TopicReplyId = reply.Id,
                    Url = s.Url,
                    Sort = s.Sort
                });
                topic.SetImages(images);
                //添加图片
                _topicReplyImageRepository.AddOrUpdate(images);
            }

            if (topicDto.Attachment != null)
            {
                var hasAttachment = topic.SetAttachment(reply.Id, topicDto.Attachment.Content, topicDto.Attachment.AttchId, topicDto.Attachment.AttachUrl, topicDto.Attachment.Type);
                if (hasAttachment)
                    //添加关联
                    _topicReplyAttachmentRepository.Add(topic.Attachment);
            }

            //避免无效值
            topic.Type = topic.Type == 0 ? 1 : topic.Type;
            _topicRepository.Add(topic);

            _unitOfWork.Commit();
            ImportTopicData(topicId);

            //消息通知话题添加成功,  给用户发送消息
            _eventBus.Publish(new SyncTopicAddMessage()
            {
                Id = topicId
            });

            return AppServiceResultDto.Success<object>(new { Id = topicId });
        }

        /// <summary>
        /// 达人同步话题 - 新增/更新
        /// </summary>
        /// <param name="topicDto"></param>
        /// <returns></returns>
        public AppServiceResultDto AddAutoSyncTopic(TopicAddDto topicDto)
        {
            //获取达人的话题圈
            var circle = _circleRepository.GetByUserId(topicDto.UserId);
            if (circle == null)
                return AppServiceResultDto.Failure("话题圈已关闭");
            if (topicDto.Attachment == null)
                return AppServiceResultDto.Failure("话题圈已关闭");

            var attachId = topicDto.Attachment.AttchId;
            if (attachId == null)
                return AppServiceResultDto.Failure("自动同步的附件无Id");

            var topic = _topicRepository.GetAutoSyncTopic(attachId.Value);
            if (topic == null)
            {
                //未添加过, 新增
                topicDto.CircleId = circle.Id;
                topicDto.IsOpen = true;
                topicDto.IsAutoSync = true;
                return Add(topicDto);
            }

            topicDto.IsOpen = true;
            //已有, 更新
            return UpdateAutoSyncTopic(topic, topicDto);
        }

        /// <summary>
        /// 达人同步话题 - 更新
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="topicDto"></param>
        /// <returns></returns>
        private AppServiceResultDto UpdateAutoSyncTopic(Topic topic, TopicAddDto topicDto)
        {
            var attachment = _topicReplyAttachmentRepository.GetList(new List<Guid> { topic.Id }).FirstOrDefault();
            if (attachment == null)
                return AppServiceResultDto.Failure("附件为空");

            topic.Content = topicDto.Content;
            topic.Updator = topicDto.UserId;

            _unitOfWork.BeginTransaction();
            //更新话题内容
            _topicRepository.UpdateContent(topic);

            //更新话题对应的评论的内容
            _topicReplyRepository.UpdateContent(new TopicReply()
            {
                Id = topic.Id,
                Content = topic.Content,
                Updator = topic.Updator
            });

            //更新附件内容
            attachment.Content = topicDto.Attachment.Content;
            attachment.AttachUrl = topicDto.Attachment.AttachUrl;
            _topicReplyAttachmentRepository.Update(attachment);

            _unitOfWork.Commit();
            return AppServiceResultDto.Success("自动同步的附件无Id");
        }

        private List<SearchTopic> GetSearchTopicData(Guid id)
        {
            var topic = _topicRepository.Get(id);
            if (topic == null)
                return null;
            var dto = _mapper.Map<Topic, SearchTopic>(topic);

            var topicTags = topic.Tags.ToList();
            IEnumerable<SimpleTagDto> tags = GetTags().Result;
            dto.Tags = tags.Where(s => topicTags.Exists(q => q.TagId == s.Id)).Select(s => new SearchTopic.SearchTag()
            {
                Id = s.Id,
                Name = $"{s.ParentName}|{s.Name}"
            }).ToList();

            return new List<SearchTopic>() { dto };
        }

        public void ImportTopicData(Guid id)
        {
            Task.Run(() =>
            {
                var data = GetSearchTopicData(id);
                if (data != null)
                {
                    _dataImport.ImportTopicData(data);
                }
            }).Wait();
        }

        public void UpdateTopicData(Guid id)
        {
            Task.Run(() =>
            {
                var data = GetSearchTopicData(id);
                if (data != null)
                {
                    _dataImport.UpdateTopicData(data);
                }
            }).Wait();
        }

        public void DeleteTopicData(Guid id)
        {
            var topic = Get(id);
            if (topic == null)
                return;
            var dto = _mapper.Map<Topic, SearchTopic>(topic);
            var data = new List<SearchTopic>() { dto };
            _dataImport.UpdateTopicData(data);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="topicDto"></param>
        /// <returns></returns>
        public AppServiceResultDto Edit(TopicAddDto topicDto)
        {
            var topicId = topicDto.Id;
            var replyId = topicId;

            var circle = _circleRepository.Get(topicDto.CircleId);
            if (circle == null)
                return AppServiceResultDto.Failure("话题圈已关闭");
            Topic topic = _topicRepository.Get(topicId);
            if (topic == null)
                return AppServiceResultDto.Failure("话题已关闭");
            TopicReply reply = _topicReplyRepository.Get(replyId);
            if (reply == null)
                return AppServiceResultDto.Failure("话题已关闭");
            if (topicDto.Tags != null && topicDto.Tags.Count > 3)
                return AppServiceResultDto.Failure("最多选择3个标签");
            if (topicDto.Images != null && topicDto.Images.Count > 9)
                return AppServiceResultDto.Failure("最多选择9张图片");

            //发帖人是圈主
            if (circle.UserId == topicDto.UserId && !topicDto.IsAutoSync)
            {
                if (topicDto.Tags == null || topicDto.Tags.Count == 0)
                    return AppServiceResultDto.Failure<object>("请您选择标签");

                //圈主发帖页无“只给圈主看”“标为问题”选项
                topicDto.IsQA = false;
                topicDto.IsOpen = true;
            }

            //对于attachemnt无内容. 或者类型错误,  置为空
            if (topicDto.Attachment != null && (string.IsNullOrWhiteSpace(topicDto.Attachment.Content) || topicDto.Attachment.Type == TopicType.None))
                topicDto.Attachment = null;


            var transaction = _unitOfWork.BeginTransaction();

            topic.SetTopic(topicDto.IsOpen, topicId, topicDto.Content, topicDto.IsQA, topic.Creator, topicDto.UserId, circle);

            #region Tags
            if (topicDto.Tags != null && topicDto.Tags.Count > 0)
            {
                var tags = topicDto.Tags.Select(s => new TopicTag()
                {
                    TopicId = topicId,
                    TagId = s
                });
                //添加标签
                _topicTagRepository.AddOrUpdate(tags);
            }

            //被删除的标签
            IEnumerable<TopicTag> deleteTagss = topic.Tags?.Where(s => !topicDto.Tags.Exists(e => e == s.TagId));
            if (deleteTagss != null && deleteTagss.Count() > 0)
            {
                _topicTagRepository.Delete(deleteTagss);
            }
            #endregion

            #region Images
            // 1.被删除的图片
            IEnumerable<TopicReplyImage> deleteImages = topic.Images?.Where(s => !topicDto.Images.Exists(e => e.Id == s.Id));
            if (deleteImages != null && deleteImages.Count() > 0)
            {
                _topicReplyImageRepository.Delete(deleteImages);
            }

            //2. 保留的图片, 并把图片赋值到topic
            if (topicDto.Images != null && topicDto.Images.Count > 0)
            {
                var images = topicDto.Images.Select(s => new TopicReplyImage()
                {
                    Id = s.Id == default ? Guid.NewGuid() : s.Id,
                    TopicReplyId = reply.Id,
                    Url = s.Url,
                    Sort = s.Sort
                });
                topic.SetImages(images);
                //添加图片
                _topicReplyImageRepository.AddOrUpdate(topic.Images);
            }
            #endregion

            #region Attachment
            TopicReplyAttachment attachment = topic.Attachment;
            //系统有  传入无, 则删除
            if (attachment != null && topicDto.Attachment == null)
            {
                attachment.IsDeleted = 1;
                _topicReplyAttachmentRepository.Delete(attachment);
            }
            //传入有, 则更新/添加
            else if (topicDto.Attachment != null)
            {
                var hasAttachment = topic.SetAttachment(reply.Id, topicDto.Attachment.Content, topicDto.Attachment.AttchId, topicDto.Attachment.AttachUrl, topicDto.Attachment.Type);
                if (hasAttachment)
                    _topicReplyAttachmentRepository.AddOrUpdate(topic.Attachment);
            }
            #endregion

            //修改话题评论
            reply.Content = topicDto.Content;
            reply.Updator = topicDto.UserId;
            _topicReplyRepository.Update(reply, transaction, new string[] { "Content", "Updator" });

            //修改话题
            _topicRepository.Update(topic);

            _unitOfWork.Commit();
            UpdateTopicData(topicId);
            return AppServiceResultDto.Success();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public AppServiceResultDto Delete(Guid id, Guid userId)
        {
            var topic = _topicRepository.Get(id);
            if (topic == null)
                return AppServiceResultDto.Failure("话题已关闭");
            var circle = _circleRepository.Get(topic.CircleId);
            if (circle == null)
                return AppServiceResultDto.Failure("话题圈已关闭");

            //仅 创建人 和 圈主 可以删除话题
            if (userId != topic.Creator && userId != circle.UserId)
                return AppServiceResultDto.Failure("只能删除自己的话题");

            _unitOfWork.BeginTransaction();

            topic.Updator = userId;
            //删除话题
            _topicRepository.Delete(topic);
            if (topic.Replies != null && topic.Replies.Count() > 0)
            {
                //删除话题评论
                topic.Replies.AsParallel().ForAll(reply =>
                {
                    reply.Updator = userId;
                    _topicReplyRepository.Delete(reply);
                });
            }

            _unitOfWork.Commit();

            DeleteTopicData(id);
            return AppServiceResultDto.Success();
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id"></param>
        /// <param name="topicReplyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public AppServiceResultDto DeleteReply(Guid id, Guid topicReplyId, Guid userId)
        {
            var topic = _topicRepository.Get(id);
            if (topic == null)
                return AppServiceResultDto.Failure("话题已关闭");
            var topicReply = topic.Replies.Where(s => s.Id == topicReplyId).FirstOrDefault();
            if (topicReply == null || topicReply.Depth == 0)
                return AppServiceResultDto.Failure("评论已关闭");

            _unitOfWork.BeginTransaction();

            //删除评论
            topicReply.Updator = userId;
            _topicReplyRepository.Delete(topicReply);
            //删除评论下的回复
            var children = topic.GetReplyChildren(topicReplyId);
            children.AsParallel().ForAll(reply =>
            {
                reply.Updator = userId;
                _topicReplyRepository.Delete(reply);
            });

            //评论数 = 原先评论数 - 已删除评论数
            var replyCount = topic.ReplyCount - children.Count() - 1;
            _topicRepository.UpdateReplyTotal(id, replyCount, null);

            _unitOfWork.Commit();

            return AppServiceResultDto.Success();
        }

        /// <summary>
        /// 收藏/取消收藏
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public AppServiceResultDto Follow(Guid id, Guid userId)
        {
            var topic = _topicRepository.Get(id);
            var collection = _collectionRepository.GetCollection(new List<Guid>() { id }, userId).FirstOrDefault();

            if (topic == null && (collection == null || collection == Guid.Empty))
                return AppServiceResultDto.Failure("话题已关闭");

            //对于删除话题, 关联没有删除的
            //执行, 删除关联

            int diff;
            bool result;
            if (collection == null || collection == Guid.Empty)
            {
                diff = 1;
                result = _collectionRepository.AddCollection(userId, id, (byte)CollectionDataType.Topic);
            }
            else
            {
                diff = -1;
                result = _collectionRepository.RemoveCollection(userId, id);
            }

            if (result && topic != null)
            {
                _topicRepository.UpdateFollowTotal(id, topic.FollowCount + diff);
            }
            return AppServiceResultDto.Success();
        }

        /// <summary>
        /// 获取话题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleTopicDto GetSimple(Guid id)
        {
            return _topicRepository.GetSimple(id);
        }

        /// <summary>
        /// 话题详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TopicDto> Get(Guid id, Guid? userId, int childCount = 0)
        {
            var topic = _topicRepository.Get(id);

            var dto = _mapper.Map<TopicDto>(topic);
            if (dto == null) return dto;

            //获取达人的话题圈
            var circle = _circleRepository.Get(topic.CircleId);
            if (circle == null) return dto;

            dto.CircleUserId = circle.UserId;
            dto.CircleName = circle.Name;
            dto.FollowCount = circle.FollowCount;
            dto.CircleTopicCount = circle.TopicCount;
            dto.CircleIntro = circle.Intro;
            dto.CircleFollowCount = circle.FollowCount;
            dto.CircleTopicCount = circle.TopicCount;

            //获取发帖人信息
            var user = _userRepository.GetUserInfo(topic.Creator);
            if (user == null) return dto;

            dto.UserName = user.NickName;
            dto.HeadImgUrl = user.HeadImgUrl;

            var replies = _mapper.Map<List<TopicReplyDto>>(topic.GetChildren(childCount));

            //评论
            dto.Replies = replies.Where(s => s.Depth == 1).ToList();
            //评论的评论
            dto.Replies.ForEach(s => s.Children = replies.Where(t => t.FirstParentId == s.Id).ToList());

            var topicTags = topic.Tags.ToList();
            IEnumerable<SimpleTagDto> tags = await GetTags();
            dto.Tags = tags.Where(s => topicTags.Exists(q => q.TagId == s.Id)).ToList();

            //话题前十个点赞人
            IEnumerable<TopicReplyLike> likes = _topicReplyLikeRepository.GetPagination(id, null, 1, 10);
            dto.LikeUserNames = likes.Select(s => s.UserName).ToList();

            //用户的点赞和收藏情况
            if (userId != null && userId != Guid.Empty)
            {
                //是否登录
                dto.IsLogin = true;
                //登录人是否是话题圈的粉丝
                dto.IsCircleFollower = _circleFollowerRepository.GetBy(" UserId =@userId and CircleId = @circleId ", new { userId, circleId = circle.Id }).Any();

                var replyIds = topic.Replies.Select(s => s.Id).ToList();
                IEnumerable<Guid> likeDataIds = _topicReplyLikeRepository.GetIdList(replyIds, userId.Value);
                List<Guid> collectionDataIds = _collectionRepository.GetCollection(replyIds, userId.Value);

                dto.Like = likeDataIds.Contains(id);
                dto.Follow = collectionDataIds.Contains(id);

                dto.IsCircleOwner = circle?.UserId == dto.Creator;
                dto.IsLoginUserOwner = userId == dto.Creator;

                //评论点赞情况
                replies.ForEach(s =>
                {
                    s.Like = likeDataIds.Contains(s.Id);
                    s.IsLoginUserOwner = s.Creator == userId;
                });
            }
            return dto;
        }

        /// <summary>
        /// 获取话题列表
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="userId"></param>
        /// <param name="topType"></param>
        /// <param name="isGood"></param>
        /// <param name="isQA"></param>
        /// <returns></returns>
        public IEnumerable<TopicDto> GetList(Guid? circleId, Guid? userId, TopicTopType? topType, bool? isGood, bool? isQA)
        {
            IEnumerable<Topic> source = _topicRepository.GetList(circleId, userId, topType, isGood, isQA);
            return _mapper.Map<IEnumerable<TopicDto>>(source);
        }

        /// <summary>
        /// 我的圈子动态(显示条数：3条)
        /// 显示用户关注所有圈子里有最新动态的帖子。最新动态指：有最新回复的帖子，或最新发出/编辑的帖子
        /// 按帖子最新动态的时间降序排列。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<TopicDto> GetDynamicList(Guid userId, int pageIndex = 1, int pageSize = 3)
        {
            //关注的圈子
            IEnumerable<CircleFollower> followers = _circleFollowerRepository.GetBy(" UserId = @userId ", new { userId });
            IEnumerable<Circle> circles = _circleRepository.GetCircles(userId);
            if (!followers.Any() && !circles.Any())
                return default;

            var circleIds = followers.Where(s => s.CircleId != null).Select(s => s.CircleId);
            circleIds = circles.Select(s => s.Id).Concat(circleIds);

            bool? isOpen = true;
            var topics = _topicRepository.GetPagination(string.Empty, circleIds, null, null, null, null, null, isOpen, tags: null, null, null, TopicSort.Time, pageIndex, pageSize);
            var data = _mapper.Map<IEnumerable<TopicDto>>(topics);
            return data;
        }

        /// <summary>
        /// 获取圈子内热门话题(显示数量：5个)
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        public IEnumerable<TopicDto> GetCircleHotList(Guid circleId, int pageIndex, int pageSize)
        {
            bool? isOpen = true;
            var topics = _topicRepository.GetPagination(string.Empty, circleId, null, type: null, topType: null, isGood: null, isQA: null, isOpen, tags: null, null, null, TopicSort.Time, pageIndex, pageSize);
            var data = _mapper.Map<IEnumerable<TopicDto>>(topics);
            AssembleImageAttachment(data.Select(s => s.Id).ToList(), data);
            return data;
        }

        /// <summary>
        /// 获取热门话题列表(显示总数量：10个)
        /// 1.管理员置顶话题
        /// 2.同城达人的话题
        /// 3.全无, 搜其他话题
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        public IEnumerable<TopicDto> GetHotList(Guid? userId, int? cityCode)
        {
            //热门话题最大数量
            int max = 10;

            //1.管理员置顶话题
            var topics = _topicRepository.GetPagination(circleId: null, TopicSort.Reply, 1, max).ToList();
            var total = topics.Count;
            if (total < max)//不足10条
            {
                //int? cityCode = null;
                if (userId != null && userId != Guid.Empty)
                    cityCode = _userRepository.GetUserInfo(userId.Value)?.City;

                //排除已置顶的话题
                var excludeIds = topics.Select(s => s.Id).ToList();
                if (!excludeIds.Any())
                    excludeIds.Add(Guid.Empty);

                //2.同城话题
                var surplus = max - total;
                var cityTopics = _topicRepository.GetPagination(cityCode, excludeIds, 1, surplus,true);
                topics.AddRange(cityTopics);
            }

            var data = _mapper.Map<IEnumerable<TopicDto>>(topics);
            AssembleImageAttachment(data.Select(s => s.Id).ToList(), data);
            return data;
        }

        public async Task<IEnumerable<SimpleTopicDto>> GetHotList(Guid? userId, int? cityCode, int? count)
        {
            //热门话题最大数量
            int max = count ?? 10;

            var topics = new List<SimpleTopicDto>();

            //1.管理员置顶话题
            var finds = _topicRepository.GetPagination(circleId: null, TopicSort.Reply, 1, max).ToList();
            if (finds?.Any() == true) topics.AddRange(finds);
            if (topics.Count() < 10)
            {
                if (userId != null && userId != Guid.Empty) cityCode = _userRepository.GetUserInfo(userId.Value)?.City;

                //2.同城话题
                var cityTopics = _topicRepository.GetPagination(cityCode, topics.Select(p => p.Id), 1, max - topics.Count(), true);
                if (cityTopics?.Any() == true) topics.AddRange(cityTopics);
            }

            if (topics.Any(p => p.Type >= 2))
            {
                var images = _topicReplyImageRepository.GetList(topics.Where(p => p.Type >= 2).Select(p => p.Id)) ?? new List<TopicReplyImage>();
                var attachments = _topicReplyAttachmentRepository.GetList(topics.Where(p => p.Type >= 2).Select(p => p.Id)) ?? new List<TopicReplyAttachment>();

                foreach (var item in topics.Where(p => p.Type >= 2))
                {
                    item.Images = images.Where(p => p.TopicReplyId == item.Id);
                    item.Attachment = attachments.FirstOrDefault(p => p.TopicReplyId == item.Id);
                }
            }
            if (topics.Any() == true && userId.HasValue)
            {
                var checks = await _circleFollowerRepository.CheckIsFollowCircle(topics.Select(p => p.CircleId).Distinct(), userId.Value);
                topics.ForEach(item =>
                {
                    item.IsFollowed = checks.FirstOrDefault(p => p.Key == item.CircleId).Value;
                });
            }
            return topics;
        }

        /// <summary>
        /// 获取置顶话题列表
        /// 1.管理员推广链接(最多两个)
        /// 2.圈主置顶话题(最多三个)
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        public TopDto GetTopList(Guid circleId)
        {
            var toppings = _toppingRepository.GetBy(" Status = 1 and IsDeleted = 0 and StartTime < getdate() and EndTime > getdate() ", null, "UpdateTime DESC")
                .Take(2).Select(s => new TopDto.ToppingDto()
                {
                    Title = s.Title,
                    Url = s.Url,
                }).ToList();

            var topics = _topicRepository.GetPagination(circleId, TopicTopType.Circle, TopicSort.Top, 1, 3)
                .Select(s => new TopDto.ToppingDto()
                {
                    Id = s.Id,
                    Title = s.Content,
                }).ToList();

            return new TopDto
            {
                Toppings = toppings,
                Topics = topics
            };
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyword"></param>
        /// <param name="circleId"></param>
        /// <param name="topType"></param>
        /// <param name="isGood"></param>
        /// <param name="isQA"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PaginationModel<TopicDto>> GetPagination(Guid? userId, string keyword, Guid? circleId, int? type, bool? isGood, bool? isQA,
            List<int> tags, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, bool getCoverAndFollow = true)
        {
            var sort = true == isGood ? TopicSort.Good : TopicSort.Time;

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var total = _topicRepository.GetTotal(keyword, circleId, null, type, topType: null, isGood, isQA, tags, startTime, endTime);
            bool? isOpen = null;
            IEnumerable<Topic> source = _topicRepository.GetPagination(keyword, circleId, null, type, topType: null, isGood, isQA, isOpen, tags, startTime, endTime, sort, pageIndex, pageSize);
            List<TopicDto> data = _mapper.Map<List<TopicDto>>(source);

            if (getCoverAndFollow)
            {
                var topicIds = data.Select(s => s.Id).ToList();
                await AssembleTopicDto(topicIds, data);
                AssembleLikeFollow(userId, topicIds, data);
            }
            return PaginationModel<TopicDto>.Build(data, total);
        }


        public async Task<List<TagDetailDto>> GetTagsById(List<int> tagIds)
        {
            List<TagDetailDto> tagsDto = new List<TagDetailDto>();
            if (tagIds == null || tagIds.Count == 0)
                return tagsDto;

            //全部标签
            var tags = await GetTags();

            //存在的标签
            var existsTags = tags.Where(s => tagIds.Contains(s.Id)).ToList();

            //找出顶层标签
            existsTags.ForEach(s =>
            {
                if (!tagsDto.Exists(dto => dto.Id == s.ParentId))
                    tagsDto.Add(new TagDetailDto { Id = s.ParentId, Name = s.ParentName });
            });

            //找出子标签
            tagsDto.ForEach(s =>
            {
                s.Children = existsTags.Where(c => c.ParentId == s.Id)
                .Select(c => new TagDetailDto { Id = c.Id, Name = c.Name }).ToList();
            });
            return tagsDto;
        }

        public async Task<PaginationModel<TopicDto>> GetPaginationByEs(Guid? userId, string keyword, Guid? circleId, bool? isCircleOwner, int? type, bool? isGood, bool? isQA,
            List<string> tags, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            if (endTime != null)
            {
                //endTime.Value.Date.AddDays(1).AddMilliseconds(-1);
                endTime = endTime.Value.Date.AddDays(1);
            }

            //圈主的话题
            Guid? creator = null;
            //圈主
            Guid? circleUserId = null;
            //登录人
            Guid? loginUserId = userId;
            if (circleId != null)
            {
                circleUserId = _circleRepository.Get(circleId.Value)?.UserId;
                if (true == isCircleOwner)
                    creator = circleUserId;

                if (circleUserId == null)
                    return PaginationModel<TopicDto>.Build(default, 0);
            }

            //从es中拿到数据
            var esTopics = _dataSearch.SearchTopic(out long total, keyword, circleId, circleUserId, loginUserId, creator, type, isGood, isQA, tags, startTime, endTime, pageIndex, pageSize);
            var topicIds = esTopics.Select(s => s.Id).ToList();


            IEnumerable<SimpleTopicDto> source = _topicRepository.GetList(topicIds);
            //保持es顺序
            List<TopicDto> data = new List<TopicDto>();
            foreach (var item in esTopics)
            {
                var topic = source.Where(s => s.Id == item.Id).FirstOrDefault();
                if (topic == null)
                    continue;
                //使用高亮数据
                topic.Content = item.Content;
                data.Add(_mapper.Map<TopicDto>(topic));
            }

            await AssembleTopicDto(topicIds, data);
            AssembleImageAttachment(topicIds, data);
            AssembleLikeFollow(userId, topicIds, data);


            var pagination = PaginationModel<TopicDto>.Build(data, total);
            if (esTopics.Any())
            {
                //搜索结果集中包含的所有tag
                var existsTagIds = _dataSearch.SearchTopicTags(keyword, circleId, circleUserId, loginUserId, creator, type, isGood, isQA, tags, startTime, endTime);
                var existsTags = await GetTagsById(existsTagIds);
                pagination.Statistics = new
                {
                    Tags = existsTags
                };
            }
            return pagination;
        }

        /// <summary>
        /// 填充data
        /// 0. 话题的图片和附件
        /// 1. 话题前10个点赞人
        /// 2, 话题前8个回复
        /// 3. 话题包含的标签
        /// 4. 当前用户的点赞和收藏情况
        /// </summary>
        /// <param name="topicIds"></param>
        /// <param name="data"></param>
        /// 
        /// <returns></returns>
        private async Task AssembleTopicDto(List<Guid> topicIds, List<TopicDto> data)
        {
            //话题前10个点赞人
            IEnumerable<TopicReplyLike> likes = _topicReplyLikeRepository.GetList(topicIds, 10);
            //话题前8个回复
            IEnumerable<TopicReplyDto> replies = _mapper.Map<IEnumerable<TopicReplyDto>>(_topicReplyRepository.GetTopList(topicIds, 8));
            //标签
            List<TopicTag> topicTags = _topicTagRepository.GetList(topicIds).ToList();
            IEnumerable<SimpleTagDto> allTags = await GetTags();

            data.AsParallel().ForAll(topic =>
            {
                topic.Replies = replies.Where(s => s.TopicId == topic.Id).ToList();
                topic.LikeUserNames = likes.Where(s => s.TopicReplyId == topic.Id).Select(s => s.UserName).ToList();
                topic.Tags = allTags.Where(s => topicTags.Exists(q => q.TopicId == topic.Id && q.TagId == s.Id)).ToList();
            });

        }

        /// <summary>
        /// 已登录的用户, 是否点赞, 收藏
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="topicIds"></param>
        /// <param name="data"></param>
        public void AssembleLikeFollow(Guid? userId, List<Guid> topicIds, IEnumerable<TopicDto> data)
        {
            //用户的点赞和收藏情况
            if (userId != null && userId != Guid.Empty)
            {
                IEnumerable<Guid> likeDataIds = _topicReplyLikeRepository.GetIdList(topicIds, userId.Value);
                List<Guid> collectionDataIds = _collectionRepository.GetCollection(topicIds, userId.Value);
                data.AsParallel().ForAll(topic =>
                {
                    topic.IsLoginUserOwner = topic.Creator == userId;
                    topic.Like = likeDataIds.Contains(topic.Id);
                    topic.Follow = collectionDataIds.Contains(topic.Id);
                });
            }
        }


        /// <summary>
        /// 话题图片和附件
        /// </summary>
        /// <param name="topicIds"></param>
        /// <param name="data"></param>
        private void AssembleImageAttachment(IEnumerable<Guid> topicIds, IEnumerable<TopicDto> data)
        {
            IEnumerable<TopicReplyImage> images = _topicReplyImageRepository.GetList(topicIds);
            IEnumerable<TopicReplyAttachment> attachments = _topicReplyAttachmentRepository.GetList(topicIds);
            data.AsParallel().ForAll(topic =>
            {
                topic.Images = images.Where(s => s.TopicReplyId == topic.Id).OrderBy(s => s.Sort).ToList();
                topic.Attachment = attachments.Where(s => s.TopicReplyId == topic.Id).FirstOrDefault();
            });
        }

        /// <summary>
        /// 设为精品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AppServiceResultDto Good(Guid id, Guid userId, bool isGood)
        {
            Topic topic = _topicRepository.Get(id);
            if (topic == null)
                return AppServiceResultDto.Failure("话题已关闭");
            Circle circle = _circleRepository.Get(topic.CircleId);
            if (circle == null || circle.UserId != userId)
                return AppServiceResultDto.Failure("您没有权限哦");//圈主可以设为精品

            _topicRepository.Good(id, isGood);
            UpdateTopicData(id);
            return AppServiceResultDto.Success();
        }

        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AppServiceResultDto Top(Guid id, Guid userId, bool cancel)
        {
            Topic topic = _topicRepository.Get(id);
            if (topic == null)
                return AppServiceResultDto.Failure("话题已关闭");
            Circle circle = _circleRepository.Get(topic.CircleId);
            if (circle == null || circle.UserId != userId)
                return AppServiceResultDto.Failure("您没有权限哦");//圈主可以置顶

            var topicTop = (TopicTopType)topic.TopType;
            if (cancel)
                topicTop ^= TopicTopType.Circle;
            else
                topicTop |= TopicTopType.Circle;

            _topicRepository.Top(id, topicTop);
            UpdateTopicData(id);
            return AppServiceResultDto.Success();
        }

        public async Task<IEnumerable<SimpleTopicDto>> GetListByTopType(int type, int count = 10)
        {
            var finds = await _topicRepository.GetByTopType(type, count);
            if (finds?.Any() == true)
            {
                if (finds.Any(p => p.Type > 2 && p.Type < 512))
                {
                    var images = _topicReplyImageRepository.GetList(finds.Where(p => p.Type > 2 && p.Type < 512).Select(p => p.Id)) ?? new List<TopicReplyImage>();
                    var attachments = _topicReplyAttachmentRepository.GetList(finds.Where(p => p.Type > 2 && p.Type < 512).Select(p => p.Id)) ?? new List<TopicReplyAttachment>();

                    foreach (var item in finds.Where(p => p.Type > 2 && p.Type < 512))
                    {
                        item.Images = images.Where(p => p.TopicReplyId == item.Id);
                        item.Attachment = attachments.FirstOrDefault(p => p.TopicReplyId == item.Id);
                    }
                }
                return finds;
            }
            return new List<SimpleTopicDto>();
        }

        public async Task<IEnumerable<SimpleTopicDto>> GetListByIsHandPick(bool isHandPick, int count = 10)
        {
            var finds = await _topicRepository.GetByIsHandPick(isHandPick, count);
            if (finds?.Any() == true)
            {
                if (finds.Any(p => p.Type > 2 && p.Type < 512))
                {
                    //2是图片，1是文字，除了文字意外，所有类型的帖子都有可能有图片
                    var images = _topicReplyImageRepository.GetList(finds.Where(p => p.Type >= 2).Select(p => p.Id)) ?? new List<TopicReplyImage>();
                    var attachments = _topicReplyAttachmentRepository.GetList(finds.Where(p => p.Type > 2 && p.Type < 512).Select(p => p.Id)) ?? new List<TopicReplyAttachment>();

                    foreach (var item in finds.Where(p => p.Type >= 2))
                    {
                        item.Images = images.Where(p => p.TopicReplyId == item.Id);
                        item.Attachment = attachments.FirstOrDefault(p => p.TopicReplyId == item.Id);
                    }
                }
                return finds;
            }
            return new List<SimpleTopicDto>();
        }

        public async Task<IEnumerable<SimpleTopicDto>> GetIsGoodList(int count = 10)
        {
            var finds = await _topicRepository.GetIsGood(count);
            if (finds?.Any() == true)
            {
                if (finds.Any(p => p.Type > 2 && p.Type < 512))
                {
                    var images = _topicReplyImageRepository.GetList(finds.Where(p => p.Type > 2 && p.Type < 512).Select(p => p.Id)) ?? new List<TopicReplyImage>();
                    var attachments = _topicReplyAttachmentRepository.GetList(finds.Where(p => p.Type > 2 && p.Type < 512).Select(p => p.Id)) ?? new List<TopicReplyAttachment>();

                    foreach (var item in finds.Where(p => p.Type > 2 && p.Type < 512))
                    {
                        item.Images = images.Where(p => p.TopicReplyId == item.Id);
                        item.Attachment = attachments.FirstOrDefault(p => p.TopicReplyId == item.Id);
                    }
                }
                return finds;
            }
            return new List<SimpleTopicDto>();
        }

        public IEnumerable<Topic> MoreDiscuss(Guid circleID, int offset, int limit)
        {
            return _topicRepository.MoreDiscuss(circleID, offset, limit);
        }

        public async Task<IEnumerable<SimpleTopicDto>> GetNewestDynamicTimeTopics(Guid circleID, int count = 10)
        {
            if (count < 1) return new List<SimpleTopicDto>();
            return await _topicRepository.GetTopicOrderByDynamicTime(circleID, count);
        }

        public async Task<IEnumerable<SimpleTopicDto>> GetRelatedTopics(Guid topicID, IEnumerable<int> tagIDs, int offset = 0, int limit = 5, bool inCircle = false)
        {
            Guid? circleID = null;
            if (inCircle)
            {
                circleID = _topicRepository.Get(topicID)?.CircleId;
            }
            var relatedIDs = await _topicRepository.GetRelatedTopicIDs(topicID, tagIDs, offset, limit, circleID);
            if (relatedIDs?.Any() == true)
            {
                return _topicRepository.GetList(relatedIDs);
            }
            return new SimpleTopicDto[0];
        }

        public async Task<IEnumerable<SimpleTopicDto>> GetByReplyCount(Guid circleID, int offset = 0, int limit = 5, IEnumerable<Guid> excludeTopicIDs = null)
        {
            var result = new List<SimpleTopicDto>();
            var str_Exclude = string.Empty;
            if (excludeTopicIDs?.Any() == true) str_Exclude = $" AND ID not in @excludeTopicIDs";
            var finds = _topicRepository.GetBy("circleID = @circleID AND OpenUserID is null AND IsDeleted = 0" + str_Exclude, new { circleID, offset, limit, excludeTopicIDs }, "ReplyCount Desc OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY", new string[] { "*" });
            if (finds?.Any() == true)
            {
                //foreach (var item in finds)
                //{
                //    var entity = _mapper.Map<SimpleTopicDto>(item);
                //    if (entity?.Id != Guid.Empty) result.Add(entity);
                //}

                return finds.Select(p => new SimpleTopicDto()
                {
                    Content = p.Content,
                    ReplyCount = p.ReplyCount,
                    CircleId = p.CircleId,
                    Id = p.Id
                });
            }
            return await Task.Run(() =>
            {
                return result;
            });
        }
    }
}
