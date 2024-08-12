using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto.Reply;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ProductManagement.Framework.Foundation;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.UserManage.Domain.IRepositories;
using PMS.UserManage.Domain.Entities;
using System.Linq.Expressions;

namespace PMS.CommentsManage.Application.Services.Comment
{
    public class SchoolCommentReplyService : ISchoolCommentReplyService
    {
        private readonly ISchoolCommentReplyRepository _repository;
        private readonly IGiveLikeRepository _giveLike;
        private readonly ISchoolCommentRepository _schoolComment;
        private readonly IUserRepository _userRepository;

        public SchoolCommentReplyService(IGiveLikeRepository giveLike,
            ISchoolCommentReplyRepository repository,
            ISchoolCommentRepository schoolComment,
            IUserRepository userRepository)
        {
            _repository = repository;
            _giveLike = giveLike;
            _schoolComment = schoolComment;
            _userRepository = userRepository;
        }

        public IEnumerable<SchoolCommentReply> GetList(Expression<Func<SchoolCommentReply, bool>> where)
        {
            return _repository.GetList(where);
        }

        public int Add(SchoolCommentReply schoolComment,out Guid? replyId)
        {
            var rez = _repository.Add(schoolComment);
            replyId = schoolComment.Id;
            if (rez > 0)
            {
                Guid commentId = default(Guid);
                if (schoolComment.ReplyId != null)
                {
                    commentId = (Guid)schoolComment.ReplyId;
                    //修改二级总数
                    rez = _repository.UpdateCommentReplyLikeorReplayCount(commentId, 1, false);
                }
                else
                {
                    commentId = schoolComment.SchoolCommentId;
                    rez = _schoolComment.UpdateCommentLikeorReplayCount(commentId, 1, false);
                }
                if (rez > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// 根据点评ID分页获取回复
        /// </summary>
        public List<SchoolCommentReply> PageCommentReply(Guid CommentId, int PageIndex, int PageSize, out int total)
        {
            return _repository.CommentReply(CommentId, PageIndex, PageSize,out total);
        }

        public List<ReplyDto> CommentReply(Guid CommentId, Guid UserId, int PageIndex, int PageSize, out int total)
        {
            var newReply = _repository.CommentReply(CommentId, PageIndex, PageSize,out total);

            List<Guid> userIds = newReply.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            var Users = _userRepository.ListUserInfo(userIds);

            List<Guid> replyIds = newReply.GroupBy(q=>q.Id).Select(q => q.Key).ToList();
            var Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, replyIds);

            return newReply.Select(x=> ConvterReplyDto(x, Likes, Users))?.ToList();
        }

        /// <summary>
        /// 根据回复ID分页获取下级回复
        /// </summary>
        public List<SchoolCommentReplyExt> PageCommentReplyById(Guid replyId,int ordertype, int PageIndex, int PageSize, out int total)
        {
            total = _repository.CommentReplyTotalById(replyId);
            return _repository.CommentReplyById(replyId, ordertype, PageIndex, PageSize);
        }
        public List<SchoolCommentReplyExt> PageDialog(Guid replyId, int PageIndex, int PageSize)
        {
            //获取该条回复的详情
            SchoolCommentReplyExt reply =  _repository.QueryCommentReply(replyId);
            if (reply == null)
                return new List<SchoolCommentReplyExt>();
            return _repository.PageDialog(reply.Id, new List<Guid> { reply.UserId,(Guid)reply.ReplyUserId }, PageIndex, PageSize);
        }


        public SchoolCommentReply GetCommentReply(Guid replyId)
        {
            var result =   _repository.GetCommentReply(replyId);
            if (result == null)
                throw new Exception("没有此点评回复");

            return result;
        }


        public List<SchoolCommentReply> SelectedCommentReply(Guid commentId)
        {
            return _repository.SelectedCommentReply(commentId);
        }

        public List<ReplyDto> SelectedCommentReply(Guid commentId, Guid UserId)
        {
            var reply = _repository.SelectedCommentReply(commentId);

            List<Guid> userIds = reply.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            var Users = _userRepository.ListUserInfo(userIds);

            List<Guid> replyIds = reply.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            var Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, replyIds);

            return reply.Select(x => ConvterReplyDto(x, Likes, Users))?.ToList();
        }

        private ReplyDto ConvterReplyDto(SchoolCommentReply commentReply,List<GiveLike> Likes,List<UserInfo> Users)
        {
            if (commentReply == null)
                return null;

            return new ReplyDto() {
                Id = commentReply.Id,
                SchoolCommentId = commentReply.SchoolCommentId,
                AddTime = commentReply.CreateTime.ConciseTime(),
                Content = commentReply.Content,
                isLike = Likes.FirstOrDefault(q => q.SourceId == commentReply.Id) != null,
                LikeCount = commentReply.LikeCount,
                ReplayCount = commentReply.ReplyCount,
                isStudent = commentReply.IsAttend,
                isAnonymou = commentReply.IsAnony,
                isAttend = commentReply.IsAttend,
                ReplyUserId = commentReply.UserId,
                ParentID = commentReply.ParentId
            };
        }

        public List<SchoolCommentReplyExt> CommentReplyExtNewest(List<Guid> ReplyIds)
        {
            return _repository.CommentReplyExtNewest(ReplyIds);
        }

        public List<ReplyDto> GetCommentReplyByIds(List<Guid> Ids, Guid UserId)
        {
            var replys = _repository.GetList(x => Ids.Contains(x.Id));
            List<Guid> userIds = replys.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            var Users = _userRepository.ListUserInfo(userIds);

            List<Guid> replyIds = replys.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            var Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, replyIds);

            return replys.Select(x => ConvterReplyDto(x, Likes, Users))?.ToList();
        }

        public List<ReplyDto> GetNewestCommentReplyByUserId(int PageIndex,int PageSize,Guid UserId)
        {
            var replys = _repository.GetList(x => x.UserId == UserId).OrderByDescending(x => x.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize); ;
            List<Guid> userIds = replys.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            var Users = _userRepository.ListUserInfo(userIds);

            List<Guid> replyIds = replys.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            var Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, replyIds);

            return replys.Select(x => ConvterReplyDto(x, Likes, Users))?.ToList();
        }

        public List<ReplyDto> GetCurretUserNewestReply(int PageIndex, int PageSize, Guid UserId)
        {
            var replys = _repository.GetCurretUserNewestReply(PageIndex, PageSize, UserId);
            List<Guid> userIds = replys.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            var Users = _userRepository.ListUserInfo(userIds);

            List<Guid> replyIds = replys.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            var Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, replyIds);

            return replys.Select(x => ConvterReplyDto(x, Likes, Users))?.ToList();
        }

        public ReplyDto GetTopLevel(Guid Id)
        {
           return ConvterReplyDto(_repository.GetTopLevel(Id),new List<GiveLike>(),new List<UserInfo>());
        }

        public int CommentReplyTotal(Guid userId)
        {
            return _repository.GetList(x=>x.UserId == userId).Count();
        }

        public int ReplyTotal(Guid userId)
        {
            return _repository.GetList(x=>x.UserId == userId && x.ParentId != null).Count();
        }

        public List<CommentReplyAndReply> CurrentSchoolCommenReplyAndReply(Guid userId, int PageIndex, int PageSize)
        {
            return _repository.CurrentSchoolCommenReplyAndReply(userId, PageIndex, PageSize);
        }

        public List<CommentReplyAndReply> CurrentUserLikeCommentAndReply(Guid userId, int PageIndex, int PageSize)
        {
            return _repository.CurrentUserLikeCommentAndReply(userId, PageIndex, PageSize);
        }


        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <returns></returns>
        public bool UpdateReplyViewCount(Guid replyId)
        {
            return _repository.UpdateViewCount(replyId);
        }

        public List<SchoolCommentReplyExt> GetSchoolCommentHottestReplies(Guid replyId)
        {
            return _repository.GetSchoolCommentHottestReplies(replyId);
        }

        public List<ParentReply> GetParentReplyByIds(List<Guid> Ids)
        {
            return _repository.GetParentReplyByIds(Ids);
        }
    }
}
