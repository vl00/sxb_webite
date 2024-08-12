using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PMS.MediatR.Events;

namespace PMS.MediatR.Handle
{
    public class SendMessageHandler : INotificationHandler<AddArticleEvent>, INotificationHandler<AddTopicEvent>
    {
        public SendMessageHandler()
        {
        }

        //处理新增文章的发送站内消息事件
        public Task Handle(AddArticleEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //处理新增话题的发送站内消息事件
        public Task Handle(AddTopicEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
