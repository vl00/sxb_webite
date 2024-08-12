using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface ILSRFSchoolDetailRepository
    {
        /// <summary>
        /// 获取学校【排除广告位数据】
        /// </summary>
        /// <param name="courseType"></param>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<LSRFSchoolDetail> GetLSRFSchools(CourseType courseType, List<Guid> SchIds, int PageNo, int PageSize = 6);

        /// <summary>
        /// 获取该课程类型的广告位数据
        /// </summary>
        /// <param name="courseType"></param>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<LSRFSchoolDetail> GetAdvertisementSchool(CourseType courseType, LSRFSchoolType DataType, List<Guid> SchIds, int PageNo = 1, int PageSize = 6);

        /// <summary>
        /// 根据学校id得到该学校所在rank值
        /// </summary>
        /// <param name="SchId"></param>
        /// <returns></returns>
        List<SingSchoolRank> GetSchoolRankByEid(List<Guid> SchId);

        /// <summary>
        /// 获取课程设置下的广告位总数值
        /// </summary>
        /// <param name="courseType"></param>
        /// <returns></returns>
        KeyValue GetCourseTypeAdveTotal(CourseType courseType);

        /// <summary>
        /// 获取该课程类型下的总条数
        /// </summary>
        /// <param name="courseType"></param>
        /// <returns></returns>
        KeyValue GetCurrentCourseTotal(CourseType courseType,List<Guid> SchIds);

        /// <summary>
        /// 检测该学校是否可以进行留资操作
        /// </summary>
        /// <param name="SchId"></param>
        /// <returns></returns>
        bool CheckSchIsLeaving(Guid SchId);

        /// <summary>
        /// 学校去重
        /// </summary>
        /// <param name="courseType"></param>
        /// <returns></returns>
        List<LSRFSchoolDetail> SchDistinct(int courseType);
    }
}
