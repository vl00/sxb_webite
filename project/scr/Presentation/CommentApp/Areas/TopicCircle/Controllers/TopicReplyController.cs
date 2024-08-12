using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.RabbitMQ.Message;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using ProductManagement.Framework.AspNetCoreHelper.Filters;
using ProductManagement.Framework.RabbitMQ;
using Sxb.Web.Areas.TopicCircle.Filters;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Authentication.Attribute;
using Sxb.Web.Controllers;
using Sxb.Web.Filters;
using Sxb.Web.Response;

namespace Sxb.Web.Areas.TopicCircle.Controllers
{
    [Route("tpc/[controller]/[action]")]
    [ApiController]
    public class TopicReplyController : ApiBaseController
    {
        public readonly IEventBus _eventBus;

        public readonly ITopicService _topicService;
        public readonly ITopicReplyService _topicReplyService;

        public TopicReplyController(ITopicService topicService, ITopicReplyService topicReplyService, IEventBus eventBus)
        {
            _topicService = topicService;
            _topicReplyService = topicReplyService;
            _eventBus = eventBus;
        }

        /// <summary>
        /// 新增评论
        /// </summary>
        /// <param name="topicReplyDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [BindMobile]
        [ValidateWebContent(ContentParam = "topicReplyDto")]
        public ResponseResult<object> Add([FromBody] TopicReplyAddRequest topicReplyDto)
        {
            topicReplyDto.UserId = UserId.Value;
            AppServiceResultDto<object> result = _topicReplyService.Add(topicReplyDto);
            if (result.Status)
            {
                //更新es
                _topicService.UpdateTopicData(topicReplyDto.TopicId);
            }
            return result;
        }

        /// <summary>
        /// 编辑评论
        /// </summary>
        /// <param name="topicReplyDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateWebContent(ContentParam = "topicReplyDto")]
        public ResponseResult Edit([FromBody] TopicReplyAddRequest topicReplyDto)
        {
            topicReplyDto.UserId = UserId.Value;
            var result = _topicReplyService.Edit(topicReplyDto);
            return result;
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="topicReplyId"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult Delete(Guid topicId, Guid topicReplyId)
        {
            var result = _topicService.DeleteReply(topicId, topicReplyId, UserId.Value);
            return result;
        }

        /// <summary>
        /// 点赞评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult Like(Guid id)
        {
            return _topicReplyService.Like(id, UserId.Value);

            //_eventBus.Publish(new SyncTopicLikeMessage()
            //{
            //    Id = id,
            //    UserId = userId.Value
            //});
            //return ResponseResult.Success();
        }


        /// <summary>
        /// 获取顶级评论, 及其前5条Children评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ResponseResult GetPaginationByTopic(Guid topicId, int pageIndex = 1, int pageSize = 10, bool createTimeDesc = false, int topReplyCount = 5)
        {
            var data = _topicReplyService.GetPaginationByTopic(topicId, pageIndex, pageSize, createTimeDesc, topReplyCount);
            if (User.Identity.IsAuthenticated && data?.Data?.Any() == true)
            {
                var replyIDs = data.Data.Select(p => p.Id).ToList();
                #region 是否登陆者本身的数据
                foreach (var item in data.Data.Where(p => p.Creator == UserId.GetValueOrDefault(Guid.Empty)))
                {
                    item.IsLoginUserOwner = true;
                }
                foreach (var children in data.Data.Select(p => p.Children))
                {
                    if (children?.Any() == true)
                    {
                        replyIDs.AddRange(children.Select(p => p.Id));
                        foreach (var item in children.Where(p => p.Creator == UserId.GetValueOrDefault(Guid.Empty)))
                        {
                            item.IsLoginUserOwner = true;
                        }
                    }
                }
                #endregion

                var isLike = _topicReplyService.GetIsLike(UserId.Value, replyIDs);
                if (isLike?.Any() == true)
                {
                    data.Data.ForEach(item =>
                    {
                        item.Like = isLike[item.Id];
                    });
                    foreach (var children in data.Data.Select(p => p.Children))
                    {
                        if (children?.Any() == true)
                        {
                            foreach (var item in children.Where(p => p.Creator == UserId.GetValueOrDefault(Guid.Empty)))
                            {
                                item.Like = isLike[item.Id];
                            }
                        }
                    }
                }
            }
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取顶级评论的子评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="firstParentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ResponseResult GetChildren(Guid topicId, Guid firstParentId, int pageIndex = 1, int pageSize = 9999)
        {
            var data = _topicReplyService.GetChildren(topicId, firstParentId, pageIndex, pageSize);
            return ResponseResult.Success(data);
        }



        /// <summary>
        /// 获取话题的所有评论(Children二级嵌套)
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public ResponseResult GetTopicReplies(Guid topicId)
        {
            var data = _topicReplyService.GetTopicReplies(topicId);
            if (User.Identity.IsAuthenticated && data?.Any() == true)
            {
                foreach (var item in data.Where(p => p.Creator == UserId.GetValueOrDefault(Guid.Empty)))
                {
                    item.IsLoginUserOwner = true;
                }
            }
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 查看对话
        /// </summary>
        /// <param name="replyID"></param>
        /// <returns></returns>
        [Description("查看对话")]
        public ResponseResult GetLevelReplies(Guid replyID)
        {
            var data = _topicReplyService.GetLevelReplies(replyID);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 查看对话
        /// </summary>
        /// <param name="replyID"></param>
        /// <returns></returns>
        [Description("查看对话")]
        public async Task<ResponseResult> GetLevelRepliesEx(Guid replyID, int limit = 7)
        {
            var data = await _topicReplyService.GetLevelRepliesEx(replyID, limit);

            if (User.Identity.IsAuthenticated)
            {
                var isLike = _topicReplyService.GetIsLike(UserId.Value, data.Select(p => p.Id));
                foreach (var item in data)
                {
                    item.IsLoginUserOwner = item.Creator == UserId;
                    if (isLike?.Any() == true)
                    {
                        item.Like = isLike[item.Id];
                    }
                }
            }

            return ResponseResult.Success(data);
        }
        /// <summary>
        /// 获取话题的前n条评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ResponseResult GetTopTopicReplies(Guid topicId, int size = 8)
        {
            var data = _topicReplyService.GetTopTopicReplies(topicId, size);
            return ResponseResult.Success(data);
        }
    }
}
