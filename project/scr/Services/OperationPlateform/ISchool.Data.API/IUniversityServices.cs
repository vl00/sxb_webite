using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iSchool.Data.API
{
   public interface IUniversityServices
    {

        [Get("/api/College/GetByIds")]
        Task<Model.QueryUniversityResult> GetByIds([Query(CollectionFormat.Multi)] string[] ids);

        [Get("/api/College/GetByName")]
        Task<Model.QueryUniversityResult> GetSchoolBy([Query][AliasAs("Name")] string name, [Query][AliasAs("Count")] int count = 20);

        [Post("/api/College/GetlistByName")]
        Task<Model.QueryUniversityResult> GetlistByName([Body(BodySerializationMethod.Serialized)] string[] names);

    }
}
