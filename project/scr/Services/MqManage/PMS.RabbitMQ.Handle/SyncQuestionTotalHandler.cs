using Microsoft.Extensions.Logging;
using PMS.School.Application.ModelDto;
using PMS.RabbitMQ.Message;
using PMS.School.Application.IServices;
using ProductManagement.Framework.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.RabbitMQ.Handle
{
    public class SyncQuestionTotalHandler : IEventHandler<SyncSchoolQuestionTotalMessage>
    {
        private readonly ISchoolScoreService _scoreService;

        private readonly ILogger<SyncQuestionTotalHandler> _logger;

        public SyncQuestionTotalHandler(ILoggerFactory loggerFactory,
            ISchoolScoreService scoreService)
        {
            _scoreService = scoreService;
            _logger = loggerFactory.CreateLogger<SyncQuestionTotalHandler>();
        }

        public Task Handle(SyncSchoolQuestionTotalMessage message)
        {
            try
            {
                //调用目标service业务代码
                var list = message._sysSchoolQuestionTotal;
                //_logger.LogWarning($"开始统计问题：{list.Count()}",String.Join(",",list.Select(x=>x.QuestionCount)));
                foreach (var schoolScore in list)
                {
                    _scoreService.SyncSchoolQuestionTotal(new SchoolScoreDto
                    {
                        SchoolId = schoolScore.SchoolId,
                        SchoolSectionId = schoolScore.SchoolSectionId,
                        LastQuestionTime = schoolScore.LastQuestionTime,
                        QuestionCount = schoolScore.QuestionCount
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "更新学校统计问题,mq同步错误：{0}",
                    String.Join(",", message._sysSchoolQuestionTotal.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToArray()));
            }
            return Task.CompletedTask;
        }
    }
}
