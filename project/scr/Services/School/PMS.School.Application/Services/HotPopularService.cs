using AutoMapper;
using iSchool;
using Newtonsoft.Json.Linq;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Application.ModelDto.Query;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using PMS.School.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class HotPopularService : IHotPopularService
    {
        IEasyRedisClient _easyRedisClient;
        IHotPopularRepository hotPopularRepository;

        const string prexK_hot0 = "hot0exts:";
        const string prexK_hotNear = "hotnearexts:";

        public HotPopularService(IEasyRedisClient _easyRedisClient, IHotPopularRepository hotPopularRepository)
        {
            this._easyRedisClient = _easyRedisClient;
            this.hotPopularRepository = hotPopularRepository;
        }

        // 热门学校
        public async Task<SimpleHotSchoolDto[]> GetHotVisitSchools(int citycode, SchFType0[] schtypes = null, int count = 6)
        {
            if ((schtypes?.Length ?? 0) == 0) schtypes = new SchFType0[] { default };

            var ls = new List<SimpleHotSchoolDto>();
            foreach (var schtype in schtypes)
            {
                var k = !schtype.Equals(default) ? $"{prexK_hot0}{citycode}:{schtype}" : $"{prexK_hot0}{citycode}";
                var json = await _easyRedisClient.GetAsync<string>(k);
                var dtos = json.ToObject<SimpleHotSchoolDto[]>();
                if (dtos == null || dtos.Length < 50)
                {
                    dtos = hotPopularRepository.GetHotVisitSchools(citycode, schtype.Equals(default) ? (SchFType0?)null : schtype, count);

                    if (!await _easyRedisClient.AddAsync(k, dtos.ToJson(), TimeSpan.FromMinutes(15)))
                        throw new Exception($"set to redis error, k={k}");
                }
                ls.AddRange(dtos);
            }

            return ls.OrderByDescending(_ => _.Usercount).ToArray();
        }

        // 周边学校
        public async Task<SmpNearestSchoolDto[]> GetNearestSchools(Guid eid, int count = 6)
        {
            var k = prexK_hotNear + eid.ToString();
            var dtos = await _easyRedisClient.GetAsync<SmpNearestSchoolDto[]>(k);
            if (dtos != null) return dtos;

            dtos = hotPopularRepository.GetNearestSchools(eid, count);

            if (!await _easyRedisClient.AddAsync(k, dtos, TimeSpan.FromMinutes(15)))
                throw new Exception($"set to redis error, k={k}");

            return dtos;
        }

        // 周边学校
        public async Task<SmpNearestSchoolDto[]> GetNearestSchools(int citycode, (double Lng, double Lat) lnglat, SchFType0[] schtypes = null, int count = 6)
        {
            if ((schtypes?.Length ?? 0) == 0) schtypes = new SchFType0[] { default };

            var ls = new List<SmpNearestSchoolDto>();
            foreach (var schtype in schtypes)
            {
                var k = !schtype.Equals(default) ? $"{prexK_hotNear}{citycode}:{schtype}" : $"{prexK_hotNear}{citycode}";
                var dtos = await _easyRedisClient.GetAsync<SmpNearestSchoolDto[]>(k);
                if (dtos == null)
                {
                    dtos = hotPopularRepository.GetNearestSchools(citycode, lnglat, !schtype.Equals(default) ? new[] { schtype } : null, count);

                    if (!await _easyRedisClient.AddAsync(k, dtos, TimeSpan.FromMinutes(15)))
                        throw new Exception($"set to redis error, k={k}");
                }
                ls.AddRange(dtos);
            }

            return ls.OrderBy(_ => _._order).OrderBy(_ => _.Distance).ToArray();
        }

        //// 热评学校
        //public async Task<(Guid Sid, Guid Eid, string SchoolName, int CommentCount)[]> GetHotCommentSchools(int citycode, SchFType0 schtype, int count = 6)
        //{
        //    //var arr = await _easyRedisClient.GetAsync<JArray>($"CommentSchools:CityCode_{citycode}");
        //    //if ((arr?.Count ?? 0) <= 0) return new (Guid, Guid, string, int)[0];
        //    //return arr.Select(a =>
        //    //{
        //    //    try { return (Guid.Parse(a["SchoolId"].ToString()), Guid.Parse(a["SchoolSectionId"].ToString()), a["SchoolName"].ToString(), Convert.ToInt32(a["Total"])); }
        //    //    catch { return default; }
        //    //})
        //    //.Where(a => a.Item1 != Guid.Empty)
        //    //.ToArray();

        //    throw new NotImplementedException();
        //}

        //// 热问学校
        //public async Task<(Guid Sid, Guid Eid, string SchoolName, int QuestionCount)[]> GetHotQuestionSchools(int citycode, SchFType0 schtype, int count = 6)
        //{
        //    //var arr = await _easyRedisClient.GetAsync<JArray>($"QuestionSchools:CityCode_{citycode}");
        //    //if ((arr?.Count ?? 0) <= 0) return new (Guid, Guid, string, int)[0];
        //    //return arr.Select(a =>
        //    //{
        //    //    try { return (Guid.Parse(a["SchoolId"].ToString()), Guid.Parse(a["SchoolSectionId"].ToString()), a["SchoolName"].ToString(), Convert.ToInt32(a["Total"])); }
        //    //    catch { return default; }
        //    //})
        //    //.Where(a => a.Item1 != Guid.Empty)
        //    //.ToArray();

        //    throw new NotImplementedException();
        //}
    }
}
