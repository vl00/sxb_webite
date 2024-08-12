using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Option;
using ProductManagement.API.Http.Option.Live;
using ProductManagement.API.Http.Result;
using ProductManagement.API.Http.Result.Live;
using ProductManagement.Tool.HttpRequest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Service
{
    public class LiveServiceClient : HttpBaseClient<LiveConfig>, ILiveServiceClient
    {
        private readonly ILogger _log;
        public LiveServiceClient(HttpClient client, IOptions<LiveConfig> config, ILoggerFactory log)
                    : base(client, config.Value, log)
        {
            //Config = config.Value;
            _log = log.CreateLogger<LiveServiceClient>();
        }
        public async Task<LecturesResult> QueryLectures(List<Guid> ids, Dictionary<string, string> cookie)
        {
            QueryLecturesOption option = new QueryLecturesOption(ids, cookie);
            return await PostAsync<LecturesResult, QueryLecturesOption>(option);
        }

        public async Task<LecturesResult> StickLectures(int city)
        {
            try
            {
                StickLecturesOption option = new StickLecturesOption(city);
                return await GetAsync<LecturesResult, StickLecturesOption>(option);
            }
            catch(Exception ex)
            {
                _log.LogError("获取置顶直播出错", ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 根据Id获取讲师的课程
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<LecturesResult> LectorLectures(Guid userId,int? status = null,int page = 1)
        {
            try
            {
                ///api/Lecture/LectorLectures
                LectorLecturesOption option = new LectorLecturesOption(userId, status, page);
                return await GetAsync<LecturesResult, LectorLecturesOption>(option);
            }
            catch (Exception ex)
            {
                _log.LogError("根据Id获取讲师的课程出错", ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 我的收藏课程
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<LecturesCollectionResult> MyCollections(Dictionary<string, string> cookie, int page = 1)
        {
            try
            {
                LiveCollectionsOption option = new LiveCollectionsOption(cookie, page);
                return await GetAsync<LecturesCollectionResult, LiveCollectionsOption>(option);
            }
            catch (Exception ex)
            {
                _log.LogError("获取我的收藏课程出错", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 直播历史
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<LecturesHistoryResult> MyHistory(Dictionary<string, string> cookie, int page = 1)
        {
            try
            {
                LiveHistoryOption option = new LiveHistoryOption(cookie, page);
                return await GetAsync<LecturesHistoryResult, LiveHistoryOption>(option);
            }
            catch (Exception ex)
            {
                _log.LogError("获取直播历史出错", ex.Message);
                return null;
            }
        }

        public async Task<LiveActivityExistCustomerChannelPhoneResult> ExistCustomerChannelPhone(int customer, string phone)
        {
            try
            {
                LiveActivityExistCustomerChannelPhoneOption option = new LiveActivityExistCustomerChannelPhoneOption(customer, phone);
                return await GetAsync<LiveActivityExistCustomerChannelPhoneResult, LiveActivityExistCustomerChannelPhoneOption>(option);
            }
            catch (Exception ex)
            {
                _log.LogError("获取直播历史出错", ex.Message);
                return null;
            }
        }
    }
}
