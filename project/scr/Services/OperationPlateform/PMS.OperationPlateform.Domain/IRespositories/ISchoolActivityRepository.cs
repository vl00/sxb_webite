using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface ISchoolActivityRepository : IRepository<SchoolActivity>
    {

        Task<IEnumerable<SchoolActivityProcess>> GetProcesses(Guid activityId);
        Task<IEnumerable<SchoolActivityRegisteExtension>> GetRegisteFields(Guid activityId);

        /// <summary>
        /// 获取留资信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        Task<IEnumerable<SchoolActivityRegiste>> GetRegiste(Guid activityId);

        /// <summary>
        /// 判断是否已签到
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<bool> HasSignIn(Guid activityId, string phone);

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="phone"></param>
        /// <param name="signType"></param>
        /// <returns></returns>
        Task<bool> SignIn(Guid activityId, string phone, SignType signType);


        /// <summary>
        /// 获取学校留资信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SchoolActivity Get(Guid id);

        /// <summary>
        /// 获取最新的统一留资信息  以最后编辑时间为准
        /// </summary>
        /// <param name="schoolExtId"></param>
        /// <returns></returns>
        SchoolActivity GetCommon();

        /// <summary>
        /// 获取最新的单一学校留资信息  以最后编辑时间为准
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="isEnable">是否是启用的活动</param>
        /// <param name="isCover">是否替换留资信息</param>
        /// <returns></returns>
        SchoolActivity GetBySchoolExtId(Guid extId, bool? isEnable, bool? isCover);

        /// <summary>
        /// 获取留资字段
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        IEnumerable<SchoolActivityRegisteExtensionDto> GetExtensions(Guid activityId, int version);

        /// <summary>
        /// 获取留资图片
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        IEnumerable<SchoolActivityExtension> GetImages(Guid activityId);

        /// <summary>
        /// 报名
        /// </summary>
        /// <returns></returns>
        bool Register(SchoolActivityRegiste schoolActivityRegiste);

        /// <summary>
        /// 获取当前活动报名字段版本
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        int GetVersion(Guid activityId);
        bool ExistCustomerChannelPhone(List<int> customer, string channel, string phone);
        bool ExistChannelPhone(Guid activityId, string channel, string phone);
        bool ExistPhone(Guid activityId, string phone);
    }
}
