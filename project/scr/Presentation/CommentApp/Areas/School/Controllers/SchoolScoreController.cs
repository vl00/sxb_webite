using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.IServices;
using PMS.School.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using Sxb.PC.Areas.School.Models;
using Sxb.Web.Areas.School.Models;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PC.Areas.School.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SchoolScoreController : ApiBaseController
    {
        ISchoolScoreService _schoolScoreService;
        ISchoolService _schoolService;
        ISchoolCommentService _schoolCommentService;
        IEasyRedisClient _easyRedisClient;
        public SchoolScoreController(ISchoolService schoolService, ISchoolCommentService schoolCommentService, IEasyRedisClient easyRedisClient, ISchoolScoreService schoolScoreService)
        {
            _schoolScoreService = schoolScoreService;
            _easyRedisClient = easyRedisClient;
            _schoolCommentService = schoolCommentService;
            _schoolService = schoolService;
        }

        public async Task<ResponseResult> GetSchoolExtScore(Guid extID)
        {
            var result = ResponseResult.Success();
            if (extID == Guid.Empty) return ResponseResult.Failed("Params error");
            var indexs = await _schoolService.schoolExtScoreIndexsAsync();
            var scores = await _schoolService.GetSchoolValidExtScoreAsync(extID);

            if (indexs?.Any() == true && scores?.Any() == true)
            {
                var resultItem = new SchoolScoreResponse()
                {
                    CueentIndex = new IndexItem()
                    {
                        ID = 22,
                        Name = indexs.FirstOrDefault(p => p.Id == 22).Index_Name,
                        ParentID = 0
                    },
                    Score = scores.FirstOrDefault(p => p.IndexId == 22)?.Score
                };

                var indexIDs = scores.Select(p => p.IndexId).ToArray();
                var secondLeve = indexs.Where(p => indexIDs.Contains(p.Id) && p.ParentId == 22);
                var thirdLevel = indexs.Where(p => indexIDs.Contains(p.Id) && p.ParentId != 22);

                resultItem.SubItems = new List<SchoolScoreResponse>();

                foreach (var item in secondLeve)
                {
                    var secondSubItem = new SchoolScoreResponse()
                    {
                        CueentIndex = new IndexItem()
                        {
                            ID = item.Id,
                            Name = item.Index_Name,
                            ParentID = item.ParentId
                        },
                        Score = scores.FirstOrDefault(p => p.IndexId == item.Id)?.Score,
                        SubItems = new List<SchoolScoreResponse>()
                    };

                    var subItems = thirdLevel.Where(p => p.ParentId == item.Id);
                    if (subItems?.Any() == true)
                    {
                        foreach (var thirdItem in subItems)
                        {
                            secondSubItem.SubItems.Add(new SchoolScoreResponse()
                            {
                                CueentIndex = new IndexItem()
                                {
                                    ID = thirdItem.Id,
                                    Name = thirdItem.Index_Name,
                                    ParentID = thirdItem.ParentId
                                },
                                Score = scores.FirstOrDefault(p => p.IndexId == thirdItem.Id)?.Score
                            });

                        }
                    }
                    resultItem.SubItems.Add(secondSubItem);
                }

                result.Data = resultItem;
            }

            return result;
        }

        public async Task<ResponseResult> GetCommentScoreStatistics(Guid extID, bool _asqeezcEqwe = false)
        {
            var result = ResponseResult.Success();

            if (_asqeezcEqwe)
            {
                await _easyRedisClient.RemoveAsync($"CommentScoreStatistics:{extID}",StackExchange.Redis.CommandFlags.FireAndForget);
            }

            var response = await _easyRedisClient.GetOrAddAsync($"CommentScoreStatistics:{extID}", async () =>
            {
                var finds = _schoolCommentService.GetSchoolCommentScoreStatistics(extID);
                if (finds?.Any() == true)
                {
                    var avgScore = _schoolScoreService.GetSchoolScore(Guid.Empty, extID);

                    return new GetCommentScoreStatisticsResponse()
                    {
                        HeadImgUrls = finds.OrderByDescending(p => p.AddTime).Select(p => p.HeadImgUrl).Distinct().Take(3),
                        AvgStars = SchoolScoreToStart.GetCurrentSchoolstart(avgScore.AggScore),
                        AvgScore = Math.Round(avgScore.AggScore / 20, 1).ToString("N1"),
                        FivePercent = (int)(finds.Count(p => p.AggScore >= 81) / (decimal)finds.Count() * 100),
                        FourPercent = (int)(finds.Count(p => p.AggScore >= 61 && p.AggScore <= 80) / (decimal)finds.Count() * 100),
                        ThreePercent = (int)(finds.Count(p => p.AggScore >= 41 && p.AggScore <= 60) / (decimal)finds.Count() * 100),
                        TwoPercent = (int)(finds.Count(p => p.AggScore >= 21 && p.AggScore <= 40) / (decimal)finds.Count() * 100),
                        OnePercent = (int)(finds.Count(p => p.AggScore >= 1 && p.AggScore <= 20) / (decimal)finds.Count() * 100),
                        CommentCount = finds.Count()
                    };
                }
                return null;
            }, TimeSpan.FromHours(12));

            result.Data = response;

            return result;
        }
    }
}

