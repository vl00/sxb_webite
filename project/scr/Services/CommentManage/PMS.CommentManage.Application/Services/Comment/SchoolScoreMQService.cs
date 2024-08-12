using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.RabbitMQ.Message;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Comment
{
    public class SchoolScoreMQService : ISchoolScoreMQService
    {
        private readonly IEventBus _eventBus;

        public SchoolScoreMQService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        //发布到RabbitMQ
        public void SendSyncSchoolScoreMessage(SchoolScoreDto schoolScore)
        {
            _eventBus.Publish(new SyncSchoolScoreMessage(new List<SyncSchoolScoreMessage.SyncSchoolScoreModel> {
                new SyncSchoolScoreMessage.SyncSchoolScoreModel{ SchoolId = schoolScore.SchoolId,
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
                }
            }));
        }

        public void SendSyncSchoolScoreMessage(List<SchoolScoreDto> schoolScore)
        {
            _eventBus.Publish(new SyncSchoolScoreMessage(
                schoolScore.Select(q=>new SyncSchoolScoreMessage.SyncSchoolScoreModel {
                    SchoolId = q.SchoolId,
                    SchoolSectionId = q.SchoolSectionId,
                    AggScore = q.AggScore,
                    TeachScore = q.TeachScore,
                    ManageScore = q.ManageScore,
                    LifeScore = q.LifeScore,
                    HardScore = q.HardScore,
                    EnvirScore = q.EnvirScore,
                    CommentCount = q.CommentCount,
                    AttendCommentCount = q.AttendCommentCount,
                    LastCommentTime = q.LastCommentTime
                }).ToList()));
        }
    }
}
