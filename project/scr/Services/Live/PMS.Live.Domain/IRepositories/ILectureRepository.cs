using PMS.Live.Domain.Dtos;
using PMS.Live.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.Live.Domain.IRepositories
{
    public interface ILectureRepository
    {
        /// <summary>
        /// 获取直播状态
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<LectureStatusDto>> GetLectureStatus(IEnumerable<Guid> ids);

        /// <summary>
        /// 获取用户们的直播状态
        /// </summary>
        /// <param name="userIDs">用户们</param>
        /// <returns></returns>
        Task<IEnumerable<LectorLiveStatusDto>> GetLectorLiveStatus(IEnumerable<Guid> userIDs);
        /// <summary>
        /// 批量获取数据
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        Task<IEnumerable<LectureInfo>> GetByIDs(IEnumerable<Guid> IDs);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        Task<LectureInfo> GetByID(Guid ID);

        /// <summary>
        /// 按时间段获取直播
        /// </summary>
        Task<IEnumerable<LectureInfo>> GetByDate(DateTime beginTime, DateTime endTime);
        /// <summary>
        /// 按时间段获取直播
        /// </summary>
        Task<IEnumerable<LectureInfo>> GetByDate(DateTime beginTime, DateTime endTime, int count = 0);

        /// <summary>
        /// 获取直播信息带讲师用户ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        Task<(LectureInfo, Guid)> GetByIDWithLectorUserID(Guid id);

    }
}
