using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;

namespace PMS.OperationPlateform.Repository.Repositories
{
    public class HuaneAsksRepository: IHuaneAsksRepository
    {
        private OperationCommandDBContext _DB;
        public HuaneAsksRepository(OperationCommandDBContext context)
        {
            _DB = context;
        }

        public async Task<List<HuaneAsksQuestion>> GetQuestionList(int pageIndex = 1,int pageSize = 10)
        {
            var offset = pageSize * (--pageIndex);
            var limit = pageSize;

            var str_SQL = $@"select q.id,q.question,q.create_time as createTime,q.click_times as clickTimes,A.content as answer from ( 
					select Id,content,qid,ROW_NUMBER() OVER(PARTITION BY qid ORDER By create_time Desc) AS Row_Sort,create_time
                    from HuaneQuestionAnswers where qid in (
						select q.id from HuaneQuestions q order by create_time desc  OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY
				)) A
				left join HuaneQuestions q on q.id = A.qid
				where A.Row_Sort=1;";
            return (await _DB.QueryAsync<HuaneAsksQuestion>(str_SQL, new { offset, limit })).ToList();
        }

        public async Task<int> GetQuestionCount()
        {
            var str_SQL = $@"select count(1) from HuaneQuestions;";
            return await _DB.QuerySingleAsync<int>(str_SQL, new {  });
        }

        public async Task<HuaneAsksQuestion> GetQuestion(int id)
        {
            var str_SQL = $@"select top 1 q.id,q.question,q.create_time as createTime,q.click_times as clickTimes
                from HuaneQuestions q where q.id = @id;";
            return await _DB.QuerySingleAsync<HuaneAsksQuestion>(str_SQL, new { id });
        }

        public async Task<List<HuaneAsksAnswer>> GetAnswerList(int qid ,int pageIndex = 1, int pageSize = 20)
        {
            var offset = pageSize * (--pageIndex);
            var limit = pageSize;
            var str_SQL = $@"select id, content,create_time as createTime,floor_number as floorNumber, likes, dislikes
            from HuaneQuestionAnswers where qid = @qid order by create_time asc 
                OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";
            return (await _DB.QueryAsync<HuaneAsksAnswer>(str_SQL, new { qid, offset, limit })).ToList();
        }
        public async Task<List<HuaneAsksQuestion>> GetRecommendQuestionList(int qid, int pageIndex = 1, int pageSize = 12)
        {
            var offset = pageSize * (--pageIndex);
            var limit = pageSize;
            var str_SQL = $@"	select q.id,q.question,q.create_time as createTime,q.click_times as clickTimes,A.content as answer from ( 
					select Id,content,qid,ROW_NUMBER() OVER(PARTITION BY qid ORDER By create_time Desc) AS Row_Sort,create_time from HuaneQuestionAnswers where qid in (
						select q.id from HuaneQuestions q 
						INNER JOIN HuaneQuestionRecommends r on r.recommend_qid = q.id
						where r.qid = @qid
						order by create_time desc
						OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY
				)) A
				inner join HuaneQuestions q on q.id = A.qid
				where A.Row_Sort=1;";
            return (await _DB.QueryAsync<HuaneAsksQuestion>(str_SQL, new { qid, offset, limit })).ToList();
        }
    }
}
