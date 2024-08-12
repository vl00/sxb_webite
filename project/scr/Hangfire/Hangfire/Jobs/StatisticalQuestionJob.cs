using Hangfire.Console;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(90)]
    public class StatisticalQuestionJob : IRecurringJob
    {

        private readonly IQuestionInfoService _questionService;
        private readonly ISchoolQuestionTotalMQService _mqService;
        private readonly ISchoolCommentScoreService _scoreService;
        public StatisticalQuestionJob(IQuestionInfoService questionService, ISchoolCommentScoreService scoreService, ISchoolQuestionTotalMQService mqService)
        {
            _questionService = questionService;
            _mqService = mqService;
            _scoreService = scoreService;
        }

        public void Execute(PerformContext context)
        {
            DateTime lastUpdateTime = _scoreService.GetQuestionLastUpdateTime();
            DateTime nowTime = DateTime.Now;

            int pageSize = 1000;
            int total = _questionService.SchoolCommentQuestionCountByTime(lastUpdateTime, nowTime);

            int totalPage = total / pageSize + 1;

            for (int pageIndex = 1; pageIndex <= totalPage; pageIndex++)
            {
                var questionTotals = _questionService.PageQuestionTotalTime(lastUpdateTime, nowTime, pageIndex, pageSize);

                if (questionTotals.Count > 0)
                {
                    //var schoolSectionIds = questionTotals.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToArray();
                    context.WriteLine($"正在更新第{pageIndex}页，共{questionTotals.Count}条，{questionTotals.Count()}所学校分部");

                    int i = 1;
                    foreach (var section in questionTotals)
                    {
                        //var questionTotal = questionTotals.Where(q => q.SchoolSectionId == sectionId).FirstOrDefault();
                        _scoreService.UpdateSchoolScore(section);
                    }
                }
            }

            var list = _scoreService.ListNewQuestion(lastUpdateTime);
            Parallel.For(0, list.Count / 100 + 1, new ParallelOptions { MaxDegreeOfParallelism = 10 }, t =>
            {
                var result = list.Skip(t * 100).Take(100).ToList();
                _mqService.SendSyncSchoolScoreMessage(result);
            });
        }


    }
}
