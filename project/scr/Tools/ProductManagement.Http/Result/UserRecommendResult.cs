using System;
using System.Collections.Generic;

namespace ProductManagement.API.Http.Result
{
    [Serializable]
    public class UserRecommendResult
    {
        public string ContentId { get; set; }

        public int BuType { get; set; }
    }
}
