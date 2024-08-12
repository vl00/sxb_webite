using System;
using System.Collections.Generic;
using PMS.OperationPlateform.Domain.MongoModel;
using PMS.OperationPlateform.Domain.MongoModel.Base;

namespace PMS.OperationPlateform.Domain.IMongo
{
    public interface IStatisticRepository
    {
        /// <summary>
        ///  获取链接浏览数（PV、UV）
        /// </summary>
        List<UrlViewCount> GetUrlViewCount(List<string> urls, List<string> fw, List<DateTime> dates);
        /// <summary>
        /// 获取链接浏览时长
        /// </summary>
        List<UrlTimes> GetUrlTimes(List<string> urls, List<DateTime> dates);
        /// <summary>
        /// 获取链接跳转数
        /// </summary>
        int GetUrlJumpCount(List<string> urls, DateTime startDate);
        /// <summary>
        /// 获取链接分享数
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        int GetUrlShareCount(List<string> urls, DateTime startDate);

        /// <summary>
        /// 获取指定时间之后访问过的url
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="starTime"></param>
        /// <returns></returns>
        List<string> GetHitUrls(Guid userId, string[] urls, DateTime starTime);
        UrlViewCount GetUrlViewCount(string url, DateTime starTime);
        List<DaysStatistics> GetUrlViewCount(string matchUrl, DateTime starTime, DateTime endTime);
    }
}
