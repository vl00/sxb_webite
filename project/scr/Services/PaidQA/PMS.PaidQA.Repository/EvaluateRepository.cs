using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class EvaluateRepository : Repository<Evaluate, PaidQADBContext>, IEvaluateRepository
    {
        PaidQADBContext _paidQADBContext;

        public EvaluateRepository(PaidQADBContext dBContext) : base(dBContext)
        {
            _paidQADBContext = dBContext;
        }

        public async Task<int> Count(string str_Where, object param)
        {
            var str_SQL = $"Select Count(1) From [Evaluate] Where {str_Where}";
            return await _paidQADBContext.QuerySingleAsync<int>(str_SQL, param);
        }

        public async Task<double> GetAvgScoreByTalentUserID(System.Guid talentUserID)
        {
            var str_SQL = $@"SELECT 
                                ISNULL(AVG(e.Score) , 0) AS [AvgScore]
                            FROM
	                            Evaluate AS e
	                            LEFT JOIN [Order] AS o ON o.ID = e.OrderID 
                            WHERE
		                        e.Score > 0
	                            AND o.AnswerID = @talentUserID";
            return await _paidQADBContext.QuerySingleAsync<double>(str_SQL, new { talentUserID });
        }

        public async Task<(IEnumerable<Evaluate>, int)> PageByAnwserUserID(System.Guid anwserUserID, int pageIndex = 1, int pageSize = 10, System.Guid? tagID = null)
        {
            var offset = --pageIndex * pageSize;
            var str_TagLeftJoin = string.Empty;
            var str_Where = string.Empty;
            var str_Fields = "Count(1)";
            var str_SQL_Format = "SELECT {0} FROM Evaluate AS e LEFT JOIN [Order] AS o ON o.ID = e.OrderID {1} WHERE o.AnswerID = @anwserUserID {2}";
            var total = _paidQADBContext.QuerySingle<int>(string.Format(str_SQL_Format, str_Fields, str_TagLeftJoin, str_Where), new { anwserUserID });
            if (total < 1) return (null, 0);
            str_SQL_Format += $" ORDER BY e.CreateTime DESC OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            str_Fields = "e.*";
            if (tagID.HasValue && tagID.Value != System.Guid.Empty)
            {
                str_TagLeftJoin = " LEFT JOIN EvaluateTagRelation as etr on etr.EvaluateID = e.ID";
                str_Where = " AND etr.TagID = @tagID";
            }
            return (await _paidQADBContext.QueryAsync<Evaluate>(string.Format(str_SQL_Format, str_Fields, str_TagLeftJoin, str_Where), new { anwserUserID, tagID }), total);

        }
    }
}
