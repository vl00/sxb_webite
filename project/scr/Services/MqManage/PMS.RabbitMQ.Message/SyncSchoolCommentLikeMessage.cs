using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("SchoolComment_Like")]
    public class SyncSchoolCommentLikeMessage : IMessage
    {
        public SyncSchoolCommentLikeMessage(SyncSchoolCommentModel schoolCommentModel)
        {
            SchoolCommentModel = schoolCommentModel;
        }

        public SyncSchoolCommentModel SchoolCommentModel { get; set; }

        public class SyncSchoolCommentModel
        {
            public Guid UserId { get; set; }
            public Guid CommentId { get; set; }
            public int LikeType { get; set; }
            public string Channel { get; set; }
        }
    }
}
