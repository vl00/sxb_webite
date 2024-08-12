using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface IGiveLikeRepository
    {
        List<GiveLike> GiveLikeEntitys(Expression<Func<GiveLike, bool>> where);
        int AddLike(GiveLike giveLike, out LikeStatus status);
        bool CheckCurrentUserIsLike(Guid UserId, Guid SourceId);
        List<GiveLike> CheckCurrentUserIsLikeBulk(Guid UserId, List<Guid> SourceIds);
        LikeCommentAndAnswerTotal CommentAndAnswerTotals(Guid UserId);

        LikeCommentAndAnswerTotal StatisticalSum(Guid UserId);

        List<GiveLike> CheckLike(List<Guid> DataSourceId, Guid UserId);

        int GetLikeCount(Guid dataID);
    }
}
