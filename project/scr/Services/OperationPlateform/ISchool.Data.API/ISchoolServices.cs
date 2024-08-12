using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iSchool.Data.API
{
    public interface ISchoolServices
    {

        [Get("/api/School/GetByIds")]
        Task<Model.QuerySchoolResult> GetByIds([Query(CollectionFormat.Multi)] string[] ids);


        [Get("/api/School/GetByName")]
        Task<Model.QuerySchoolResult> GetSchoolBy([Query][AliasAs("Name")] string name,[Query][AliasAs("Count")] int count=20);

        [Post("/api/School/GetlistByName")]
        Task<Model.QueryUniversityResult> GetlistByName([Body(BodySerializationMethod.Serialized)] string[] names);

        /// <summary>
        /// 搜索文章相关的学校分部
        /// </summary>
        /// <returns></returns>
        [Post("/api/School/GetByFinalType")]
        Task<Model.QuerySchoolExtentionReuslt> GetByFinalType([Body(BodySerializationMethod.Serialized)] Model.GetByFinalTypeRequest req );

    }
}
