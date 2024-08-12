using PMS.CommentsManage.Application.Services.Settlement;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices.IMQService
{
    public interface IPrtTimeJobMQService
    {
        void SendPartTimeJobMoney(PartTimeJobDto PartTimeJobDtos);
    }
}
