using PMS.CommentsManage.Application.IServices.IMQService;
using PMS.CommentsManage.Application.Services.Settlement;
using PMS.RabbitMQ.Message;
using ProductManagement.Framework.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PMS.CommentsManage.Application.Services.MQService
{
    public class PrtTimeJobMQService : IPrtTimeJobMQService
    {
        private readonly IEventBus _eventBus;
        public PrtTimeJobMQService(IEventBus eventBus) 
        {
            _eventBus = eventBus;
        }

        public void SendPartTimeJobMoney(PartTimeJobDto PartTimeJobDtos)
        {
            _eventBus.Publish(new SysPartTimeJobMessage() {
                JobId = PartTimeJobDtos.JobId,
                SettlementAmount = PartTimeJobDtos.SettlementAmount,
                OpenId = PartTimeJobDtos.OpenId,
                UserId = PartTimeJobDtos.UserId,
                ActivityName = PartTimeJobDtos.ActivityName,
                Remark = PartTimeJobDtos.Remark,
                Blessings = PartTimeJobDtos.Blessings,
                Type = PartTimeJobDtos.Type,
                AppName = PartTimeJobDtos.AppName
            });
        }

    }
}
