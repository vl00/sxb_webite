using AutoMapper;
using iSchool;
using iSchool.Internal.API.OperationModule;
using iSchool.Internal.API.RankModule;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Application.ModelDto.Query;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using PMS.School.Infrastructure.Common;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class SchService : ISchService
    {
        readonly IMapper mapper;
        readonly IEasyRedisClient _easyRedisClient;
        readonly ISchRepository _schRepository;
        readonly ISchoolScoreRepository _schoolScoreRepository;
        readonly RankApiServices _rankApiServices;
        readonly ILogger<SchService> _logger;

        const string _AddRankKey = "addRank";
        const string _splext = "splext:";
        const string _splext_score = "splext:score:";

        public SchService(IMapper mapper, RankApiServices rankApiServices, IEasyRedisClient _easyRedisClient, ISchRepository schRepository, ISchoolScoreRepository schoolScoreRepository, ILogger<SchService> logger)
        {
            this.mapper = mapper;
            this._easyRedisClient = _easyRedisClient;
            this._schRepository = schRepository;
            _rankApiServices = rankApiServices;
            _schoolScoreRepository = schoolScoreRepository;
            _logger = logger;
        }

        public async Task<SchExtDto0> GetSchextSimpleInfo(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var k = _splext + eid.ToString("n");
            var dto = await _easyRedisClient.GetAsync<SchExtDto0>(k);
            if (dto != null)
            {
                if (dto.ShortSchoolNo == "gg")
                {
                    await _easyRedisClient.RemoveAsync(k);
                }
                else
                {
                    return dto;
                }
            }
            dto = _schRepository.GetSchextSimpleInfo(eid);
            if (dto != null)
                await _easyRedisClient.AddAsync(k, dto, TimeSpan.FromMinutes(30));
            return dto;
        }

        public async Task<SchExtDto0[]> GetNearSchoolsBySchType((double Lng, double Lat) location, SchFType0[] schFTypes = null, int count = 8)
        {
            var schFTypesKey = string.Empty;
            if (schFTypes?.Any() == true)
            {
                var schFTypesCodes = string.Join("_", schFTypes.Select(p => p.Code));
                schFTypesKey = "_" + schFTypesCodes;
            }
            var redisKey = $"NearSchool:{location.Lng}_{location.Lat}_{count}{schFTypesKey}";

            return await _easyRedisClient.GetOrAddAsync(redisKey, async () =>
            {
                var data = _schRepository.GetNearSchoolsBySchType(location, schFTypes, count);
                _logger.LogDebug("GetNearSchoolsBySchType-{0}", redisKey);
                _logger.LogDebug(JsonConvert.SerializeObject(data));
                var scores = await _schoolScoreRepository.GetExt22Scores(data.Select(s => s.Eid));
                //var schoolNos = GetSchoolextNo(data.Select(s => s.Eid).ToArray());
                //if (schoolNos.Count() > 0)
                //{
                //    foreach (var item in schoolNos)
                //    {
                //        var find = data.FirstOrDefault(p => p.Eid == item.Item1);
                //        if (find != null && find.Eid != Guid.Empty) find.SchoolNo = item.Item2;
                //    }
                //}
                foreach (var item in data)
                {
                    var score = scores.Where(s => s.ExtId == item.Eid);
                    if (score.Any())
                    {
                        item.TotalScore = score.FirstOrDefault().Score;
                    }
                }
                return data;
            }, TimeSpan.FromHours(8));
        }

        public async Task<IEnumerable<SchExtDto0>> GetNearSchoolsByEID(Guid eid, int count = 8)
        {
            if (eid == Guid.Empty) return Enumerable.Empty<SchExtDto0>();

            var data = await _schRepository.GetNearSchoolsByEID(eid, count);
            var scores = await _schoolScoreRepository.GetExt22Scores(data.Select(s => s.Eid));
            foreach (var item in data)
            {
                var score = scores.Where(s => s.ExtId == item.Eid);
                if (score.Any())
                {
                    item.TotalScore = score.FirstOrDefault().Score;
                }
            }
            return data;
        }

        /// <summary>
        /// 获取毕业去向的大学榜单
        /// </summary>
        public async Task<List<AchievementInfos>> GetCollegeRankList(int type = 1)
        {
            var rank = await _easyRedisClient.GetAsync<List<AchievementInfos>>(_AddRankKey);
            if (rank != null)
            {
                return rank;
            }

            var responseResult = await _rankApiServices.GetInternationalCollegeRank();
            if (responseResult == null) { return null; }
            var data = JsonHelper.JSONToObject<AchievementData>(responseResult);
            if (data.ErrCode == 0) { return null; }
            //保存缓存中
            await _easyRedisClient.AddAsync(_AddRankKey, data.Data, DateTime.Now.AddDays(3));
            return data.Data;
        }

        public async Task<(Guid, string)[]> GetLocalColleges()
        {
            var k = "local_colleges";
            var ls = await _easyRedisClient.GetAsync<(Guid, string)[]>(k);
            if (ls != null) return ls;

            ls = _schRepository.GetLocalColleges();
            await _easyRedisClient.AddAsync(k, ls);

            return ls;
        }

        public async Task<(Guid, string)[]> GetInternationalRankSchools(string txt, int year, int count = 10)
        {
            var rls = await GetCollegeRankList(1);

            return DistinctBy(
                    rls.SelectMany(_ => _.List)
                        .Where(_ => _.Year == year).SelectMany(_ => _.Items)
                        .Where(_ => string.IsNullOrEmpty(txt) || _.SchoolName.IndexOf(txt) > -1)
                        .Select(_ => (_.SchoolId, _.SchoolName)),
                    _ => _.SchoolId
                )
                .Take(10)
                .ToArray();
        }

        static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
            }
        }

        public async Task<SchExtDto0> GetSchextSimpleInfoViaShortNo(string schoolNo)
        {
            if (string.IsNullOrWhiteSpace(schoolNo)) return null;
            var eid = _schRepository.GetSchoolextID(schoolNo);
            return await GetSchextSimpleInfo(eid);
        }

        public (Guid, int)[] GetSchoolextNo(Guid[] eids)
        {
            if (eids == null || eids.Length < 1) return null;
            return _schRepository.GetSchoolextNo(eids);
        }

        /// <summary>
        /// 毕业生去往国外大学是否在国际大学榜单
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns></returns>
        public async Task<IEnumerable<KeyValueDto<Guid, string, double, int, int>>> GetForeignCollegeList(List<KeyValueDto<Guid, string, double, int>> achievement)
        {
            if (null == achievement)
            {
                return new List<KeyValueDto<Guid, string, double, int, int>>();
            }
            var list = await GetCollegeRankList();
            if (null == list || list.Count == 0)
            {
                return new List<KeyValueDto<Guid, string, double, int, int>>();
            }
            var result = (from items in list
                          from da in achievement
                          from item in items?.List
                          from it in item?.Items
                          where da.Key == it?.SchoolId && da.Data == item.Year
                          select new KeyValueDto<Guid, string, double, int, int>()
                          {
                              Key = it.SchoolId,
                              Value = it.SchoolName,
                              Message = da.Message,
                              Data = it.Sort,
                              Other = items.RankName == "QS" ? 1 : items.RankName == "US News" ? 2 : items.RankName == "Times" ? 3 : -1
                          }).OrderBy(o => o.Other);
            int listCategory = 0;
            var ls = new List<KeyValueDto<Guid, string, double, int, int>>();
            foreach (var item in result)
            {
                if (item.Other != listCategory && listCategory != 0)
                {
                    break;
                }
                listCategory = item.Other;
                ls.Add(item);
            }
            return ls.OrderBy(o => o.Data);
        }

        /// <summary>
        /// 毕业生去往国内大学是否在国内大学榜单
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns></returns>
        public async Task<IEnumerable<KeyValueDto<Guid, string, double, int, int>>> GetDomesticCollegeList(List<KeyValueDto<Guid, string, double, int>> achievement)
        {
            if (null == achievement)
            {
                return new List<KeyValueDto<Guid, string, double, int, int>>();
            }
            var list = await GetLocalColleges();
            if (null == list)
            {
                return new List<KeyValueDto<Guid, string, double, int, int>>();
            }
            var result = (from im in achievement
                          from it in list
                          where im.Key == it.Item1
                          select new KeyValueDto<Guid, string, double, int, int>()
                          {
                              Key = it.Item1,
                              Value = it.Item2,
                              Message = im.Message,
                              Data = im.Data,
                              Other = im.Data
                          })
                          .GroupBy(g => g.Key).Select(s => s.First()).OrderByDescending(o => o.Message);
            return result;
        }
    }
}
