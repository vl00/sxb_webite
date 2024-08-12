using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("SyncSchoolQuestionTotalMessage")]
    public class SyncSchoolQuestionTotalMessage : IMessage
    {
        public SyncSchoolQuestionTotalMessage(List<SysSchoolQuestionTotal> sysSchoolQuestionTotal)
        {
            _sysSchoolQuestionTotal = sysSchoolQuestionTotal;
        }

        public List<SysSchoolQuestionTotal> _sysSchoolQuestionTotal { get; set; }

        public class SysSchoolQuestionTotal
        {
            public Guid SchoolId { get; set; }

            public Guid SchoolSectionId { get; set; }

            /// <summary>
            /// 问题数
            /// </summary>
            /// <value>The Question count.</value>
            public int QuestionCount { get; set; }

            /// <summary>
            /// 最后发布提问时间
            /// </summary>
            /// <value>The last Question time.</value>
            public DateTime LastQuestionTime { get; set; }
        }
    }
}
