using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class HotQuestionRepository : Repository<HotQuestion, PaidQADBContext>, IHotQuestionRepository
    {
        PaidQADBContext _paidQADBContext;
        public HotQuestionRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }

        public async Task<int> Count(string str_Where, object param)
        {
            var str_SQL = $"Select Count(1) From [HotQuestion] Where {str_Where}";
            return await _paidQADBContext.QuerySingleAsync<int>(str_SQL, param);
        }

        public async Task<HotQuestion> GetByOrderID(Guid id)
        {
            return await _paidQADBContext.QuerySingleAsync<HotQuestion>("Select Top 1 * From HotQuestion Where OrderID = @id", new { id });
        }

        public async Task<IEnumerable<(Guid, Guid)>> GetRandomOrderIDByGradeSort(int sort, int num = 3)
        {
            //var str_SQL = $@"SELECT TOP {num} 
            //                 hq.OrderID 
            //                FROM
            //                 HotQuestion AS hq
            //                 LEFT JOIN OrderTag AS ot ON ot.OrderID = hq.OrderID
            //                 LEFT JOIN HotType AS ht ON ht.ID = hq.HotTypeID
            //                 LEFT JOIN Grade AS g ON g.ID = ot.TagID 
            //                WHERE
            //                 g.Sort = @sort
            //                ORDER BY NEWID()";
            var str_SQL = $@"SELECT TOP 15
	                            hq.OrderID,
	                            o1.AnswerID 
                            FROM
	                            HotQuestion AS hq
	                            LEFT JOIN [Order] AS o1 ON o1.ID = hq.OrderID 
                            WHERE
	                            EXISTS (
	                            SELECT TOP
		                            3 * 
	                            FROM
		                            (SELECT 
                                        DISTINCT(o.AnswerID) 
		                            FROM
			                            HotQuestion AS hq
			                            LEFT JOIN [Order] AS o ON o.ID = hq.OrderID
			                            LEFT JOIN TalentGrade AS tg ON tg.TalentUserID = o.AnswerID
			                            LEFT JOIN Grade AS g ON g.ID = tg.GradeID
		                            WHERE
			                            g.Sort = @sort
			                            AND hq.OrderID = o1.ID 
		                            ) AS [tmp] 
	                            ORDER BY
		                            NEWID() 
	                            ) 
                            ORDER BY
	                            NEWID()";
            return await _paidQADBContext.QueryAsync<(Guid, Guid)>(str_SQL, new { sort });
        }


        public async Task<IEnumerable<(Guid, Guid)>> GetRandomOrderIDByGradeSorts(IEnumerable<int> sorts, int num = 3)
        {
            //var str_SQL = $@"SELECT TOP {num} 
            //                 hq.OrderID 
            //                FROM
            //                 HotQuestion AS hq
            //                 LEFT JOIN OrderTag AS ot ON ot.OrderID = hq.OrderID
            //                 LEFT JOIN HotType AS ht ON ht.ID = hq.HotTypeID
            //                 LEFT JOIN Grade AS g ON g.ID = ot.TagID 
            //                WHERE
            //                 g.Sort = @sort
            //                ORDER BY NEWID()";
            var str_SQL = $@"SELECT TOP 15
	                            hq.OrderID,
	                            o1.AnswerID 
                            FROM
	                            HotQuestion AS hq
	                            LEFT JOIN [Order] AS o1 ON o1.ID = hq.OrderID 
                            WHERE
	                            EXISTS (
	                            SELECT TOP
		                            3 * 
	                            FROM
		                            (SELECT 
                                        DISTINCT(o.AnswerID) 
		                            FROM
			                            HotQuestion AS hq
			                            LEFT JOIN [Order] AS o ON o.ID = hq.OrderID
			                            LEFT JOIN TalentGrade AS tg ON tg.TalentUserID = o.AnswerID
			                            LEFT JOIN Grade AS g ON g.ID = tg.GradeID
		                            WHERE
			                            g.Sort in @sorts
			                            AND hq.OrderID = o1.ID 
		                            ) AS [tmp] 
	                            ORDER BY
		                            NEWID() 
	                            ) 
                            ORDER BY
	                            NEWID()";
            return await _paidQADBContext.QueryAsync<(Guid, Guid)>(str_SQL, new { sorts });
        }

        public async Task<HotQuestionExtend> GetWithTypeNameByOrderID(Guid id)
        {
            var str_SQL = $@"Select hq.*,ht.name as [HotTypeName] From HotQuestion as hq Left Join HotType as ht on ht.ID = hq.HotTypeID Where hq.orderID = @id";
            return await _paidQADBContext.QuerySingleAsync<HotQuestionExtend>(str_SQL, new { id });
        }
    }
}
