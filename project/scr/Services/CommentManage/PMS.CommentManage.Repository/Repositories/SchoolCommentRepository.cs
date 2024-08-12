using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Domain.Query;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class SchoolCommentRepository : EntityFrameworkRepository<SchoolComment>, ISchoolCommentRepository
    {
        private readonly ISchoolImageRepository efRepositoryImage;

        private readonly ISchoolCommentScoreRepository _commentScoreRepository;
        private readonly CommentsManageDbContext _dbContext;
        public SchoolCommentRepository(CommentsManageDbContext dbContext, ISchoolImageRepository efRepositoryImage,
            ISchoolCommentScoreRepository commentScoreRepository) : base(dbContext)
        {
            _commentScoreRepository = commentScoreRepository;
            _dbContext = dbContext;
            this.efRepositoryImage = efRepositoryImage;
        }

        public new void TranCommit()
        {
            base.TranCommit();
        }

        public new void BeginTransaction()
        {
            base.BeginTransaction();
        }

        public new void Rollback()
        {
            base.Rollback();
        }

        public new void TranAdd(SchoolComment comment)
        {
            base.TranAdd(comment);
        }

        public new int Delete(Guid Id)
        {
            return base.Delete(Id);
        }

        public new IEnumerable<SchoolComment> GetList(Expression<Func<SchoolComment, bool>> where = null)
        {
            return base.GetList(where);
        }

        public SchoolComment GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public int Insert(SchoolComment model)
        {
            return base.Add(model);
        }

        public bool isExists(Expression<Func<SchoolComment, bool>> where)
        {
            return base.GetList(where) == null;
        }

        public new int Update(SchoolComment model)
        {
            return base.Update(model);
        }

        public bool UserAgreement(Guid AdminId)
        {
            return GetList(x => x.CommentUserId == AdminId).Count() > 0;
        }

        public List<SchoolComment> GetSchoolCommentByUserId(Guid Id, int PageIndex, int PageSize, out int Total)
        {
            Total = 0;
            if (PageIndex == 0)
            {
                var rez = base.GetList(x => x.CommentUserId == Id && x.AddTime >= DateTime.Parse("2019-12-11 00:00:00")).OrderByDescending(x => x.AddTime);
                if (rez != null)
                {
                    Total = rez.Count();
                    return rez.ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Total = base.GetList(x => x.CommentUserId == Id && x.AddTime >= DateTime.Parse("2019-12-11 00:00:00")).Count();
                var rez = base.GetList(x => x.CommentUserId == Id && x.AddTime >= DateTime.Parse("2019-12-11 00:00:00")).OrderByDescending(x => x.AddTime).Skip((PageIndex - 1) * PageSize).Take(PageSize);
                if (rez != null)
                {
                    return rez.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<SchoolComment> PageSchoolComment(int pageIndex, int pageSize, DateTime startTime, DateTime endTime, out int total, List<Guid> schoolIds = null)
        {
            var result = base.GetList(x => (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight));
            if (schoolIds != null)
            {
                if (schoolIds.Count > 0)
                    result = result.Where(q => schoolIds.Contains(q.SchoolId));
                else
                    result = result.Where(q => false);
            }
            if (startTime > DateTime.MinValue && endTime < DateTime.MaxValue)
            {
                result = result.Where(p => p.AddTime >= startTime && p.AddTime < endTime);
            }

            total = result.Count();
            var rez = result.OrderByDescending(x => x.AddTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            if (rez != null)
            {
                return rez.ToList();
            }
            return null;
        }

        public List<SchoolComment> PageSchoolCommentByExamineState(int pageIndex, int pageSize, int examineState, out int total)
        {
            var result = base.GetList();
            if (examineState == 0)
            {
                result = result.Where(q => q.State == ExamineStatus.Unread).OrderByDescending(q => q.AddTime);
            }
            else
            {
                result = result.Where(q => q.State != ExamineStatus.Unread).Include(q => q.SchoolCommentExamine).OrderByDescending(q => q.SchoolCommentExamine.UpdateTime);
            }

            total = result.Count();
            var rez = result.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            if (rez != null)
            {
                return rez.ToList();
            }
            return null;
        }

        public List<SchoolComment> GetSchoolCommentByCommentId(List<Guid> commentIds)
        {
            IQueryable<SchoolComment> comments = base.GetList(s => commentIds.Contains(s.Id)).Include(q => q.SchoolCommentScore);
            return comments.ToList();
        }


        public List<SchoolComment> PageCommentByUserId(Guid userId, int pageIndex, int pageSize, bool isSelf = true)
        {
            IQueryable<SchoolComment> comments = base.GetList(s => s.CommentUserId == userId && (s.State == ExamineStatus.Unknown || s.State == ExamineStatus.Unread || s.State == ExamineStatus.Readed || s.State == ExamineStatus.Highlight)).Include(q => q.SchoolCommentScore);

            if (!isSelf)
            {
                //他人查看达人主页时 达人发布匿名点评时 其他人不可查看
                comments = comments.Where(x => x.IsAnony == false);
            }

            comments = comments.OrderByDescending(x => x.AddTime);
            return comments.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<SchoolComment> PageCommentByCommentIds(List<Guid> commentIds, bool isSelf = true)
        {
            IQueryable<SchoolComment> comments = base.GetList(s => commentIds.Contains(s.Id) && (s.State == ExamineStatus.Unknown || s.State == ExamineStatus.Unread || s.State == ExamineStatus.Readed || s.State == ExamineStatus.Highlight)).Include(q => q.SchoolCommentScore);

            if (!isSelf)
            {
                //他人查看达人主页时 达人发布匿名点评时 其他人不可查看
                comments = comments.Where(x => x.IsAnony == false);
            }

            return comments.OrderByDescending(x => x.AddTime).ToList();
        }

        public List<SchoolComment> PageSchoolCommentBySchoolSectionIds(Guid schoolId, QueryCondition query, CommentListOrder commentListOrder, int pageIndex, int pageSize, out int total)
        {
            IQueryable<SchoolComment> comments;

            if (query == QueryCondition.Other)
            {
                comments = base.GetList(s => s.SchoolSectionId == schoolId && (s.State == ExamineStatus.Unknown || s.State == ExamineStatus.Unread || s.State == ExamineStatus.Readed || s.State == ExamineStatus.Highlight)).Include(q => q.SchoolCommentScore);
            }
            else
            {
                comments = base.GetList(s => s.SchoolId == schoolId && (s.State == ExamineStatus.Unknown || s.State == ExamineStatus.Unread || s.State == ExamineStatus.Readed || s.State == ExamineStatus.Highlight)).Include(q => q.SchoolCommentScore);
            }

            switch (query)
            {
                case QueryCondition.Selected:
                    comments = comments.Where(x => x.State == ExamineStatus.Highlight || x.ReplyCount > 10);
                    break;

                case QueryCondition.IsAttend:
                    comments = comments.Where(x => x.SchoolCommentScore.IsAttend);
                    break;

                case QueryCondition.Rumor:
                    comments = comments.Where(x => x.RumorRefuting);
                    break;

                case QueryCondition.IsGood:
                    comments = comments.Where(x => x.SchoolCommentScore.AggScore >= 80);
                    break;

                case QueryCondition.IsBad:
                    comments = comments.Where(x => x.SchoolCommentScore.AggScore <= 40);
                    break;

                case QueryCondition.IsImage:
                    comments = comments.Where(x => x.IsHaveImagers);
                    break;
            }
            if ((int)commentListOrder == 1)
            {
                comments = comments.OrderByDescending(x => x.LikeCount + x.ReplyCount);
            }
            else
            {
                comments = comments.OrderByDescending(x => x.AddTime);
            }

            total = comments.Count();
            return comments.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<SchoolComment> PageSchoolCommentBySchoolSectionId(Guid schoolSectionId, int pageIndex, int pageSize, out int total)
        {
            total = base.GetList(q => q.SchoolSectionId == schoolSectionId && (q.State == ExamineStatus.Unknown || q.State == ExamineStatus.Unread || q.State == ExamineStatus.Readed || q.State == ExamineStatus.Highlight)).Count();
            return base.GetPageList(q => q.SchoolSectionId == schoolSectionId && (q.State == ExamineStatus.Unknown || q.State == ExamineStatus.Unread || q.State == ExamineStatus.Readed || q.State == ExamineStatus.Highlight), " AddTime Desc ", pageIndex, pageSize).ToList();
        }

        public bool SetTop(Guid SchoolCommentId, bool isTop)
        {
            return ExecuteNonQuery("update SchoolComments Set IsTop = @isTop Where Id = @id",
                 new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@isTop",isTop),
                    new System.Data.SqlClient.SqlParameter("@id",SchoolCommentId)
                  }) > 0;
        }

        public bool SetNotTopBySchoolId(Guid schoolId, Guid schoolSectionId)
        {
            return ExecuteNonQuery("update SchoolComments Set IsTop = 'false' Where schoolId = @schoolId and schoolSectionId = @schoolSectionId;",
                 new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@schoolId",schoolId),
                    new System.Data.SqlClient.SqlParameter("@schoolSectionId",schoolSectionId)
                  }) > 0;
        }

        public int SchoolTotalComment(Guid SchoolId)
        {
            return base.GetCount(x => x.SchoolSectionId == SchoolId && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight));
        }

        public SchoolComment SchoolTopComment(Guid SchoolId)
        {
            var SetTopSchoolComment = base.GetList(x => x.SchoolId == SchoolId && x.IsTop == true && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).FirstOrDefault();
            return SetTopSchoolComment;
        }

        public SchoolComment PraiseAndReplyTotalTop(Guid SchoolId)
        {
            return base.GetList(x => x.SchoolId == SchoolId && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).OrderByDescending(x => x.LikeCount + x.SchoolCommentReplys.Count()).FirstOrDefault();
        }

        public SchoolComment SelectedComment(Guid schoolSectionId)
        {
            var isTopComment = base.GetList(x => x.SchoolSectionId == schoolSectionId && x.IsTop == true && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).FirstOrDefault();
            //暂无置顶点评，则取（回答数+点赞数）最高的点评
            if (isTopComment == null)
            {
                return base.GetList(x => x.SchoolSectionId == schoolSectionId && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).OrderByDescending(x => x.LikeCount + x.SchoolCommentReplys.Count()).FirstOrDefault();
            }
            else
            {
                return isTopComment;
            }
        }

        public List<SchoolComment> GetSchoolSelectedComment(List<Guid> schoolSectionId, int order)
        {
            List<SqlParameter> para = schoolSectionId.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", schoolSectionId.Select((q, idx) => "@id" + idx));
            string sql = "";

            if (order == 3 || order == -1)
            {
                sql = @"select s.* from SchoolComments s
	                        RIGHT JOIN
                        (
	                        SELECT
		                        Id,
	                        CASE

			                        WHEN PostUserRole = '1' THEN
			                        1 ELSE 0
		                        END AS isSchoolUser,
		                        ROW_NUMBER() OVER(partition BY SchoolSectionId order by IsTop DESC,( LikeCount + ReplyCount ) DESC) AS Row_Index
	                        FROM
		                        SchoolComments
	                        WHERE
		                        SchoolSectionId IN (" + strpara + @")
		                        AND State in (0,1,2,3)
                        ) as hot
	                        on s.Id = hot.Id where hot.Row_Index = 1
		                        order by hot.isSchoolUser desc";
            }
            else if (order == 1)
            {
                sql = @"select s.* from SchoolComments as s
			                RIGHT JOIN
		                (select
			                Id,
		                ROW_NUMBER() OVER( partition BY SchoolSectionId order by AddTime desc) AS Row_Index
		                from SchoolComments
			                where SchoolSectionId IN (" + strpara + @")
                            AND State in (0,1,2,3)
		                ) as new on new.Id = s.Id
			                where new.Row_Index = 1";
            }

            var comment = Query<SchoolComment>(sql, para.ToArray())?.ToList();

            if (comment != null && comment.Any())
            {
                var score = _commentScoreRepository.GetSchoolScoreByCommentIds(comment.Select(x => x.Id).ToList()).ToList();
                comment.ForEach(x =>
                {
                    x.SchoolCommentScore = score.Where(s => s.CommentId == x.Id).FirstOrDefault();
                });
            }
            return comment;
        }

        public List<SchoolComment> GetCommentLikeTotal(List<Guid> CommentId)
        {
            List<SqlParameter> para = CommentId.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", CommentId.Select((q, idx) => "@id" + idx));

            string sql = $"select Id,LikeCount,ReplyCount from SchoolComments where Id in ({strpara}) and State in (0,1,2,3)";
            return Query<SchoolComment>(sql, para.ToArray())?.ToList();
        }

        public List<SchoolComment> GetCommentsBySchoolExtIds(List<Guid> SchoolSectionIds)
        {
            if (SchoolSectionIds.Count() == 0)
                return null;

            List<SqlParameter> para = SchoolSectionIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", SchoolSectionIds.Select((q, idx) => "@id" + idx));

            string sql = @"
                select * from
                (
                select *,
                 row_number () OVER ( partition BY comment.SchoolSectionId ORDER BY comment.IsTop, comment.isSchoolUser DESC, ( LikeCount + ReplyCount ) DESC ) AS TopLike
                from (
                select *,
                            case
							                when PostUserRole = '1' then 1
							                else 0 end as isSchoolUser
						                from SchoolComments
                            where
							                SchoolSectionId in (" + strpara + @")
							                and State in (0,1,2,3)
						                )
						                as comment
                ) as CommentList where CommentList.TopLike = 1";

            var comments = Query<SchoolComment>(sql, para.ToArray())?.ToList();

            if (comments != null)
            {
                List<Guid> commentIds = comments.Select(x => x.Id).ToList();
                var commentScore = _commentScoreRepository.GetSchoolScoreByCommentIds(commentIds);

                comments.ForEach(x =>
                {
                    x.SchoolCommentScore = commentScore.Where(s => s.CommentId == x.Id).FirstOrDefault();
                });

                return comments;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据点评id列表获取点评信息
        /// </summary>
        public List<SchoolComment> GetCommentsByIds(List<Guid> CommentIds)
        {
            var list = _dbContext.SchoolComments.Include(q => q.SchoolCommentScore)
                .Where(q => CommentIds.Contains(q.Id) && (q.State == ExamineStatus.Unknown || q.State == ExamineStatus.Unread || q.State == ExamineStatus.Readed || q.State == ExamineStatus.Highlight)).ToList();
            return list;

            //string sql = @"select * from SchoolComments where id in ({0})  and State in (0,1,2,3);";

            //List<SqlParameter> paras = CommentIds.Select((item, i) => new SqlParameter("@id" + i, item)).ToList();
            //string strs = string.Join(',', CommentIds.Select((item, i) => "@id" + i));
            //var list = Query<SchoolComment>(
            //    string.Format(sql, strs),
            //    paras.ToArray()
            //).ToList();
            //return list;
        }

        /// <summary>
        /// 根据点评id获取点评信息
        /// </summary>
        public SchoolComment GetCommentById(Guid CommentId)
        {
            var result = _dbContext.SchoolComments.Include(q => q.SchoolCommentScore)
                .Where(q => CommentId == q.Id && (q.State == ExamineStatus.Unknown || q.State == ExamineStatus.Unread || q.State == ExamineStatus.Readed || q.State == ExamineStatus.Highlight)).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 根据点评no获取点评信息
        /// </summary>
        public SchoolComment GetCommentByNo(long no)
        {
            var result = _dbContext.SchoolComments.Include(q => q.SchoolCommentScore)
                .Where(q => q.No == no && (q.State == ExamineStatus.Unknown || q.State == ExamineStatus.Unread || q.State == ExamineStatus.Readed || q.State == ExamineStatus.Highlight)).FirstOrDefault();
            return result;
        }

        public List<SchoolComment> SelectedThreeComment(Guid schoolId, QueryCondition query)
        {
            string sqlwhere = "";
            switch (query)
            {
                case QueryCondition.Selected:
                    sqlwhere += " and ( sc.State = '3' or sc.ReplyCount > 10 )";
                    break;

                case QueryCondition.IsAttend:
                    sqlwhere += " and scs.IsAttend = 1 ";
                    break;

                case QueryCondition.Rumor:
                    sqlwhere += " and sc.RumorRefuting = 1 ";
                    break;

                case QueryCondition.IsGood:
                    sqlwhere += " and scs.AggScore >=80  ";
                    break;

                case QueryCondition.IsBad:
                    sqlwhere += " and scs.AggScore < 40  ";
                    break;

                case QueryCondition.IsImage:
                    sqlwhere += " and scs.IsHaveImagers = 1 ";
                    break;
            }

            string sql = @"select Top 3 sc.*,
            scs.IsAttend ,scs.AggScore, scs.TeachScore,scs.HardScore,scs.EnvirScore,scs.ManageScore,scs.LifeScore,
            case when sc.PostUserRole = '1' then 1 else 0 end as isSchoolUser
            from SchoolComments sc
            left join SchoolCommentScores scs on scs.CommentId = sc.Id
            where sc.State in (0,1,2,3) and SchoolId = @SchoolId " + sqlwhere + @"
            order by sc.IsTop desc,isSchoolUser desc, (sc.LikeCount + sc.ReplyCount) desc";

            //var cmd = new SqlCommand(sql);
            //SqlParameter para = new SqlParameter("@SchoolId", schoolId);
            //var param = cmd;

            SqlParameter[] param = {
                new SqlParameter("@SchoolId",schoolId)
            };

            var list = Query<SchoolCommentExt>(sql, param);
            return list.Select(q => new SchoolComment
            {
                Id = q.Id,
                No = q.No,
                CommentUserId = q.CommentUserId,
                Content = q.Content,
                IsAnony = q.IsAnony,
                IsHaveImagers = q.IsHaveImagers,
                IsSettlement = q.IsSettlement,
                IsTop = q.IsTop,
                LikeCount = q.LikeCount,
                ReplyCount = q.ReplyCount,
                PostUserRole = q.PostUserRole,
                RumorRefuting = q.RumorRefuting,
                SchoolId = q.SchoolId,
                SchoolSectionId = q.SchoolSectionId,
                State = q.State,
                AddTime = q.AddTime,
                SchoolCommentScore = new SchoolCommentScore
                {
                    CommentId = q.Id,
                    IsAttend = q.IsAttend,
                    AggScore = q.AggScore,
                    EnvirScore = q.EnvirScore,
                    HardScore = q.HardScore,
                    LifeScore = q.LifeScore,
                    ManageScore = q.ManageScore,
                    TeachScore = q.TeachScore
                }
            }).ToList();
        }

        /// <summary>
        /// 获取该校下最精选点评
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        public SchoolComment GetSchoolSelectedCommentBySchoolId(Guid SchoolId)
        {
            var isTopComment = base.GetList(x => SchoolId == x.SchoolSectionId && x.IsTop == true && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).OrderByDescending(x => x.LikeCount + x.ReplyCount).FirstOrDefault();
            //暂无置顶点评，则取（回答数+点赞数）最高的点评
            if (isTopComment != null)
            {
                return isTopComment;
            }
            else
            {
                //获取校方点评
                var schoolUserComment = base.GetList(x => SchoolId == x.SchoolSectionId && x.PostUserRole == UserManage.Domain.Common.UserRole.School && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).OrderByDescending(x => x.LikeCount + x.ReplyCount).FirstOrDefault();
                if (schoolUserComment != null)
                {
                    return schoolUserComment;
                }
                else
                {
                    return base.GetList(x => SchoolId == x.SchoolSectionId && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).OrderByDescending(x => x.LikeCount + x.SchoolCommentReplys.Count()).FirstOrDefault();
                }
            }
        }

        public List<SchoolComment> GetSchoolCommentThree(List<Guid> ids, List<SchoolComment> ThreeComment, int Take)
        {
            return base.GetList(x => ids.Contains(x.SchoolSectionId) && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight) && !ThreeComment.Select(s => s.Id).Contains(x.Id)).OrderByDescending(x => x.LikeCount + x.SchoolCommentReplys.Count()).Take(Take).ToList();
        }

        public List<SchoolCommentTotal> CurrentCommentTotalBySchoolId(Guid SchoolId)
        {
            ////根据学校分部id得到学校id
            //Guid School = _Repository.GetSchoolExtension(SchoolId).SchoolId;
            ////该分部下所有学校
            //var AllExtension = _Repository.GetSchoolExtName(School);
            string sql = @"
                            SELECT
	                        1 AS TotalType,
	                        COUNT ( 1 ) AS Total,
	                        '00000000-0000-0000-0000-000000000000' AS SchoolSectionId 
                        FROM
	                        SchoolComments AS c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
                        WHERE
	                        SchoolId = @SchoolId 
	                        AND State in (0,1,2,3)
	                        AND e.IsValid = 1 UNION
                        SELECT
	                        2 AS TotalType,
	                        COUNT ( 1 ) AS Total,
	                        '00000000-0000-0000-0000-000000000000' AS SchoolSectionId
                        FROM
	                        SchoolComments as c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
                        WHERE
	                        SchoolId = @SchoolId 
	                        AND ( State = 3 OR ReplyCount > 10 ) 
	                        AND e.IsValid = 1 
	                        AND State in (0,1,2,3) UNION
                        SELECT
	                        3 AS TotalType,
	                        COUNT ( 1 ) AS Total,
	                        '00000000-0000-0000-0000-000000000000' AS SchoolSectionId 
                        FROM
	                        SchoolComments AS c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id
	                        LEFT JOIN SchoolCommentScores AS s ON c.Id = s.CommentId 
                        WHERE
	                        s.IsAttend = 1 
	                        AND c.SchoolId = @SchoolId 
	                        AND c.State in (0,1,2,3)
	                        AND e.IsValid = 1 UNION
                        SELECT
	                        4 AS TotalType,
	                        COUNT ( 1 ) AS Total,
	                        '00000000-0000-0000-0000-000000000000' AS SchoolSectionId 
                        FROM
	                        SchoolComments as c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
                        WHERE
	                        SchoolId = @SchoolId 
	                        AND RumorRefuting = 1 
	                        AND State in (0,1,2,3) 
	                        AND e.IsValid = 1 UNION
                        SELECT
	                        5 AS TotalType,
	                        COUNT ( 1 ) AS Total,
	                        '00000000-0000-0000-0000-000000000000' AS SchoolSectionId 
                        FROM
	                        SchoolComments AS s
	                        LEFT JOIN SchoolCommentScores AS c ON s.Id = c.CommentId
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON s.SchoolSectionId = e.id 
                        WHERE
	                        s.SchoolId = @SchoolId 
	                        AND c.AggScore >= 80 
	                        AND s.State in (0,1,2,3) 
	                        AND e.IsValid = 1 UNION
                        SELECT
	                        6 AS TotalType,
	                        COUNT ( 1 ) AS Total,
	                        '00000000-0000-0000-0000-000000000000' AS SchoolSectionId 
                        FROM
	                        SchoolComments AS s
	                        LEFT JOIN SchoolCommentScores AS c ON s.Id = c.CommentId
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON s.SchoolSectionId = e.id 
                        WHERE
	                        s.SchoolId = @SchoolId 
	                        AND c.AggScore <= 40 
	                        AND s.State in (0,1,2,3)
	                        AND e.IsValid = 1 UNION
                        SELECT
	                        7 AS TotalType,
	                        COUNT ( 1 ) AS Total,
	                        '00000000-0000-0000-0000-000000000000' AS SchoolSectionId 
                        FROM
	                        SchoolComments as c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
                        WHERE
	                        IsHaveImagers = 1 
	                        AND SchoolId = @SchoolId 
	                        AND State in (0,1,2,3) 
	                        AND e.IsValid = 1 UNION
                        SELECT
	                        8 AS TotalType,
	                        COUNT ( 1 ) AS Total,
	                        SchoolSectionId 
                        FROM
	                        SchoolComments AS c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
                        WHERE
	                        c.SchoolId = @SchoolId 
	                        AND c.State in (0,1,2,3) 
	                        AND e.IsValid = 1 
                        GROUP BY
	                        c.SchoolSectionId";

            SqlParameter[] SchoolIdPara = {
                new SqlParameter("@SchoolId",SchoolId)
            };

            return Query<SchoolCommentTotal>(sql, SchoolIdPara)?.ToList();
            //List<SchoolCommentTotal> CurrentSchoolTotalInfo = Query<SchoolCommentTotal>(sql, SchoolIdPara)?.ToList();

            //for (int i = 0; i < AllExtension.Count(); i++)
            //{
            //    var item = CurrentSchoolTotalInfo.Where(x => x.SchoolSectionId == AllExtension[i].Value).FirstOrDefault();
            //    int currentIndex = CurrentSchoolTotalInfo.IndexOf(item);
            //    if (item == null)
            //    {
            //        CurrentSchoolTotalInfo.Add(new SchoolCommentTotal() { Name = AllExtension[i].Key, SchoolSectionId = AllExtension[i].Value, TotalType = QueryCondition.Other });
            //    }
            //    else
            //    {
            //        item.Name = AllExtension[i].Key;
            //        CurrentSchoolTotalInfo[currentIndex] = item;
            //    }
            //}
            //return CurrentSchoolTotalInfo;
        }

        /// <summary>
        /// 获取当前学校点赞+回复数最高的点评
        /// </summary>
        /// <param name="BranchId"></param>
        /// <returns></returns>
        public CommentExhibitionDto LikeAnswerTotalTop(Guid BranchId)
        {
            CommentExhibitionDto exhibitionDto = new CommentExhibitionDto();
            exhibitionDto.schoolComment = GetList(x => x.SchoolSectionId == BranchId && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).OrderByDescending(x => x.LikeCount + x.ReplyCount).FirstOrDefault();
            exhibitionDto.CommentImages = efRepositoryImage.GetSchoolImageList(x => x.DataSourcetId == exhibitionDto.schoolComment.Id && x.ImageType == ImageType.Comment)?.ToList();
            return exhibitionDto;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="CommentId"></param>
        /// <param name="Field">true：修改点赞次数，false：修改点评总次数</param>
        /// <returns></returns>
        public int UpdateCommentLikeorReplayCount(Guid CommentId, int operaValue, bool Field)
        {
            string sql = "";
            if (Field)
            {
                sql = "update SchoolComments set LikeCount = LikeCount + @operaValue where id = @Id";
            }
            else
            {
                sql = "update SchoolComments set ReplyCount = ReplyCount + @operaValue where id = @Id";
            }
            SqlParameter[] para = {
                new SqlParameter("@Id",CommentId),
                new SqlParameter("@operaValue",operaValue)
            };
            return ExecuteNonQuery(sql, para);
        }

        //public int CurrentSchoolCommentTotal(Guid SchoolId)
        //{
        //    List<Guid> SchoolIds = new List<Guid>();

        //    //根据学校分部id得到学校id
        //    Guid School = _Repository.GetSchoolExtension(SchoolId).SchoolId;
        //    //该分部下所有学校
        //    SchoolIds.AddRange(_Repository.GetAllSchoolBranch(School).Select(x => x.Id));

        //    List<SqlParameter> para = SchoolIds.Select((item, index) => new SqlParameter("@id" + index, item)).ToList();
        //    string strs = string.Join(',', SchoolIds.Select((item, index) => "@id" + index));

        //    string sql = $"select count(1) from SchoolComments where SchoolSectionId in ({strs})";
        //    return (int)ExecuteScalar(sql, para.ToArray());
        //}

        public List<SchoolSectionCommentOrQuestionTotal> GetTotalBySchoolSectionIds(List<Guid> SchoolSectionIds)
        {
            List<SchoolSectionCommentOrQuestionTotal> totals = new List<SchoolSectionCommentOrQuestionTotal>();
            return base.GetList(x => SchoolSectionIds.Contains(x.SchoolSectionId) && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).GroupBy(x => x.SchoolSectionId).Select(x => new SchoolSectionCommentOrQuestionTotal() { SchoolSectionId = x.Key, Total = x.Count(c => c.SchoolSectionId == x.Key) })?.ToList();
        }

        public List<Guid> GetHotSchoolSectionId()
        {
            string sql = @"SELECT top 10 SchoolSectionId as Id FROM SchoolComments as s right join
                                (select
	                                Id,
	                                row_number() over(partition by SchoolSectionId order by ((LikeCount+ReplyCount)) desc) as TopLike
                                from SchoolComments where State in (0,1,2,3)) as Hot
	                                on s.Id = Hot.Id where Hot.TopLike = 1 order by (LikeCount+ReplyCount) desc";

            return Query<SchoolIds>(sql, null).Select(x => x.Id)?.ToList();
        }

        //学校分数统计
        public PMS.CommentsManage.Domain.Entities.SchoolScore GetSchoolScoreBySchoolId(Guid SchoolSectionId)
        {
            string sql = @"SELECT * FROM SchoolScores where SchoolSectionId = @SchoolSectionId";

            SqlParameter[] para = {
                new SqlParameter("@SchoolSectionId",SchoolSectionId)
            };

            var comment = Query<PMS.CommentsManage.Domain.Entities.SchoolScore>(sql, para).FirstOrDefault();

            if (comment != null)
            {
                return comment;
            }
            else
            {
                return null;
            }
        }

        public List<SchoolComment> GetSchoolCommentBySchoolIdOrConente(Guid SchoolId, string Conent = "")
        {
            List<SqlParameter> para = new List<SqlParameter>();

            string sql = @"select  top 20 * from SchoolComments WHERE State in (0,1,2,3) and SchoolSectionId = @SchoolId";
            if (Conent != "" && Conent != null)
            {
                sql += " and CONTAINS (Content,@Conent)";
                para.Add(new SqlParameter("@Conent", Conent));
            }
            sql += " order by AddTime desc";

            para.Add(new SqlParameter("@SchoolId", SchoolId));
            return Query<SchoolComment>(sql, para.ToArray())?.ToList();
        }

        public List<SchoolComment> GetCommentData(int pageNo, int pageSize, DateTime lastTime)
        {
            //return GetList(q=>q.AddTime > lastTime && q.State != ExamineStatus.Block).Skip(pageNo * pageSize).Take(pageSize).OrderBy(q => q.AddTime).ToList();
            if (lastTime > Convert.ToDateTime("1999-01-01"))
            {
                var list = _dbContext.SchoolComments.Include(q => q.SchoolCommentScore)
                .Where(q => q.AddTime > lastTime)
                .Skip(pageNo * pageSize).Take(pageSize).OrderBy(q => q.AddTime).ToList();
                return list;
            }
            else
            {
                var list = _dbContext.SchoolComments.Include(q => q.SchoolCommentScore)
                .Where(q => q.AddTime > lastTime && (q.State == ExamineStatus.Unknown || q.State == ExamineStatus.Unread || q.State == ExamineStatus.Readed || q.State == ExamineStatus.Highlight))
                .Skip(pageNo * pageSize).Take(pageSize).OrderBy(q => q.AddTime).ToList();
                return list;
            }

            //string sql = @"select
            //             c.*,
            //             s.IsAttend,
            //             s.AggScore,
            //             s.TeachScore,
            //             s.HardScore,
            //             s.EnvirScore,
            //             s.ManageScore,
            //             s.LifeScore
            //            from SchoolComments c
            //            inner join SchoolCommentScores s on c.Id = s.CommentId
            //            where c.AddTime > @lastTime and c.State in (0,1,2,3)
            //            order by c.AddTime asc
            //                OFFSET @OFFSET ROWS FETCH NEXT @pageSize ROWS ONLY";
            //List<SqlParameter> para = new List<SqlParameter>() {
            //    new SqlParameter("@OFFSET", pageNo * pageSize),
            //    new SqlParameter("@pageSize", pageSize),
            //    new SqlParameter("@lastTime", lastTime),
            //};
            //var list = Query<SchoolCommentExt>(sql, para.ToArray()).ToList();
            //return list.Select(q => new SchoolComment
            //{
            //    Id = q.Id,
            //    CommentUserId = q.CommentUserId,
            //    Content = q.Content,
            //    IsAnony = q.IsAnony,
            //    IsHaveImagers = q.IsHaveImagers,
            //    IsSettlement = q.IsSettlement,
            //    IsTop = q.IsTop,
            //    LikeCount = q.LikeCount,
            //    ReplyCount = q.ReplyCount,
            //    PostUserRole = q.PostUserRole,
            //    RumorRefuting = q.RumorRefuting,
            //    SchoolId = q.SchoolId,
            //    SchoolSectionId = q.SchoolSectionId,
            //    State = q.State,
            //    AddTime = q.AddTime,
            //    SchoolCommentScore = new SchoolCommentScore
            //    {
            //        CommentId = q.Id,
            //        IsAttend = q.IsAttend,
            //        AggScore = q.AggScore,
            //        EnvirScore = q.EnvirScore,
            //        HardScore = q.HardScore,
            //        LifeScore = q.LifeScore,
            //        ManageScore = q.ManageScore,
            //        TeachScore = q.TeachScore
            //    }
            //}).ToList();
        }

        public List<CommentAndReply> GetCommentAndReplies(Guid UserId, int PageIndex, int PageSize)
        {
            string sql = @"select * from
                        (SELECT
	                        s.Id as CommentId,
	                        '00000000-0000-0000-0000-000000000000' as ParentReplyId,
	                        SchoolId,
	                        SchoolSectionId,
	                        CommentUserId AS UserId,
	                        Content,
	                        ReplyCount,
	                        LikeCount,
	                        IsAnony,
	                        '00000000-0000-0000-0000-000000000000' as ReplyId,
	                        s.AddTime as CommmentAddTime,
	                        '' as ReplyAddTime,
	                        s.AddTime as AddTime,
	                        0 as Type,
	                        (case
		                        when i.ImageTotal is NULL THEN 0
		                        when i.ImageTotal is NOT NULL THEN i.ImageTotal
	                        end) as ImageTotal
                        FROM
	                        SchoolComments as s
		                        left join (select DataSourcetId,count(1) as ImageTotal from SchoolImage group by DataSourcetId) as i on s.Id = i.DataSourcetId
                        UNION
                        SELECT
	                        c.Id as CommentId,
	                        r.Id as ParentReplyId,
	                        c.SchoolId,
	                        c.SchoolSectionId,
	                        r.UserId AS UserId,
	                        r.Content,
	                        r.ReplyCount,
	                        r.LikeCount,
	                        r.IsAnony,
	                        (case
		                        when r.ReplyId is NULL THEN '00000000-0000-0000-0000-000000000000'
		                        when r.ReplyId is NOT NULL THEN r.ReplyId
	                        end) as ReplyId,
	                        c.AddTime as CommmentAddTime,
	                        r.CreateTime as ReplyAddTime,
                          r.CreateTime as AddTime,
	                        1 as Type,
	                        0 as ImageTotal
                        FROM
	                        SchoolCommentReplies AS r
	                        LEFT JOIN SchoolComments AS c ON r.SchoolCommentId = c.Id
                        ) as c
                        where c.UserId = @userId order by AddTime desc
                            OFFSET @pageIndex ROWS FETCH NEXT @pageSize ROWS ONLY
                    ";
            return Query<CommentAndReply>(sql, new SqlParameter[] { new SqlParameter("@userId",UserId),
                        new SqlParameter("@pageIndex",PageIndex),
                    new SqlParameter("@pageSize",PageSize) })?.ToList();
        }

        public int CommentTotal(Guid userId)
        {
            return base.GetCount(x => x.CommentUserId == userId && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight));
        }

        public List<HotCommentSchool> GetHotCommentSchools(DateTime starTime, DateTime endTime, int count = 6)
        {
            //        string sql = @"select top 6 * from
            //        (
            //         select
            //          s.name+'-'+e.name as SchoolName,
            //          t.SchoolSectionId,
            //                s.Id as SchoolId,
            //          t.Total,
            //          t.city as City,
            //                            e.grade,
            //				e.type,
            //				e.discount,
            //				e.diglossia,
            //				e.chinese,
            //          ROW_NUMBER() over(partition by t.City order by t.Total desc) rowNum
            //         from (
            //          select
            //		            s.SchoolSectionId,
            //		            s.Total,
            //		            e.city
            //	            from [iSchoolData].[dbo].[OnlineSchoolExtContent] as e
            //          RIGHT JOIN
            //           (select SchoolSectionId,sum(ReplyTotal) as Total from
            //            (
            //	            select s.SchoolSectionId,sum(r.ReplyTotal) ReplyTotal from [SchoolComments] as s
            //		            RIGHT JOIN
            //			            (select count(Id) as ReplyTotal,SchoolCommentId from [SchoolCommentReplies]
            //				            where (CreateTime >= @starTime and CreateTime <= @endTime)
            //					            GROUP BY SchoolCommentId) as r
            //						            ON s.Id = r.SchoolCommentId
            //		            GROUP BY SchoolSectionId
            //	            UNION
            //	            select s.SchoolSectionId,sum(l.CommentLike) as LikeTotal from [SchoolComments] as s
            //			            RIGHT JOIN
            //				            (select count(Id) as CommentLike,SourceId from [SchoolCommentLikes]
            //					            where (CreateTime >= @starTime and CreateTime <= @endTime) and LikeType = 1
            //										            GROUP BY SourceId) as l
            //			            ON s.Id = l.SourceId
            //		            GROUP BY s.SchoolSectionId
            //	            UNION
            //	            select s.SchoolSectionId,sum(r.CommentLike) as LikeTotal from [SchoolComments] as s
            //		            RIGHT JOIN
            //			            (select r.SchoolCommentId,l.CommentLike from [SchoolCommentReplies] as r
            //				            RIGHT JOIN
            //					            (select count(Id) as CommentLike,SourceId from [SchoolCommentLikes]
            //						            where (CreateTime >= @starTime and CreateTime <= @endTime) and LikeType = 3
            //							            GROUP BY SourceId) as l
            //				            ON r.Id = l.SourceId) as r
            //		            ON s.Id = r.SchoolCommentId
            //	            GROUP BY s.SchoolSectionId
            //            ) as total
            //	            GROUP BY SchoolSectionId) as s
            //	            ON e.eid = s.SchoolSectionId
            //          ) as t
            //           left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e	ON t.SchoolSectionId = e.Id
            //           left join [iSchoolData].[dbo].[OnlineSchool] as s ON e.sid = s.Id
            //        ) as t where t.rowNum >= 1 and t.rowNum <= 6 and t.SchoolSectionId is not null and t.SchoolId is not null
            //order by t.Total desc";


            string sql = $@"
                    select c.SchoolSectionId,s.Id as SchoolId,c.Total,s.name+'-'+e.name as SchoolName from 
 (
                        select top {count} SchoolSectionId,count(1) as Total from SchoolComments as c
														left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e ON c.SchoolSectionId = e.Id
														left join [iSchoolData].[dbo].[OnlineSchool] as s ON e.sid = s.Id
													WHERE c.AddTime >= @starTime and c.AddTime <= @endTime and e.IsValid = 1 and s.IsValid = 1 and s.status = 3 and c.State in (0,1,2,3)
	                        GROUP BY SchoolSectionId
	                        order by count(1) desc 
													) as c
                        left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e	ON c.SchoolSectionId = e.Id
                        left join [iSchoolData].[dbo].[OnlineSchool] as s ON e.sid = s.Id
	                        ORDER BY c.Total desc
                ";

            SqlParameter[] para = {
                new SqlParameter("@starTime",starTime),
                new SqlParameter("@endTime",endTime)
            };

            return Query<HotCommentSchool>(sql, para)?.ToList();
        }

        public List<HotComment> GetHotComments(DateTime date)
        {
            DateTime starTime = date.AddDays(-2).Date;
            DateTime endTime = date.AddDays(1).Date;

            string sql = @"select
				o.name+'-'+s.name as SchoolName,
				s.id as SchoolId,
				t.*
				 from (
                    select
	                    t.*,
	                    ROW_NUMBER() over(partition by t.City order by t.Total desc) rowNum from
                    (
	                    select
		                    s.Id,
		                    sum(s.Total) as Total,
		                    min(s.SchoolSectionId) as SchoolSectionId,
		                    max(s.City) as City
	                    from(
		                    select
				                    s.Id,
				                     s.SchoolSectionId,
				                     t.Total,
				                     e.City
				                     from SchoolComments as s
			                    RIGHT JOIN
			                    (
						                    select SourceId,
						                    count(Id) as Total
							                    from SchoolCommentLikes
							                    where LikeType = 1 and  CreateTime >= @starTime and CreateTime<= @endTime
							                    GROUP BY SourceId
			                    ) as t
			                    ON s.Id = t.SourceId
			                    LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtContent] as e
			                    ON s.SchoolSectionId = e.eid
		                    UNION
				                    select
						                    s.Id,
						                     s.SchoolSectionId,
						                     t.Total,
						                     e.City
						                     from SchoolComments as s
					                    RIGHT JOIN
					                    (
								                    select SchoolCommentId,count(Id) as Total
			                    from SchoolCommentReplies
			                    where CreateTime >= @starTime and CreateTime<= @endTime
		                    GROUP BY SchoolCommentId
					                    ) as t
					                    ON s.Id = t.SchoolCommentId
					                    LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtContent] as e
					                    ON s.SchoolSectionId = e.eid
		                    UNION
	                    select c.Id,c.SchoolSectionId,s.Total,e.City from SchoolCommentReplies as r
		                    RIGHT JOIN
			                    (select SourceId,
								                    count(Id) as Total
									                    from SchoolCommentLikes
									                    where LikeType = 3 and  CreateTime >= @starTime and CreateTime<= @endTime
									                    GROUP BY SourceId) as s
		                    ON r.Id = s.SourceId
		                    LEFT JOIN SchoolComments as c on r.SchoolCommentId = c.Id
		                    LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtContent] as e
			                    ON c.SchoolSectionId = e.eid
		                    ) as s
		                    GROUP BY s.Id
                    ) as t
                    ) as t
	                    LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] as o ON t.SchoolSectionId = o.Id
	                    LEFT JOIN [iSchoolData].[dbo].[OnlineSchool] as s ON s.Id = o.sid
                    where t.rowNum >= 1 and t.rowNum <= 6";

            SqlParameter[] para = {
                new SqlParameter("@starTime",starTime),
                new SqlParameter("@endTime",endTime)
            };

            return Query<HotComment>(sql, para)?.ToList();
        }

        public bool CheckLogout(Guid userId)
        {
            DateTime beginTime = DateTime.Now.AddDays(-7).Date, endTime = DateTime.Now.AddDays(1).Date;

            SqlParameter[] para = {
                new SqlParameter("@userId",userId),
                new SqlParameter("@beginTime",beginTime),
                new SqlParameter("@endTime",endTime)
            };

            string sql = @"select sum(t.Total) as Total from (
	                select count(1) as Total from SchoolComments where
		                CommentUserId = @userId
		                and AddTime >= @beginTime and AddTime <= @endTime
	                union
	                select count(1) as Total from QuestionInfos where
		                UserId = @userId
		                and CreateTime >= @beginTime and CreateTime <= @endTime
	                union
	                select count(1) as Total from SchoolCommentReplies where
		                UserId = @userId
		                and CreateTime >= @beginTime and CreateTime <= @endTime
	                union
	                select count(1) as Total from QuestionsAnswersInfos where
		                UserId = @userId
		                and CreateTime >= @beginTime and CreateTime <= @endTime) as t";
            return Query<CheckIsLogout>(sql, para).FirstOrDefault().Total > 0;
        }

        /// <summary>
        /// PC端 点评列表
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public List<SchoolComment> CommentList_Pc(int City, int PageIndex, int PageSize, out int Total)
        {
            List<SqlParameter> para = new List<SqlParameter>();

            string sql = @"select
	                        t.*,
	                        s.IsAttend,
	                        s.AggScore,
	                        s.TeachScore,
	                        s.HardScore,
	                        s.EnvirScore,
	                        s.ManageScore,
	                        s.LifeScore
                        from
	                        (select comment.* from SchoolScores as score
		                        left join (SELECT
			                        s.*,
			                        row_number () OVER ( partition BY s.SchoolSectionId ORDER BY s.RumorRefuting DESC, s.Essence DESC, s.AddTime DESC ) AS row
		                        FROM
			                        ( SELECT *, ( CASE WHEN State = 4 THEN 1 ELSE 0 END ) AS Essence FROM SchoolComments ) AS s) as comment
			                        on score.SchoolSectionId = comment.SchoolSectionId
		                          LEFT JOIN [iSchoolData].dbo.OnlineSchoolExtContent AS e ON comment.SchoolSectionId = e.eid
			                        where score.CommentCount <> 0 and State in (0,1,2,3)
                        ";
            if (City > 0)
            {
                sql += " and e.city = @city ";
                para.Add(new SqlParameter("@city", City));
            }

            sql += @"
	                         ) as t
                        LEFT JOIN SchoolCommentScores AS s ON s.CommentId = t.Id
                        WHERE
	                        t.row = 1
                        order by t.IsTop desc,
	                        t.State desc,
	                        t.RumorRefuting desc,
	                        t.AddTime desc
                         OFFSET @pageindex ROWS FETCH NEXT @pagesize ROWS ONLY";

            para.Add(new SqlParameter("@pageindex", (PageIndex - 1) * PageSize));
            para.Add(new SqlParameter("@pagesize", PageSize));

            Total = CommentListTotal_Pc(City);

            if (Total == 0)
            {
                return new List<SchoolComment>();
            }
            else
            {
                var list = Query<SchoolCommentExt>(sql, para.ToArray());
                return list.Select(q => new SchoolComment
                {
                    Id = q.Id,
                    No = q.No,
                    CommentUserId = q.CommentUserId,
                    Content = q.Content,
                    IsAnony = q.IsAnony,
                    IsHaveImagers = q.IsHaveImagers,
                    IsSettlement = q.IsSettlement,
                    IsTop = q.IsTop,
                    LikeCount = q.LikeCount,
                    ReplyCount = q.ReplyCount,
                    PostUserRole = q.PostUserRole,
                    RumorRefuting = q.RumorRefuting,
                    SchoolId = q.SchoolId,
                    SchoolSectionId = q.SchoolSectionId,
                    State = q.State,
                    AddTime = q.AddTime,
                    SchoolCommentScore = new SchoolCommentScore
                    {
                        CommentId = q.Id,
                        IsAttend = q.IsAttend,
                        AggScore = q.AggScore,
                        EnvirScore = q.EnvirScore,
                        HardScore = q.HardScore,
                        LifeScore = q.LifeScore,
                        ManageScore = q.ManageScore,
                        TeachScore = q.TeachScore
                    }
                }).ToList();
            }
        }

        /// <summary>
        /// 统计该城市的点评总数
        /// </summary>
        /// <param name="City"></param>
        /// <returns></returns>
        public int CommentListTotal_Pc(int City)
        {
            string sql = @"select count(s.SchoolSectionId) as Total from SchoolScores as s
                            left join[iSchoolData].dbo.OnlineSchoolExtContent AS e ON s.SchoolSectionId = e.eid";
            if (City > 0)
            {
                sql += "  where e.city = @city ";
                return Query<SchoolTotal>(sql, new SqlParameter[]
                {
                    new SqlParameter("@city",City)
                }).FirstOrDefault().Total;
            }
            else
            {
                return Query<SchoolTotal>(sql, null).FirstOrDefault().Total;
            }
        }

        public List<HotCommentSchool> HottestSchool(HotCommentQuery query, bool queryAll)
        {
            List<SqlParameter> para = new List<SqlParameter>();
            string sql = @"
                    SELECT
	                    l.eid AS SchoolSectionId,
	                    l.sid AS SchoolId,
	                    l.Schname+ '-' + Extname AS SchoolName,
	                    s.Total
                    FROM
	                    [iSchoolData].dbo.Lyega_OLschextSimpleInfo AS l
	                    RIGHT JOIN (
	                    SELECT
		                    SchoolSectionId,
		                    SUM ( ReplyTotal ) AS Total
	                    FROM
		                    (
		                    SELECT
			                    s.SchoolSectionId,
			                    SUM ( r.ReplyTotal ) ReplyTotal
		                    FROM
			                    [SchoolComments] AS s
			                    RIGHT JOIN ( SELECT COUNT ( Id ) AS ReplyTotal, SchoolCommentId FROM [SchoolCommentReplies] WHERE ( CreateTime >= @startTime AND CreateTime <= @endTime ) GROUP BY SchoolCommentId ) AS r ON s.Id = r.SchoolCommentId
		                    GROUP BY
			                    SchoolSectionId UNION
		                    SELECT
			                    s.SchoolSectionId,
			                    SUM ( l.CommentLike ) AS LikeTotal
		                    FROM
			                    [SchoolComments] AS s
			                    RIGHT JOIN (
			                    SELECT COUNT
				                    ( Id ) AS CommentLike,
				                    SourceId
			                    FROM
				                    [SchoolCommentLikes]
			                    WHERE
				                    ( CreateTime >= @startTime AND CreateTime <= @endTime )
				                    AND LikeType = 1
			                    GROUP BY
				                    SourceId
			                    ) AS l ON s.Id = l.SourceId
		                    GROUP BY
			                    s.SchoolSectionId UNION
		                    SELECT
			                    s.SchoolSectionId,
			                    SUM ( r.CommentLike ) AS LikeTotal
		                    FROM
			                    [SchoolComments] AS s
			                    RIGHT JOIN (
			                    SELECT
				                    r.SchoolCommentId,
				                    l.CommentLike
			                    FROM
				                    [SchoolCommentReplies] AS r
				                    RIGHT JOIN (
				                    SELECT COUNT
					                    ( Id ) AS CommentLike,
					                    SourceId
				                    FROM
					                    [SchoolCommentLikes]
				                    WHERE
					                    ( CreateTime >= @startTime AND CreateTime <= @endTime )
					                    AND LikeType = 3
				                    GROUP BY
					                    SourceId
				                    ) AS l ON r.Id = l.SourceId
			                    ) AS r ON s.Id = r.SchoolCommentId
		                    GROUP BY
			                    s.SchoolSectionId
		                    ) AS total
	                    GROUP BY
		                    SchoolSectionId
	                    ) AS s ON l.eid = s.SchoolSectionId ";

            if (!queryAll)
            {
                sql += " WHERE l.city = @city ";
                para.Add(new SqlParameter("@city", query.City));

                if (query.Condition)
                {
                    sql += @"
	                    AND l.grade = @grade
	                    AND l.type = @type
	                    AND l.discount = @discount
	                    AND l.diglossia = @diglossia
	                    AND l.chinese = @chinese ";
                    para.Add(new SqlParameter("@grade", query.Grade));
                    para.Add(new SqlParameter("@type", query.Type));
                    para.Add(new SqlParameter("@discount", query.Discount));
                    para.Add(new SqlParameter("@diglossia", query.Diglossia));
                    para.Add(new SqlParameter("@chinese", query.Chinese));
                }
            }

            para.Add(new SqlParameter("@startTime", query.StartTime));
            para.Add(new SqlParameter("@endTime", query.EndTime));

            sql += " ORDER BY s.Total DESC OFFSET 1 ROWS FETCH NEXT 6 ROWS ONLY  ";

            return Query<HotCommentSchool>(sql, para.ToArray())?.ToList();
        }

        public List<SchoolComment> HottestComment(DateTime StartTime, DateTime EndTime)
        {
            string sql = @"
                    select s.*,t.Total,c.AggScore from SchoolComments as s
	                    RIGHT JOIN
	                    (select s.SourceId,sum(s.Total) as Total from
		                    (select SourceId,count(1) Total from SchoolCommentLikes
			                    where LikeType = 1
				                    and CreateTime >= @startTime
				                    and CreateTime <= @endTime
			                    GROUP BY SourceId
		                    union
		                    select SchoolCommentId as SourceId,count(1) as Total from SchoolCommentReplies
			                    where
				                    CreateTime >= @startTime
				                    and CreateTime <= @endTime
			                    GROUP BY SchoolCommentId) as s GROUP BY s.SourceId) as t
	                    on s.Id = t.SourceId
	                    LEFT JOIN SchoolScores as c on s.SchoolSectionId = c.SchoolSectionId
                            where Id is not null and State in (0,1,2,3)
		                    ORDER BY t.Total desc
		                    OFFSET 1 ROWS FETCH NEXT 6 ROWS ONLY";

            SqlParameter[] para = {
                new SqlParameter("@startTime",StartTime),
                new SqlParameter("@endTime",EndTime)
            };

            var list = Query<SchoolCommentExt>(sql, para);
            return list.Select(q => new SchoolComment
            {
                Id = q.Id,
                No = q.No,
                CommentUserId = q.CommentUserId,
                Content = q.Content,
                IsAnony = q.IsAnony,
                IsHaveImagers = q.IsHaveImagers,
                IsSettlement = q.IsSettlement,
                IsTop = q.IsTop,
                LikeCount = q.LikeCount,
                ReplyCount = q.ReplyCount,
                PostUserRole = q.PostUserRole,
                RumorRefuting = q.RumorRefuting,
                SchoolId = q.SchoolId,
                SchoolSectionId = q.SchoolSectionId,
                State = q.State,
                AddTime = q.AddTime,
                SchoolCommentScore = new SchoolCommentScore
                {
                    CommentId = q.Id,
                    IsAttend = q.IsAttend,
                    AggScore = q.AggScore,
                    EnvirScore = q.EnvirScore,
                    HardScore = q.HardScore,
                    LifeScore = q.LifeScore,
                    ManageScore = q.ManageScore,
                    TeachScore = q.TeachScore
                }
            }).ToList();
        }

        public List<SchoolComment> HotComment(HotCommentQuery CommentQuery)
        {
            List<SqlParameter> para = new List<SqlParameter>();

            string sql = @"select s.*,c.AggScore from SchoolComments as s
	                    RIGHT JOIN
		                    (select s.SourceId,sum(s.Total) as Total from
				                    (select SourceId,count(1) Total from SchoolCommentLikes
					                    where LikeType = 1
						                    and CreateTime >= @StartTime
						                    and CreateTime <= @EndTime
					                    GROUP BY SourceId
				                    union
				                    select SchoolCommentId as SourceId,count(1) as Total from SchoolCommentReplies
					                    where
						                    CreateTime >= @StartTime
						                    and CreateTime <= @EndTime
					                    GROUP BY SchoolCommentId) as s GROUP BY s.SourceId) as t
	                    on s.Id = t.SourceId
		                    LEFT JOIN [iSchoolData].dbo.Lyega_OLschextSimpleInfo as l
	                    on s.SchoolSectionId = l.eid
		                    LEFT JOIN SchoolScores as c on s.SchoolSectionId = c.SchoolSectionId";
            if (CommentQuery.City > 0)
            {
                sql += " where l.city = @City and s.State IN (0,1,2,3) ";
            }
            else
            {
                //sql += " where 1 = 1 ";
                sql += " Where s.State IN (0,1,2,3) ";
            }

            para.Add(new SqlParameter("@City", CommentQuery.City));
            if (CommentQuery.Condition)
            {
                sql += @"and l.grade = @Grade
		                    and l.type = @Type
		                    and l.discount = @Discount
		                    and l.diglossia = @Diglossia
		                    and l.chinese = @Chinese";
                para.Add(new SqlParameter("@Grade", CommentQuery.Grade));
                para.Add(new SqlParameter("@Type", CommentQuery.Type));
                para.Add(new SqlParameter("@Discount", CommentQuery.Discount));
                para.Add(new SqlParameter("@Diglossia", CommentQuery.Diglossia));
                para.Add(new SqlParameter("@Chinese", CommentQuery.Chinese));
            }

            sql += " ORDER BY t.Total desc";

            para.Add(new SqlParameter("@StartTime", CommentQuery.StartTime));
            para.Add(new SqlParameter("@EndTime", CommentQuery.EndTime));

            var list = Query<SchoolCommentExt>(sql, para.ToArray());
            return list.Select(q => new SchoolComment
            {
                Id = q.Id,
                No = q.No,
                CommentUserId = q.CommentUserId,
                Content = q.Content,
                IsAnony = q.IsAnony,
                IsHaveImagers = q.IsHaveImagers,
                IsSettlement = q.IsSettlement,
                IsTop = q.IsTop,
                LikeCount = q.LikeCount,
                ReplyCount = q.ReplyCount,
                PostUserRole = q.PostUserRole,
                RumorRefuting = q.RumorRefuting,
                SchoolId = q.SchoolId,
                SchoolSectionId = q.SchoolSectionId,
                State = q.State,
                AddTime = q.AddTime,
                SchoolCommentScore = new SchoolCommentScore
                {
                    CommentId = q.Id,
                    IsAttend = q.IsAttend,
                    AggScore = q.AggScore,
                    EnvirScore = q.EnvirScore,
                    HardScore = q.HardScore,
                    LifeScore = q.LifeScore,
                    ManageScore = q.ManageScore,
                    TeachScore = q.TeachScore
                }
            }).ToList();
        }

        public bool Checkisdistinct(string content)
        {
            return base.GetList(x => x.Content == content).FirstOrDefault() == null;
        }

        public DateTime QueryCommentTime(Guid CommentId)
        {
            string sql = "select AddTime from SchoolComments where Id = @CommentId";
            return Query<SchoolComment>(sql, new SqlParameter[] { new SqlParameter("@CommentId", CommentId) }).FirstOrDefault().AddTime;
        }

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public bool UpdateViewCount(Guid commentId)
        {
            string sql = "update SchoolComments set ViewCount = ViewCount + 1 where id = @Id";
            return ExecuteNonQuery(sql, new SqlParameter[] { new SqlParameter("@Id", commentId) }) > 0;
        }

        /// <summary>
        /// 通过学部ID统计点评数据
        /// </summary>
        /// <param name="SchoolSectionId"></param>
        /// <returns></returns>
        public SchCommentData GetSchoolCommentDataByID(Guid SchoolSectionId)
        {
            string sql = @"select sum(s.LikeCount) as LikeCount,sum(s.ReplyCount) as ReplyCount,sum(s.ViewCount) as CommentViewCount, sum(sr.ViewCount) as CommentRepliyViewCount
                            from SchoolComments as s left join SchoolCommentReplies AS sr ON s.Id = sr.SchoolCommentId
                            where s.SchoolSectionId = @SchoolSectionId  AND s.State in (0,1,2,3)
                            GROUP BY s.SchoolSectionId";

            return Query<SchCommentData>(sql, new SqlParameter[]
                {
                    new SqlParameter("@SchoolSectionId",SchoolSectionId)
                }).FirstOrDefault();
        }

        public List<SchoolTotal> GetCommentCountBySchool(List<Guid> schoolSectionId)
        {
            if (schoolSectionId.Count == 0)
            {
                return new List<SchoolTotal>();
            }

            List<SqlParameter> para = schoolSectionId.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", schoolSectionId.Select((q, idx) => "@id" + idx));

            string sql = @"select SchoolSectionId as Id,CommentCount as Total from SchoolScores where SchoolSectionId in (" + strpara + ");";

            return Query<SchoolTotal>(sql, para.ToArray()).ToList();
        }
        public List<SchoolTotal> GetCommentCountBySchoolSectionIDs(List<Guid> schoolSectionIDs)
        {
            if (schoolSectionIDs == null || !schoolSectionIDs.Any()) { return new List<SchoolTotal>(); }

            var ids = string.Join(",", schoolSectionIDs.Select(p => $"'{p}'"));

            string sql = $@"SELECT
	                            SchoolSectionId AS ID,
	                            COUNT ( id ) AS Total 
                            FROM
	                            SchoolComments 
                            WHERE
	                            SchoolSectionId In ({ids})
	                            AND state in (0,1,2,3)
	                            AND SchoolId != '00000000-0000-0000-0000-000000000000'
                            GROUP BY
	                            SchoolSectionId";

            return Query<SchoolTotal>(sql).ToList();
        }

        public List<SchoolTotal> GetReplyCount(List<Guid> commentIds)
        {
            if (commentIds.Count == 0)
            {
                return new List<SchoolTotal>();
            }

            List<SqlParameter> para = commentIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", commentIds.Select((q, idx) => "@id" + idx));

            string sql = @"select Id,ReplyCount as Total from SchoolComments where Id in (" + strpara + ");";

            return Query<SchoolTotal>(sql, para.ToArray()).ToList();
        }

        public List<SchoolTotal> GetCommentReplyCount(int pageNo, int pageSize)
        {
            string sql = @"select Id,ReplyCount as Total from SchoolComments
                            Where ReplyCount > 0
                            ORDER BY AddTime desc
		                    OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";
            SqlParameter[] para = {
                new SqlParameter("@offset",pageNo * pageSize),
                new SqlParameter("@limit",pageSize)
            };
            return Query<SchoolTotal>(sql, para.ToArray()).ToList();
        }

        public int ChangeCommentState(Guid commentId, int state)
        {
            string sql = "update SchoolComments set State = @state where Id = @commentId";
            SqlParameter[] para = {
                new SqlParameter("@state",state),
                new SqlParameter("@commentId",commentId)
            };
            return ExecuteNonQuery(sql, new SqlParameter[] { new SqlParameter("@state", state), new SqlParameter("@commentId", commentId) });
        }

        public List<LikeTotal> GetLikeCount(List<Guid> ids)
        {
            List<SqlParameter> para = ids.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", ids.Select((q, idx) => "@id" + idx));
            string sql = @"select SourceId,LikeType,count(1) count
                from SchoolCommentLikes
                where SourceId in (" + strpara + @")
                GROUP BY SourceId,LikeType;";
            return Query<LikeTotal>(sql, para.ToArray()).ToList();
        }

        public IEnumerable<(DateTime, string, decimal)> GetSimpleCommentScores(Guid extID)
        {
            var str_SQL = @"SELECT
	                            sc.AddTime,
	                            ui.HeadImgUrl,
	                            ssc.AggScore
                            FROM
	                            SchoolCommentScores AS ssc
	                            LEFT JOIN SchoolComments AS sc ON ssc.CommentId = sc.ID 
	                            LEFT JOIN iSchoolUser.dbo.UserInfo as ui on ui.id = sc.CommentUserId
                            WHERE
	                            sc.SchoolSectionId = @extID
	                            AND sc.State != 4
                            ORDER BY sc.AddTime DESC";

            var finds = Query<SchoolCommentScoreStatisticsDto>(str_SQL, new SqlParameter[] { new SqlParameter("@extID", extID) });
            if (finds?.Any() == true)
            {
                var result = new List<(DateTime, string, decimal)>();
                foreach (var item in finds)
                {
                    result.Add((item.AddTime, item.HeadImgUrl, item.AggScore));
                }
                return result;
            }
            return null;
        }


        public List<UserCommentQueryDto> GetUserComments(IEnumerable<Guid> Ids, Guid? loginUserId)
        {
            if (!Ids.Any())
            {
                return new List<UserCommentQueryDto>();
            }

            List<SqlParameter> para = Ids.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", Ids.Select((q, idx) => "@id" + idx));
            para.Add(new SqlParameter("@loginUserId", loginUserId));

            string sql = @"
SELECT
	SC.Id,
	SC.No,
	SC.CommentUserId as UserId,
	SC.SchoolSectionId AS ExtId,
	SC.Content,
	SC.AddTime as CreateTime,
	SC.LikeCount,
	SC.ReplyCount,
	SC.IsAnony,
	SC.IsTop,
	SC.RumorRefuting,
	IsNULL(SCS.IsAttend, 0) AS IsAttend,
	ISNULL(SCS.AggScore, 0) AS AggScore,
	ISNULL((
		SELECT TOP 1 L.Id  FROM dbo.SchoolCommentLikes L
		WHERE L.SourceId = SC.Id AND L.UserId = @loginUserId AND L.LikeStatus = 1
	), 0) AS LikeId,
	(
		SELECT Count(1) FROM dbo.SchoolComments SC2 
		WHERE SC2.SchoolSectionId = SC.SchoolSectionId AND SC.State <> 4
	) AS SchoolCommentCount
FROM
   dbo.SchoolComments SC
   LEFT JOIN SchoolCommentScores SCS ON SCS.CommentId = SC.Id
WHERE 
	SC.State <> 4 AND SC.Id in (" + strpara + ")";

            return Query<UserCommentQueryDto>(sql, para.ToArray()).ToList();
        }

        public IEnumerable<Guid> GetAvailableIds(IEnumerable<Guid> Ids)
        {
            if (!Ids.Any())
            {
                return new List<Guid>();
            }
            List<SqlParameter> para = Ids.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", Ids.Select((q, idx) => "@id" + idx));
            var sql = $@"
SELECT
	SC.Id
FROM
   dbo.SchoolComments SC
WHERE 
	SC.State <> 4 AND SC.Id in (" + strpara + ")";

            return Query<IdQueryDto>(sql, para.ToArray()).Select(s => s.Id);
        }
    }
}