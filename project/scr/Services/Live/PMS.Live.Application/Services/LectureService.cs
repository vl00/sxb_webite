using PMS.Live.Application.IServices;
using PMS.Live.Domain.Dtos;
using PMS.Live.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PMS.Live.Application.Services
{
    public class LectureService : ILectureService
    {
        ILectureRepository _lectureRepository;
        IEasyRedisClient _easyRedisClient;
        public LectureService(ILectureRepository lectureRepository, IEasyRedisClient easyRedisClient)
        {
            _easyRedisClient = easyRedisClient;
            _lectureRepository = lectureRepository;
        }

        public async Task<IEnumerable<LectorLiveStatusDto>> GetLectorLiveStatus(IEnumerable<Guid> userIDs)
        {
            if (userIDs?.Any() == true)
            {
                return await _lectureRepository.GetLectorLiveStatus(userIDs);
            }
            return null;
        }

        public async Task<IEnumerable<LectureDetailDto>> GetLectureByDate(DateTime beginTime, DateTime endTime)
        {
            if (beginTime == default) return null;
            if (endTime == default) endTime = DateTime.Now;
            var finds = await _lectureRepository.GetByDate(beginTime, endTime);
            if (finds?.Any() == true)
            {
                return finds.Select(p => new LectureDetailDto()
                {
                    ID = p.Id,
                    ViewCount = p.UserCount
                });
            }
            return null;
        }

        public async Task<LectureDetailDto> GetLectureDetail(Guid id)
        {
            if (id != default)
            {
                var find = await _lectureRepository.GetByID(id);
                if (find != null)
                {
                    return new LectureDetailDto()
                    {
                        CoverUrl = find.Cover_home,
                        ID = find.Id,
                        LectorID = find.Lector_id,
                        Status = (Domain.Enums.LectureStatus)find.Status,
                        ViewCount = find.UserCount,
                        CreateTime = find.Createtime,
                        Title = find.Subject
                    };
                }
            }
            return null;
        }

        public async Task<IEnumerable<LectureDetailDto>> GetLectureDetails(IEnumerable<Guid> ids)
        {
            if (ids?.Any() == true)
            {
                var finds = await _lectureRepository.GetByIDs(ids);
                if (finds?.Any() == true)
                {
                    var result = new List<LectureDetailDto>();

                    foreach (var item in finds)
                    {
                        var entity = new LectureDetailDto()
                        {
                            CoverUrl = item.Cover_home,
                            ID = item.Id,
                            Status = (Domain.Enums.LectureStatus)item.Status
                        };
                        entity.ViewCount = await _easyRedisClient.GetOrAddAsync<long>($"recode_lecture_usercount_lectureid_{item.Id}", () =>
                        {
                            return item.UserCount;
                        }, TimeSpan.FromMinutes(5));
                        result.Add(entity);
                    }
                    return result;
                }
            }
            return new List<LectureDetailDto>();
        }

        public async Task<LectureDetailDto> GetLectureDetailWithLectorUserID(Guid id)
        {
            var find = await _lectureRepository.GetByIDWithLectorUserID(id);
            if (find.Item1?.Id != default)
            {
                return new LectureDetailDto()
                {
                    CoverUrl = find.Item1.Cover_home,
                    CreateTime = find.Item1.Createtime,
                    ID = find.Item1.Id,
                    LectorID = find.Item1.Lector_id,
                    LectorUserID = find.Item2,
                    Status = (Domain.Enums.LectureStatus)find.Item1.Status,
                    Title = find.Item1.Subject,
                    //ViewCount = find.Item1.UserCount
                    ViewCount = await _easyRedisClient.GetOrAddAsync<long>($"recode_lecture_usercount_lectureid_{find.Item1.Id}", () =>
                    {
                        return find.Item1.UserCount;
                    }, TimeSpan.FromMinutes(5))
                };
            }
            return null;
        }

        public async Task<Dictionary<Guid, int>> GetLectureStatus(IEnumerable<Guid> ids)
        {
            if (ids?.Any() == true)
            {
                var finds = await _lectureRepository.GetLectureStatus(ids);
                if (finds?.Any() == true)
                {
                    return finds.ToDictionary(k => k.ID, v => (int)v.Status);
                }
            }
            return null;
        }

        public async Task<IEnumerable<LectureDetailDto>> GetNewestLecture(int count)
        {
            var finds = await _lectureRepository.GetByDate(DateTime.Now.AddYears(-1), DateTime.Now, 1);
            if (finds?.Any() == true)
            {
                return finds.Select(p => new LectureDetailDto()
                {
                    CoverUrl = p.Cover_home,
                    CreateTime = p.Createtime,
                    ID = p.Id,
                    LectorID = p.Lector_id,
                    Status = (Domain.Enums.LectureStatus)p.Status,
                    Title = p.Subject,
                    ViewCount = p.UserCount
                });
            }
            return null;
        }
    }
}
