using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class SchoolCommentScoreRepository : EntityFrameworkRepository<SchoolCommentScore>,  ISchoolCommentScoreRepository
    {
        private readonly ISchoolScoreRepository schoolScoreRepository;

        public SchoolCommentScoreRepository(CommentsManageDbContext dbContext,
             ISchoolScoreRepository schoolScoreRepo):base(dbContext)
        {
            schoolScoreRepository = schoolScoreRepo;
        }

        /// <summary>
        /// 根据学校id获取学校数据
        /// </summary>
        /// <param name="SchoolIds"></param>
        /// <returns></returns>
        public List<SchoolScore> SchoolScoreOrder(List<Guid> SchoolIds)
        {
            List<SqlParameter> para = new List<SqlParameter>();
            string sql = "select * from SchoolScores where SchoolId in ( ";
            for (int i = 0; i < SchoolIds.Count; i++)
            {
                sql += "@" + i + ",";
                para.Add(new SqlParameter("@" + i, SchoolIds[i]));
            }

            sql = sql.Remove(sql.Length - 1, 1);
            sql += ") ";
            return Query<SchoolScore>(sql, para.ToArray())?.ToList();
        }

        public int AddSchoolComment(SchoolCommentScore commentScore)
        {
           return base.Add(commentScore);
        }

        public decimal GetAvgScoreBybaraBranchSchool(Guid SchooId)
        {
            string sql = "select s.AggScore from SchoolCommentScores as s left join  SchoolComments as c on s.CommentId = c.Id where c.SchoolSectionId = @SchoolId ";
            SqlParameter[] para = {
                new SqlParameter("@SchoolId",SchooId)
            };
            object obj = ExecuteScalar(sql, para);
            if (obj != null)
                return (decimal)obj;
            else
                return 0;
        }

        public bool UpdateSchoolQuestionTotal(SchoolScore schoolScore)
        {
            string sql = @"update [dbo].[SchoolScores] Set 
             QuestionCount = @questionCount,
             LastQuestionTime = @lastQuestionTime
             where SchoolId = @schoolId and SchoolSectionId = @schoolSectionId and LastQuestionTime < @lastQuestionTime;";

            SqlParameter[] para = {
                new SqlParameter("@schoolSectionId",schoolScore.SchoolSectionId),
                new SqlParameter("@schoolId",schoolScore.SchoolId),
                new SqlParameter("@lastQuestionTime",schoolScore.LastQuestionTime),
                new SqlParameter("@questionCount",schoolScore.QuestionCount)
            };
            return ExecuteNonQuery(sql, para) > 0;
        }

        public List<SchoolCommentScoreTotal> PageSchoolCommentScore(List<Guid> schoolIds)
        {

            List<SqlParameter> para= schoolIds.Select((q,idx)=>new SqlParameter("@id" + idx, q)).ToList();

            string strpara = string.Join(",", schoolIds.Select((q, idx) => "@id" + idx));
            string where = "";
            if (schoolIds.Count > 0)
            {
                where += " and  sc.SchoolSectionId in (" + strpara + ") ";
            }


            string sql = @"select SchoolId,SchoolSectionId,AVG(AggScore) as AggScore  from SchoolComments sc 
            inner join SchoolCommentScores scs on scs.CommentId = sc.Id where 1=1  " + where + @"
            GROUP BY SchoolId,SchoolSectionId;";

            var result = Query<SchoolCommentScoreTotal>(sql, para.ToArray());

            return result==null?new List<SchoolCommentScoreTotal>() : result.ToList();
        }

        public SchoolCommentScoreTotal GetSchoolCommentScore(Guid SchooId, Guid SchoolSectionIdd)
        {
            string sql = @"select school.SchoolId,school.SchoolSectionId,
            scores1.AggScore,scores2.TeachScore,scores2.ManageScore,scores2.LifeScore,
            scores2.HardScore,scores2.EnvirScore
            from 
            (select @schoolSectionId as SchoolSectionId,@schoolId as SchoolId
            ) as school
            left join
            (select SchoolComments.SchoolId,SchoolComments.SchoolSectionId,AVG(AggScore) as AggScore
                    from SchoolComments
                    inner join SchoolCommentScores scs on scs.CommentId = SchoolComments.Id
                    where SchoolComments.SchoolSectionId =@schoolSectionId
                    GROUP BY SchoolComments.SchoolId,SchoolComments.SchoolSectionId) as scores1 on scores1.SchoolSectionId = school.SchoolSectionId
            left join
            (select SchoolComments.SchoolId,SchoolComments.SchoolSectionId,
                    AVG(scs.TeachScore) TeachScore,AVG(scs.ManageScore) ManageScore,AVG(scs.LifeScore) LifeScore,
                    AVG(scs.HardScore) HardScore,AVG(scs.EnvirScore) EnvirScore
                    from
                    SchoolComments
                    left join SchoolCommentScores scs on CommentId = SchoolComments.Id and scs.IsAttend = 1 
                    where SchoolComments.SchoolSectionId =@schoolSectionId
                    GROUP BY SchoolComments.SchoolId,SchoolComments.SchoolSectionId) as scores2 on scores2.SchoolSectionId = school.SchoolSectionId
            ;";
            SqlParameter[] para = {
                new SqlParameter("@schoolSectionId",SchoolSectionIdd),
                new SqlParameter("@schoolId",SchooId)
            };
            var result = Query<SchoolCommentScoreTotal>(sql, para).FirstOrDefault();

            return result;
        }

        public DateTime GetLastUpdateTime()
        {
            var result = schoolScoreRepository.GetList().OrderByDescending(q => q.LastCommentTime).Take(1).ToList();
            return result.Count > 0 ? result[0].LastCommentTime : DateTime.MinValue;
        }

        public DateTime GetQuestionLastUpdateTime()
        {
            var result = schoolScoreRepository.GetList().OrderByDescending(q => q.LastQuestionTime).Take(1).ToList();
            if(result.Count > 0)
            {
                if(result[0].LastQuestionTime != default(DateTime))
                {
                    return result[0].LastQuestionTime;
                }
                else
                {
                    return new DateTime(2000,1,1);
                }
            }
            else
            {
                return new DateTime(2000, 1, 1);
            }
        }

        public List<SchoolCommentScore> PageSchoolCommentScoreByTime(DateTime startTime, DateTime endTime, int pageNo, int pageSize)
        {
            var result = base.GetList(q => q.SchoolComment.AddTime > startTime && q.SchoolComment.AddTime <= endTime)
                                        .OrderBy(q=> q.SchoolComment.AddTime)
                                        .Skip(--pageNo * pageSize).Take(pageSize).ToList();
            return result;
        }

        public bool AddSchoolScore(SchoolScore schoolScore)
        {
            return schoolScoreRepository.Insert(schoolScore)>0;
        }

        public SchoolScore GetSchoolScore(Guid SchoolSectionId,Guid SchoolId) 
        {
            return schoolScoreRepository.GetList(x => x.SchoolSectionId == SchoolSectionId && x.SchoolId == SchoolId).FirstOrDefault();
        }

        public bool UpdateQuestionTotal(SchoolScore schoolScore)
        {
            string sql = "update [dbo].[SchoolScores] Set  QuestionCount = QuestionCount + @QuestionCount,LastQuestionTime = @LastQuestionTime where  SchoolId = @schoolId and SchoolSectionId = @schoolSectionId; ";
            SqlParameter[] para = {
                new SqlParameter("@schoolSectionId",schoolScore.SchoolSectionId),
                new SqlParameter("@schoolId",schoolScore.SchoolId),
                new SqlParameter("@QuestionCount",schoolScore.QuestionCount),
                new SqlParameter("@LastQuestionTime",schoolScore.LastQuestionTime)
            };
            return schoolScoreRepository.ExecuteNonQuery(sql, para) > 0;
        }

        public bool UpdataQuestionTotalNewTime(Guid SchoolSectionId, DateTime AddTime) 
        {
            string sql = "update SchoolScores set QuestionCount += 1,LastQuestionTime = @AddTime where SchoolSectionId = @SchoolSectionId";
            return schoolScoreRepository.ExecuteNonQuery(sql, new SqlParameter[] { 
                new SqlParameter("@AddTime",AddTime),
                new SqlParameter("@SchoolSectionId",SchoolSectionId)
            }) > 0;
        }

        public bool UpdateSchoolScore(SchoolScore schoolScore)
        {
            string sql = @"update [dbo].[SchoolScores] Set 
            AggScore = (AggScore * CommentCount + @aggScore) / (CommentCount +@commentCount),
            CommentCount = CommentCount + @commentCount,
            TeachScore = CASE WHEN (AttendCommentCount + @attendCount)<=0 THEN 0
						ELSE (TeachScore * AttendCommentCount + @teachScore)/(AttendCommentCount + @attendCount) END, 
            HardScore = CASE WHEN (AttendCommentCount + @attendCount)<=0 THEN 0
						ELSE (HardScore * AttendCommentCount + @hardScore)/(AttendCommentCount + @attendCount) END, 
            EnvirScore = CASE WHEN (AttendCommentCount + @attendCount)<=0 THEN 0
						ELSE (EnvirScore * AttendCommentCount + @envirScore)/(AttendCommentCount + @attendCount) END, 
            ManageScore = CASE WHEN (AttendCommentCount + @attendCount)<=0 THEN 0
						ELSE (ManageScore * AttendCommentCount + @manageScore)/(AttendCommentCount + @attendCount) END, 
            LifeScore = CASE WHEN (AttendCommentCount + @attendCount)<=0 THEN 0
						ELSE (LifeScore * AttendCommentCount + @lifeScore)/(AttendCommentCount + @attendCount) END,
            AttendCommentCount = AttendCommentCount + @attendCount,
            LastCommentTime = @lastCommentTime,
            UpdateTime = GETDATE()
            where SchoolId = @schoolId and SchoolSectionId = @schoolSectionId;";

            SqlParameter[] para = {
                new SqlParameter("@schoolSectionId",schoolScore.SchoolSectionId),
                new SqlParameter("@schoolId",schoolScore.SchoolId),
                new SqlParameter("@commentCount",schoolScore.CommentCount),
                new SqlParameter("@aggScore",schoolScore.AggScore),
                new SqlParameter("@teachScore",schoolScore.TeachScore),
                new SqlParameter("@hardScore",schoolScore.HardScore),
                new SqlParameter("@envirScore",schoolScore.EnvirScore),
                new SqlParameter("@manageScore",schoolScore.ManageScore),
                new SqlParameter("@lifeScore",schoolScore.LifeScore),
                new SqlParameter("@attendCount",schoolScore.AttendCommentCount),
                new SqlParameter("@lastCommentTime",schoolScore.LastCommentTime)
            };
            return schoolScoreRepository.ExecuteNonQuery(sql, para) > 0;
        }

        public bool IsExistSchoolScore(Guid schoolSectionId)
        {
            return schoolScoreRepository.GetList(q => q.SchoolSectionId == schoolSectionId).Any();
        }

        public int SchoolCommentScoreCountByTime(DateTime startTime, DateTime endTime)
        {
            var result = base.GetList(q => q.SchoolComment.AddTime > startTime && q.SchoolComment.AddTime <= endTime)
                                        .Count();
            return result;
        }

        /// <summary>
        /// 根据学校分部id
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        public List<SchoolScore> GetSchoolScoreBySchool(List<Guid> SchoolId)
        {
            return schoolScoreRepository.GetList(x => SchoolId.Contains(x.SchoolSectionId))?.ToList();
        }

        public SchoolScore GetSchoolScoreById(Guid SchoolSectionId)
        {
            return schoolScoreRepository.GetModelById(SchoolSectionId);
        }

        public SchoolScore GetSchoolScoreBySchoolId(Guid SchoolId)
        {
            string sql = @"
                    SELECT
	                    SchoolId,
                    CASE
		                    WHEN SUM ( CommentCount ) <= 0 THEN 0
		                    ELSE SUM ( AggScore * CommentCount ) / SUM ( CommentCount )
	                    END AS AggScore,
	                    SUM ( CommentCount ) AS CommentCount,
                    CASE
		
		                    WHEN SUM ( AttendCommentCount ) <= 0 THEN
		                    0 ELSE SUM ( TeachScore * AttendCommentCount ) / SUM ( AttendCommentCount ) 
	                    END AS TeachScore,
                    CASE
		                    WHEN SUM ( AttendCommentCount ) <= 0 THEN
		                    0 ELSE SUM ( EnvirScore * AttendCommentCount ) / SUM ( AttendCommentCount ) 
	                    END AS EnvirScore,
                    CASE
		
		                    WHEN SUM ( AttendCommentCount ) <= 0 THEN
		                    0 ELSE SUM ( LifeScore * AttendCommentCount ) / SUM ( AttendCommentCount ) 
	                    END AS LifeScore,
                    CASE
		                    WHEN SUM ( AttendCommentCount ) <= 0 THEN
		                    0 ELSE SUM ( HardScore * AttendCommentCount ) / SUM ( AttendCommentCount ) 
	                    END AS HardScore,
                    CASE
		                    WHEN SUM ( AttendCommentCount ) <= 0 THEN
		                    0 ELSE SUM ( ManageScore * AttendCommentCount ) / SUM ( AttendCommentCount ) 
	                    END AS ManageScore,
	                    SUM ( AttendCommentCount ) AS AttendCommentCount 
                    FROM
	                    SchoolScores 
                    WHERE
	                    SchoolId = @schoolId
                    GROUP BY
	                    SchoolId
	                    ";
            SqlParameter[] para = {
                new SqlParameter("@schoolId",SchoolId)
            };
            return schoolScoreRepository.Query(sql, para).FirstOrDefault();
        }

        public List<SchoolScore> ListNewSchoolScores(DateTime oldTime)
        {
            return schoolScoreRepository.GetList(q=>q.LastCommentTime > oldTime).ToList();
        }

        public List<SchoolScore> ListNewQuestion(DateTime oldTime)
        {
            return schoolScoreRepository.GetList(q => q.LastQuestionTime > oldTime).ToList();
        }

        public List<SchoolCommentScore> GetSchoolScoreByCommentIds(List<Guid> Ids)
        {
            return base.GetList(x => Ids.Contains(x.CommentId))?.ToList();
        }
    }
}
