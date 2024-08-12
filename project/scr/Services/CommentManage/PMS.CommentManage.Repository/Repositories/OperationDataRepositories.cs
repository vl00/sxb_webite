using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.SqlClient;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class OperationDataRepositories : EntityFrameworkRepository<OperationData>, IOperationDataRepositories
    {
        public OperationDataRepositories(CommentsManageDbContext dbContext, CommentsEFRepository<OperationData> eFRepo) :base(dbContext)
        {
        }

        public List<OperationData> OperationLastedData(OperationType OperationType, Guid UserId,List<Guid> DataSourcese)
        {
            string condition = String.Join(",", DataSourcese.Select((elm, index) => "@" + index).ToList());
            List<SqlParameter> para = DataSourcese.Select((elm, index) => new SqlParameter("@" + index,elm)).ToList();
            para.Add(new SqlParameter("@UserId",UserId));


            string TableName = "";
            if (OperationType == OperationType.Comment)
            {
                TableName = " SchoolComments ";
            }
            else if (OperationType == OperationType.Reply)
            {
                TableName = " SchoolCommentReplies ";
            }
            else if (OperationType == OperationType.Question)
            {
                TableName = " QuestionInfos ";
            }
            else 
            {
                TableName = " QuestionsAnswersInfos ";
            }
            string sql = @"select 
		                    a.Id,
		                    (case
			                    when l.Id is not null and l.UserId = @UserId then 1
			                    else 0
		                    end) as IsPraise,
		                    a.LikeCount,
		                    a.ReplyCount
	                    from "+ TableName + @" as a
		                    left join SchoolCommentLikes 	as l on a.Id = l.SourceId
	                    where a.Id in ("+ condition + ")";

            return Query(sql, para.ToArray())?.ToList();
        }
    }
}
