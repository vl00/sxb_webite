using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using PMS.School.Domain.IRespository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class GeneralTagService : IGeneralTagService
    {
        IGeneralTagRespository _generalTagRespository;
        IArticleService _articleService;

        public GeneralTagService(IGeneralTagRespository generalTagRespository, IArticleService articleService)
        {
            _articleService = articleService;
            _generalTagRespository = generalTagRespository;
        }

        public async Task<IEnumerable<KeyValuePair<Guid, string>>> GetByDataId(Guid dataId)
        {
            return await _generalTagRespository.GetByDataId(dataId);
        }

        public async Task<Dictionary<Guid, string>> GetByIDs(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any()) return new Dictionary<Guid, string>();
            return await _generalTagRespository.GetByIDs(ids.Distinct());
        }


        public async Task<Dictionary<Guid, string>> GetByNames(IEnumerable<string> names)
        {
            if (names == null || !names.Any()) return new Dictionary<Guid, string>();
            return await _generalTagRespository.GetByNames(names.Distinct());
        }

        public async Task<Guid> GetTagIDByNameForArticle(string name)
        {
            var result = await GetByNames(new string[] { name });
            if (result?.Any() == true)
            {
                var exist = await _articleService.CheckTagIDInbind(result.Keys);
                if (exist?.Any() == true) return exist.FirstOrDefault().Key;
            }
            return Guid.Empty;
        }
    }
}
