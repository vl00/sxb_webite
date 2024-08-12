using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using PMS.Search.Domain.IRepositories;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.Services
{
    public class HistoryService : IHistoryService
    {
        public IsHost _IsHost { get; set; }

        private IHistoryRepository _history;
        private IArticleRepository _articleRepository;
        private IArticleCoverRepository _articleCoverRepository;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly School.Domain.IRepositories.ISchoolRepository _schoolRepository;
        private readonly ISchoolCommentRepository _schoolCommentRepository;
        private readonly IQuestionInfoRepository _questionInfoRepository;
        private readonly IDataImport _dataImport;

        public HistoryService(IHistoryRepository history, IArticleRepository articleRepository, IArticleCoverRepository articleCoverRepository, IOptions<IsHost> IsHost, IEasyRedisClient easyRedisClient, School.Domain.IRepositories.ISchoolRepository schoolRepository, ISchoolCommentRepository schoolCommentRepository, IQuestionInfoRepository questionInfoRepository, IDataImport dataImport)
        {
            _history = history;
            _articleRepository = articleRepository;
            _articleCoverRepository = articleCoverRepository;
            _IsHost = IsHost.Value;
            _easyRedisClient = easyRedisClient;
            _schoolRepository = schoolRepository;
            _schoolCommentRepository = schoolCommentRepository;
            _questionInfoRepository = questionInfoRepository;
            _dataImport = dataImport;
        }

        public bool AddHistory(Guid userID, Guid dataID, byte dataType)
        {
            return _history.AddHistory(userID, dataID, dataType);
        }

        public List<Guid> GetUserHistory(Guid userID, byte dataType, int page = 1, int pageSize = 10)
        {
            return _history.GetUserHistory(userID, dataType, page, pageSize);
        }

        public bool RemoveHistory(Guid userID, Guid dataID)
        {
            return _history.RemoveHistory(userID, dataID);
        }

        public List<Guid> GetUserHistory(Guid userID, byte dataType, int page)
        {
            return _history.GetUserHistory(userID, dataType, page, 10);
        }
        public List<SchoolModel> GetSchoolHistory(Guid userID, double? lat = null, double? lng = null, int page = 1)
        {
            var iDList = GetUserHistory(userID, 1, page);
            var list = HttpHelper.HttpPost<List<SchoolModel>>(
                $"{_IsHost.SiteHost_M}/School/GetCollectionExt", Newtonsoft.Json.JsonConvert.SerializeObject(iDList), "application/json");
            foreach (var school in list)
            {
                if (lat != null && lng != null && school.Latitude != null && school.Longitude != null)
                {
                    school.Distance = CommonHelper.FormatDistance(CommonHelper.GetDistance(lat.Value, lng.Value, school.Latitude.Value, school.Longitude.Value));
                }
            }
            return list;
        }
        public VoBase GetCommentHistory(Guid userID, string cookieStr, int page = 1)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", cookieStr);
            var iDList = GetUserHistory(userID, 3, page);
            List<object> requestObj = new List<object>();
            foreach (var id in iDList)
            {
                requestObj.Add(new { dataIdType = 1, dataId = id.ToString() });
            }
            var res = HttpHelper.HttpPost<VoBase>(
                $"{_IsHost.SiteHost_M}/SchoolComment/GetSchooCommentOrReply",
                Newtonsoft.Json.JsonConvert.SerializeObject(requestObj), contentType: "application/json", headers: headers);
            return res;
        }
        public VoBase GetQAHistory(Guid userID, string cookieStr, int page = 1)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", cookieStr);
            var iDList = GetUserHistory(userID, 2, page);
            List<object> requestObj = new List<object>();
            foreach (var id in iDList)
            {
                requestObj.Add(new { dataIdType = 2, dataId = id.ToString() });
            }
            var res = HttpHelper.HttpPost<VoBase>(
                $"{_IsHost.SiteHost_M}/Question/GetQuestionOrAnswer",
                Newtonsoft.Json.JsonConvert.SerializeObject(requestObj), contentType: "application/json", headers: headers);
            return res;
        }
        public List<Data> GetArticleHistory(Guid userID, int page = 1)
        {
            var iDList = GetUserHistory(userID, 0, page);
            return GetArticle(iDList);
        }

        public List<Data> GetArticle(List<Guid> Ids)
        {
            var articles = _articleRepository.SelectByIds(Ids.ToArray());

            if (articles == null) return new List<Data>();


            //查询出目标背景图片
            var effactiveIds = articles.Select(a => a.id).ToArray();
            var covers = this._articleCoverRepository.GetCoversByIds(effactiveIds);


            List<article> _articles = new List<article>();
            if (Ids != null)
            {
                foreach (var id in Ids)
                {
                    var article = articles.FirstOrDefault(a => a.id == id);
                    if (article != null)
                    {
                        article.Covers = covers.Where(c => c.articleID == article.id).ToList();
                        _articles.Add(article);
                    }
                }
            }
            var result = _articles.Select(a => new Data()
            {
                id = a.id,
                title = a.title,
                isShow = a.show,
                time = a.time.GetValueOrDefault(),
                covers = covers.Where(c => c.articleID == a.id).Select(c => $"https://cdn.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}").ToArray(),
                digest = a.overview,
                layout = a.layout,
                viewCount = a.viewCount.GetValueOrDefault() + a.viewCount_r.GetValueOrDefault()

            }).ToList();
            return result;
        }


        public List<Data> ApiArticle(List<Guid> Ids)
        {
            List<Data> OutputResult = new List<Data>();
            if (!Ids.Any())
            {
                return OutputResult;
            }
            var result = HttpHelper.HttpPost<ArticleModel>(
                $"{_IsHost.ConsoleHost_Operation}/api/ArticleApi/GetArticlesByIds",
                Newtonsoft.Json.JsonConvert.SerializeObject(Ids), "application/json");

            if (result == null || result.data == null)
            {
                return OutputResult;
            }

            foreach (Guid id in Ids)
            {
                var r = result.data.Find(a => a.id == id);
                if (r == null)
                {
                    OutputResult.Add(new Data() { id = id, isShow = false });
                }
                else
                {
                    OutputResult.Add(r);
                }
            }
            return OutputResult;
        }

        public bool ChangeHistoryState(List<Guid> Ids, Guid UserId)
        {
            return _history.ChangeHistoryState(Ids, UserId);
        }

        public bool ClearAllHistory(Guid userId, int type)
        {
            return _history.ClearAllHistory(userId, type);
        }

        public async Task<IEnumerable<Guid>> RefreshAndGetTop(MessageDataType dataType)
        {
            //最大条数
            int size = 100;
            //最小条数
            int minSize = 10;
            //48小时内
            int days = -2;

            //48h内的数据
            var date = DateTime.Now.AddDays(days);
            var data = (await _history.GetHistoryTopId(dataType, date, DateTime.Now, size)).Distinct().ToList();
            var count = data.Count;
            if (count < minSize)
            {
                //不足, 取两天之前所有数据
                var allData = await _history.GetHistoryTopId(dataType, startTime: null, endTime: date, size - count);
                data.AddRange(allData);
            }
            data = data.Distinct().ToList();

            //排除被删除/禁用/下架的数据
            var avaliableIds = Enumerable.Empty<Guid>();
            switch (dataType)
            {
                case MessageDataType.School:
                    avaliableIds = await _schoolRepository.GetAvailableExtIds(data);
                    break;
                case MessageDataType.Question:
                    avaliableIds = _questionInfoRepository.GetAvailableIds(data);
                    break;
                case MessageDataType.Comment:
                    avaliableIds = _schoolCommentRepository.GetAvailableIds(data);
                    break;
            }
            //维持原有顺序
            data = data.Where(s => avaliableIds.Contains(s)).ToList();

            var key = string.Format(RedisKeys.HotHistoryKey, dataType.ToString().ToLower());
            await _easyRedisClient.AddAsync(key, data, TimeSpan.FromMinutes(70));

            return data;
        }

        public async Task<IEnumerable<Guid>> GetTop(MessageDataType dataType, int takeSize)
        {
            var key = string.Format(RedisKeys.HotHistoryKey, dataType.ToString().ToLower());
            var tops = await _easyRedisClient.GetAsync<IEnumerable<Guid>>(key);
            if (tops?.Any() != true)
            {
                tops = await RefreshAndGetTop(dataType);
            }
            return tops?.Take(takeSize) ?? Enumerable.Empty<Guid>();
        }

        public async Task<IEnumerable<Guid>> GetRandomTop(MessageDataType dataType, int takeSize, IEnumerable<Guid> excludeIds)
        {
            var key = string.Format(RedisKeys.HotHistoryKey, dataType.ToString().ToLower());
            var tops = await _easyRedisClient.GetAsync<IEnumerable<Guid>>(key);
            if (tops?.Any() != true)
            {
                tops = await RefreshAndGetTop(dataType);
            }

            var ids = CommonHelper.ListRandom(tops, excludeIds);
            return ids.Take(takeSize);
        }

        public async Task<bool> BulkSchoolViewCount()
        {
            //默认查询前一天的所有数据
            DateTime? startTime = null;// DateTime.Now.AddDays(-1);
            DateTime? endTime = null;// DateTime.Today;

            int pageIndex = 1;
            int pageSize = 1000;
            bool end = false;
            while (!end)
            {
                var histories = await _history.GetHistoryTop(MessageDataType.School, startTime, endTime, pageIndex++, pageSize);
                var data = histories.Select(s => new Search.Domain.Entities.SearchSchoolViewCount()
                {
                    Id = s.DataId,
                    ViewCount = s.ViewCount,
                    HotValue = s.ViewCount
                }).ToList();
                _dataImport.UpdateSchoolViewCount(data);

                //数据不足, 终止
                if (data.Count < pageSize)
                {
                    end = true;
                }
            }

            return true;
        }
    }
}
