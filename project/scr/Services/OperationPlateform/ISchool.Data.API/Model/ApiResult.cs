using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Data.API.Model
{
    public class AchievementRequest {
        public int Year { get; set; }
        public bool IsCollege { get; set; }
        public Guid[] SchoolIds { get; set; }
    }

    public class GetByFinalTypeRequest
    {
        public string schoolName { get; set; }
        public List<_SchoolType> SchoolType { get; set; }
        public int Top { get; set; } = 10;
        public string[] CityCodes { get; set; }
        public class _SchoolType
        {
            /// <summary>
            /// 年级
            /// </summary>
            public int? grade { get; set; }
            /// <summary>
            /// 办学类型
            /// </summary>
            public int? runType { get; set; }
            /// <summary>
            /// 是否普惠
            /// </summary>
            public bool? discount { get; set; }
            /// <summary>
            /// 是否双语
            /// </summary>
            public bool? diglossia { get; set; }
            /// <summary>
            /// 是否中国人
            /// </summary>
            public bool? chinese { get; set; }

        }
    }



    public class BaseApiResult<T> where T:class
    {
        public string msg { get; set; }
        public int errCode { get; set; }
        public bool isOk { get; set; }
        public T data { get; set; }

    }

    public class QueryTagsResult : BaseApiResult<List<Model.Tag>>
    {

    }

    public class AddTagResult : BaseApiResult<List<Model.Tag>>
    {

    }

    public class QuerySchoolResult : BaseApiResult<List<Model.OnlineSchool>>
    {
    }
    public class QueryUniversityResult : BaseApiResult<List<Model.College>>
    {
       
    }
    public class QueryAchievementResult : BaseApiResult<List<Model.Achievements>>
    {
  
    }

    public class QuerySchoolExtentionReuslt : BaseApiResult<List<Model.SchoolExt>>
    {
       
    }




}
