using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.MSSQLAccessor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Hangfire.ConsoleWeb.Repository;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Application.IServices;
using Hangfire;

namespace Hangfire.ConsoleWeb.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(90)]
    public class ImportChangeJob : IRecurringJob
    {
        private CommentQueryRepository _commentQueryRep;
        private CommentExecuteRepository _commentExecute;

        private readonly ISchoolScoreMQService _mqService;
        private readonly ISchoolQuestionTotalMQService _mqqService;

        private readonly ISchoolCommentScoreService _scoreService;

        public ImportChangeJob(ISchoolCommentScoreService scoreService,
            ISchoolScoreMQService mqService,
            ISchoolQuestionTotalMQService mqqService) 
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot Configuration = builder.Build();

            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger logger = loggerFactory.CreateLogger<DBContext<DataDbContext>>();

            var PGconfig = Configuration.GetSection("DbConnections").Get<List<ConnectionConfig<DataDbContext>>>().FirstOrDefault();
            var Manager = new ConnectionsManager<DataDbContext>(PGconfig);

            _commentQueryRep = new CommentQueryRepository(new DataDbContext(Manager, loggerFactory.CreateLogger<DBContext<DataDbContext>>()));

            var ExecPGconfig = Configuration.GetSection("ExecuteDbConnections").Get<List<ConnectionConfig<DataDbContext>>>().FirstOrDefault();
            var ExecManager = new ConnectionsManager<DataDbContext>(ExecPGconfig);

            _commentExecute = new CommentExecuteRepository(new DataDbContext(ExecManager, loggerFactory.CreateLogger<DBContext<DataDbContext>>()), new DataDbContext(Manager, loggerFactory.CreateLogger<DBContext<DataDbContext>>()));

            _scoreService = scoreService;

            _mqService = mqService;
            _mqqService = mqqService;
        }

        public void Execute(PerformContext context)
        {
            //删除已成功导入，且导入类型为生成的点评、提问、回答、点评分数，条件为该学校已删除
            //_commentQueryRep.DeleteCreate();

            //定时启动 每半个小时启动一次，获取过去半个小时内产生的数据
            int pageSize = 2000;

            //获取点评
            //List<SchoolComment> comments = _commentQueryRep.QueryImportComments(1, pageSize);

            //if (comments.Any()) 
            //{
            //    //点评推送正式服务器
            //    _commentExecute.ExecuteTransaction(comments);
            //}

            //统计点评数据
            //int commentTotal = _commentQueryRep.GetStatisticsComment();
            //if (commentTotal > 0) 
            //{
            //    int totalPage = commentTotal / pageSize + 1;

            //    List<SchoolScoreDto> scoreDtos = new List<SchoolScoreDto>();

            //    for (int pageIndex = 1; pageIndex <= totalPage; pageIndex++)
            //    {


            //        //点评分数统计【学校】
            //        //var schoolSectionIds = comments.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToArray();

            //        //获取时间状态
            //        //var schoolSectionTime = _commentExecute.GetSchoolSectionSchoolStatusTime(schoolSectionIds.ToList());

            //        ////获取分数
            //        //var commentScores = comments.Select(x => x.SchoolCommentScore).ToList();

            //        //commentScores.ForEach(x => {
            //        //    x.SchoolComment = comments.Where(c => c.Id == x.CommentId).FirstOrDefault();
            //        //});

            //        //foreach (var sectionId in schoolSectionIds)
            //        //{
            //        //    //var commentIds = comments.Where(x => x.SchoolSectionId == sectionId).Select(x=>x.Id).ToList();
            //        //    var schoolScores = commentScores.Where(q => q.SchoolComment.SchoolSectionId == sectionId).ToList();

            //        //    var attendScore = schoolScores.Where(q => q.IsAttend == true).ToList();

            //        //    //获取当前学校是否存在最新数据信息
            //        //    var SchoolSectionTime = schoolSectionTime.Where(x => x.SchoolSectionId == sectionId).FirstOrDefault();

            //        //    //当前统计中最新的时间
            //        //    DateTime newTime = schoolScores.OrderByDescending(q => q.SchoolComment.AddTime).FirstOrDefault().SchoolComment.AddTime;

            //        //    DateTime lastCommentTime = SchoolSectionTime == null ? newTime : SchoolSectionTime.LastCommentTime > newTime ? SchoolSectionTime.LastCommentTime : newTime;

            //        //    SchoolScoreDto schoolScore = new SchoolScoreDto
            //        //    {
            //        //        SchoolId = schoolScores.FirstOrDefault().SchoolComment.SchoolId,
            //        //        SchoolSectionId = sectionId,
            //        //        AggScore = schoolScores.Sum(q => q.AggScore),
            //        //        CommentCount = schoolScores.Count(),
            //        //        AttendCommentCount = attendScore.Count(),
            //        //        EnvirScore = attendScore.Sum(q => q.EnvirScore),
            //        //        HardScore = attendScore.Sum(q => q.HardScore),
            //        //        LifeScore = attendScore.Sum(q => q.LifeScore),
            //        //        ManageScore = attendScore.Sum(q => q.ManageScore),
            //        //        TeachScore = attendScore.Sum(q => q.TeachScore),
            //        //        LastCommentTime = lastCommentTime
            //        //    };

            //        //    scoreDtos.Add(schoolScore);
            //        //    _scoreService.UpdateSchoolScore(schoolScore);
            //        //}
            //    }

            //    ////推送至学校库【学校点评分数表】
            //    //Parallel.For(0, scoreDtos.Count / 100 + 1, new ParallelOptions { MaxDegreeOfParallelism = 10 }, t =>
            //    //{
            //    //    var result = scoreDtos.Skip(t * 100).Take(100).ToList();
            //    //    _mqService.SendSyncSchoolScoreMessage(result);
            //    //});
            //}

            //获取提问

            //List<QuestionInfo> questions = _commentQueryRep.QueryImportQuestion(1, pageSize);
            //if (questions.Any())
            //{
            //  _commentExecute.QAExecuteTransaction(questions);
            //}


            //统计提问数据
            //int questionTotal = _commentQueryRep.GetStatisticsQuestion();
            //if (questionTotal > 0) 
            //{
            //    List<SchoolScoreDto> scoreDtos = new List<SchoolScoreDto>();

            //    int totalPage = questionTotal / pageSize + 1;

            //    for (int pageIndex = 1; pageIndex <= totalPage; pageIndex++)
            //    {
            //        //获取提问
            //        List<QuestionInfo> questions = _commentQueryRep.QueryImportQuestion(pageIndex,pageSize);

            //        _commentExecute.QAExecuteTransaction(questions);

            //        ////QuestionTotalByTimeDto
            //        ////点评分数统计【学校】
            //        //var schoolSectionIds = questions.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToArray();

            //        ////获取时间状态
            //        //var schoolSectionTime = _commentExecute.GetSchoolSectionSchoolStatusTime(schoolSectionIds.ToList());

            //        //List<QuestionTotalByTimeDto> question = new List<QuestionTotalByTimeDto>();
            //        //foreach (var item in schoolSectionIds)
            //        //{
            //        //    var SectionQuestion = questions.Where(x => x.SchoolSectionId == item).ToList();

            //        //    //获取当前学校是否存在最新数据信息
            //        //    var SchoolSectionTime = schoolSectionTime.Where(x => x.SchoolSectionId == item).FirstOrDefault();

            //        //    //当前统计中最新的时间
            //        //    DateTime newTime = SectionQuestion.OrderByDescending(x => x.CreateTime).Select(x => x.CreateTime).FirstOrDefault();

            //        //    DateTime lastQuestionTime = SchoolSectionTime == null ? newTime : SchoolSectionTime.LastCommentTime > newTime ? SchoolSectionTime.LastCommentTime : newTime;

            //        //    QuestionTotalByTimeDto totalByTimeDto = new QuestionTotalByTimeDto() 
            //        //    {
            //        //        School = SectionQuestion.FirstOrDefault().SchoolId,
            //        //        SchoolSectionId = item,
            //        //        CreateTime = lastQuestionTime,
            //        //        Total = SectionQuestion.Count()
            //        //    };

            //        //    scoreDtos.Add(new SchoolScoreDto() {
            //        //        QuestionCount = totalByTimeDto.Total,
            //        //        SchoolId = totalByTimeDto.School,
            //        //        SchoolSectionId = totalByTimeDto.SchoolSectionId,
            //        //        LastQuestionTime = totalByTimeDto.CreateTime
            //        //    });

            //        //    _scoreService.UpdateSchoolScore(totalByTimeDto);
            //        //}
            //    }

            //    Parallel.For(0, scoreDtos.Count / 100 + 1, new ParallelOptions { MaxDegreeOfParallelism = 10 }, t =>
            //    {
            //        var result = scoreDtos.Skip(t * 100).Take(100).ToList();
            //        _mqService.SendSyncSchoolScoreMessage(result);
            //    });
            //}


            //获取回答
            //int answerTotal = _commentQueryRep.GetStatisticsAnswer();
            //if (answerTotal > 0)
            //{
            //    int totalPage = answerTotal / pageSize + 1;

            //    for (int pageIndex = 1; pageIndex <= totalPage; pageIndex++)
            //    {
            //        List<QuestionsAnswersInfo> temp = _commentQueryRep.QueryImportAnswer(pageIndex, pageSize);
            //        foreach (var item in temp)
            //        {
            //            if (item.CreateTime < DateTime.Parse("2020-01-01")) 
            //            {
            //                item.CreateTime = GetRandomTime(DateTime.Parse("2020-06-15"), 2);
            //            }
            //        }

            //        _commentExecute.ExecuteAnswerTransaction(temp);
            //    }
            //}

            List<QuestionsAnswersInfo> answersInfos = _commentQueryRep.QueryImportAnswer(1, pageSize);
            if (answersInfos.Any())
            {
                foreach (var item in answersInfos)
                {
                    if (item.CreateTime < DateTime.Parse("2020-01-01"))
                    {
                        item.CreateTime = GetRandomTime(DateTime.Parse("2020-06-15"), 2);
                    }
                }
                _commentExecute.ExecuteAnswerTransaction(answersInfos);
            }
        }

        public static DateTime GetRandomTime(DateTime time, int Day)
        {
            int d = 0, h = 0, m = 0, s = 0;

            Random random = new Random();

            int minDay = time.Day;
            int maxDay = time.AddDays(Day + 1).Day;

            if (minDay >= maxDay)
            {
                int a = 0;
            }

            d = random.Next(minDay, maxDay);
            if (d == time.Day)
            {
                if (time.Hour + 1 > 22)
                {
                    h = 23;
                }
                else
                {
                    h = random.Next(time.Hour + 1, 22);
                }
            }
            else
            {
                h = random.Next(7, 22);
            }

            if (h == 23)
            {
                m = random.Next(0, 30);
            }
            else
            {
                m = random.Next(0, 60);
            }

            s = random.Next(0, 60);

            DateTime newDateTime = new DateTime(time.Year, time.Month, d, h, m, s);
            return newDateTime;
        }

    }
}
