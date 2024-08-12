using PMS.CommentsManage.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ISchoolQuestionTotalMQService
    {
        void SendSyncSchoolScoreMessage(SchoolScoreDto schoolScore);
        void SendSyncSchoolScoreMessage(List<SchoolScoreDto> schoolScore);
    }
}
