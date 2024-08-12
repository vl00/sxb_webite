using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Application.IServices
{
    public interface IHottestComment_SchoolService
    {
        /// <summary>
        /// 学校热门点评【分地区】
        /// </summary>
        /// <param name="CommentQuery"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<List<SchoolCommentDto>> HotComment(HotCommentQuery CommentQuery, Guid UserId);

        /// <summary>
        /// 两天时间内最热门的点【全国】 hangfire统计方法
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        List<SchoolCommentDto> HottestComment(DateTime StartTime, DateTime EndTime);

        /// <summary>
        /// 热评学校
        /// </summary>
        /// <param name="query"></param>
        /// <param name="queryAll">true：全国，false：指定类型的学校</param>
        /// <returns></returns>
        Task<List<HotCommentSchoolDto>> HottestSchool(HotCommentQuery query, bool queryAll);
    }
}
