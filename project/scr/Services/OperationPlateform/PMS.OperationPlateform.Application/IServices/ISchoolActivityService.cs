using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface ISchoolActivityService
    {
        Task<IEnumerable<SchoolActivityProcess>> GetProcesses(Guid activityId);


        /// <summary>
        /// 查询活动信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SchoolActivity> GetBy(Guid id);


        /// <summary>
        /// 是否已经签到
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<bool> HasSignIn(Guid activityId, string phone);

        /// <summary>
        /// 活动签到
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<bool> SignIn(Guid activityId, string phone, SignType signType);

        /// <summary>
        /// 判断电话是在指定活动里留资过
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        bool ExistPhone(Guid activityId, string phone);


        AppServiceResultDto ExistCustomerChannelPhone(List<int> customers, string phone, string channel);

        /// <summary>
        /// 获取留资信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        AppServiceResultDto<SchoolActivityDetailDto> GetActivity(Guid activityId);

        /// <summary>
        /// 获取留资信息
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        AppServiceResultDto<SchoolActivityDetailDto> GetActivityByExtId(Guid extId, bool? isEnable = null, bool? isCover = null);

        /// <summary>
        /// 获取学校的单一留资详情, 如果没有, 则返回统一留资
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        AppServiceResultDto<SchoolActivityDetailDto> GetActivityByExtIdOrDefault(Guid extId);

        /// <summary>
        /// 获取留资信息
        /// </summary>
        /// <param name="schoolExtId"></param>
        /// <returns></returns>
        AppServiceResultDto<SchoolActivityDetailDto> GetCommonActivity();

        /// <summary>
        /// 提交报名
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        AppServiceResultDto<ResponseSchoolActivityRegister> Register(SchoolActivityRegisterDto registerDto);

        /// <summary>
        /// 发送报名验证码
        /// </summary>
        /// <param name="extensionId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<AppServiceResultDto> SendCode(Guid extensionId, string phone);

        /// <summary>
        /// 获取留资信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        Task<IEnumerable<SchoolActivityRegiste>> GetRegiste(Guid activityId);


        /// <summary>
        /// 获取注册字段
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        Task<IEnumerable<SchoolActivityRegisteExtension>> GetRegisteFields(Guid activityId);
        AppServiceResultDto<ResponseSchoolActivityRegister> UnCheckRegister(SchoolActicityUnCheckRegisterDto registerDto);
    }
}
