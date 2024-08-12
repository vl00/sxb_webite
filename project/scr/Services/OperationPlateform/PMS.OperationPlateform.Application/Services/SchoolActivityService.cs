using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using PMS.School.Domain.IRepositories;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.Services
{
    /// <summary>
    /// 学校留资活动
    /// </summary>
    public class SchoolActivityService : ISchoolActivityService
    {

        private readonly ISchoolActivityRepository _schoolActivityRepository;
        private readonly ISmsService _smsService;
        private readonly School.Domain.IRepositories.ISchoolRepository _schoolRepository;
        private readonly ILiveServiceClient _liveServiceClient;

        public SchoolActivityService(ISchoolActivityRepository schoolActivityRepository, ISmsService smsService, School.Domain.IRepositories.ISchoolRepository schoolRepository, ILiveServiceClient liveServiceClient)
        {
            _schoolActivityRepository = schoolActivityRepository;
            _smsService = smsService;
            _schoolRepository = schoolRepository;
            _liveServiceClient = liveServiceClient;
        }

        public AppServiceResultDto<SchoolActivityDetailDto> GetCommonActivity()
        {
            var activity = _schoolActivityRepository.GetCommon();
            if (activity == null)
                return AppServiceResultDto<SchoolActivityDetailDto>.Failure("无留资活动");

            return GetActivity(activity.Id);
        }

        public AppServiceResultDto<SchoolActivityDetailDto> GetActivityByExtIdOrDefault(Guid extId)
        {
            var result = GetActivityByExtId(extId, isEnable: true, isCover: true);
            if (!result.Status)
                result = GetCommonActivity();

            return result;
        }

        public AppServiceResultDto<SchoolActivityDetailDto> GetActivityByExtId(Guid extId, bool? isEnable, bool? isCover)
        {
            if (extId == Guid.Empty)
                return AppServiceResultDto<SchoolActivityDetailDto>.Failure("请选择学校");

            var activity = _schoolActivityRepository.GetBySchoolExtId(extId, isEnable, isCover);
            if (activity == null)
                return AppServiceResultDto<SchoolActivityDetailDto>.Failure("无留资活动");

            return GetActivity(activity.Id);
        }

        /// <summary>
        /// 获取留资信息
        /// </summary>
        /// <param name="schoolExtId"></param>
        /// <returns></returns>
        public AppServiceResultDto<SchoolActivityDetailDto> GetActivity(Guid activityId)
        {
            var activity = _schoolActivityRepository.Get(activityId);
            if (activity == null)
                return AppServiceResultDto<SchoolActivityDetailDto>.Failure("无留资活动");

            var version = _schoolActivityRepository.GetVersion(activityId);
            var extensions = _schoolActivityRepository.GetExtensions(activity.Id, version).ToList();
            if (extensions.Count == 0)
                return AppServiceResultDto<SchoolActivityDetailDto>.Failure("无留资活动");

            var images = _schoolActivityRepository.GetImages(activity.Id).ToList();
            if (images.Count == 0)
            {
                //return AppServiceResultDto<SchoolActivityDetailDto>.Failure("无留资图片");

                //默认图片
                images.Add(new SchoolActivityExtension()
                {
                    Value = ConstantValue.DefaultSchoolActivityBannerImg,
                    Value2 = ConstantValue.DefaultSchoolActivityBannerImg,
                    Sort = 0
                });
            }

            SchoolActivityDetailDto dto = CommonHelper.MapperProperty<SchoolActivity, SchoolActivityDetailDto>(activity);

            dto.Extensions = extensions;
            dto.Images = images.Select(s => new SchoolActivityDetailDto.Image() { Url = s.Value, Thunmbnail = s.Value2, Sort = s.Sort }).ToList();

            if (dto.ExtId != null && dto.ExtId != Guid.Empty)
            {
                dto.SchoolName = _schoolRepository.GetSchoolName(new List<Guid>() { dto.ExtId.Value })?.FirstOrDefault().SchoolName;
            }

            return AppServiceResultDto.Success(dto);
        }

        /// <summary>
        /// 报名
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        public AppServiceResultDto<ResponseSchoolActivityRegister> Register(SchoolActivityRegisterDto registerDto)
        {
            Guid activityId = registerDto.ActivityId;
            var activity = _schoolActivityRepository.Get(activityId);
            List<int> customers = string.IsNullOrWhiteSpace(activity.Customer) ? new List<int>() :
                activity.Customer.Split(',').Select(q => Convert.ToInt32(q)).ToList();
            if (activity == null)
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("无留资活动");

            var now = DateTime.Now;
            if (activity.Status == (byte)SchoolActivityStatus.Disable)
            {
                return ResponseSchoolActivityRegister.ToFailure(ResponseSchoolActivityRegister.Code.Expired, "活动已结束");
            }
            if (activity.Type == (byte)SchoolActivityType.Limit)
            {
                if (activity.StartTime > now)
                {
                    return ResponseSchoolActivityRegister.ToFailure(ResponseSchoolActivityRegister.Code.NoTime, "活动未开始");
                }
                if (activity.EndTime < now)
                {
                    return ResponseSchoolActivityRegister.ToFailure(ResponseSchoolActivityRegister.Code.Expired, "活动已结束");
                }
            }

            Guid? extId = Guid.Empty;
            if (activity.Category == (byte)SchoolActivityCategory.Common)
            {
                if (registerDto.ExtId == null || registerDto.ExtId == Guid.Empty)
                    return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("系统错误, 无学校信息");
                extId = registerDto.ExtId.Value;
            }
            if (activity.Category == (byte)SchoolActivityCategory.Single)
            {
                //if (activity.ExtId == null || activity.ExtId == Guid.Empty)
                //return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("系统错误, 无学校信息");
                extId = activity.ExtId;//可为空
            }

            var version = _schoolActivityRepository.GetVersion(activityId);
            var extensions = _schoolActivityRepository.GetExtensions(activityId, version).ToList();
            if (extensions.Count == 0)
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("无留资活动");

            //和数据库模板匹配的信息
            var extensionsDto = registerDto.Extensions
                .Where(s => s.Id != Guid.Empty && !string.IsNullOrWhiteSpace(s.Value))
                .Where(s => extensions.Exists(e => e.Id == s.Id))
                .ToList();

            if (extensionsDto == null || extensionsDto.Count == 0)
            {
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure($"{ extensions.FirstOrDefault().Name ?? "请填写信息"}");
            }
            if (extensionsDto.Count != extensions.Count)
            {
                var nullExtension = extensions.FirstOrDefault(s => !registerDto.Extensions.Exists(p => p.Id == s.Id));
                var name = nullExtension?.Name ?? "请填写信息";
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure($"{name}");
            }
            //由手机号,  验证验证码
            var smsResult = ValidateSmsCode(activityId, extensions, extensionsDto, out string firstPhone);
            if (smsResult.ErrorCode != ResponseSchoolActivityRegister.Code.OK)
            {
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure(smsResult, smsResult.ErrorMsg);
            }
            //渠道x, 手机不能重复
            if (ExistRegister(activity,customers, registerDto.Channel, firstPhone))
            {
                return ResponseSchoolActivityRegister.ToFailure(ResponseSchoolActivityRegister.Code.ExistChannelXPhone, "您已提交过报名信息，无法重新提交");
            }

            //组装报名信息
            var userId = registerDto.CreatorId == Guid.Empty ? null : registerDto.CreatorId;
            var register = new SchoolActivityRegiste()
            {
                Id = Guid.NewGuid(),
                Phone = firstPhone,
                ActivityId = activityId,
                Channel = registerDto.Channel,
                ExtId = extId,
                Version = version,
                IsDeleted = 0,
                Status = 0, //状态，0：未确认，1：已确认
                CreatorId = userId,
                CreateTime = now,
                ModifierId = userId,
                LastModifyTime = now,
                SignInType = registerDto.IsSignInFromScene ? 2:0
            };
            //设置报名字段 Field0 - Field9
            SetRegisterFields(ref register, extensions, extensionsDto);

            var result = _schoolActivityRepository.Register(register);
            if (!result)
            {
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("报名失败, 请稍后再试");
            }
            return AppServiceResultDto<ResponseSchoolActivityRegister>.Success(new ResponseSchoolActivityRegister(), "报名成功");
        }

        public AppServiceResultDto<ResponseSchoolActivityRegister> UnCheckRegister(SchoolActicityUnCheckRegisterDto registerDto)
        {
            //仅支持凤凰通, 以后扩展
            if (registerDto.Key != "8ACA4465-5DDB-4765-BDDB-148E11D2FC07")
            {
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("无留资活动");
            }

            Guid activityId = registerDto.ActivityId;

            var activity = _schoolActivityRepository.Get(activityId);
            if (activity == null)
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("无留资活动");

            List<int> customers = string.IsNullOrWhiteSpace(activity.Customer) ? new List<int>() :
                activity.Customer.Split(',').Select(q => Convert.ToInt32(q)).ToList();

            var now = DateTime.Now;
            if (activity.Status == (byte)SchoolActivityStatus.Disable)
            {
                return ResponseSchoolActivityRegister.ToFailure(ResponseSchoolActivityRegister.Code.Expired, "活动已结束");
            }
            if (activity.Type == (byte)SchoolActivityType.Limit)
            {
                if (activity.StartTime > now)
                {
                    return ResponseSchoolActivityRegister.ToFailure(ResponseSchoolActivityRegister.Code.NoTime, "活动未开始");
                }
                if (activity.EndTime < now)
                {
                    return ResponseSchoolActivityRegister.ToFailure(ResponseSchoolActivityRegister.Code.Expired, "活动已结束");
                }
            }
            if (activity.Category != (byte)SchoolActivityCategory.Single)
            {
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("系统错误, 仅支持单一活动");
            }
            if (registerDto.Extensions == null || registerDto.Extensions.Count == 0)
            {
                return ResponseSchoolActivityRegister.ToFailure(ResponseSchoolActivityRegister.Code.NoField, "无报名数据");
            }

            var version = _schoolActivityRepository.GetVersion(activityId);
            var extensions = _schoolActivityRepository.GetExtensions(activityId, version).ToList();
            if (extensions.Count == 0)
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("无留资活动");

            var extensionPhones = extensions.Where(s => s.Type == (byte)SchoolActivityExtensionType.Phone);
            var firstPhone = registerDto.Extensions.Where(s => extensionPhones.Any(e => e.Name == s.Name)).FirstOrDefault()?.Value ?? string.Empty;

            //组装报名信息
            Guid? userId = null;
            Guid? extId = activity.ExtId;
            var register = new SchoolActivityRegiste()
            {
                Id = Guid.NewGuid(),
                Phone = firstPhone,
                ActivityId = activityId,
                Channel = string.Empty,
                ExtId = extId,
                Version = version,
                IsDeleted = 0,
                Status = 0, //状态，0：未确认，1：已确认
                CreatorId = userId,
                CreateTime = now,
                ModifierId = userId,
                LastModifyTime = now
            };
            //设置报名字段 Field0 - Field9
            SetRegisterFields(ref register, extensions, registerDto.Extensions);

            var result = _schoolActivityRepository.Register(register);
            if (!result)
            {
                return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure("报名失败, 请稍后再试");
            }
            return AppServiceResultDto<ResponseSchoolActivityRegister>.Success(new ResponseSchoolActivityRegister(), "报名成功");
        }

        private bool ExistRegister(SchoolActivity activity,List<int> customers, string channel, string firstPhone)
        {
            if (firstPhone == string.Empty)
            {
                return false;
            }

            var x = "x";

            //用户参加活动之前报名正常渠道，该次报名正常渠道  报名可成功
            //用户参加活动之前报名x渠道，该次报名x渠道  报名不成功
            //用户参加活动之前报名正常渠道，该次报名x渠道  报名可成功
            //用户参加活动之前报名x渠道，该次报名正常渠道  报名不成功

            if (customers.Count() > 0 && channel == x)
            {
                bool isAfterExsitPhoneX = CheckChannelPhone(customers, channel, firstPhone);
                //只要之前报名存在在x渠道的手机信息，都不可报名成功
                if (isAfterExsitPhoneX)
                {
                    return true;
                }
            }

            //单个活动启用排重，判断所有渠道报名都不可重复，否则只判断x渠道不可重复
            if (activity.IsUnique)
            {
                bool existChannelPhone = _schoolActivityRepository.ExistChannelPhone(activity.Id, channel, firstPhone);

                if (existChannelPhone)
                {
                    return true;
                }
            }
            else
            {
                bool existXChannelPhone = _schoolActivityRepository.ExistChannelPhone(activity.Id, x, firstPhone);

                if (existXChannelPhone)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查 留资或直播留资是否已存在同金主, 电话, 渠道的留资
        /// </summary>
        /// <param name="customers"></param>
        /// <param name="phone"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool CheckChannelPhone(List<int> customers, string channel, string phone)
        {
            //本系统是否有留资
            var ret = _schoolActivityRepository.ExistCustomerChannelPhone(customers, channel, phone);
            //if (!ret)
            //{
            //    //直播系统是否有留资
            //    var liveRet = _liveServiceClient.ExistCustomerChannelPhone(customers, phone).Result;
            //    return liveRet != null && liveRet.Exists;
            //}
            return ret;
        }

        public AppServiceResultDto ExistCustomerChannelPhone(List<int> customers, string phone, string channel)
        {
            var ret = _schoolActivityRepository.ExistCustomerChannelPhone(customers, channel, phone);

            if (ret)
            {
                return AppServiceResultDto.Success();
            }
            return AppServiceResultDto.Failure();
        }

        /// <summary>
        /// 设置字段的值 Field0 - Field9
        /// </summary>
        /// <param name="register"></param>
        /// <param name="extensions"></param>
        /// <param name="extensionsDto"></param>
        private void SetRegisterFields(ref SchoolActivityRegiste register, List<SchoolActivityRegisteExtensionDto> extensions, List<SchoolActivityRegisterDto.Extension> extensionsDto)
        {
            var extensionsCount = extensions.Count;
            var type = typeof(SchoolActivityRegiste);
            var properties = type.GetProperties();
            //字段,  使用sort后的顺序
            for (int i = 0; i < extensionsCount; i++)
            {
                var extension = extensions[i];
                var extensionDto = extensionsDto.FirstOrDefault(s => s.Id == extension.Id);
                if (extensionDto != null)
                {
                    var colName = $"Field{i}";//Field0
                    var prop = properties.FirstOrDefault(s => s.Name.ToLower() == colName.ToLower());
                    if (prop != null)
                    {
                        prop.SetValue(register, extensionDto.Value);
                    }
                }
            }
        }


        private void SetRegisterFields(ref SchoolActivityRegiste register, List<SchoolActivityRegisteExtensionDto> extensions, List<SchoolActicityUnCheckRegisterDto.Extension> extensionsDto)
        {
            var extensionsCount = extensions.Count;
            var type = typeof(SchoolActivityRegiste);
            var properties = type.GetProperties();
            //字段,  使用sort后的顺序
            for (int i = 0; i < extensionsCount; i++)
            {
                var extension = extensions[i];
                var extensionDto = extensionsDto.FirstOrDefault(s => s.Name == extension.Name);
                if (extensionDto != null)
                {
                    var colName = $"Field{i}";//Field0
                    var prop = properties.FirstOrDefault(s => s.Name.ToLower() == colName.ToLower());
                    if (prop != null)
                    {
                        prop.SetValue(register, extensionDto.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="extensions"></param>
        /// <param name="extensionsDto"></param>
        /// <returns></returns>
        public ResponseSchoolActivityRegister ValidateSmsCode(Guid activityId, List<SchoolActivityRegisteExtensionDto> extensions, List<SchoolActivityRegisterDto.Extension> extensionsDto, out string firstPhone)
        {
            var phoneType = (byte)SchoolActivityExtensionType.Phone;
            var phoneExtensions = extensions.Where(s => s.Type == phoneType).ToList();

            firstPhone = string.Empty;
            foreach (var extension in phoneExtensions)
            {
                var extensionDto = extensionsDto.FirstOrDefault(s => s.Id == extension.Id);

                //短信验证
                if (extensionDto == null || string.IsNullOrWhiteSpace(extensionDto.SmsCode))
                    return new ResponseSchoolActivityRegister() { ErrorCode = ResponseSchoolActivityRegister.Code.SmsCodeError, ErrorMsg = "请输入验证码" };

                //输入的手机,验证码
                var phone = extensionDto.Value;
                var smsCode = extensionDto.SmsCode;

                if (string.IsNullOrWhiteSpace(phone))
                    return new ResponseSchoolActivityRegister() { ErrorCode = ResponseSchoolActivityRegister.Code.NoField, ErrorMsg = $"{extension.Name}" };

                if (firstPhone == string.Empty) firstPhone = phone;
                string key = $"SchoolActivity-{extension.Id}-{phone}";

                var validated = _smsService.ValidateRedisCode(key, smsCode).Result;
                if (!validated)
                    return new ResponseSchoolActivityRegister() { ErrorCode = ResponseSchoolActivityRegister.Code.SmsCodeError, ErrorMsg = "验证码输入错误" };
            }
            return new ResponseSchoolActivityRegister();
        }

        public async Task<AppServiceResultDto> SendCode(Guid extensionId, string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return AppServiceResultDto.Failure("请输入手机号");

            string key = $"SchoolActivity-{extensionId}-{phone}";
            var code = await _smsService.GetOrAddRedisCode(key);
            var resp = _smsService.SendCode(phone, code);
            return new AppServiceResultDto()
            {
                Status = resp.code == 200,
                Msg = resp.message
            };
        }

        public bool ExistPhone(Guid activityId, string phone)
        {
            return _schoolActivityRepository.ExistPhone(activityId, phone);
        }

        public async Task<bool> SignIn(Guid activityId, string phone, SignType signType)
        {
            return await _schoolActivityRepository.SignIn(activityId, phone, signType);
        }

        public async Task<bool> HasSignIn(Guid activityId, string phone)
        {
            return await _schoolActivityRepository.HasSignIn(activityId, phone);
        }

        public async Task<IEnumerable<SchoolActivityRegiste>> GetRegiste(Guid activityId)
        {
            return await _schoolActivityRepository.GetRegiste(activityId);
        }

        public async Task<IEnumerable<SchoolActivityRegisteExtension>> GetRegisteFields(Guid activityId)
        {
            return await _schoolActivityRepository.GetRegisteFields(activityId);
        }

        public async Task<SchoolActivity> GetBy(Guid id)
        {
           return await _schoolActivityRepository.GetAsync(id);
        }

        public async Task<IEnumerable<SchoolActivityProcess>> GetProcesses(Guid activityId)
        {
            return await _schoolActivityRepository.GetProcesses(activityId);
        }
    }
}
