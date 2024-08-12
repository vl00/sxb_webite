using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public class GradeService : ApplicationService<Grade>, IGradeService
    {
        IGradeRepository _gradeRepository;
        public GradeService(IGradeRepository gradeRepository) : base(gradeRepository)
        {
            _gradeRepository = gradeRepository;
        }

        public bool AddTalentGrades(Guid talentUserID, IEnumerable<Guid> gradeIDs)
        {
            if (talentUserID == Guid.Empty || gradeIDs == null || !gradeIDs.Any()) return false;
            return _gradeRepository.AddTalentGreades(talentUserID, gradeIDs).Result > 0;
        }

        public async Task<IEnumerable<Grade>> GetAllGrades()
        {
            var str_SQL = $"IsValid = 1";
            return await Task.Run(() =>
            {
                return _gradeRepository.GetBy(str_SQL, new { }, fileds: new string[1] { "*" })?.OrderBy(p => p.Sort);
            });
        }

        public async Task<IEnumerable<Grade>> GetByTalentUserID(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return null;
            return await _gradeRepository.GetByTalentUserID(talentUserID);
        }

        public Task<Dictionary<Guid, IEnumerable<Grade>>> GetByTalentUserIDs(IEnumerable<Guid> talentUserIDs)
        {
            throw new NotImplementedException();
        }

        public bool RemoveTalentGrades(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return false;
            return _gradeRepository.RemoveTalentGreades(talentUserID).Result > 0;
        }
    }
}
