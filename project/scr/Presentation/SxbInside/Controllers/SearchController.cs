using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Application.IServices;
using Sxb.Inside.Models;
using Sxb.Inside.Response;
using Sxb.Inside.Utils;
using ProductManagement.Framework.Foundation;
using PMS.OperationPlateform.Domain.Enums;
using PMS.Search.Domain.Entities;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Inside.Common;
using System.Threading;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PMS.UserManage.Domain.Common;
using ProductManagement.API.Http.Interface;
using System.ComponentModel;
using Sxb.Inside.RequestModel;

namespace Sxb.Inside.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly ImageSetting _imageSetting;
        private readonly ISearchService _dataSearch;
        private readonly IImportService _dataImport;
        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionInfoService _questionService;
        private readonly ICityInfoService _cityService;
        private readonly ISchoolService _schoolService;
        private readonly IUserService _userService;
        private readonly IArticleService _articleService;
        private readonly ISchoolRankService _schoolRankService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IMapper _mapper;

        private readonly IOperationClient _operationClient;

        private readonly IArticleCoverService _articleCoverService;
        private readonly IArticleCommentService _articleCommentService;

        public SearchController(ISearchService dataSearch, IImportService dataImport, IMapper mapper,
            ISchoolCommentService commentService, IQuestionInfoService questionService,
            ISchoolInfoService schoolInfoService, IOptions<ImageSetting> set, IEasyRedisClient easyRedisClient,
            ISchoolService schoolService, ICityInfoService cityService, IUserService userService,
            IArticleService articleService, ISchoolRankService schoolRankService, IOperationClient operationClient,
            IArticleCoverService articleCoverService, IArticleCommentService articleCommentService)
        {
            _dataSearch = dataSearch;
            _dataImport = dataImport;
            _schoolService = schoolService;
            _cityService = cityService;
            _commentService = commentService;
            _questionService = questionService;
            _mapper = mapper;
            _schoolInfoService = schoolInfoService;
            _imageSetting = set.Value;
            _userService = userService;
            _articleService = articleService;
            _schoolRankService = schoolRankService;
            _easyRedisClient = easyRedisClient;
            _articleCoverService = articleCoverService;
            _articleCommentService = articleCommentService;
            _operationClient = operationClient;
        }

        public ResponseResult CreateSchoolIndex()
        {
            _dataImport.CreateSchoolIndex();
            return ResponseResult.Success();
        }
        public ResponseResult CreateSvsSchoolIndex()
        {
            _dataImport.CreateSvsSchoolIndex();
            return ResponseResult.Success();
        }

        public ResponseResult CreateArticleIndex()
        {
            _dataImport.CreateArticleIndex();
            return ResponseResult.Success();
        }

        public ResponseResult CreateCommentIndex()
        {
            _dataImport.CreateCommentIndex();
            return ResponseResult.Success();
        }
        public ResponseResult CreateQuestionIndex()
        {
            _dataImport.CreateQuestionIndex();
            return ResponseResult.Success();
        }
        public ResponseResult CreateAnswerIndex()
        {
            _dataImport.CreateAnswerIndex();
            return ResponseResult.Success();
        }
        public ResponseResult CreateLiveIndex()
        {
            _dataImport.CreateLiveIndex();
            return ResponseResult.Success();
        }

        public ResponseResult CreateEEIndex()
        {
            //_dataSearch.CreateIndex();
            _dataImport.CreateEESchoolIndex();
            return ResponseResult.Success();
        }
        public ResponseResult ImportEESchool()
        {
            string indexName = "allschool";

            var lastDate = _dataImport.GetLastUpdate(indexName);
            var time = lastDate.FirstOrDefault(q => q.Name == indexName).LastTime > Convert.ToDateTime("0001-01-01") ?
                        lastDate.FirstOrDefault(q => q.Name == indexName).LastTime : Convert.ToDateTime("1999-01-01");

            //var time = Convert.ToDateTime("2019-08-25");

            bool isFirst = time == Convert.ToDateTime("1999-01-01");

            bool isTrue = _schoolService.InsertEESchoolData(isFirst);
            if (isTrue)
            {

                for (int i = 0; i < 200; i++)
                {
                    var data = _schoolService.GetEESchoolData(i, 2000, time);

                    if (data.Count == 0)
                    {
                        break;
                    }
                    Console.Write(i + "," + data.First().Time + "," + data.Last().Time);
                    _dataImport.ImportEESchoolData(data.Select(q => new SearchEESchool
                    {
                        Id = q.Id,
                        Name = q.Name,
                        Grade = q.Grade,
                        Type = q.Type,
                        Province = q.Province,
                        City = q.City,
                        Area = q.Area,
                        Cityarea = q.Province + q.City + q.Area,
                        Status = q.Status,
                        UpdateTime = q.Time
                    }).ToList());
                    Console.WriteLine("...√");
                    Thread.Sleep(10000);
                }
            }
            return ResponseResult.Success();
        }

        public ActionResult GetEESchool(string SchoolName)
        {
            var result = _dataSearch.SearchEESchool(SchoolName);

            foreach (var item in result)
            {
                if (item.Status == "已下线")
                {
                    item.Status = "已删除";
                }
            }
            if (Request.IsAjax())
            {
                return Json(ResponseResult.Success(result));
            }
            else
            {
                ViewBag.Result = result;
                return View();
            }            //return ResponseResult.Success(result);
        }

        public ResponseResult ImportSchool()
        {
            string indexName = "schoolindex_07";
            var lastDate = _dataImport.GetLastUpdate(indexName);
            var time = lastDate.FirstOrDefault(q => q.Name.StartsWith(indexName)).LastTime > Convert.ToDateTime("0001-01-01") ?
                        lastDate.FirstOrDefault(q => q.Name.StartsWith(indexName)).LastTime : Convert.ToDateTime("1999-01-01");

            //time = Convert.ToDateTime("2019-09-16");
            for (int i = 0; i < 200; i++)
            {
                Console.Write(i);
                var data = _schoolService.GetSchoolData(i, 3000, time);

                if (data.Count == 0)
                {
                    break;
                }
                _dataImport.ImportSchoolData(data.Select(q => new SearchSchool
                {
                    Id = q.Id,
                    SchoolId = q.SchoolId,
                    Name = q.Name,
                    City = q.City,
                    Area = q.Area,
                    CityCode = q.CityCode,
                    AreaCode = q.AreaCode,
                    Cityarea = q.Cityarea,
                    Location = new SearchGeo { Lon = q.Location.Longitude, Lat = q.Location.Latitude },
                    Canteen = q.Canteen,
                    Lodging = q.Lodging,
                    Studentcount = q.Studentcount,
                    Teachercount = q.Teachercount,
                    Abroad = q.Abroad,
                    Authentication = q.Authentication,
                    Characteristic = q.Characteristic,
                    Courses = q.Courses,
                    MetroLineId = q.MetroLineId,
                    MetroStationId = q.MetroStationId,
                    Tuition = q.Tuition,
                    UpdateTime = q.UpdateTime,
                    Grade = q.Grade,
                    Schooltype = q.Schooltype,
                    SchooltypeNewCode = q.SchooltypeNewCode,
                    SchooltypeCode = q.SchooltypeCode,
                    Tags = q.Tags,
                    Score = q.Score != null ? new SearchSchoolScore
                    {
                        Composite = q.Score.Score15,
                        Teach = q.Score.Score16,
                        Hard = q.Score.Score17,
                        Course = q.Score.Score18,
                        Learn = q.Score.Score19,
                        Cost = q.Score.Score20,
                        Envir = q.Score.Score21,
                        Total = q.Score.Score22
                    } : null,
                    IsDeleted = !(q.IsValid && (q.Status == 3))
                }).ToList());
                Console.WriteLine("...√");
                Thread.Sleep(5000);
            }
            return ResponseResult.Success();
        }
        //导入学校分数
        public ResponseResult ImportSchoolScore()
        {
            //string redisKey = "ImportSchoolScoreToES";

            //var isCreate = _schoolService.InsertSchoolScore();
            if (true)
            {
                //int index = await _easyRedisClient.GetAsync<int>(redisKey);

                for (int i = 0; i < 2000; i++)
                {
                    Console.Write(i);
                    //await _easyRedisClient.AddAsync(redisKey, i, DateTime.Now.AddHours(3));
                    var data = _schoolService.GetSchoolScoreData(i, 2000);

                    if (data.Count == 0)
                    {
                        break;
                    }

                    _dataImport.UpdateSchoolData(data.Select(q => new SearchSchool
                    {
                        Id = q.Eid,
                        Score = new SearchSchoolScore
                        {
                            Composite = q.Score15,
                            Teach = q.Score16,
                            Hard = q.Score17,
                            Course = q.Score18,
                            Learn = q.Score19,
                            Cost = q.Score20,
                            Envir = q.Score21,
                            Total = q.Score22
                        }
                    }).ToList());
                    Console.WriteLine("...√");
                    Thread.Sleep(2000);
                }
            }
            //await _easyRedisClient.RemoveAsync(redisKey);
            return ResponseResult.Success();
        }

        //导入点评的回复数
        public ResponseResult ImportCommentReplyCount()
        {
            //string redisKey = "ImportSchoolScoreToES";

            //var isCreate = _schoolService.InsertSchoolScore();
            if (true)
            {
                //int index = await _easyRedisClient.GetAsync<int>(redisKey);

                for (int i = 0; i < 10000; i++)
                {
                    Console.Write(i);
                    //await _easyRedisClient.AddAsync(redisKey, i, DateTime.Now.AddHours(3));
                    var data = _commentService.GetCommentReplyCount(i, 2000);

                    if (data.Count == 0)
                    {
                        break;
                    }

                    _dataImport.UpdateCommentData(data.Select(q => new SearchComment
                    {
                        Id = q.Id,
                        ReplyCount = q.Total
                    }).ToList());
                    Console.WriteLine("...√");
                    Thread.Sleep(1000);
                }
            }
            //await _easyRedisClient.RemoveAsync(redisKey);
            return ResponseResult.Success();
        }
        //导入问题的回答数
        public ResponseResult ImportQuestionReplyCount()
        {
            //string redisKey = "ImportSchoolScoreToES";

            //var isCreate = _schoolService.InsertSchoolScore();
            if (true)
            {
                //int index = await _easyRedisClient.GetAsync<int>(redisKey);

                for (int i = 0; i < 10000; i++)
                {
                    Console.Write(i);
                    //await _easyRedisClient.AddAsync(redisKey, i, DateTime.Now.AddHours(3));
                    var data = _questionService.GetQuestionAnswerCount(i, 2000);

                    if (data.Count == 0)
                    {
                        break;
                    }

                    _dataImport.UpdateQuestionData(data.Select(q => new SearchQuestion
                    {
                        Id = q.Id,
                        ReplyCount = q.Total
                    }).ToList());
                    Console.WriteLine("...√");
                    Thread.Sleep(1000);
                }
            }
            //await _easyRedisClient.RemoveAsync(redisKey);
            return ResponseResult.Success();
        }


        /// <summary>
        /// 更新学校评论数接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult UpdateCommentData(List<CommentCountData> data)
        {
            _dataImport.UpdateSchoolData(data.Select(q => new SearchSchool
            {
                Id = q.Eid,
                Commentcount = q.Count,
                Star = q.Star
            }).ToList());
            return ResponseResult.Success();
        }
        /// <summary>
        /// 更新学校问题数接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult UpdateQuestionData(List<QuestionCountData> data)
        {
            _dataImport.UpdateSchoolData(data.Select(q => new SearchSchool
            {
                Id = q.Eid,
                Questioncount = q.Count
            }).ToList());
            return ResponseResult.Success();
        }

        public async Task<ResponseResult> ImportComment()
        {
            var lastDate = _dataImport.GetLastUpdate();
            var time = lastDate.FirstOrDefault(q => q.Name.StartsWith("commentindex")).LastTime > Convert.ToDateTime("0001-01-01") ?
                        lastDate.FirstOrDefault(q => q.Name.StartsWith("commentindex")).LastTime : Convert.ToDateTime("1999-01-01");

            for (int i = 0; i < 10000; i++)
            {
                var data = _commentService.GetCommentData(i, 2000, time);
                if (data.Count == 0)
                {
                    break;
                }
                Console.Write(i);
                List<Guid> Ids = data.GroupBy(q => q.SchoolSectionId).Select(x => x.Key)?.ToList();
                var SchoolInfos = await _schoolService.ListExtSchoolByBranchIds(Ids);

                _dataImport.ImportCommentData(data.Select(q => new SearchComment
                {
                    Id = q.Id,
                    Eid = q.SchoolSectionId,
                    Context = q.Content,
                    ReplyCount = q.ReplyCount,
                    PublishTime = q.CreateTime,
                    UpdateTime = q.CreateTime,
                    Score = q.Score.IsAttend ? (double)q.Score.AggScore :
                            (double)(q.Score.HardScore + q.Score.LifeScore + q.Score.ManageScore + q.Score.TeachScore + q.Score.EnvirScore) / 5,
                    CityCode = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.CityCode ?? 0,
                    AreaCode = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.AreaCode ?? 0,
                    Grade = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.Grade ?? 0,
                    Type = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.Type ?? 0,
                    Lodging = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.Lodging,
                    IsDeleted = (int)q.State == 4
                }).ToList());
                Console.WriteLine("...√");
                Thread.Sleep(3000);
            }
            return ResponseResult.Success();
        }
        public async Task<ResponseResult> ImportQuestion()
        {
            var lastDate = _dataImport.GetLastUpdate();
            var time = lastDate.FirstOrDefault(q => q.Name.StartsWith("questionindex")).LastTime > Convert.ToDateTime("0001-01-01") ?
                        lastDate.FirstOrDefault(q => q.Name.StartsWith("questionindex")).LastTime : Convert.ToDateTime("1999-01-01");

            for (int i = 0; i < 10000; i++)
            {
                var data = _questionService.GetQuestionData(i, 2000, time);
                if (data.Count == 0)
                {
                    break;
                }
                Console.Write(i);
                List<Guid> Ids = data.GroupBy(q => q.SchoolSectionId).Select(x => x.Key)?.ToList();
                var SchoolInfos = await _schoolService.ListExtSchoolByBranchIds(Ids);

                _dataImport.ImportQuestionData(data.Select(q => new SearchQuestion
                {
                    Id = q.Id,
                    Eid = q.SchoolSectionId,
                    Context = q.QuestionContent,
                    ReplyCount = q.AnswerCount,
                    PublishTime = q.QuestionCreateTime,
                    UpdateTime = q.QuestionCreateTime,
                    CityCode = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.CityCode ?? 0,
                    AreaCode = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.AreaCode ?? 0,
                    Grade = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.Grade ?? 0,
                    Type = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.Type ?? 0,
                    Lodging = SchoolInfos.FirstOrDefault(s => s.ExtId == q.SchoolSectionId)?.Lodging,
                    IsDeleted = (int)q.State == 4
                }).ToList());
                Console.WriteLine("...√");
                Thread.Sleep(2000);
            }
            return ResponseResult.Success();
        }
        public async Task<ResponseResult> ImportRank()
        {
            var lastDate = _dataImport.GetLastUpdate();
            var time = lastDate.FirstOrDefault(q => q.Name.StartsWith("schoolrankindex")).LastTime > Convert.ToDateTime("0001-01-01") ?
                        lastDate.FirstOrDefault(q => q.Name.StartsWith("schoolrankindex")).LastTime : Convert.ToDateTime("1999-01-01");

            int pageCount = 1;
            for (int i = 1; i <= pageCount; i++)
            {
                var data = await _operationClient.GetRankByAfterDate(time, i, 3000);

                if (data == null || data.ErrCode != 1)
                {
                    break;
                }
                pageCount = data.Data.Total / 3000 + 1;

                _dataImport.ImportSchoolRankData(data.Data.Rows.Select(q => new SearchSchoolRank
                {
                    Id = q.Id,
                    Title = q.Title,
                    IsDeleted = !q.IsView,
                    UpdateTime = q.UpdateTime,
                    Schools = q.BindSchools.Select(p => new SchoolRankItem
                    {
                        Name = p.Name,
                        RankNo = p.Sort
                    }).ToList()
                }).ToList());
            }
            return ResponseResult.Success();
        }
        public ResponseResult ImportArticle()
        {
            var lastDate = _dataImport.GetLastUpdate();
            var time = lastDate.FirstOrDefault(q => q.Name.StartsWith("articleindex")).LastTime > Convert.ToDateTime("0001-01-01") ?
                        lastDate.FirstOrDefault(q => q.Name.StartsWith("articleindex")).LastTime : Convert.ToDateTime("1999-01-01");

            for (int i = 0; i < 100; i++)
            {
                var data = _articleService.GetArticleData(i, 3000, time);

                if (data == null || !data.Any())
                    break;

                _dataImport.ImportArticleData(data.Select(q => new SearchArticle
                {
                    Id = q.id,
                    Title = q.title,
                    Tags = q.TagName == null ? new List<string>() : q.TagName.Split(",").ToList(),
                    UpdateTime = q.updateTime ?? DateTime.Now,
                    IsDeleted = !(q.time < DateTime.Now && q.show)
                }).ToList());
            }

            return ResponseResult.Success();
        }

        /// <summary>
        /// 中职批量启用学校
        /// </summary>
        /// <param name="province"></param>
        /// <param name="isDeleted"></param>
        public void UpdateSvsSchool(long province,string isDeleted)
        {
            _dataImport.UpdateSvsSchool(province, isDeleted);
        }

        public ResponseResult CreateCourseIndex()
        {
            _dataImport.CreateCourseIndex();
            return ResponseResult.Success();
        }

        public ResponseResult CreateOrganizationIndex()
        {
            _dataImport.CreateOrganizationIndex();
            return ResponseResult.Success();
        }

        public ResponseResult CreateEvaluationIndex()
        {
            _dataImport.CreateEvaluationIndex();
            return ResponseResult.Success();
        }
    }
}
