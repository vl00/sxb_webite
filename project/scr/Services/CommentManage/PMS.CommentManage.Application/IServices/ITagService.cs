using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ITagService
    {
        int Add(CommentTag tag);
        CommentTag CheckTagIsExists(string TagName);
        void TranAdd(CommentTag entity);
    }
}
