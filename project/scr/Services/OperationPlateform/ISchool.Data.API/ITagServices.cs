using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iSchool.Data.API
{


    /// <summary>
    /// tag标签管理接口
    /// 适用于restful 风格的api
    /// </summary>
    public interface ITagServices
    {

        [Get("/api/GeneralTag/GetByIds")]
        Task<Model.QueryTagsResult> GetByIds([Query(CollectionFormat.Multi)] [AliasAs("Ids")] string[] ids);

        [Get("/api/GeneralTag/name/{name}/{like}")]
        Task<Model.QueryTagsResult> GetByName([AliasAs("name")] string name,[AliasAs("like")] bool like);

        [Post("/api/GeneralTag/AddTags")]
        Task<Model.AddTagResult> AddTags([Body(BodySerializationMethod.Serialized)] string[] names);

        
       

    }
}
