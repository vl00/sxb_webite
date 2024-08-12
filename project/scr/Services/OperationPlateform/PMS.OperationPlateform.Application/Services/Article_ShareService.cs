using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.IMongo;
using PMS.OperationPlateform.Domain.IRespositories;

namespace PMS.OperationPlateform.Application.Services
{
    public class Article_ShareService: IArticle_ShareService
    {
        private readonly IStatisticRepository _statisticRepo;
        private readonly IArticle_ShareRepository _shareRepository;
        public Article_ShareService(IStatisticRepository statisticRepo, IArticle_ShareRepository shareRepository)
        {
            _statisticRepo = statisticRepo;
            _shareRepository = shareRepository;
        }


        /// <summary>
        /// 统计文章分享的浏览数据
        /// </summary>
        public void StatisticArticleShare()
        {
            var statisticsArticle =  _shareRepository.GetUnstatisticsArticle();

            var urls = statisticsArticle.GroupBy(q => q.ArticleUrl).Select(q=>q.Key).ToList();
            foreach(var url in urls)
            {
                var statistics = statisticsArticle.Where(q => q.ArticleUrl == url);

                var fws = statistics.GroupBy(q => q.Fw)
                                           .Select(q => q.Key).ToList();
                var times = statistics.GroupBy(q => q.StatisticsDate)
                                           .Select(q => q.Key).ToList();
                var viewResults = _statisticRepo.GetUrlViewCount(new List<string> { url }, fws, times);

                foreach(var stat in statistics)
                {
                    var vr = viewResults.Where(q => q.Fw == stat.Fw && q.Date == stat.StatisticsDate.ToString("yyyy-MM-dd"));
                    if (vr == null) continue;

                    stat.PV = vr.Sum(q=>q.Pv);
                    stat.UV = vr.Sum(q => q.Uv);
                }

                if (viewResults.Count > 0)
                {
                    var fwUrls = viewResults.GroupBy(q => new { q.Fw }).Select(q => new 
                    {
                        q.Key.Fw,
                        Urls = q.Where(p=>p.Params.ContainsValue(q.Key.Fw)).Select(m =>
                        {
                            var shareUrl = ConvertFullUrl(m.Url, m.Params);
                            return shareUrl;
                        }).ToList()
                    }).ToList();

                    //获取各渠道的停留时间
                    var shareUrls = new List<string>();
                    foreach (var item in fwUrls)
                    {
                        shareUrls.AddRange(item.Urls.Where(p=>!string.IsNullOrWhiteSpace(p)));
                    }

                    var timesResult = _statisticRepo.GetUrlTimes(shareUrls.GroupBy(q=>q).Select(q=>q.Key).ToList(), times);
                    foreach (var item in timesResult)
                    {
                        var f = statistics.FirstOrDefault(q => q.Fw == item.fw && q.StatisticsDate.ToString("yyyy-MM-dd") == item.date);
                        if (f == null) continue;
                        f.TimeSpent =(int)(Convert.ToDouble(item.times)/ Convert.ToDouble(f.PV));
                    }

                    //获取跳转次数
                    foreach (var item in statistics)
                    {
                        var refererUrls = fwUrls.FirstOrDefault(q => q.Fw == item.Fw)?.Urls;
                        if (refererUrls == null) continue;
                        refererUrls = refererUrls.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                        if (refererUrls.Count == 0) continue;
                        var u = refererUrls.GroupBy(q => q).Select(q => q.Key).ToList();
                        item.JumpCount = _statisticRepo.GetUrlJumpCount(u, item.StatisticsDate);
                        item.ShareCount = _statisticRepo.GetUrlShareCount(u, item.StatisticsDate);
                    }
                }
            }
            if (statisticsArticle.Any())
            {
                _shareRepository.InsertStatisticsArticle(statisticsArticle);
            }
        }

        public void Test()
        {
        }

        private string ConvertFullUrl(string url, Dictionary<string, string> param)
        {
            if (param.Count() == 0)
                return "";
            string paramStr = string.Join("&", param.Select(q => q.Key + "=" + q.Value));
            return url + "?" + paramStr;
        }

    }
}
