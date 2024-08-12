using System;
using System.Collections.Generic;
using PMS.CommentsManage.Application.ModelDto;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ISchoolScoreMQService
    {
        void SendSyncSchoolScoreMessage(SchoolScoreDto schoolScore);
        void SendSyncSchoolScoreMessage(List<SchoolScoreDto> schoolScore);
    }
}
