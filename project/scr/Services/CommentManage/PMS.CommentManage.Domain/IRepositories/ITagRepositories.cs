using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface ITagRepositories
    {
        int Add(CommentTag tag);
        CommentTag CheckTagIsExists(string TagName);
        void TranAdd(CommentTag entity);
    }
}
