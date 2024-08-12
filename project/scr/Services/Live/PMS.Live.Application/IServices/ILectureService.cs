using PMS.Live.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.Live.Application.IServices
{
    /// <summary>
    /// 直播服务
    /// </summary>
    public interface ILectureService
    {
        /// <summary>
        /// 获取直播状态
        /// </summary>
        /// <param name="ids">直播们</param>
        Task<Dictionary<Guid, int>> GetLectureStatus(IEnumerable<Guid> ids);

        /// <summary>
        /// 根据批量用户获取直播状态
        /// </summary>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        Task<IEnumerable<LectorLiveStatusDto>> GetLectorLiveStatus(IEnumerable<Guid> userIDs);

        /// <summary>
        /// 获取直播
        /// </summary>
        Task<LectureDetailDto> GetLectureDetail(Guid ID);

        /// <summary>
        /// 获取直播带讲师用户ID
        /// </summary>
        Task<LectureDetailDto> GetLectureDetailWithLectorUserID(Guid ID);
        /// <summary>
        /// 批量获取直播
        /// </summary>
        Task<IEnumerable<LectureDetailDto>> GetLectureDetails(IEnumerable<Guid> IDs);

        /// <summary>
        /// 按时间段获取直播
        /// </summary>
        Task<IEnumerable<LectureDetailDto>> GetLectureByDate(DateTime beginTime, DateTime endTime);

        /// <summary>
        /// 获取最新直播
        /// </summary>
        Task<IEnumerable<LectureDetailDto>> GetNewestLecture(int count);
    }
}
