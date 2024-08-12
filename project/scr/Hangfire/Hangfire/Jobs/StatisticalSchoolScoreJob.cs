using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Console;
using Hangfire.ConsoleWeb.Repository;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using ProductManagement.Framework.MSSQLAccessor;

namespace Hangfire.ConsoleWeb.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(90)]
    public class StatisticalSchoolScoreJob : IRecurringJob
    {
        private readonly ISchoolCommentScoreService _scoreService;
        private readonly ISchoolScoreMQService _mqService;

        private CommentExecuteRepository _commentExecute;

        public StatisticalSchoolScoreJob(ISchoolCommentScoreService scoreService,ISchoolScoreMQService mqService)
        {
            _scoreService = scoreService;
            _mqService = mqService;

            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot Configuration = builder.Build();

            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger logger = loggerFactory.CreateLogger<DBContext<DataDbContext>>();

            var PGconfig = Configuration.GetSection("DbConnections").Get<List<ConnectionConfig<DataDbContext>>>().FirstOrDefault();
            var Manager = new ConnectionsManager<DataDbContext>(PGconfig);

            //_commentQueryRep = new CommentQueryRepository(new DataDbContext(Manager, loggerFactory.CreateLogger<DBContext<DataDbContext>>()));

            var ExecPGconfig = Configuration.GetSection("ExecuteDbConnections").Get<List<ConnectionConfig<DataDbContext>>>().FirstOrDefault();
            var ExecManager = new ConnectionsManager<DataDbContext>(ExecPGconfig);

            _commentExecute = new CommentExecuteRepository(new DataDbContext(ExecManager, loggerFactory.CreateLogger<DBContext<DataDbContext>>()), new DataDbContext(Manager, loggerFactory.CreateLogger<DBContext<DataDbContext>>()));
        }



        public void Execute(PerformContext context)
        {
            context.WriteLine($"开始统计学校分数：{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");

            //开始计时
            var sw = Stopwatch.StartNew();
            //WebConsole进度条
            var progressBar = context.WriteProgressBar();


            DateTime lastUpdateTime = _scoreService.GetLastUpdateTime();
            DateTime nowTime = DateTime.Now;
            context.WriteLine($"获取到最后更新的点评记录时间：{lastUpdateTime.ToString("yyyy/MM/dd HH:mm:ss")}");

            int pageSize = 1000;

            //进度条加1%
            progressBar.SetValue(5);


            int total = _scoreService.SchoolCommentScoreCountByTime(lastUpdateTime, nowTime);
            context.WriteLine($"{total}条点评新记录需要统计更新");

            int totalPage = total / pageSize + 1;

            for (int pageIndex = 1; pageIndex <= totalPage; pageIndex++)
            {
                var commentScores = _scoreService.PageSchoolCommentScoreByTime(lastUpdateTime, nowTime, pageIndex, pageSize);

                if (commentScores.Count > 0)
                {
                    var userInfos = _commentExecute.GetUserInfo(commentScores.Select(x => x.UserId).ToList()).Select(x=>x.Id);

                    var schoolSectionIds = commentScores.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToArray();
                    context.WriteLine($"正在更新第{pageIndex}页，共{commentScores.Count}条，{schoolSectionIds.Count()}所学校分部");

                    int i = 1;
                    foreach (var sectionId in schoolSectionIds)
                    {
                        var schoolScores = commentScores.Where(q => q.SchoolSectionId == sectionId && !userInfos.Contains(q.UserId)).ToList();

                        var attendScore = schoolScores.Where(q => q.IsAttend == true && !userInfos.Contains(q.UserId)).ToList();

                        SchoolScoreDto schoolScore = new SchoolScoreDto
                        {
                            SchoolId = schoolScores.FirstOrDefault().SchoolId,
                            SchoolSectionId = sectionId,
                            AggScore = schoolScores.Sum(q => q.AggScore),
                            CommentCount = schoolScores.Count,
                            AttendCommentCount = attendScore.Count,
                            EnvirScore = attendScore.Sum(q => q.EnvirScore),
                            HardScore = attendScore.Sum(q => q.HardScore),
                            LifeScore = attendScore.Sum(q => q.LifeScore), 
                            ManageScore = attendScore.Sum(q => q.ManageScore),
                            TeachScore = attendScore.Sum(q => q.TeachScore),
                            LastCommentTime = schoolScores.OrderByDescending(q => q.UpdateTime).FirstOrDefault().UpdateTime
                        };
                        _scoreService.UpdateSchoolScore(schoolScore);

                        double pro = 100 * (pageIndex / Convert.ToDouble(totalPage)) * ((i++) / schoolSectionIds.Count());
                        progressBar.SetValue(pro);
                    }
                }
            }

            //把更新的所有学校分数发布到RabbitMQ
            var list =_scoreService.ListNewSchoolScores(lastUpdateTime);
            //foreach (var itemScore in list)
            //{
            //    _mqService.SendSyncSchoolScoreMessage(itemScore);
            //}

            Parallel.For(0, list.Count/100 + 1, new ParallelOptions { MaxDegreeOfParallelism = 10 }, t =>
            {
                var result = list.Skip(t * 100).Take(100).ToList();
                _mqService.SendSyncSchoolScoreMessage(result);
            });


            progressBar.SetValue(100);
            context.WriteLine($"耗时：{sw.Elapsed.TotalSeconds.ToString()}s");
        }

        public void SchoolCommentScore() 
        {
        
        }

    }
}
