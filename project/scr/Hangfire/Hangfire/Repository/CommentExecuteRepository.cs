using PMS.CommentsManage.Domain.Entities;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Repository
{
    public class CommentExecuteRepository
    {
        private DataDbContext _context;

        private DataDbContext _updateContext;

        public CommentExecuteRepository(DataDbContext context,DataDbContext updateContext)
        {
            _context = context;
            _updateContext = updateContext;
        }

        /// <summary>
        /// 多表操作--事务
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="databaseOption"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public Tuple<bool, string> ExecuteTransaction(List<SchoolComment> comment)
        {
            //开启事务
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    List<SchoolCommentScore> score = comment.Select(x => x.SchoolCommentScore).ToList();

                    string addComment = @"insert into SchoolComments(Id,SchoolId,SchoolSectionId,CommentUserId,Content,State,IsTop,ReplyCount,LikeCount,IsSettlement,IsAnony,RumorRefuting,AddTime,IsHaveImagers,PostUserRole,UserInfoExId) values(@Id,@SchoolId,@SchoolSectionId,@CommentUserId,@Content,0,0,0,0,0,0,0,@AddTime,0,@PostUserRole,null)";
                    string addScore = @"insert into SchoolCommentScores(Id,CommentId,IsAttend,AggScore,TeachScore,HardScore,EnvirScore,ManageScore,LifeScore) values(@Id,@CommentId,@IsAttend,@AggScore,@TeachScore,@HardScore,@EnvirScore,@ManageScore,@LifeScore)";

                    int successCommentTotal = _context.Execute(addComment, comment, transaction);
                    int successScoreTotal = _context.Execute(addScore, score, transaction);

                    if (successCommentTotal == successScoreTotal)
                    {
                        //修改状态
                        UpdateState(comment.Select(x => x.Id).ToList(), 1);
                        transaction.Commit();
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
        /// 插入回答
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public Tuple<bool, string> ExecuteAnswerTransaction(List<QuestionsAnswersInfo> answers) 
        {
            //开启事务
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    string addAnswer = @"insert into QuestionsAnswersInfos(Id,QuestionInfoId,State,UserId,IsSchoolPublish,IsAttend,IsAnony,Content,LikeCount,IsSettlement,CreateTime,IsTop,ParentId,ReplyCount,PostUserRole,UserInfoExId,FirstParentId)
                                                                    values(@Id,@QuestionInfoId,@State,@UserId,@IsSchoolPublish,@IsAttend,@IsAnony,@Content,@LikeCount,@IsSettlement,@CreateTime,@IsTop,null,@ReplyCount,@PostUserRole,null,null)";

                    int total = _context.Execute(addAnswer, answers,transaction);

                    if(total == answers.Count()) 
                    {
                        //修改状态
                        UpdateState(answers.Select(x => x.Id).ToList(), 3);
                        transaction.Commit();
                        return new Tuple<bool, string>(true, "插入成功");
                    }
                    else 
                    {
                        transaction.Rollback();
                        return new Tuple<bool, string>(false, "插入失败");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Tuple<bool, string>(false, "插入失败："+ex.Message);
                }
                finally 
                {
                    _context.Dispose();
                }
            }    
        }

        public int UpdateState(List<Guid> Ids,int type) 
        {
            string table = "";
            switch (type)
            {
                case 1:
                    table = "ImportSchoolComments";
                    break;
                case 2:
                    table = "ImportQuestionInfos";
                    break;
                case 3:
                    table = "ImportQuestionsAnswersInfos";
                    break;
                default:
                    break;
            }
            string sql = $"update {table} set IsImport = 1 where Id in @Ids";
            return _updateContext.Execute(sql, new { Ids });
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
                    string addQuestion = @"insert into QuestionInfos(Id,State,SchoolId,SchoolSectionId,UserId,Content,IsAnony,CreateTime,LikeCount,IsHaveImagers,IsTop,ReplyCount,PostUserRole,UserInfoExId) 
                            values(@Id,@State,@SchoolId,@SchoolSectionId,@UserId,@Content,@IsAnony,@CreateTime,@LikeCount,@IsHaveImagers,@IsTop,@ReplyCount,@PostUserRole,null)";

                    int successQuestionsTotal = _context.Execute(addQuestion, questions, transaction);

                    if(questions.Count() == successQuestionsTotal) 
                    {
                        UpdateState(questions.Select(x => x.Id).ToList(), 2);

                        transaction.Commit();
                        return new Tuple<bool, string>(true, "完成");
                    }
                    else 
                    {
                        transaction.Rollback();
                        return new Tuple<bool, string>(true, "插入数据异常");
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
        /// 检测导入数据时间是否为最新数据状态
        /// </summary>
        /// <param name="eIds"></param>
        /// <returns></returns>
        public List<SchoolScore> GetSchoolSectionSchoolStatusTime(List<Guid> eIds) 
        {
            string sql = "select SchoolSectionId,LastCommentTime,LastQuestionTime from SchoolScores where SchoolSectionId in @eIds";
            return _updateContext.Query<SchoolScore>(sql, new { eIds })?.ToList();
        }

        /// <summary>
        /// 获取用户状态
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public List<UserInfo> GetUserInfo(List<Guid> Ids) 
        {
            string sql = "select id from userInfo where id in @Ids and channel = '8'";
            return _updateContext.Query<UserInfo>(sql, new { Ids })?.ToList();
        }

    }
}
