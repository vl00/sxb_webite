using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(90)]
    public class HottestCommentJob : IRecurringJob
    {
        private ISchoolCommentService _commentService;
        private IEasyRedisClient _easyRedisClient;
        private readonly ISchoolInfoService _schoolInfoService;

        public HottestCommentJob(ISchoolCommentService commentService, ISchoolInfoService schoolInfoService, IEasyRedisClient easyRedisClient)
        {
            _commentService = commentService;
            _easyRedisClient = easyRedisClient;
            _schoolInfoService = schoolInfoService;
        }

        public void Execute(PerformContext context)
        {
            DateTime old = DateTime.Now.AddMonths(-3);

            DateTime startTime = new DateTime(old.Year, old.Month, old.Day, 0, 0, 0);
            DateTime entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            var comment = _commentService.HottestComment(startTime, entTime);

            //var schoolExt = _schoolInfoService.GetSchoolName(comment.Select(x=>x.SchoolSectionId).ToList());
            //comment.ForEach(x =>
            //{
            //    x.SchoolInfo = new SchoolInfoDto()
            //    {
            //        SchoolSectionId = x.SchoolSectionId,
            //        SchoolId = x.SchoolId,
            //        SchoolName = schoolExt.Where(s => s.SchoolSectionId == x.SchoolSectionId).FirstOrDefault().SchoolName
            //    };
            //});
            string key = $"Comment:Hottest";
            _easyRedisClient.AddAsync(key, comment);

            var b = _easyRedisClient.GetAsync<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto>(key);
        }

    }
}
