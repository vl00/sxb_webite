using MediatR;
using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.Comment
{
    public class CommentLikeEvent : INotification
    {
        public GiveLike GiveLike { private set; get; }
        public CommentLikeEvent(GiveLike giveLike)
        {
            GiveLike = giveLike;
        }
    }
}
