using PMS.Search.Domain.QueryModel;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public interface ICircleService : IApplicationService<Circle>
    {

        /// <summary>
        /// 提供达人构建圈子的服务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AppServiceResultDto<CircleCreateReturnDto> Create(CircleCreateRequestDto input);

        /// <summary>
        /// 获取达人创建的圈子
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        AppServiceResultDto<IEnumerable<CircleItemDto>> GetCircles(Guid userId);

        /// <summary>
        /// 获取达人创建的圈子详情信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        AppServiceResultDto<CircleDetailDto> GetTalentCircleDetail(Guid userID);

        /// <summary>
        /// 获取圈子的详情信息
        /// </summary>
        /// <param name="circleID"></param>
        /// <returns></returns>
        AppServiceResultDto<CircleDetailDto> GetDetail(Guid circleID, DateTime? time);

        /// <summary>
        /// 加入粉丝圈
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AppServiceResultDto JoinCircle(CircleJoinRequestDto input);

        /// <summary>
        /// 退出粉丝圈
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AppServiceResultDto ExitCircle(CircleExitRequestDto input);

        /// <summary>
        /// 编辑达人圈子
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AppServiceResultDto Edit(CircleEditRequestDto input);

        /// <summary>
        /// 校验圈子操作人是否有权限
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AppServiceResultDto CheckPermission(CircleCheckPermissionRequestDto input);

        /// <summary>
        /// 获取推荐圈子
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        AppServiceResultDto<IEnumerable<CircleDetailDto>> GetRecommends(int cityCode);


        /// <summary>
        /// 获取”达人圈子“列表
        /// </summary>
        /// <param name="timeNode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        AppServiceResultDto<IEnumerable<MyCircleItemDto>> GetTalentCircles(Guid userId);

        /// <summary>
        /// 获取"我的圈子"列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AppServiceResultDto<IEnumerable<MyCircleItemDto>> GetMyCircles(GetMyCirclesRequestDto input);
        /// <summary>
        /// 获取"我的圈子"列表
        /// </summary>
        /// <returns></returns>
        Task<AppServicePageResultDto<IEnumerable<MyCircleItemDto>>> GetMyCircles(Guid userID, int offset = 0, int limit = 10);


        /// <summary>
        /// 切换话题圈的禁用状态
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        AppServiceResultDto<CircleToggleDisableStatuDto> ToggleDisableStatu(Guid circleId);

        /// <summary>
        /// 搜索话题圈
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="loginUserId"></param>
        /// <returns></returns>
        List<SearchCircleDto> GetCircles(IEnumerable<Guid> ids, Guid? loginUserId);


        /// <summary>
        /// 检查用户是否已经创建了话题圈
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool CheckIsHasCircle(Guid userId);
        AppServicePageResultDto<List<SearchCircleDto>> SearchCircles(Guid? loginUserId, SearchBaseQueryModel queryModel);
        List<SearchCircleDto> SearchCircles(IEnumerable<Guid> ids, Guid? loginUserId);

        IEnumerable<Circle> GetByIDs(IEnumerable<Guid> ids);

        /// <summary>
        /// 获取话题圈(可以根据省份)
        /// </summary>
        /// <param name="count">获取条数</param>
        /// <param name="provinceCode">省份代码 -> 如440000</param>
        /// <param name="cityCode">城市代码 -> 如440100</param>
        /// <returns></returns>
        Task<IEnumerable<CircleDetailDto>> GetCircles(int count = 8, int provinceCode = 0, int cityCode = 0, IEnumerable<Guid> notInIDs = null);

        Task<(IEnumerable<CircleDetailDto>, int)> GetAllCircles(Guid userID, int offset = 0, int limit = 12, int order = 1);

        /// <summary>
        /// 更新话题圈统计数据
        /// <para>
        /// etc. ReplyCount | TopicCount | FollowCount
        /// </para>
        /// </summary>
        /// <returns></returns>
        Task<int> UpdateCircleCountData();


        /// <summary>
        /// /生成加入话题圈场景的二维码
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        Task<string> GenerateWeChatCode(Guid circleId, string callbackUrl,bool isMiniApp = false);

        Dictionary<Guid, bool> GetCirclesHasNews(IEnumerable<Guid> ids, Guid userID);

        Task<IEnumerable<dynamic>> ExportStaticData(DateTime btime, DateTime etime);
    }
}
