using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface ILSRFSchoolDetailService
    {
        List<LSRFSchoolDetail> GetLSRFSchools(CourseType courseType, List<Guid> SchIds, int PageNo = 1, int PageSize = 6);

        List<LSRFSchoolDetail> GetAdvertisementSchool(CourseType courseType, LSRFSchoolType DataType, List<Guid> SchIds, int PageNo = 1, int PageSize = 6);

        List<SingSchoolRank> GetSchoolRankByEid(List<Guid> SchId);

        List<LSRFSchoolDetail> SchDistinct(int courseType);

        /// <summary>
        /// 获取课程设置下的广告位总数值
        /// </summary>
        /// <param name="courseType"></param>
        /// <returns></returns>
        KeyValue GetCourseTypeAdveTotal(CourseType courseType);

        /// <summary>
        /// 获取该课程类型下的总条数【普通数据】
        /// </summary>
        /// <param name="courseType"></param>
        /// <returns></returns>
        KeyValue GetCurrentCourseTotal(CourseType courseType, List<Guid> SchIds);

        bool CheckSchIsLeaving(Guid SchId);
    }
}
