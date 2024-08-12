using MediatR;
using PMS.PaidQA.Domain.Dtos;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{
    public class SendMessageEvent: INotification
    {
        public Message Message { get;  }
        public Order Order { get; set; }

        public SendMessageOrigin  SendMessageOrigin { get; set; }
        public SendMessageEvent(Message message,Order order, SendMessageOrigin sendMessageOrigin)
        {
            Message = message;
            Order = order;
            SendMessageOrigin = sendMessageOrigin;
        }

    }
}
