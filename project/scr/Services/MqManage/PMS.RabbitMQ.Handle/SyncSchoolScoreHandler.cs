using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PMS.RabbitMQ.Message;
using PMS.School.Application.IServices;
using ProductManagement.Framework.RabbitMQ;
using System.Linq;
using PMS.School.Application.ModelDto;

namespace PMS.RabbitMQ.Handle
{
    public class SyncSchoolScoreHandler : IEventHandler<SyncSchoolScoreMessage>
    {
        private readonly ISchoolScoreService _scoreService;

        private readonly ILogger<SyncSchoolScoreHandler> _logger;

        public SyncSchoolScoreHandler(ILoggerFactory loggerFactory,
            ISchoolScoreService scoreService)
        {
            _scoreService = scoreService;
            _logger = loggerFactory.CreateLogger<SyncSchoolScoreHandler>();
        }

        public Task Handle(SyncSchoolScoreMessage message)
        {
            try
            {
                //调用目标service业务代码
                var list = message.SchoolScoresList;
                foreach (var schoolScore in list)
                {
                    _scoreService.SyncSchoolScore(new SchoolScoreDto
                    {
                        SchoolId = schoolScore.SchoolId,
                        SchoolSectionId = schoolScore.SchoolSectionId,
                        AggScore = schoolScore.AggScore,
                        TeachScore = schoolScore.TeachScore,
                        ManageScore = schoolScore.ManageScore,
                        LifeScore = schoolScore.LifeScore,
                        HardScore = schoolScore.HardScore,
                        EnvirScore = schoolScore.EnvirScore,
                        CommentCount = schoolScore.CommentCount,
                        AttendCommentCount = schoolScore.AttendCommentCount,
                        LastCommentTime = schoolScore.LastCommentTime
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "更新学校统计分数,mq同步错误：{0}",
                    String.Join(",", message.SchoolScoresList.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToArray()));
            }
            return Task.CompletedTask;
        }
    }
}
