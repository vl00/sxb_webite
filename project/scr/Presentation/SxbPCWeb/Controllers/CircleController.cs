using Microsoft.AspNetCore.Mvc;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Controllers
{
    public class CircleController : BaseController
    {
        ICircleService _circleService;
        ICircleFollowerService _circleFollowerService;
        ITopicService _topicService;

        public CircleController(ICircleService circleService, ICircleFollowerService circleFollowerService, ITopicService topicService)
        {
            _topicService = topicService;
            _circleFollowerService = circleFollowerService;
            _circleService = circleService;
        }

        /// <summary>
        /// 获取我的圈子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> My()
        {
            if (!userId.HasValue) return AjaxNoLogin();
            var result = new List<MyCircleItemDto>();
            var selfCirclesResult = _circleService.GetCircles(userId.Value);
            if (selfCirclesResult.Status && selfCirclesResult.Data?.Any() == true)//一个达人只有一个自建圈子 , 不用考虑排序
            {
                foreach (var item in selfCirclesResult.Data)
                {
                    result.Add(new MyCircleItemDto()
                    {
                        CircleId = item.Id,
                        Cover = item.Cover,
                        IsCircleMaster = true,
                        Name = item.Name,
                        CircleMasterName = item.CircleMasterName
                    });
                }

            }
            if (result.Count < 4)
            {
                var joinCircles = await _circleService.GetMyCircles(userId.Value, 4 - result.Count());
                if (joinCircles.Status && joinCircles.Data?.Any() == true)
                {
                    foreach (var item in joinCircles.Data)
                    {
                        result.Add(new MyCircleItemDto()
                        {
                            CircleId = item.CircleId,
                            Cover = item.Cover,
                            Name = item.Name,
                            NEWTOPICCOUNT = item.NEWTOPICCOUNT,
                            NEWREPLYCOUNT = item.NEWREPLYCOUNT,
                            CircleMasterName = item.CircleMasterName,
                            FOLLOWERCOUNT = item.FOLLOWERCOUNT
                        });
                    }
                }
            }

            if (result.Any())
            {
                foreach (var item in result)
                {
                    item.NewestTopics = await _topicService.GetNewestDynamicTimeTopics(item.CircleId, 3);
                }
                //var topics = _topicService.
                return AjaxSuccess(data: result);
            }
            return null;
        }
    }
}
