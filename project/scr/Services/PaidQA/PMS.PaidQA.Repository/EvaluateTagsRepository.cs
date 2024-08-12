using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class EvaluateTagsRepository : Repository<EvaluateTags, PaidQADBContext>, IEvaluateTagsRepository
    {
        PaidQADBContext _paidQADBContext;
        public EvaluateTagsRepository(PaidQADBContext dBContext) : base(dBContext)
        {
            _paidQADBContext = dBContext;
        }

        public async Task<IEnumerable<KeyValuePair<Guid, IEnumerable<EvaluateTags>>>> GetByEvaluateIDs(IEnumerable<Guid> ids)
        {
            var str_SQL = $@"SELECT
								etr.EvaluateID,
								etr.TagID,
								et.Name 
							FROM
								EvaluateTagRelation AS etr
								LEFT JOIN EvaluateTags AS et ON et.ID = etr.TagID 
							WHERE
								et.IsValid = 1 
								AND etr.EvaluateID IN @ids";
            var finds = await _paidQADBContext.QueryAsync<(Guid, Guid, string)>(str_SQL, new { ids });
            if (finds?.Any() == true)
            {
                var result = new Dictionary<Guid, IEnumerable<EvaluateTags>>();
                foreach (var item in finds.Select(p => p.Item1).Distinct())
                {
                    if (!result.ContainsKey(item))
                    {
                        result.Add(item, finds.Where(p => p.Item1 == item).Select(p => new EvaluateTags()
                        {
                            ID = p.Item2,
                            IsValid = true,
                            Name = p.Item3
                        }));
                    }
                }
                return result;
            }
            return null;
        }

        public async Task<IEnumerable<EvaluateTagCountingExtend>> GetEvaluateTagCountingByTalentUserID(Guid talentUserID)
        {
            var str_SQL = $@"SELECT
								et.ID,
								et.Name,
								COUNT (1) as [EvaluateCount]
							FROM
								EvaluateTags AS et
								LEFT JOIN EvaluateTagRelation as etr on et.ID = etr.TagID
								LEFT JOIN Evaluate AS e ON e.ID = etr.EvaluateID
								LEFT JOIN [Order] AS o ON o.ID = e.OrderID 
							WHERE
								et.IsValid = 1
								AND 
								o.AnswerID = @talentUserID
								AND
								e.IsValid = 1
								AND
								o.IsBlocked = 0
							GROUP BY
								et.ID,
								et.Name";
            return await _paidQADBContext.QueryAsync<EvaluateTagCountingExtend>(str_SQL, new { talentUserID });
        }
    }
}
