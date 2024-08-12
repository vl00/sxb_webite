using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Talent;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.Search.Application.Services
{
    public class TalentSearchService : ITalentSearchService
    {
        private readonly ITalentSearch _talentSearch;

        public TalentSearchService(ITalentSearch talentSearch)
        {
            _talentSearch = talentSearch;
        }

        public List<SearchTalentDto> SearchTalents(SearchBaseQueryModel queryModel)
        {
            var data = _talentSearch.SearchTalents(queryModel);
            return CommonHelper.MapperProperty<SearchTalent, SearchTalentDto>(data).ToList();
        }
    }
}
