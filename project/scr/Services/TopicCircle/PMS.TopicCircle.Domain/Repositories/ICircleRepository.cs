using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface ICircleRepository : IRepository<Circle>
    {
        /// <summary>
        /// 事务创建Circle
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cover"></param>
        /// <returns></returns>
        bool CreateTran(Circle entity, CircleCover cover);

        /// <summary>
        /// 获取达人创建的圈子
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<Circle> GetCircles(Guid userId);

        /// <summary>
        /// 事务编辑Circle
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cover"></param>
        /// <returns></returns>
        bool EditTran(Circle entity, CircleCover cover);

        /// <summary>
        /// 执行USP_STATISTICNEWSCIRCLE存储过程
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="takeCount"></param>
        /// <returns></returns>
        IEnumerable<Circle> ExcuteUSP_STATISTICNEWSCIRCLE(int cityCode, int takeCount = 5);

        /// <summary>
        /// 执行USP_STATICCIRCLENEWSINFO存储过程
        /// </summary>
        /// <param name="timeNode"></param>
        /// <param name="circleIds"></param>
        /// <returns></returns>
        IEnumerable<USPSTATICCIRCLENEWSINFO> ExcuteUSP_STATICCIRCLENEWSINFO(DateTime? timeNode, List<Guid> circleIds);

        /// <summary>
        /// 执行USP_QUERYMYCIRCLES存储过程
        /// </summary>
        /// <param name="timeNode">时间节点，从该节点起统计新消息</param>
        /// <param name="userId">目标用户ID，采用该ID下的圈子集做统计</param>
        /// <returns></returns>
        IEnumerable<USPQUERYMYCIRCLESDto> ExcuteUSP_QUERYMYCIRCLES(DateTime timeNode, Guid userId);

        IEnumerable<SimpleCircleDto> GetList(IEnumerable<Guid> ids);
        Circle GetByUserId(Guid userId);

        /// <summary>
        /// 获取关注者所关注的圈子
        /// </summary>
        /// <param name="follower"></param>
        /// <returns></returns>
        IEnumerable<Circle> GetByFollower(CircleFollower follower);
        Task<(IEnumerable<Circle>, int)> GetByFollower(Guid UserID, int offset = 0, int limit = 10);

        /// <summary>
        /// 获取话题圈列表(优先省份)
        /// </summary>
        /// <param name="count">获取条数</param>
        /// <param name="provinceCode">省份
        /// <para>
        /// 类似440
        /// </para>
        /// </param>
        /// <param name="cityCode">城市</param>
        /// <param name="notInIDs"></param>
        /// <returns></returns>
        Task<IEnumerable<Circle>> GetList(int count = 8, int provinceCode = 0, int cityCode = 0, IEnumerable<Guid> notInIDs = null);

        /// <summary>
        /// 话题圈里话题数量
        /// </summary>
        /// <param name="circleID"></param>
        /// <returns></returns>
        int TopicCount(Guid circleID);

        /// <summary>
        /// 统计昨日话题数
        /// </summary>
        /// <param name="circleID"></param>
        /// <returns></returns>
        int YesterdayTopicCount(Guid circleID);

        /// <summary>
        /// 统计昨日粉丝数
        /// </summary>
        /// <param name="circleID"></param>
        /// <returns></returns>
        int YesterdayFollowerCount(Guid circleID);

        /// <summary>
        /// 获取话题圈列表
        /// </summary>
        /// <param name="count">获取条数</param>
        /// <param name="provinceCode">省份
        /// <para>
        /// 类似440
        /// </para>
        /// </param>
        /// <returns></returns>
        Task<IEnumerable<Circle>> GetList(int count = 8, int provinceCode = 0);

        /// <summary>
        /// 圈子统计
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limie"></param>
        /// <returns></returns>
        Task<IEnumerable<CircleStatisticsDto>> GetStatistics(IEnumerable<Guid> circleIDs);

        /// <summary>
        /// 获取话题圈(自己创建与关注的优先)
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<SimpleCircleDto>> GetSelfFirstCircles(Guid userID, int offset = 0, int limit = 10, string orderBy = null);

        Task<int> Count(string where = null);

        /// <summary>
        /// 根据昨日活跃度排序获取圈子
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<SimpleCircleDto>> GetCircleOrderByYesterdayActive(Guid? userID, int offset = 0, int limit = 10);

        /// <summary>
        /// 更新话题圈统计数据
        /// <para>
        /// etc. ReplyCount | TopicCount | FollowCount
        /// </para>
        /// </summary>
        /// <returns></returns>
        Task<int> UpdateCircleCountData();

        Task<IEnumerable<dynamic>> ExportStaticData(DateTime btime, DateTime etime);
    }
}
