using System;
using System.Collections.Generic;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("iSchoolData_school_onoff_queue")]
    public class SyncEsSchoolUpdateMessage : IMessage
    {
        public Guid Sid { get; set; }
        public DateTime T { get; set; }
    }
}
