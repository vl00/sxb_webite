using System;
using System.Collections.Generic;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("BulkSyncSchoolScore_QUEUE")]
    public class SyncSchoolScoreMessage : IMessage
    {
        public SyncSchoolScoreMessage(List<SyncSchoolScoreModel> schoolScoresList)
        {
            SchoolScoresList = schoolScoresList;
        }

        public List<SyncSchoolScoreModel> SchoolScoresList { get; set; }

        public class SyncSchoolScoreModel 
        {
            public Guid SchoolId { get; set; }

            public Guid SchoolSectionId { get; set; }

            /// <summary>
            /// 总分
            /// </summary>
            public decimal AggScore { get; set; }

            /// <summary>
            /// 师资力量分
            /// </summary>
            public decimal TeachScore { get; set; }

            /// <summary>
            /// 硬件设施分
            /// </summary>
            public decimal HardScore { get; set; }

            /// <summary>
            /// 环境周边分
            /// </summary>
            public decimal EnvirScore { get; set; }

            /// <summary>
            /// 学风管理分
            /// </summary>
            public decimal ManageScore { get; set; }

            /// <summary>
            /// 校园生活分
            /// </summary>
            public decimal LifeScore { get; set; }

            /// <summary>
            /// 评论数
            /// </summary>
            /// <value>The comment count.</value>
            public int CommentCount { get; set; }


            /// <summary>
            /// 就读评论数
            /// </summary>
            /// <value>The attend count.</value>
            public int AttendCommentCount { get; set; }

            /// <summary>
            /// 最后点评时间
            /// </summary>
            /// <value>The last comment time.</value>
            public DateTime LastCommentTime { get; set; }

        }
    }
}
