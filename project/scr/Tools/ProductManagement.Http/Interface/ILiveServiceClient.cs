using ProductManagement.API.Http.Result;
using ProductManagement.API.Http.Result.Live;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Interface
{
    public interface ILiveServiceClient
    {
        /// <summary>
        /// 置顶的直播
        /// </summary>
        /// <returns></returns>
        Task<LecturesResult> StickLectures(int city);

        /// <summary>
        /// 批量获取直播列表
        /// </summary>
        /// <returns></returns>
        Task<LecturesResult> QueryLectures(List<Guid> ids, Dictionary<string, string> cookie);

        /// <summary>
        /// 根据Id获取讲师的课程
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<LecturesResult> LectorLectures(Guid userId, int? status = null,int page = 1);
        /// <summary>
        /// 我的直播收藏
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<LecturesCollectionResult> MyCollections(Dictionary<string, string> cookie, int page = 1);

        Task<LecturesHistoryResult> MyHistory(Dictionary<string, string> cookie, int page = 1);
        Task<LiveActivityExistCustomerChannelPhoneResult> ExistCustomerChannelPhone(int customer, string phone);
    }
}
