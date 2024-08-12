using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using PMS.OperationPlateform.Domain.Enums;
using PMS.School.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(90)]
    public class PCHottestScoreSchoolJob : IRecurringJob
    {
        private ISchoolService _schoolService;
        private IEasyRedisClient _easyRedisClient;

        //评分最高：全国当前最热门的学校信息【点评数最高的】
        private readonly string _CommentHottestKey = "ext:WholeCountryScoreSchools:{0}";

        //获取学段类型
        private readonly int[] _Grade = new int[] { (int)SchoolGrade.Kindergarten, (byte)SchoolGrade.PrimarySchool, (int)SchoolGrade.JuniorMiddleSchool, (int)SchoolGrade.SeniorMiddleSchool };

        public PCHottestScoreSchoolJob(ISchoolService schoolService, IEasyRedisClient easyRedisClient)
        {
            _schoolService = schoolService;
            _easyRedisClient = easyRedisClient;
        }

        public void Execute(PerformContext context)
        {
            var now = DateTime.Now;
            var time = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            foreach (var item in _Grade)
            {
                var list = _schoolService.GetSchoolExtListAsync(0, 0, 0, 0, 0, new int[] { item }, orderBy: 1, new int[] { }, 0, new int[] { }, 1, 10).Result;
                _easyRedisClient.AddAsync(string.Format(_CommentHottestKey, item), list.List, time);
            }
        }

    }
}
