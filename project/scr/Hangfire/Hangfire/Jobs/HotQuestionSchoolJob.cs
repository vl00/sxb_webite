using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using PMS.CommentsManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(90)]
    public class HotQuestionSchoolJob : IRecurringJob
    {
        private IEasyRedisClient _easyRedisClient;
        private IQuestionInfoService _questionInfo;

        public HotQuestionSchoolJob(IEasyRedisClient easyRedisClient,IQuestionInfoService questionInfo)
        {
            _easyRedisClient = easyRedisClient;
            _questionInfo = questionInfo;
        }

        public void Execute(PerformContext context)
        {
            DateTime old = DateTime.Now.AddMonths(-6);

            DateTime startTime = new DateTime(old.Year, old.Month, old.Day, 0, 0, 0);
            DateTime entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            string key = "Question:HottestSchool";
            var HotQuestionSchool = _questionInfo.GetHotQuestionSchools(startTime, entTime);

            var a =  _easyRedisClient.AddAsync(key, HotQuestionSchool).Result;
        }
    }
}
