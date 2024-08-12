using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Application.IServices
{
    public interface IGiveLikeService
    {
        /// <summary>
        /// 获取点赞详情实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        List<GiveLike> GiveLikeEntitys(Expression<Func<GiveLike, bool>> where);
        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="giveLike"></param>
        /// <returns></returns>
        int AddLike(GiveLike giveLike, out LikeStatus status);
        /// <summary>
        /// 检测当前用户是否点赞，true ： 点赞 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="SourceId"></param>
        /// <returns></returns>
        bool CheckCurrentUserIsLike(Guid UserId, Guid SourceId);

        List<LikeDto> CheckCurrentUserIsLikeBulk(Guid UserId, List<Guid> SourceIds);

        /// <summary>
        /// 修改点赞、回复统计总数
        /// </summary>
        /// <param name="ReplayId"></param>
        /// <param name="operaValue"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        int UpdateCommentReplyLikeorReplayCount(Guid ReplayId);

        LikeCommentAndAnswerTotal CommentAndAnswerTotals(Guid UserId);

        LikeCommentAndAnswerTotal StatisticalSum(Guid UserId);

        /// <summary>
        /// 点评、问答数据为缓存数据，但点赞数据需要实时性，在所有数据查询之后再次进行反查得到最新点赞状态
        /// </summary>
        /// <param name="DataSourceId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<Guid> CheckLike(List<Guid> DataSourceId, Guid UserId);

        int GetLikeCount(Guid dataID);
    }
}
