using iSchool;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    /// <summary>
    /// 热门信息：(现在不要同城同总类型)
    ///   热门学校 - 两天内浏览人数最多的6所学校, 计算UV    ///   周边学校 - 距离当前学校最近的6所学校       ///   热评学校 - 两天内发布、点赞和回复点评总数最高的6所学校     ///   热问学校 - 两天内发布、回复问答总数最高的6所学校           ///   热门攻略 - 一周内浏览量最高的5条攻略                   //计算UV    ///   热门点评 - 两天内点赞和回复总数最高的5条点评,在该用户的定位范围内(城市) //--不用计算UV 
    ///   热门问答 - 两天内点赞和回复总数最高的5条问答 在该用户的定位范围内(城市) //
    /// </summary>
    public interface IHotPopularService
    {
        /// <summary>
        /// 热门学校
        /// </summary>
        Task<SimpleHotSchoolDto[]> GetHotVisitSchools(int citycode, SchFType0[] schtypes = null, int count = 6);
        /// <summary>
        /// 周边学校
        /// </summary>
        Task<SmpNearestSchoolDto[]> GetNearestSchools(Guid eid, int count = 6);
        /// <summary>
        /// 周边学校(其他没有学校eid的页面)
        /// </summary>
        Task<SmpNearestSchoolDto[]> GetNearestSchools(int citycode, (double Lng, double Lat) lnglat, SchFType0[] schtypes = null, int count = 6);

        /// <summary>
        /// 热评学校
        /// </summary>
        //Task<(Guid Sid, Guid Eid, string SchoolName, int CommentCount)[]> GetHotCommentSchools(int citycode, SchFType0? schtype, int count = 6);
        /// <summary>
        /// 热问学校
        /// </summary>
        //Task<(Guid Sid, Guid Eid, string SchoolName, int QuestionCount)[]> GetHotQuestionSchools(int citycode, SchFType0? schtype, int count = 6);

        ///热门攻略 
        //(IArticleService articleService).HotPointArticles(0,null,0,5);
    }
}
