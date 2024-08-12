using PMS.Search.Application.ModelDto.Talent;
using PMS.Search.Domain.QueryModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Application.IServices
{
    public interface ITalentSearchService
    {
        List<SearchTalentDto> SearchTalents(SearchBaseQueryModel queryModel);
    }
}
