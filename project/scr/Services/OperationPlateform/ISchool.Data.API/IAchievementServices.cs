using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iSchool.Data.API
{
    public interface IAchievementServices
    {
        [Post("/api/School/GetAchievementsBy")]
        Task<Model.QueryAchievementResult> GetAchievementsBy([Body(BodySerializationMethod.Serialized)] Model.AchievementRequest req);
    }
}
