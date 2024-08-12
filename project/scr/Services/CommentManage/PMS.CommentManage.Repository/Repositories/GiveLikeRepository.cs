using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using PMS.CommentsManage.Domain.Common;
using System.Data.SqlClient;
using PMS.CommentsManage.Domain;
using PMS.UserManage.Domain.IRepositories;
using static PMS.UserManage.Domain.Common.EnumSet;
using System.Threading.Tasks;
using PMS.CommentsManage.Domain.Entities.Total;

namespace PMS.CommentsManage.Repository.Repositories
{
    /// <summary>
    /// 点赞、回复
    /// </summary>
    public class GiveLikeRepository : EntityFrameworkRepository<GiveLike>, IGiveLikeRepository
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ISchoolCommentRepository efschoolCommentRepository;
        private readonly IQuestionInfoRepository efquestionInfoRepository;
        private readonly ISchoolCommentReplyRepository _replyRepository;
        private readonly IQuestionsAnswersInfoRepository _questionsAnswers;
        private readonly CommentsManageDbContext _dbContext;


        private readonly ISchoolCommentRepository _commentRepository;

        public GiveLikeRepository(IQuestionsAnswersInfoRepository questionsAnswers, CommentsManageDbContext dbContext, ISchoolCommentRepository commentRepository, ISchoolCommentReplyRepository replyRepository, ISchoolCommentRepository efschoolCommentRepo,
            IQuestionInfoRepository efquestionInfoRepo, IMessageRepository messageRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            efschoolCommentRepository = efschoolCommentRepo;
            efquestionInfoRepository = efquestionInfoRepo;
            _commentRepository = commentRepository;
            _replyRepository = replyRepository;
            _questionsAnswers = questionsAnswers;

            _messageRepository = messageRepository;
        }

        public int AddLike(GiveLike giveLike, out LikeStatus status)
        {
            int LikeCount = 0;
            int rez;
            var likeEntity = GetList(x => x.SourceId == giveLike.SourceId && x.UserId == giveLike.UserId).FirstOrDefault();
            //检测该用户是否已经点过赞，第二次点击则为取消点赞
            if (likeEntity == null)
            {
                //第一次点击，必定是点赞
                giveLike.LikeStatus = LikeStatus.Like;
                rez = base.Add(giveLike);
            }
            else
            {
                //用户取消点赞 数据存在直接删除

                rez = Delete(likeEntity.Id);

                //检测当前操作是否为喜欢
                //if (likeEntity.LikeStatus == LikeStatus.Like)
                //{
                //    likeEntity.LikeStatus = LikeStatus.UnLike;
                //    giveLike.LikeStatus = LikeStatus.UnLike;
                //}
                //else
                //{
                //    likeEntity.LikeStatus = LikeStatus.Like;
                //    giveLike.LikeStatus = LikeStatus.Like;
                //}
                //rez = Update(likeEntity);
            }

            status = giveLike.LikeStatus;
            if (rez > 0)
            {
                //修改数据源点赞总数
                int operaValue = 0;

                if (giveLike.LikeStatus == LikeStatus.Like)
                {
                    operaValue = 1;
                }
                else
                {
                    operaValue = -1;
                }

                MessageDataType? messageDataType = null;
                Guid userId, eid;
                string title = string.Empty, content = string.Empty;

                switch (giveLike.LikeType)
                {
                    case LikeType.Comment:
                        messageDataType = MessageDataType.Comment;
                        _commentRepository.UpdateCommentLikeorReplayCount(giveLike.SourceId, operaValue, true);
                        var dataComment = efschoolCommentRepository.GetModelById(giveLike.SourceId);
                        if (dataComment == null) break;
                        LikeCount = dataComment.LikeCount;
                        userId = dataComment.CommentUserId;
                        eid = dataComment.SchoolSectionId;
                        content = dataComment.Content;
                        break;
                    case LikeType.Quesiton:
                        break;
                    case LikeType.Reply:
                        messageDataType = MessageDataType.CommentReply;
                        _replyRepository.UpdateCommentReplyLikeorReplayCount(giveLike.SourceId, operaValue, true);
                        var dataReply = _replyRepository.GetModelById(giveLike.SourceId);
                        LikeCount = dataReply.LikeCount;
                        userId = dataReply.UserId;
                        eid = dataReply.SchoolComment.SchoolSectionId;
                        title = dataReply.SchoolComment.Content;
                        content = dataReply.Content;
                        break;
                    case LikeType.Answer:
                        messageDataType = MessageDataType.Answer;
                        _questionsAnswers.UpdateAnswerLikeorReplayCount(giveLike.SourceId, operaValue, true);
                        var dataAnswer = _questionsAnswers.GetModelById(giveLike.SourceId);
                        LikeCount = dataAnswer.LikeCount;
                        userId = dataAnswer.UserId;
                        eid = dataAnswer.QuestionInfo.SchoolSectionId;
                        title = dataAnswer.QuestionInfo.Content;
                        content = dataAnswer.Content;
                        break;
                    default:
                        break;
                }

                if (messageDataType != null)
                {
                    if (giveLike.LikeStatus == LikeStatus.Like)
                    {
                        //点赞状态
                        _messageRepository.AddMessage(new PMS.UserManage.Domain.Entities.Message()
                        {
                            Id = Guid.NewGuid(),
                            DataID = giveLike.SourceId,
                            DataType = (byte)messageDataType,
                            title = title.Length > 25 ? title.Substring(0, 25) : title,
                            Content = content,
                            EID = eid,
                            Type = (byte)MessageType.Like,
                            userID = userId,
                            senderID = giveLike.UserId,
                            Read = giveLike.UserId == userId //自己给自己点赞, 发已读消息
                        });
                    }
                    else
                    {
                        //取消点赞, 消息已读, 并忽略
                        _messageRepository.UpdateMessageIgnore(true, read: true, MessageType.Like, messageDataType.Value, userId, giveLike.UserId, new List<Guid>() { giveLike.SourceId });
                    }
                }
            }
            return LikeCount;
        }

        public int Delete(int Id)
        {
            string sql = "delete from SchoolCommentLikes where Id = @Id";
            SqlParameter[] para = {
                new SqlParameter("@Id",Id)
            };
            return ExecuteNonQuery(sql, para);
        }

        public bool CheckCurrentUserIsLike(Guid UserId, Guid SourceId)
        {
            return base.GetList(x => x.LikeStatus == LikeStatus.Like && x.UserId == UserId && x.SourceId == SourceId).FirstOrDefault() != null;
        }

        public List<GiveLike> CheckCurrentUserIsLikeBulk(Guid UserId, List<Guid> SourceIds)
        {
            return base.GetList(x => x.LikeStatus == LikeStatus.Like && x.UserId == UserId && SourceIds.Contains(x.SourceId)).ToList();
        }

        public List<GiveLike> GiveLikeEntitys(Expression<Func<GiveLike, bool>> where)
        {
            return base.GetList(where).ToList();
        }

        public LikeCommentAndAnswerTotal CommentAndAnswerTotals(Guid UserId)
        {
            string sql = @"
                        select
	                        1 as Id,
	                        case		                        when count(1) = 0 then 0
		                        when count(1) <> 0 then count(1)
	                        end as Total
                        from SchoolCommentLikes where LikeType = 1 and UserId = @userId
                        union
                        select 
                        2 as Id,
                        case
		                        when count(1) = 0 then 0
		                        when count(1) <> 0 then count(1)
	                        end as Total from SchoolCommentReplies as s left join SchoolCommentLikes as t
                        on s.Id = t.SourceId and t.LikeType = 3
                        where s.ParentId is null and t.Id is not null and t.UserId = @userId
                        union
                        select 
                        3 as Id,
                        case
		                        when count(1) = 0 then 0
		                        when count(1) <> 0 then count(1)
	                        end as Total from SchoolCommentReplies as s left join SchoolCommentLikes as t
                        on s.Id = t.SourceId and t.LikeType = 3
                        where s.ParentId is not null and t.Id is not null and t.UserId = @userId
                        union
                        select 
                        4 as Id,
                        case
		                        when count(1) = 0 then 0
		                        when count(1) <> 0 then count(1)
	                        end as Total from QuestionsAnswersInfos as s left join SchoolCommentLikes as t
                        on s.Id = t.SourceId and t.LikeType = 4
                        where s.ParentId is null and  t.Id is not null and t.UserId = @userId
                        union
                        select 
                        5 as Id,
                        case
		                        when count(1) = 0 then 0
		                        when count(1) <> 0 then count(1)
	                        end as Total
	                        from SchoolCommentReplies as s left join SchoolCommentLikes as t
                        on s.Id = t.SourceId and t.LikeType = 4
                        where s.ParentId is not null and t.Id is not null and t.UserId = @userId";

            SqlParameter[] pra = {
                new SqlParameter("@userId",UserId)
            };

            List<UserPublishTotal> userPublishTotals = Query<UserPublishTotal>(sql, pra)?.ToList();
            LikeCommentAndAnswerTotal likeCommentAndAnswerTotal = new LikeCommentAndAnswerTotal();

            likeCommentAndAnswerTotal.LikeCommentTotal = userPublishTotals[0].Total;
            likeCommentAndAnswerTotal.LikeCommentReplyTotal = userPublishTotals[1].Total;
            likeCommentAndAnswerTotal.LikeReplyTotal = userPublishTotals[2].Total;
            likeCommentAndAnswerTotal.LikeAnswerReplyTotal = userPublishTotals[3].Total;
            likeCommentAndAnswerTotal.LikeAnswerTotal = userPublishTotals[4].Total;

            return likeCommentAndAnswerTotal;
        }

        public LikeCommentAndAnswerTotal StatisticalSum(Guid UserId)
        {
            string sql = @"select 1 as Id,sum(t.Total) Total from 
                            (
	                            select 1 as Id,count(1) as Total from SchoolComments where CommentUserId = @UserId
	                            union
	                            select 2 as Id,count(1) as Total from QuestionInfos where UserId = @UserId
                            ) as t
                            UNION
                            select 2 as Id,count(1) as Total from SchoolCommentLikes where UserId = @UserId
                            UNION
                            select 3 as Id,sum(t.Total) as Total from
                            (
	                            select 1 as Id,count(1) as Total from SchoolCommentReplies where UserId = @UserId
	                            UNION
	                            select 2 as Id,count(1) as Total from QuestionsAnswersInfos where UserId = @UserId
                            )as t";
            SqlParameter[] pra = {
                new SqlParameter("@userId",UserId)
            };

            List<UserPublishTotal> userPublishTotals = Query<UserPublishTotal>(sql, pra)?.ToList();
            LikeCommentAndAnswerTotal likeCommentAndAnswerTotal = new LikeCommentAndAnswerTotal();
            likeCommentAndAnswerTotal.PublishTotal = userPublishTotals[0].Total;
            likeCommentAndAnswerTotal.LikeTotal = userPublishTotals[1].Total;
            likeCommentAndAnswerTotal.AnswerAndReplyTotal = userPublishTotals[2].Total;

            return likeCommentAndAnswerTotal;
        }

        public List<GiveLike> CheckLike(List<Guid> DataSourceId, Guid UserId)
        {
            //string str = DataSourceId.Select((i, e) => { "@" + i; });
            //List<SqlParameter> para = DataSourceId.Select((i, e) => { new SqlParameter("@" + i, e); }) ;

            List<SqlParameter> para = DataSourceId.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            para.Add(new SqlParameter("@userId", UserId));
            string strpara = string.Join(",", DataSourceId.Select((q, idx) => "@id" + idx));

            string sql = $"select * from SchoolCommentLikes where SourceId in ({strpara}) and UserId = @userId";
            return Query(sql, para.ToArray())?.ToList();
        }

        public int GetLikeCount(Guid dataID)
        {
            return GetCount(p => p.SourceId == dataID && p.LikeStatus == LikeStatus.Like);
        }
    }
}
