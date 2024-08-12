using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.MediatR.Handle.PaidQA.EventTask
{
    public abstract class PaidQAEventTask<TEventData> where TEventData : INotification
    {

        public abstract Task ExcuteAsync(TEventData eventData, IServiceProvider sp);
    }

}
