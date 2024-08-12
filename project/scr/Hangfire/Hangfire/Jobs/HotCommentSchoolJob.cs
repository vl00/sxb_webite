using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(90)]
    public class HotCommentSchoolJob : IRecurringJob
    {
        private ISchoolCommentService _commentService;
        private IEasyRedisClient _easyRedisClient;

        public HotCommentSchoolJob(ISchoolCommentService commentService, IEasyRedisClient easyRedisClient)
        {
            _commentService = commentService;
            _easyRedisClient = easyRedisClient;
        }

        public void Execute(PerformContext context)
        {
            //var school = _commentService.GetHotCommentSchools(DateTime.Now);
            //var group = school.GroupBy(x => x.City).ToList();
            //string key = "";
            //foreach (var item in group)
            //{
            //    key = $"CommentSchools:CityCode_{item.Key}";
            //    _easyRedisClient.AddAsync(key, item.ToArray());
            //}
            DateTime old = DateTime.Now.AddMonths(-6);

            DateTime startTime = new DateTime(old.Year, old.Month, old.Day, 0, 0, 0);
            DateTime entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            string key = "Comment:HottestSchool";
            var HottestSchool = _commentService.HottestSchool(new HotCommentQuery()
            {
                StartTime = startTime,
                EndTime = entTime
            }, true).Result;
            _easyRedisClient.AddAsync(key, HottestSchool);

            var a = _easyRedisClient.GetAsync<HotCommentSchoolDto>(key);
        }


    }
}
