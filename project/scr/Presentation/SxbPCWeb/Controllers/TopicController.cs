using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Domain.Dtos;
using PMS.UserManage.Application.IServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace Sxb.PCWeb.Controllers
{
    public class TopicController : BaseController
    {
        IUserService _userService;
        ITopicService _topicService;
        ICircleService _circleService;

        public TopicController(ITopicService topicService, ICircleService circleService, IUserService userService)
        {
            _userService = userService;
            _circleService = circleService;
            _topicService = topicService;
        }

        /// <summary>
        /// 获取推荐话题圈
        /// <para>
        /// 【规则】
        ///1、推荐位：8个（提醒，预留后期可能会增加推荐位）
        ///2、推荐层级安排如下：
        ///①4个推荐位：推荐与用户同城市的达人所建的话题圈；
        ///②2个推荐位：推荐与用户同省但不同城市的达人所建的话题圈；
        ///③2个推荐位：其他省份的达人所建话题圈；
        ///3、思路参考如下：
        ///①检测上学帮系统内的话题圈总数是否多于8个；
        ///如否，则在推荐位显示上学帮系统内的所有话题圈；
        ///如是，进入第②步；
        ///
        ///②检测用户所在当前城市内的达人创建的话题圈是否大于4；
        ///如是，随机选出4个话题圈显示，进入第③步；
        ///如否，显示检测到的所有话题圈，置于相应数量的推荐位，进入第③步；
        ///
        ///③检测用户所在省份除当前城市以外的城市所有达人创建话题圈的数量是否大于6-②；
        ///如是，随机抽取相应数量的话题圈显示，使②+③=6，进入第④步；
        ///如否，显示检测到的所有话题圈，进入第④步；
        ///
        ///④检测其他省份的达人所建话题圈是否大于8-②-③；
        ///如是，随机抽取相应数量的话题圈，使②+③+④=8，结束；
        ///如否，显示检测到的所有话题圈，重复进行第②步但需排重掉已显示的话题圈，直到②+③+④=8，结束。
        /// </para>
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RecommendCircles(int count = 8)
        {
            var circles = new List<CircleDetailDto>();

            if (userId.HasValue)
            {
                var loginUser = _userService.GetUserInfoDetail(userId.Value);
                if (loginUser?.City.HasValue == true)
                {
                    var finds = await _circleService.GetCircles(4, 0, loginUser.City.Value);
                    if (finds?.Any() == true) circles.AddRange(finds);
                    if (finds.Count() < 4)
                    {
                        finds = await _circleService.GetCircles(6 - circles.Count(), loginUser.City.Value / 1000, 0, circles.Select(p => p.Id));
                        if (finds?.Any() == true) circles.AddRange(finds);
                        loginUser = _userService.GetUserInfoDetail(userId.Value);
                    }
                }
            }

            if (circles.Count() < count)
            {
                var finds = await _circleService.GetCircles(count - circles.Count(), 0, 0, circles.Select(p => p.Id));
                if (finds?.Any() == true) circles.AddRange(finds);
            }

            if (circles.Any())
            {
                return AjaxSuccess("Operation success", circles);
            }
            else
            {
                return AjaxFail("参数错误");
            }
        }

        /// <summary>
        /// 精选话题
        /// <para>
        /// 【规则】
        ///1、显示总数量：10个
        ///2、推荐优先级与排序规则：
        ///（1）运营人员置顶帖子，如有多条置顶帖子，按帖子的总评论数（即评论+回复总数）降序排列，如帖子均无回复，则按发帖时间降序排列；
        ///（2）
        ///①读取所有圈子（不分城市）最新动态（最新动态包括帖子内发帖时间、帖子最后编辑时间、评论时间、回复时间，时间最新的即为最新动态时间点）
        ///的设为精华的帖子（不含仅圈主可见的帖子）；
        ///②读取的帖子数=10-话题圈首页置顶的帖子数
        ///③按读取到的帖子最新动态时间点降序排列；
        ///④如最新动态时间点相同的，则随机排序
        /// </para>
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChosenTopics(int count = 10)
        {
            var result = new List<SimpleTopicDto>();
            var handPickTopics = await _topicService.GetListByIsHandPick(true, count);
            if (handPickTopics?.Any() == true) result.AddRange(handPickTopics);
            if (result.Count() < 10)
            {
                var isGoodTopics = await _topicService.GetIsGoodList(10 - handPickTopics.Count());
                if (isGoodTopics?.Any() == true) result.AddRange(isGoodTopics);
            }
            if (result?.Any() == true)
            {
                return AjaxSuccess("Operation success", result);
            }
            else
            {
                return AjaxFail("No data found");
            }
        }

        /// <summary>
        /// 热门话题
        /// </summary>
        /// <param name="count">获取条数</param>
        /// <returns></returns>
        public async Task<IActionResult> HotTopics(int count = 10)
        {
            var finds = await _topicService.GetHotList(null, null, count);
            if (finds?.Any() == true) return AjaxSuccess(data: finds);
            return AjaxFail("No data found");
        }
    }
}
