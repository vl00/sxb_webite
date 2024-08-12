using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    using PMS.OperationPlateform.Application.IServices;
    using PMS.OperationPlateform.Domain.IRespositories;


    public class ArticleCommentService : IArticleCommentService
    {
        IArticleCommentRepository ArticleCommentRepository;
        public ArticleCommentService(IArticleCommentRepository articleCommentRepository)
        {
            this.ArticleCommentRepository = articleCommentRepository;
        }


        public IEnumerable<dynamic> Statistics_CommentsCount(Guid[] forumIds)
        {
            return this.ArticleCommentRepository.Statistics_CommentsCount(forumIds); 
        }
    }
}
