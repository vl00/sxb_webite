using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PMS.RabbitMQ.Message;
using PMS.School.Application.IServices;
using PMS.Search.Application.IServices;
using PMS.Search.Domain.Entities;
using ProductManagement.Framework.RabbitMQ;

namespace PMS.RabbitMQ.Handle
{
    /// <summary>
    /// 后台学校上下线，更新ES学校数据
    /// </summary>
    public class SyncEsSchoolUpdateHandle : IEventHandler<SyncEsSchoolUpdateMessage>
    {
        private readonly IImportService _dataImport;
        private readonly ISchoolService _schoolService;

        private readonly ILogger<SyncEsSchoolUpdateMessage> _logger;

        public SyncEsSchoolUpdateHandle(ILoggerFactory loggerFactory,
            ISchoolService schoolService, IImportService dataImport)
        {
            _schoolService = schoolService;
            _dataImport = dataImport;
            _logger = loggerFactory.CreateLogger<SyncEsSchoolUpdateMessage>();
        }

        public Task Handle(SyncEsSchoolUpdateMessage message)
        {
            try
            {
                //调用目标service业务代码
                var sid = message.Sid;

                var data = _schoolService.GetSchoolDataBySchoolId(sid);

                if (data.Count == 0)
                {
                    return Task.CompletedTask;
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
                    Tags = q.Tags,
                    Tuition = q.Tuition,
                    UpdateTime = q.UpdateTime,
                    Grade = q.Grade,
                    Score = q.Score != null ? new SearchSchoolScore
                    {
                        Composite = q.Score.Score15,
                        Teach = q.Score.Score16,
                        Hard = q.Score.Score17,
                        Course = q.Score.Score18,
                        Learn = q.Score.Score19,
                        Cost = q.Score.Score20,
                        Total = q.Score.Score22
                    } : null,
                    Schooltype = q.Schooltype,
                    SchooltypeCode = q.SchooltypeCode,
                    SchooltypeNewCode = q.SchooltypeNewCode,
                    IsDeleted = !(q.IsValid && (q.Status == 3))
                }).ToList());

            }
            catch (Exception e)
            {
                _logger.LogError(e, "ES更新学校数据,mq同步错误：{0}",
                    String.Join(",", message.Sid));
            }
            return Task.CompletedTask;
        }
    }
}
