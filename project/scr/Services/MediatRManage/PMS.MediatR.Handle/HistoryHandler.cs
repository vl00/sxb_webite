using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PMS.MediatR.Events;

namespace PMS.MediatR.Handle
{
    public class HistoryHandler : INotificationHandler<BrowsePageEvent>
    {
        public HistoryHandler()
        {
        }

        //浏览页面的历史足迹
        public Task Handle(BrowsePageEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
