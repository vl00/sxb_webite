using ImportAndExport.Entity;
using ImportAndExport.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ImportAndExport.Repository
{
    public class CommentRepository
    {
        private DataDbContext _context;

        private SchoolRepository _schoolRepository;

        public CommentRepository(DataDbContext context, SchoolRepository schooslRepository)
        {
            _context = context;
            _schoolRepository = schooslRepository;
        }

        public SchoolComment GetCommentByPhoneContent(string phone,string content) 
        {
            string sql = @"
                    select cm.* from PartTimeJobAdmins as pa
	                    left join SchoolComments as cm on cm.CommentUserId = pa.Id
	                    where pa.Phone like @phone and cm.Content like '%"+content+"%'";

            return _context.Query<SchoolComment>(sql, new { phone }).FirstOrDefault();
        }

        public Guid GetUserIdByPhone(string phone) 
        {
            string serach = "select Id from PartTimeJobAdmins where Phone like '"+phone+"'";
            return _context.Query<SchoolComment>(serach, new { phone }).FirstOrDefault().Id;
        }

        public List<SchoolComment> GetSchoolCommentByUserId(List<Guid> userId) 
        {
            string sql = "select* from SchoolComments where CommentUserId in @userId";
            return _context.Query<SchoolComment>(sql, new { userId }).ToList();
        }

        public void UpdateStatus(ChangeStatusRez r) 
        {
            string update = "update SchoolComments set State = @State where Id = @Id";
            _context.Execute(update, new { State = r.Stauts,Id = r.Id });
        }


        public List<SchoolComScore> GetSchoolComScores(int PageIndex) 
        {
            string sql = @"select 
	                        com.SchoolSectionId,
	                        max(com.SchoolId) as SchoolId,
	                        avg(score.AggScore) as AggScore,
	                        count(com.Id) as CommentCount,
	                        sum(case when score.IsAttend = 1 then 1 else 0 end) as AttendCommentCount,
	                        avg(score.TeachScore) as TeachScore,
	                        avg(score.HardScore) as HardScore,
	                        avg(score.EnvirScore) as EnvirScore,
	                        avg(score.ManageScore) as ManageScore,
	                        avg(score.LifeScore) as LifeScore,
	                        max(com.AddTime) as UpdateTime,
	                        max(com.AddTime) as LastCommentTime
                        from SchoolComments as com
	                        inner join SchoolCommentScores as score on score.CommentId = com.Id
                            left join SchoolScoresTemp as temp on temp.SchoolSectionId = com.SchoolSectionId
	                        where com.AddTime <='2019-12-29 11:00'  and temp.SchoolSectionId is null
	                        GROUP BY com.SchoolSectionId
			                        order by count(1) desc 
	                        OFFSET (@PageIndex - 1) * 2000 ROWS FETCH NEXT 2000 ROWS ONLY";

            return _context.Query<SchoolComScore>(sql, new { PageIndex })?.ToList();
        }


        public List<SchoolComScore> GetQuestionSchoolComScores(int PageIndex) 
        {
            string sql = @"select 
	                        q.SchoolSectionId,
                            max(q.SchoolId) as SchoolId,
	                        count(q.Id) as QuestionCount,
	                        MAX(q.CreateTime) as LastQuestionTime,
	                        case
		                        when max(t.SchoolSectionId) is null then 0
		                        else 1
	                        end as IsExist
                        from QuestionInfos as q
	                        left join SchoolScoresTemp as t on q.SchoolSectionId = t.SchoolSectionId
		                        where q.CreateTime <= '2019-12-29 11:00'
	                        GROUP BY q.SchoolSectionId
                        order by count(q.Id) desc 
                         OFFSET (@PageIndex - 1) * 2000 ROWS FETCH NEXT 2000 ROWS ONLY";
            return _context.Query<SchoolComScore>(sql, new { PageIndex })?.ToList();
        }

        public bool UpdateSchoolScore(SchoolComScore scores) 
        {

            string sql = "update SchoolScoresTemp set QuestionCount = @QuestionCount,LastQuestionTime = @LastQuestionTime where SchoolSectionId = @SchoolSectionId";
            return  _context.Execute(sql, new { scores.QuestionCount, scores.LastQuestionTime, scores.SchoolSectionId }) > 0;        
        }

        public bool PushSchoolCommentSocre(List<SchoolComScore> schoolComScores) 
        {
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    //schoolComScores.ForEach(x => { x.CreateTime = x.LastCommentTime;x.LastQuestionTime = DateTime.Parse("1/1/1753 12:00:00"); });
                    string sql = "insert into schoolScoresTemp(SchoolSectionId,SchoolId,AggScore,commentcount,AttendCommentCount,TeachScore,HardScore,EnvirScore,ManageScore,LifeScore,CreateTime,UpdateTime,LastCommentTime,QuestionCount,LastQuestionTime) values(@SchoolSectionId,@SchoolId,@AggScore,@commentcount,@AttendCommentCount,@TeachScore,@HardScore,@EnvirScore,@ManageScore,@LifeScore,@CreateTime,@UpdateTime,@LastCommentTime,@QuestionCount,@LastQuestionTime)";
                    int ExecuteTotal = _context.Execute(sql, schoolComScores);
                    if(schoolComScores.Count() == ExecuteTotal) 
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
                finally
                {
                    _context.Dispose();
                }
            }
            return false;
        }

        //public SchoolComment GetCommentByPhoneContent(string phone,string content) 
        //{
        //    string sql = @"select s.* from SchoolComments as s where s.Content = @content";
        //    return _context.Query<SchoolComment>(sql, new { content }).FirstOrDefault();
        //}

        public List<QuestionAnswerTotal> UpdateQuestionAnswerTotal(int PageIndex) 
        {
            string sql = @"select QuestionInfoId,
			                 count(1) as AnswerTotal
			                 from ImportQuestionsAnswersInfos 
	                GROUP BY QuestionInfoId 
		                ORDER BY QuestionInfoId 
	                OFFSET (@PageIndex - 1) * 2000 ROWS FETCH NEXT 2000 ROWS ONLY ";

            return _context.Query<QuestionAnswerTotal>(sql, new { PageIndex }).ToList();
        }

        public int UpdateCommentState(SchoolComment cm) 
        {
            if (cm == null) 
            {
                return 0;
            }
            string sql = "update SchoolComments set State = 3 where Id = @Id";
            return _context.Execute(sql, new { Id = cm.Id });
        }

        public SchoolComment Test()
        {
            string sql = "select top 1 * from SchoolComments";
            return _context.Query<SchoolComment>(sql, new { }).FirstOrDefault();
        }

        public int GetCommentSelected(Guid userId) 
        {
            string sql = "select count(1) as SelectedTotal from SchoolComments where State = 3 and AddTime >= '2019-12-11 00:00:00' and CommentUserId = @UserId";
            return _context.Query<SelectedTotals>(sql, new { UserId = userId }).FirstOrDefault().SelectedTotal;
        }

        public int GetAnswerSelected(Guid userId) 
        {
            string sql = "select count(1) as SelectedTotal from QuestionsAnswersInfos where State = 3 and CreateTime >= '2019-12-11 00:00:00' and UserId = @UserId";
            return _context.Query<SelectedTotals>(sql, new { UserId = userId }).FirstOrDefault().SelectedTotal;
        }



        public int JobSelectedTotal() 
        {
            string sql = @"select count(1) as SelectedTotal from SchoolComments as s
	                        RIGHT JOIN PartTimeJobAdminRoles as r on s.CommentUserId = r.AdminId and r.Shield = 0 and r.Role = 1
                        where State = 3 ";

            return _context.Query<SelectedTotals>(sql, new { }).FirstOrDefault().SelectedTotal; ;
        }

        public List<SchoolComment> GetSelectedComment(int PageIndex) 
        {
            string sql = @"select s.* from SchoolComments as s
	                        RIGHT JOIN PartTimeJobAdminRoles as r on s.CommentUserId = r.AdminId and r.Shield = 0 and r.Role = 1
                        where State = 3 
                        order by AddTime
                        OFFSET @PageIndex * 1000 ROWS FETCH NEXT 1000 ROWS ONLY";

            return _context.Query<SchoolComment>(sql, new { PageIndex })?.ToList();
        }

        public void InsertChangeRecord(List<JobStateChangeRecord> jobStateChanges) 
        {
            string sql = "insert into JobStateChangeRecord(Type,DataSourceId) values(@Type,@DataSourceId)";
            _context.Execute(sql, jobStateChanges);
        }

        public void UpdateState(List<SchoolComment> schoolComments) 
        {
            string sql = "update SchoolComments set State = 4 where Id in @Ids";
            _context.Execute(sql, new { Ids = schoolComments.Select(x => x.Id).ToList() });
        }

        //public int Inserts(List<SchoolComment> comments) 
        //{
        //    string sql = "";
        //}

        /// <summary>
        /// 多表操作--事务
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="databaseOption"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public Tuple<bool, string> ExecuteTransaction(List<SchoolComment> comment)
        {
            if (!comment.Any()) 
            {
                return new Tuple<bool, string>(true, "暂无数据入库");
            }


            //开启事务
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    List<SchoolCommentScore> score = comment.Select(x => x.CommentScore).ToList();

                    string addComment = @"insert into ImportSchoolComments(Id,SchoolId,SchoolSectionId,CommentUserId,Content,State,IsTop,ReplyCount,LikeCount,IsSettlement,IsAnony,RumorRefuting,AddTime,IsHaveImagers,PostUserRole,UserInfoExId,IsImport,ImportTime,ImportType) values(@Id,@SchoolId,@SchoolSectionId,@CommentUserId,@Content,0,0,0,0,0,0,0,@AddTime,0,@PostUserRole,null,0,getdate(),@ImportType)";
                    string addScore = @"insert into ImportSchoolCommentScores(Id,CommentId,IsAttend,AggScore,TeachScore,HardScore,EnvirScore,ManageScore,LifeScore) values(@Id,@CommentId,@IsAttend,@AggScore,@TeachScore,@HardScore,@EnvirScore,@ManageScore,@LifeScore)";

                    int successCommentTotal = _context.Execute(addComment, comment, transaction);
                    int successScoreTotal = _context.Execute(addScore, score, transaction);

                    if (successCommentTotal == successScoreTotal)
                    {
                        transaction.Commit();
                        //更新用户池使用次数
                        _schoolRepository.Updateuserinfotmpstate(comment.GroupBy(x => x.CommentUserId).Select(x => x.Key).ToList());
                        return new Tuple<bool, string>(true, "完成");
                    }
                    else
                    {
                        transaction.Rollback();
                        return new Tuple<bool, string>(false, "插入途中错误");
                    }
                }
                catch (Exception ex)
                {
                    //todo:!!!transaction rollback can not work.

                    //回滚事务
                    transaction.Rollback();
                    return new Tuple<bool, string>(false, ex.ToString());
                }
                finally
                {
                    _context.Dispose();
                }
            }
        }

        public Tuple<bool, string> AnswerExecuteTransaction(List<QuestionsAnswersInfo> answers)
        {
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    string addAnswer = @"insert into ImportQuestionsAnswersInfos(Id,QuestionInfoId,State,UserId,IsSchoolPublish,IsAttend,IsAnony,Content,LikeCount,IsSettlement,CreateTime,IsTop,ParentId,ReplyCount,PostUserRole,UserInfoExId,FirstParentId,IsImport,ImportTime,ImportType,IsContrast)
                            values(@Id,@QuestionInfoId,@State,@UserId,@IsSchoolPublish,@IsAttend,@IsAnony,@Content,@LikeCount,@IsSettlement,@CreateTime,@IsTop,null,@ReplyCount,@PostUserRole,null,null,0,getdate(),@ImportType,@IsContrast)";

                    int answerSuccess = _context.Execute(addAnswer, answers, transaction);
                    if (answers.Count() == answerSuccess)
                    {
                        transaction.Commit();

                        List<Guid> userIds = answers.Select(x => x.UserId).ToList();

                        //更新用户池使用次数
                        _schoolRepository.Updateuserinfotmpstate(userIds);
                        return new Tuple<bool, string>(true, "完成");
                    }
                    else
                    {
                        transaction.Rollback();
                        return new Tuple<bool, string>(false, "插入途中错误");
                    }
                }
                catch (Exception ex)
                {
                    //todo:!!!transaction rollback can not work.

                    //回滚事务
                    transaction.Rollback();
                    return new Tuple<bool, string>(false, ex.ToString());
                }
                finally
                {
                    _context.Dispose();
                }
            }
        }

        /// <summary>
        /// 多表操作--事务
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="databaseOption"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public Tuple<bool, string> QAExecuteTransaction(List<QuestionInfo> questions)
        {
            //开启事务
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    List<QuestionsAnswersInfo> answers = questions.Where(x=>x.answer!=null).SelectMany(x => x.answer).Select(x => x).ToList();

                    string addQuestion = @"insert into ImportQuestionInfos(Id,State,SchoolId,SchoolSectionId,UserId,Content,IsAnony,CreateTime,LikeCount,IsHaveImagers,IsTop,ReplyCount,PostUserRole,UserInfoExId,ImportTime,IsImport,ImportType,IsContrast) 
                                                                    values(@Id,@State,@SchoolId,@SchoolSectionId,@UserId,@Content,@IsAnony,@CreateTime,@LikeCount,@IsHaveImagers,@IsTop,@ReplyCount,@PostUserRole,null,getdate(),0,@ImportType,@IsContrast)";

                    string addAnswer = @"insert into ImportQuestionsAnswersInfos(Id,QuestionInfoId,State,UserId,IsSchoolPublish,IsAttend,IsAnony,Content,LikeCount,IsSettlement,CreateTime,IsTop,ParentId,ReplyCount,PostUserRole,UserInfoExId,FirstParentId,IsImport,ImportTime,ImportType,IsContrast)
                            values(@Id,@QuestionInfoId,@State,@UserId,@IsSchoolPublish,@IsAttend,@IsAnony,@Content,@LikeCount,@IsSettlement,@CreateTime,@IsTop,null,@ReplyCount,@PostUserRole,null,null,0,getdate(),@ImportType,@IsContrast)";

                    int successQuestionsTotal = _context.Execute(addQuestion, questions, transaction);
                    int successAnswerTotal = _context.Execute(addAnswer, answers, transaction);

                    if ((successQuestionsTotal == questions.Count()) == (successAnswerTotal == answers.Count()))
                    {
                        transaction.Commit();

                        List<Guid> userIds = new List<Guid>();
                        userIds.AddRange(questions.Select(x => x.UserId).ToList());
                        userIds.AddRange(questions.SelectMany(x => x.answer).Select(x => x.UserId).ToList());

                        //更新用户池使用次数
                        _schoolRepository.Updateuserinfotmpstate(userIds);
                        return new Tuple<bool, string>(true, "完成");
                    }
                    else
                    {
                        transaction.Rollback();
                        return new Tuple<bool, string>(false, "插入途中错误");
                    }
                }
                catch (Exception ex)
                {
                    //todo:!!!transaction rollback can not work.

                    //回滚事务
                    transaction.Rollback();
                    return new Tuple<bool, string>(false, ex.ToString());
                }
                finally
                {
                    _context.Dispose();
                }
            }
        }

        public List<SchoolComment> QueryImportComments(DateTime start, DateTime end)
        {
            string sql = @"select * from ImportSchoolComments where 
	            AddTime >= @start and AddTime <= @end and IsImport = 0";

            var comments = _context.Query<SchoolComment>(sql, new { start, end }).ToList();
            if (comments.Any())
            {
                List<Guid> commentIds = comments.Select(x => x.Id).ToList();
                sql = "select * from ImportSchoolCommentScores where CommentId in @commentIds";
                var scores = _context.Query<SchoolCommentScore>(sql, new { commentIds });

                comments.ForEach(x => {
                    var score = scores.Where(s => s.CommentId == x.Id).FirstOrDefault();
                    x.CommentScore = score;
                });
            }
            return comments;
        }

        public List<QuestionInfo> QueryImportQuestion(DateTime start, DateTime end)
        {
            string sql = @"select * from ImportQuestionInfos where
	            CreateTime >= @start and CreateTime <= @end and IsImport = 0";

            return _context.Query<QuestionInfo>(sql, new { start, end })?.ToList();
        }

        public List<QuestionsAnswersInfo> QueryImportAnswer(DateTime start, DateTime end)
        {
            string sql = @"select * from ImportQuestionsAnswersInfos where
                CreateTime >= @start and CreateTime <= @end and IsImport = 0";

            return _context.Query<QuestionsAnswersInfo>(sql, new { start, end })?.ToList();
        }




    }
}