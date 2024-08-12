using Hangfire.ConsoleWeb.Entitys;
using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Repository
{
    public class CommentQueryRepository
    {
        private DataDbContext _context;

        public CommentQueryRepository(DataDbContext context)
        {
            _context = context;
        }

        public void DeleteCreate() 
        {
            string sql = @"
                            delete from ImportSchoolComments where Id in (
	                            select c.Id from ImportSchoolComments as c
		                            left join [iSchoolData].[dbo].[OnlineSchoolExtension] as ext on c.SchoolSectionId = ext.id
		                            where c.ImportType = 4 and c.IsImport = 0  and ext.IsValid = 0
                            );
                            delete from ImportSchoolCommentScores where CommentId in (
	                            select c.Id from ImportSchoolComments as c
		                            left join [iSchoolData].[dbo].[OnlineSchoolExtension] as ext on c.SchoolSectionId = ext.id
		                            where c.ImportType = 4 and c.IsImport = 0  and ext.IsValid = 0
                            )
                            delete from ImportQuestionInfos where Id in (
                            select q.Id from ImportQuestionInfos as q
	                            left join [iSchoolData].[dbo].[OnlineSchoolExtension] as ext on q.SchoolSectionId = ext.id
	                            where q.ImportType = 4 and q.IsImport = 0 and ext.IsValid = 0
                            );
                            delete from ImportQuestionsAnswersInfos where Id in (
                            select a.Id from ImportQuestionsAnswersInfos as a
	                            right join ImportQuestionInfos as q on a.QuestionInfoId = q.Id
	                            left join [iSchoolData].[dbo].[OnlineSchoolExtension] as ext on q.SchoolSectionId = ext.id
	                            where q.ImportType = 4 and q.IsImport = 0 and ext.IsValid = 0
                            );";

            _context.Execute(sql,new { });
        }

        public List<SchoolComment> QueryImportComments(int pageIndex,int pageSize) 
        {
            string sql = @"select * from ImportSchoolComments where 
	                AddTime <= getdate() and IsImport = 0
		                order by AddTime
	                OFFSET (@pageIndex - 1)* @pageSize ROWS  
                    FETCH NEXT @pageSize ROWS ONLY";

            var comments = _context.Query<SchoolComment>(sql,new { pageIndex , pageSize }).ToList();
            if (comments.Any()) 
            {
                List<Guid> commentIds = comments.Select(x => x.Id).ToList();
                sql = "select * from ImportSchoolCommentScores where CommentId in @commentIds";
                var scores = _context.Query<SchoolCommentScore>(sql, new { commentIds });

                comments.ForEach(x => {
                    var score = scores.Where(s => s.CommentId == x.Id).FirstOrDefault();
                    x.SchoolCommentScore = score;
                });
            }
            return comments;
        }   

        public List<QuestionInfo> QueryImportQuestion(int pageIndex, int pageSize) 
        {
            string sql = @"select * from ImportQuestionInfos where 
	                        CreateTime <= getdate() and IsImport = 0
		                    order by CreateTime
	                    OFFSET (@pageIndex-1)* @pageSize ROWS  
                        FETCH NEXT @pageSize ROWS ONLY";

            return _context.Query<QuestionInfo>(sql,new { pageIndex, pageSize })?.ToList();
        }

        public List<QuestionsAnswersInfo> QueryImportAnswer(int pageIndex, int pageSize) 
        {
            string sql = @"select * from ImportQuestionsAnswersInfos where 
	                                    CreateTime <= getdate() and IsImport = 0
		                    order by CreateTime asc
	                    OFFSET (@pageIndex-1)* @pageSize ROWS  
                        FETCH NEXT @pageSize ROWS ONLY";

            return _context.Query<QuestionsAnswersInfo>(sql,new { pageIndex , pageSize })?.ToList();
        }

         public int GetStatisticsComment() 
         {
            string sql = "select count(Id) as Count from ImportSchoolComments where IsImport = 0 and AddTime <= getdate()";
            return _context.Query<Total>(sql, new { }).FirstOrDefault().Count;
         }

        public int GetStatisticsQuestion() 
        {
            string sql = "select count(Id) as Count from ImportQuestionInfos where IsImport = 0 and CreateTime <= getdate()";
            return _context.Query<Total>(sql, new { }).FirstOrDefault().Count;
        }

        public int GetStatisticsAnswer()
        {
            string sql = "select count(Id) as Count from ImportQuestionsAnswersInfos where IsImport = 0 and CreateTime <= getdate()";
            return _context.Query<Total>(sql, new { }).FirstOrDefault().Count;
        }


    }
}
