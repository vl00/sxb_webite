using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class GradeRepository : Repository<Grade, PaidQADBContext>, IGradeRepository
    {
        PaidQADBContext _paidQADBContext;
        public GradeRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }

        public async Task<int> AddTalentGreades(System.Guid talentUserID, IEnumerable<System.Guid> gradeIDs)
        {
            var result = 0;
            foreach (var item in gradeIDs)
            {
                var newGuid = System.Guid.NewGuid();
                var str_SQL = $"Insert Into [TalentGrade] (ID, GradeID, TalentUserID) Values (@id, @gradeID, @talentUserID)";
                result += await _paidQADBContext.ExecuteAsync(str_SQL, new { id = newGuid, gradeID = item, talentUserID });
            }
            return result;
        }

        public async Task<int> Count(string str_Where, object param)
        {
            var str_SQL = $"Select Count(1) From [Order] Where {str_Where}";
            return await _paidQADBContext.QuerySingleAsync<int>(str_SQL, param);
        }

        public async Task<IEnumerable<Grade>> GetByTalentUserID(System.Guid talentUserID)
        {
            var str_SQL = $@"SELECT
	                            g.* 
                            FROM
	                            Grade AS g
	                            LEFT JOIN TalentGrade AS tg ON tg.GradeID = g.ID 
                            WHERE
	                            g.IsValid = 1 
	                            AND tg.TalentUserID = @talentUserID
                            order by g.Sort";
            return await _paidQADBContext.QueryAsync<Grade>(str_SQL, new { talentUserID });
        }

        public async Task<int> RemoveTalentGreades(System.Guid talentUserID)
        {
            var str_SQL = $"delete From [TalentGrade] Where TalentUserID = @talentUserID";
            return await _paidQADBContext.ExecuteAsync(str_SQL, new { talentUserID });
        }
    }
}
