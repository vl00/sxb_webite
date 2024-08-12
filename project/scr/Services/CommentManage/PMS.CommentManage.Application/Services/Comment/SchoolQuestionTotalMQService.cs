using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.RabbitMQ.Message;
using ProductManagement.Framework.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Comment
{
    public class SchoolQuestionTotalMQService : ISchoolQuestionTotalMQService
    {

        private readonly IEventBus _eventBus;

        public SchoolQuestionTotalMQService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void SendSyncSchoolScoreMessage(SchoolScoreDto schoolScore)
        {
            _eventBus.Publish(new SyncSchoolQuestionTotalMessage(new List<SyncSchoolQuestionTotalMessage.SysSchoolQuestionTotal>() {
                new SyncSchoolQuestionTotalMessage.SysSchoolQuestionTotal(){
                    SchoolId = schoolScore.SchoolId,
                    SchoolSectionId = schoolScore.SchoolSectionId,
                    LastQuestionTime = schoolScore.LastQuestionTime,
                    QuestionCount = schoolScore.QuestionCount
                }
            }));
        }

        public void SendSyncSchoolScoreMessage(List<SchoolScoreDto> schoolScore)
        {
            _eventBus.Publish(new SyncSchoolQuestionTotalMessage(
               schoolScore.Select(q => new SyncSchoolQuestionTotalMessage.SysSchoolQuestionTotal
               {
                   SchoolId = q.SchoolId,
                   SchoolSectionId = q.SchoolSectionId,
                   LastQuestionTime = q.LastQuestionTime,
                   QuestionCount = q.QuestionCount
               }).ToList()));
        }
    }
}
