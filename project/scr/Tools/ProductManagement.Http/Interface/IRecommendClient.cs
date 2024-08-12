using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagement.API.Http.Result;

namespace ProductManagement.API.Http.Interface
{
    public interface IRecommendClient
    {
        Task<dynamic> AddRedirectInsideArticle(string referShortId, string currentShortId);
        Task<dynamic> AddRedirectInsideSchool(string referShortId, string currentShortId);
        Task<IEnumerable<Guid>> RecommendArticleIds(Guid articleId, int pageIndex, int pageSize);
        Task<IEnumerable<Guid>> RecommendSchoolIds(Guid extId, int pageIndex, int pageSize);
    }
}
