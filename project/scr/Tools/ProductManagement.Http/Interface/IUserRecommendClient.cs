using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagement.API.Http.Result;

namespace ProductManagement.API.Http.Interface
{
    public interface IUserRecommendClient
    {
        Task<BaseResponseResult<List<UserRecommendResult>>> UserRecommendSchool(Guid userId, int index, int size);
    }
}
