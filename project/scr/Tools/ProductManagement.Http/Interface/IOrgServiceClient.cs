using ProductManagement.API.Http.Model.Org;
using ProductManagement.API.Http.Request.Org;
using ProductManagement.API.Http.Result.Org;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Interface
{
   public interface IOrgServiceClient
    {
        /// <summary>
        /// 查询订单封面
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<string>> OrgsOrderBanners(OrgOrderBannerRequest request);

        /// <summary>
        /// 机构长ID列表反查
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        Task<CardListResult<OrgCard>> OrgsLablesByIds(OrgsLablesByIdsRequest request);

        /// <summary>
        /// 机构短ID列表反查
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<CardListResult<OrgCard>> OrgsLablesById_ss(OrgsLablesById_SSRequest request);


        /// <summary>
        /// 机构测评长ID列表反查
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<CardListResult<OrgEvalutionCard>> EvalsLablesByIds(EvalsLablesByIdsRequest request);


        /// <summary>
        /// 机构测评短ID列表反查
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<CardListResult<OrgEvalutionCard>> EvalsLablesById_ss(EvalsLablesById_SSRequest request);


        /// <summary>
        /// 课程长ID列表反查
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        Task<CardListResult<CourseCard>> CoursesLablesByIds(CoursesLablesByIdsRequest request);



        /// <summary>
        /// 课程短ID列表反查
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        Task<CardListResult<CourseCard>> CoursesLablesById_ss(CoursesLablesById_SSRequest request);
        Task<IEnumerable<CourseDto>> GetCourses(IEnumerable<Guid> ids);
        Task<IEnumerable<EvaluationDto>> GetEvaluations(IEnumerable<Guid> ids);

        Task<IEnumerable<GG21Course>> GetGG21Courses(GetGG21CoursesRequest request);

       
        Task<IEnumerable<HotSellCourse>> GetHotSellCoursesByIds(IEnumerable<Guid> ids, IEnumerable<string> sIds);
        Task<IEnumerable<RecommendOrg>> GetRecommendOrgsByIds(IEnumerable<Guid> ids, IEnumerable<string> sIds);
    }
}
