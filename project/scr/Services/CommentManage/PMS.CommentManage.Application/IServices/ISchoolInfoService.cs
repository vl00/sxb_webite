using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.CommentsManage.Application.ModelDto;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ISchoolInfoService
    {
        Task<SchoolInfoDto> QuerySchoolInfo(Guid schoolSectionId);

        SchoolInfoDto QueryESchoolInfo(Guid eid);

        List<SchoolInfoDto> GetCurrentSchoolAllExt(Guid sid);

        /// <summary>
        /// 批量获取学校数据
        /// </summary>
        /// <param name="SchoolSectionIds"></param>
        /// <returns></returns>
        List<SchoolInfoDto> GetSchoolSectionByIds(List<Guid> SchoolSectionIds);

        List<SchoolInfoQaDto> GetSchoolSectionQaByIds(List<Guid> SchoolSectionIds);

        List<SchoolInfoDto> QuerySchoolCards(bool QueryComment, int City, int PageIndex, int PageSize);

        /// <summary>
        /// 根据分部id获取学校全称
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        List<SchoolInfoDto> GetSchoolName(List<Guid> eid);

        /// <summary>
        /// 用户中心获取学校信息接口
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        List<SchoolInfoDto> GetSchoolStatuse(List<Guid> eid);
    }
}
