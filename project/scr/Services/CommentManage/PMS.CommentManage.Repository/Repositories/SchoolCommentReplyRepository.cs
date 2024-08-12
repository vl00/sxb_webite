using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PMS.CommentsManage.Domain.Common;
using System.Data.SqlClient;
using PMS.CommentsManage.Application.ModelDto.Reply;
using System.Linq.Expressions;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using ProductManagement.Infrastructure.AppService;
using Microsoft.EntityFrameworkCore;

namespace PMS.CommentsManage.Repository.Repositories.SchoolCommentRepositories
{
    public class SchoolCommentReplyRepository : EntityFrameworkRepository<SchoolCommentReply>, ISchoolCommentReplyRepository
    {
        public SchoolCommentReplyRepository(CommentsManageDbContext repository) : base(repository)
        {
        }

        public int UpdateReplyCount(Guid SchoolCommentReplyId)
        {
            string sql = "update SchoolCommentReplies set ReplyCount = ReplyCount + 1 where id = @SchoolCommentReplyId";
            SqlParameter[] parameters = {
                new SqlParameter("@SchoolCommentReplyId",SchoolCommentReplyId)
            };
            return ExecuteNonQuery(sql, parameters);
        }


        public SchoolCommentReply GetCommentReply(Guid replyId)
        {
            return base.GetList(q=>q.Id == replyId).Include(q => q.SchoolComment).FirstOrDefault();
        }

        public override int Add(SchoolCommentReply Entity)
        {
            return base.Add(Entity);
        }

        /// <summary>
        /// 最新点评回复
        /// </summary>
        /// <param name="CommentId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public List<SchoolCommentReply> CommentReply(Guid CommentId,int PageIndex,int PageSize,out int total)
        {
            total = base.GetList(x => x.SchoolCommentId == CommentId && x.ReplyId == null).Count();
            return base.GetList(x => x.SchoolCommentId == CommentId && x.ReplyId == null).OrderByDescending(x => x.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize)?.ToList();
        }

        /// <summary>
        /// 精选点评回复
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public List<SchoolCommentReply> SelectedCommentReply(Guid commentId)
        {
            List<SchoolCommentReply> schoolComments = new List<SchoolCommentReply>();
            int SelectedTotal = 3;

            var isTop = base.GetList(x => x.SchoolCommentId == commentId && x.IsTop == true && x.ReplyId == null).FirstOrDefault();
            if (isTop != null)
            {
                schoolComments.Add(isTop);
                SelectedTotal -= 1;
            }

            //获取校方回复
            schoolComments.AddRange(base.GetList(x => !schoolComments.Contains(x) && x.SchoolCommentId == commentId && x.PostUserRole == UserManage.Domain.Common.UserRole.School && x.ReplyId == null).Take(SelectedTotal));
            SelectedTotal = SelectedTotal - schoolComments.Count;

            //获取该点评的达人回复
            schoolComments.AddRange(base.GetList(x => !schoolComments.Contains(x) && x.SchoolCommentId == commentId && (x.PostUserRole == UserManage.Domain.Common.UserRole.PersonTalent || x.PostUserRole == UserManage.Domain.Common.UserRole.OrgTalent) && x.ReplyId == null).Take(SelectedTotal));
            SelectedTotal = SelectedTotal - schoolComments.Count;
            
            //暂无手动置顶回复且无达人回答，获取点赞+ 回复最高的回复
            if (SelectedTotal > 0)
            {
                schoolComments.AddRange(base.GetList(x => !schoolComments.Contains(x) && x.SchoolCommentId == commentId && x.ReplyId == null).OrderByDescending(x => x.LikeCount + x.ReplyCount).Take(SelectedTotal));
            }
            return schoolComments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CommentId"></param>
        /// <param name="Field">true：修改点赞次数，false：修改点评总次数</param>
        /// <returns></returns>
        public int UpdateCommentReplyLikeorReplayCount(Guid ReplayId, int operaValue, bool Field)
        {
            string sql = "";
            if (Field)
            {
                sql = "update SchoolCommentReplies set LikeCount = LikeCount + @operaValue where id = @Id";
            }
            else
            {
                sql = "update SchoolCommentReplies set ReplyCount = ReplyCount + @operaValue where id = @Id";
            }
            SqlParameter[] para = {
                new SqlParameter("@Id",ReplayId),
                new SqlParameter("@operaValue",operaValue)
            };
            return base.ExecuteNonQuery(sql, para);
        }

        public int SchoolReplyTotal(Guid SchoolId)
        {
            string sql = "select count(r.Id) as SchoolReplyTotal from SchoolComments as c left join  SchoolCommentReplies as  r on r.SchoolCommentId = c.Id where c.SchoolSectionId = @SchoolId ";
            SqlParameter[] para = {
                new SqlParameter("@SchoolId",SchoolId)
            };
            return (int)base.ExecuteScalar(sql, para);
        }

        public new List<SchoolCommentReply> GetList(Expression<Func<SchoolCommentReply, bool>> where = null)
        {
            return base.GetList(where)?.ToList();
        }
        public int CommentReplyTotalById(Guid replyId)
        {
            int total = base.GetList(x => x.ReplyId == replyId).Count();
            return total;
        }

        public List<SchoolCommentReplyExt> CommentReplyById(Guid replyId,int ordertype, int pageIndex, int pageSize)
        {
            string order = "";
            if (ordertype == 0)
            {
                order += "T.LikeCount desc,";
            }

            string sql = @"WITH T
                AS( 
                    SELECT * FROM SchoolCommentReplies WHERE Id=@replyId
                    UNION ALL 
                    SELECT a.*  
                    FROM SchoolCommentReplies a INNER JOIN T ON a.ReplyId=T.Id  
                ) 
                SELECT T.*,b.UserId as ReplyUserId,b.IsAnony as ReplyUserIdIsAnony FROM T left join SchoolCommentReplies b on T.ReplyId = b.Id
                where T.ReplyId is not null
                order by " + order + @" T.CreateTime desc offset @pageIndex rows fetch next @pageSize rows only ;";
            var param = new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@replyId",replyId),
                    new System.Data.SqlClient.SqlParameter("@pageIndex",(pageIndex-1)*pageSize),
                    new System.Data.SqlClient.SqlParameter("@pageSize",pageSize),
                  };
            return Query<SchoolCommentReplyExt>(sql, param).ToList();
        }

        public SchoolCommentReplyExt QueryCommentReply(Guid replyId)
        {
            string sql = @"
                SELECT T.*,b.UserId as ReplyUserId FROM SchoolCommentReplies T left join SchoolCommentReplies b on T.ReplyId = b.Id
                where T.Id = @replyId ;";
            var param = new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@replyId",replyId)
                  };
            return Query<SchoolCommentReplyExt>(sql, param).FirstOrDefault();
        }


        public List<SchoolCommentReplyExt> PageDialog(Guid id, List<Guid> userId, int pageIndex, int pageSize)
        {
            List<SqlParameter> para = userId.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();

            string strpara = string.Join(",", userId.Select((q, idx) => "@id" + idx));



            string sql = @"WITH T1
                AS( 
                    SELECT b1.*FROM SchoolCommentReplies b1
                                        WHERE b1.Id=@replyId
                    UNION ALL 
                    SELECT a1.*
                    FROM SchoolCommentReplies a1
                                        INNER JOIN T1 ON a1.Id=T1.ReplyId 
                                        and a1.UserId in ("+ strpara + @")
                ) ,
                            T2 AS( 
                    SELECT b1.*FROM SchoolCommentReplies b1
                                        WHERE b1.Id=@replyId
                    UNION ALL 
                    SELECT a1.*
                    FROM SchoolCommentReplies a1
                                        INNER JOIN T2 ON a1.ReplyId=T2.Id 
                                        and a1.UserId in ("+ strpara + @")
                )
                                select T.*,b.UserId as ReplyUserId,b.IsAnony as ReplyUserIdIsAnony from(
                                SELECT T1.* FROM T1
                                Union
                SELECT T2.* FROM T2) T left join SchoolCommentReplies b on T.ReplyId = b.Id
                                 order by T.CreateTime offset @offset rows fetch next @limit rows only ;";
            para.Add(new SqlParameter("@replyId", id));
            para.Add(new SqlParameter("@offset", (pageIndex-1)*pageSize));
            para.Add(new SqlParameter("@limit", pageSize));
            return Query<SchoolCommentReplyExt>(sql, para.ToArray()).ToList();
        }

        public List<SchoolCommentReplyExt> CommentReplyExtNewest(List<Guid> ReplyIds)
        {
            if (ReplyIds.Count() == 0)
                return null;

            List<SqlParameter> para = ReplyIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", ReplyIds.Select((q, idx) => "@id" + idx));

            string sql = @"select * from SchoolCommentReplies as p inner join 
                (select ReplyId,max(CreateTime) as CreateTime from SchoolCommentReplies where ReplyId in ("+ strpara + @")
	                group by ReplyId) as s on p.ReplyId = s.ReplyId and p.CreateTime = s.CreateTime";

            return Query<SchoolCommentReplyExt>(sql, para.ToArray())?.ToList();
        }

        public List<SchoolCommentReply> GetCurretUserNewestReply(int PageIndex, int PageSize, Guid UserId)
        {
            string sql = @"select
                            r.Id,
	                        r.SchoolCommentId,
	                        r.ReplyId,
	                        r.UserId,
	                        r.IsSchoolPublish,
	                        r.IsAttend,
	                        r.IsAnony,
	                        r.RumorRefuting,
	                        r.Content,
	                        r.ReplyId,
	                        r.LikeCount,
	                        r.CreateTime,
	                        r.IsTop,
	                        r.PostUserRole
                           from SchoolCommentReplies as r 
	                        left join SchoolComments as s on r.SchoolCommentId = s.Id
	                        where s.CommentUserId = @userId
                        order by r.CreateTime desc OFFSET ((@pageIndex-1) * @pageSize) ROW FETCH NEXT @pageSize rows only";

            SqlParameter[] para = {
                new SqlParameter("@userId",UserId),
                new SqlParameter("@pageIndex",PageIndex),
                new SqlParameter("@pageSize",PageSize)
            };
            return Query(sql, para)?.ToList();
        }

        public SchoolCommentReply GetTopLevel(Guid Id)
        {
            string sql = @"with temp as
	        (
		        select * from SchoolCommentReplies where Id = @Id
		        union all
		        select p.* from SchoolCommentReplies as p inner join temp on  temp.ReplyId = p.Id
	        )
	        select *　from temp where ReplyId is null";

            SqlParameter[] para = {
                new SqlParameter("@Id",Id)
            };
            return Query(sql, para).FirstOrDefault();
        }

        public int CommentReplyTotal(Guid userId)
        {
            return base.GetCount(x => x.UserId == userId && x.ReplyId == null);
        }

        public int ReplyTotal(Guid userId)
        {
            return base.GetCount(x => x.UserId == userId && x.ReplyId != null);
        }

        public List<CommentReplyAndReply> CurrentSchoolCommenReplyAndReply(Guid userId,int PageIndex,int PageSize)
        {
            string sql = @"SELECT
	                    * 
                    FROM
	                    (
	                    SELECT
		                    a.Id,
		                    a.SchoolCommentId,
		                    '00000000-0000-0000-0000-000000000000' AS ReplyId,
		                    a.UserId,
		                    a.IsAnony,
		                    a.Content,
		                    a.LikeCount,
		                    a.ReplyCount,
		                    a.CreateTime,
		                    0 AS Type 
	                    FROM
		                    SchoolCommentReplies AS a 
	                    WHERE
		                    a.ReplyId IS NULL
		                    UNION
	                    SELECT
		                    a.Id,
		                    a.SchoolCommentId,
		                    a.ReplyId,
		                    a.UserId,
		                    a.IsAnony,
		                    a.Content,
		                    a.LikeCount,
		                    a.ReplyCount,
		                    a.CreateTime,
		                    1 AS Type 
	                    FROM
		                    SchoolCommentReplies AS a 
	                    WHERE
		                    a.ReplyId IS NOT NULL
	                    ) AS a 
                    WHERE
	                    a.UserId = @userId 
                    ORDER BY
	                    a.CreateTime DESC
                            OFFSET (( @PageIndex - 1 ) * @PageSize ) ROW FETCH NEXT @PageSize ROWS ONLY
                    ";

            SqlParameter[] para = {
                new SqlParameter("@userId",userId),
                new SqlParameter("@PageIndex",PageIndex),
                new SqlParameter("@PageSize",PageSize)
            };
            return Query<CommentReplyAndReply>(sql, para)?.ToList();
        }

        public List<CommentReplyAndReply> CurrentUserLikeCommentAndReply(Guid userId, int PageIndex, int PageSize)
        {
            string sql = @"
                        select * from(
                            SELECT
		                            a.Id,
		                            a.SchoolCommentId,
		                            '00000000-0000-0000-0000-000000000000' AS ReplyId,
		                            s.UserId as LikeUser,
                                    a.UserId,
		                            a.IsAnony,
		                            a.Content,
		                            a.LikeCount,
		                            a.ReplyCount,
		                            a.CreateTime,
		                            0 AS Type 
	                            FROM
		                            SchoolCommentReplies AS a RIGHT JOIN SchoolCommentLikes as s
		                            on a.Id = s.SourceId and s.LikeType = 3 and s.LikeStatus = 1
	                            WHERE
		                            a.ReplyId IS NULL and a.Id is not null
	                            UNION
                            SELECT
	                            a.Id,
	                            a.SchoolCommentId,
	                            a.ReplyId,
	                            s.UserId as LikeUser,
                                a.UserId,
	                            a.IsAnony,
	                            a.Content,
	                            a.LikeCount,
	                            a.ReplyCount,
	                            a.CreateTime,
	                            1 AS Type 
                            FROM
	                            SchoolCommentReplies AS a 
	                            RIGHT JOIN SchoolCommentLikes as s
		                            on a.Id = s.SourceId and s.LikeType = 3 and s.LikeStatus = 1
                            WHERE
	                            a.ReplyId IS NOT NULL and a.Id is NOT NULL
	                            UNION
                            select 
	                            a.Id,
	                            a.Id as SchoolCommentId,
	                            '00000000-0000-0000-0000-000000000000' AS ReplyId,
	                            s.UserId as LikeUser,
                                a.CommentUserId as UserId,
	                            a.IsAnony,
	                            a.Content,
	                            a.LikeCount,
	                            a.ReplyCount,
	                            a.AddTime as CreateTime,
	                            2 as Type
                            from SchoolComments as a RIGHT JOIN SchoolCommentLikes as s
	                            on s.SourceId = a.Id and s.LikeType = 1 and s.LikeStatus = 1
                            where a.Id  IS NOT NULL
	                            ) as a
                        WHERE
	                    a.LikeUser = @userId 
                    ORDER BY
	                    a.CreateTime DESC
                            OFFSET (( @PageIndex - 1 ) * @PageSize ) ROW FETCH NEXT @PageSize ROWS ONLY
                        ";
                SqlParameter[] para = {
                    new SqlParameter("@userId",userId),
                    new SqlParameter("@PageIndex",PageIndex),
                    new SqlParameter("@PageSize",PageSize)
                };
                return Query<CommentReplyAndReply>(sql, para)?.ToList();
        }

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <param name="replyId"></param>
        /// <returns></returns>
        public bool UpdateViewCount(Guid replyId)
        {
            string sql = "update SchoolCommentReplies set ViewCount = ViewCount + 1 where id = @Id";
            return ExecuteNonQuery(sql, new SqlParameter[] { new SqlParameter("@Id", replyId) }) > 0;
        }

        /// <summary>
        /// 根据回复id 获取最热门回复
        /// </summary>
        /// <param name="replyId"></param>
        /// <returns></returns>
        public List<SchoolCommentReplyExt> GetSchoolCommentHottestReplies(Guid replyId) 
        {
            string sql = @"select 
	                        top 3
	                        r.*,
	                        case
		                        when r.PostUserRole = 2 then 1
		                        when r.PostUserRole = 1 then 2
		                        when r.PostUserRole = 0 then 3
		                        else 4
	                        end as Weight
	                        from SchoolCommentReplies as r
	                        where r.ParentId = @replyId
	                        order by Weight desc,(r.LikeCount + r.ReplyCount) desc";

            var param = new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@replyId",replyId)
                  };
            return Query<SchoolCommentReplyExt>(sql, param).ToList();
        }

        public int Insert(SchoolCommentReply model)
        {
            throw new NotImplementedException();
        }

        public SchoolCommentReply GetModelById(Guid Id)
        {
            return base.GetList(x=>x.Id == Id).Include(x=>x.SchoolComment).FirstOrDefault();
        }

        IEnumerable<SchoolCommentReply> IqAppService<SchoolCommentReply>.GetList(Expression<Func<SchoolCommentReply, bool>> where)
        {
            throw new NotImplementedException();
        }

        public bool isExists(Expression<Func<SchoolCommentReply, bool>> where)
        {
            throw new NotImplementedException();
        }
         
        public List<ParentReply> GetParentReplyByIds(List<Guid> Ids)
        {
            List<SqlParameter> para = Ids.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", Ids.Select((q, idx) => "@id" + idx));

            return Query<ParentReply>(@"select 
					c.Content as CommentContent,
					c.CommentUserId as CommentUserId,
					c.Id as CommentId,
					c.IsAnony as CommentIsAnony,
                    c.SchoolSectionId as SchoolSectionId,
					p.Content as ParentReplyContent,
					p.UserId as ParentUserId,
					p.Id as ParentId,
					p.IsAnony as ParentIsAnony,
					r.Content as ReplyContent,
					r.UserId as ReplyUserId,
                    r.ReplyCount as ReplyCount,
					r.Id as ReplyId,
					r.IsAnony as ReplyIsAnony,
					(case when p.id is null then 1 else 0 end) as Type
				from SchoolCommentReplies as r
					left join SchoolCommentReplies as p on r.ParentId = p.Id
					left join SchoolComments as c on r.SchoolCommentId = c.Id
					where r.Id in (" + strpara + ")", para.ToArray()).ToList();
        }
    }
}
