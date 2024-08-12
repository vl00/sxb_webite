using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;

namespace Hangfire.ConsoleWeb.Jobs
{

    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(90)]
    public class HottestQuestionJob : IRecurringJob
    {
        private IEasyRedisClient _easyRedisClient;
        private IQuestionInfoService _questionInfo;

        public HottestQuestionJob(IEasyRedisClient easyRedisClient, IQuestionInfoService questionInfo)
        {
            _easyRedisClient = easyRedisClient;
            _questionInfo = questionInfo;
        }

        public void Execute(PerformContext context)
        {
            string key = "Question:Hottest";

            DateTime old = DateTime.Now.AddMonths(-6);

            DateTime startTime = new DateTime(old.Year, old.Month, old.Day,0,0,0);
            DateTime entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);


            var question = _questionInfo.GetHotQuestion(startTime, entTime);
            bool rez =  _easyRedisClient.AddAsync(key, question).Result;
        }
    }
}