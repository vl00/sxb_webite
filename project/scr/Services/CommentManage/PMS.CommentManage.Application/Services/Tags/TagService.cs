using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Tags
{
    public class TagService : ITagService
    {
        ITagRepositories _tagRepositories;
        public TagService(ITagRepositories tagRepositories)
        {
            _tagRepositories = tagRepositories;
        }

        public int Add(CommentTag tag)
        {
            return _tagRepositories.Add(tag);
        }

        public CommentTag CheckTagIsExists(string TagName)
        {
            return _tagRepositories.CheckTagIsExists(TagName);
        }

        public void TranAdd(CommentTag entity)
        {
            _tagRepositories.TranAdd(entity);
        }
    }
}
