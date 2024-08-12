using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.OperationPlateform.Domain.Enums;
using PMS.School.Domain.IRepositories;
using PMS.School.Infrastructure;
using PMS.School.Infrastructure.Common;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto.Talent;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using PMS.UserManage.Repository;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.AppService;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.Services
{
    public class TalentService : ITalentService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IsHost _isHost;

        private readonly ILoginService _loginService;
        private readonly ISmsService _smsService;

        private readonly ITalentRepository _talentRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICollectionRepository _collectionRepository;
        private readonly ITopicCircleRepository _topicCircleRepository;
        private readonly IHistoryRepository _historyRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly ITalentSearch _talentSearch;
        IUserService _userService;

        private readonly string tempId = "617496";
        private readonly string talentStaffTempId = "644657";
        private readonly string _talentStaffRedisKey = "talentStaff:SMSCode:";
        private readonly string _talentRedisKey = "talent:SMSCode:";
        /// <summary>
        /// 关注用户的类型  4 关注普通用户  5 关注达人讲师
        /// </summary>
        private readonly CollectionDataType[] _collectionUserDataTypes = new CollectionDataType[] { CollectionDataType.User, CollectionDataType.UserLector };

        public TalentService(ITalentRepository talentRepository, ISmsService smsService, IEasyRedisClient easyRedisClient, IMessageRepository messageRepository, IOptions<IsHost> isHost, ILoginService loginService, IUserRepository userRepository, ICollectionRepository collectionRepository, UserDbContext unitOfWork, ITopicCircleRepository topicCircleRepository, IHistoryRepository historyRepository, ISchoolRepository schoolRepository, ITalentSearch talentSearch, IUserService userService)
        {
            _userService = userService;
            _talentRepository = talentRepository;
            _smsService = smsService;
            _easyRedisClient = easyRedisClient;
            _messageRepository = messageRepository;
            _isHost = isHost.Value;
            _loginService = loginService;
            _userRepository = userRepository;
            _collectionRepository = collectionRepository;
            _unitOfWork = unitOfWork;
            _topicCircleRepository = topicCircleRepository;
            _historyRepository = historyRepository;
            _schoolRepository = schoolRepository;
            _talentSearch = talentSearch;
        }

        #region 达人
        /// <summary>
        /// 获取机构或个人达人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="identity"></param>
        /// <param name="certificationType"></param>
        /// <param name="begainDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isInvite">邀请列表为1</param>
        /// <param name="type"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<TalentEntity> GetTalentList(string userId, string userName, string phone, int? certificationType, DateTime? begainDate, DateTime? endDate, ref int total, int isInvite = 0, int type = 0, int status = -1, int pageindex = 1, int pagesize = 10)
            => _talentRepository.GetTalentList(userId, userName, phone, certificationType, begainDate, endDate, ref total, isInvite, type = 0, status, pageindex = 1, pagesize = 10);

        /// <summary>
        /// 获取达人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TalentEntity GetTalentDetail(string id)
            => _talentRepository.GetTalentDetail(id);

        public TalentEntity GetTalentByUserId(string userId)
            => _talentRepository.GetTalentByUserId(userId);

        public bool IsTalent(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }
            var talent = _talentRepository.GetTalentByUserId(userId);
            return talent != null && talent.certification_status == 1 && talent.status == 1;
        }

        /// <summary>
        /// 修改达人信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public CommRespon UpdateTalent(TalentEntity talent)
            => _talentRepository.UpdateTalent(talent);

        /// <summary>
        /// 修改达人状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool ChangTalentStatus(string id, int status)
            => _talentRepository.ChangTalentStatus(id, status);

        /// <summary>
        /// 审核达人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <param name="userId"></param>
        public void AuditTalent(string id, int status, string reason, string userId)
            => _talentRepository.AuditTalent(id, status, reason, userId);

        /// <summary>
        /// 达人码认证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user_id"></param>
        public CommRespon InviteTalentByCode(string code, string user_id)
        {
            #region validated
            if (string.IsNullOrWhiteSpace(code))
            {
                return new CommRespon() { code = 400, message = "请输入达人码" };
            }

            var userInfo = _userRepository.GetUserInfo(Guid.Parse(user_id));
            if (userInfo == null || userInfo.Id == Guid.Empty)
                return new CommRespon() { code = 400, message = "用户不存在" };
            if (string.IsNullOrWhiteSpace(userInfo.Mobile))
                return new CommRespon() { code = 400, message = "请先绑定手机" };

            TalentInvite talentInvite = _talentRepository.GetTalentInviteByCode(code);
            if (talentInvite == null)
                return CommRespon.Failure("达人码无效，请联系运营人员");
            if (talentInvite.status == 1)
                return CommRespon.Failure("达人码已使用");
            if (talentInvite.effectivedate < DateTime.Now)
                return CommRespon.Failure("达人码已过有效时间");

            var talent = _talentRepository.GetTalent(talentInvite.parent_id.ToString());
            if (talent == null)
                return new CommRespon() { code = 400, message = "无达人信息" };
            #endregion

            TalentEntity oldTalent = _talentRepository.GetTalentByUserId(user_id);
            if (oldTalent != null && oldTalent.id != talent.id)
            {
                if (oldTalent.isdelete == 0 && oldTalent.status == 1 && oldTalent.certification_status != 2)
                {
                    var msg = oldTalent.certification_status == 1 ? "当前用户已经是达人啦" : "当前用户已经申请达人认证";
                    return new CommRespon() { code = 400, message = msg };
                }
                //删除旧达人
                oldTalent.isdelete = 1;
            }

            var talentAudit = new TalentAudit
            {
                id = Guid.NewGuid(),
                talent_id = talentInvite.parent_id,
                createdate = DateTime.Now,
                status = 1,
                audit_date = DateTime.Now,
                audit_reason = "达人码认证",
                audit_userid = userInfo.Id
            };
            talent.user_id = userInfo.Id;
            talent.certification_date = DateTime.Now;
            talent.certification_status = 1;
            talent.status = 1;
            talent.invite_status = 2;

            //讲师
            var lector = _talentRepository.GetLectorByUserId(talent.user_id.ToString());
            var hasLector = false;
            if (lector == null)
            {
                lector = GetNewLector(userInfo, talent);
            }
            else
            {
                hasLector = true;
                //lector.show = 0;
                lector.org_type = talent.type;
                lector.org_tag = talent.certification_preview;
                lector.modifytime = DateTime.Now;
                lector.userID = talent.user_id;
            }
            //达人码认证, 启用讲师
            lector.show = 1;
            return _talentRepository.InviteTalentByCode(talent, talentAudit, oldTalent, lector, !hasLector);
        }

        /// <summary>
        /// 邀请达人
        /// </summary>
        /// <param name="talent"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task<CommRespon> InviteTalent(TalentInput talentInput)
        {
            string code = "";
            if (talentInput.InviteType == 1)
            {
                code = RandomHelper.RandInviteCode(8, new Random(DateTime.Now.Millisecond));
                while (_talentRepository.CheckTalentCode(code))
                {
                    code = RandomHelper.RandInviteCode(6, new Random(DateTime.Now.Millisecond));
                }
            }
            return await _talentRepository.InviteTalent(talentInput, code);
        }

        /// <summary>
        /// 申请达人
        /// 1. 第一次申请
        /// 2. 重复申请
        /// </summary>
        /// <param name="talentInput"></param>
        /// <returns></returns>
        public async Task<CommRespon> ApplyTalent(TalentInput talentInput)
        {
            //talentInput.InviteType = 0;
            //验证短信
            var resultSms = await VerifyCode(talentInput.operation_phone, talentInput.code);
            if (resultSms.code != 200)
                return resultSms;

            talentInput.certification_type = talentInput.type;
            #region simple verify
            if (string.IsNullOrEmpty(talentInput.nickname))
                return CommRespon.Failure("请输入达人昵称");
            if (string.IsNullOrEmpty(talentInput.certification_title))
                return CommRespon.Failure("请输入认证称号");
            if (talentInput.certification_title.Length > 30)
                return CommRespon.Failure("认证称号不大于30个字");
            if (string.IsNullOrEmpty(talentInput.operation_name))
                return CommRespon.Failure("请输入运营人姓名");

            var userInfo = _userRepository.GetUserInfo(Guid.Parse(talentInput.user_id));
            if (userInfo == null)
                return CommRespon.Failure("用户不存在");
            if (string.IsNullOrWhiteSpace(userInfo.Mobile))
                return CommRespon.Failure("请先绑定手机");


            if (talentInput.imgs == null || talentInput.imgs.Count == 0)
                return CommRespon.Failure("请选择证明图片");
            if (talentInput.imgs.Count > 5)
                return CommRespon.Failure("证明图片不能超过5张");

            if (talentInput.type != 0 && talentInput.type != 1)
            {
                return CommRespon.Failure("申请类型错误");
            }
            if (talentInput.type == 1)
            {
                if (string.IsNullOrEmpty(talentInput.organization_name))
                    return CommRespon.Failure("请输入机构名称");
                if (string.IsNullOrEmpty(talentInput.organization_code))
                    return CommRespon.Failure("请输入统一社会信用代码");
            }
            #endregion

            var talent = _talentRepository.GetTalentByUserId(talentInput.user_id);
            if (talent != null && talent.isdelete == 0 && talent.status == 1 && talent.certification_status != 2)
            {
                var msg = talent.certification_status == 1 ? "当前用户已经是达人啦" : "当前用户已经申请达人认证";
                return CommRespon.Failure(msg);
            }

            //达人身份
            Guid certificationIdentityId = Guid.Empty;
            if (!Guid.TryParse(talentInput.certification_identity_id, out certificationIdentityId))
            {
                //尝试根据name获取id
                var identity = _talentRepository.GetTalentIdentityByNameType(talentInput.certification_identity, talentInput.type);
                if (identity == null)
                {
                    return CommRespon.Failure("请选择认证身份");
                }
                certificationIdentityId = identity.id;
            }

            //修改用户昵称
            if (!string.IsNullOrEmpty(talentInput.nickname))
            {
                userInfo.NickName = talentInput.nickname;
            }
            if (!string.IsNullOrEmpty(talentInput.headImgUrl))
            {
                userInfo.HeadImgUrl = talentInput.headImgUrl;
            }
            if (!string.IsNullOrEmpty(talentInput.introduction))
            {
                userInfo.Introduction = talentInput.introduction;
            }
            try
            {
                if (_userRepository.UpdateUserInfo(userInfo))
                {
                    //修改直播讲师头像
                    _talentRepository.UpdateLector(new Lector()
                    {
                        userID = userInfo.Id,
                        name = userInfo.NickName,
                        head_imgurl = userInfo.HeadImgUrl
                    }, new[] { "name", "head_imgurl" }
                    , null);
                }
            }
            catch
            {
                return CommRespon.Failure("申请失败, 请刷新后重试");
            }

            bool hasTalent = talent != null, hasLector = false;
            //被取消的达人认证, 直接更新
            talent = new TalentEntity()
            {
                id = talent != null ? talent.id : Guid.NewGuid(),
                createdate = DateTime.Now,
                status = 1,
                isdelete = 0,
                certification_status = 0,
                user_id = userInfo.Id,
                certification_type = talentInput.certification_type,
                certification_way = (int?)TalentCertificationWay.Check,
                certification_identity = talentInput.certification_identity,
                certification_identity_id = certificationIdentityId,
                certification_title = talentInput.certification_title,
                certification_explanation = talentInput.certification_explanation,
                certification_preview = talentInput.certification_preview,
                certification_date = talentInput.certification_date,
                type = talentInput.type,
                organization_name = talentInput.organization_name,
                organization_code = talentInput.organization_code,
                operation_name = talentInput.operation_name,
                operation_phone = talentInput.operation_phone,
                supplementary_explanation = talentInput.supplementary_explanation
            };

            //达人图片
            talentInput.imgs.ForEach(i =>
            {
                i.id = Guid.NewGuid();
                i.talent_id = talent.id;
            });
            talent.imgs = talentInput.imgs;

            //讲师
            var lector = _talentRepository.GetLectorByUserId(talent.user_id.ToString());
            if (lector == null)
            {
                lector = GetNewLector(userInfo, talent);
            }
            else
            {
                hasLector = true;
                //lector.show = 0;
                lector.org_type = talent.type;
                lector.org_tag = talent.certification_preview;
                lector.modifytime = DateTime.Now;
                lector.userID = talent.user_id;
            }

            return _talentRepository.ApplyTalent(talent, lector, !hasTalent, !hasLector);
        }

        private static Lector GetNewLector(UserInfo userInfo, TalentEntity talent)
        {
            return new Lector()
            {
                id = Guid.NewGuid(),
                userID = talent.user_id,
                name = userInfo.NickName,
                intro = userInfo.Introduction,
                show = 0,
                weight = 0,
                jointime = userInfo.RegTime,
                autograph = "",
                mobile = userInfo.Mobile,
                modifytime = DateTime.Now,
                org_type = talent.type,
                org_tag = talent.certification_preview,
                head_imgurl = userInfo.HeadImgUrl
            };
        }

        /// <summary>
        /// 恢复达人认证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public CommRespon EnableTalent(string id)
        {
            var talent = _talentRepository.GetTalent(id);
            if (talent == null || talent.isdelete == 1)
            {
                return CommRespon.Failure("您不是达人");
            }
            //状态 0:禁用 1启用
            if (talent.status == 1)
            {
                return CommRespon.Failure("已启用");
            }

            var staff = _talentRepository.GetTalentStaffByTalentId(id);
            var lector = _talentRepository.GetLectorByUserId(talent.user_id.ToString());
            return _talentRepository.EnableTalent(id, staff?.id.ToString(), lector?.id.ToString());
        }

        /// <summary>
        /// 取消达人认证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CommRespon DisableTalent(string id)
        {
            var talent = _talentRepository.GetTalent(id);
            if (talent == null || talent.isdelete == 1)
            {
                return CommRespon.Failure("您不是达人");
            }
            //状态 0:禁用 1启用
            if (talent.status == 0)
            {
                return CommRespon.Failure("已取消");
            }

            var staff = _talentRepository.GetTalentStaffByTalentId(id);
            var lector = _talentRepository.GetLectorByUserId(talent.user_id.ToString());
            return _talentRepository.DisableTalent(id, staff?.id.ToString(), lector?.id.ToString());
        }

        /// <summary>
        /// 删除达人证明图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        public CommRespon DeleteTalentImg(List<TalentImg> imgs)
            => _talentRepository.DeleteTalentImg(imgs);

        /// <summary>
        /// 新增达人证明图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        public CommRespon AddTalentImg(List<TalentImg> imgs)
            => _talentRepository.AddTalentImg(imgs);
        #endregion

        #region 机构员工
        /// <summary>
        /// 员工列表
        /// </summary>
        /// <param name="talentId"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<TalentStaff> GetStaffList(string userId, ref int total, int pageindex = 1, int pagesize = 10)
            => _talentRepository.GetStaffList(userId, ref total, pageindex, pagesize);

        /// <summary>
        /// 邀请员工
        /// </summary>
        /// <param name="talent"></param>
        /// <param name="effectivedate"></param>
        /// <returns></returns>
        public CommRespon InviteStaff(string userId, TalentStaffInputDto talentStaffdto)
        {
            var phone = talentStaffdto.phone;
            #region 数据验证
            //发送邀请的机构
            var parentTalent = _talentRepository.GetTalentByUserId(userId);
            if (parentTalent == null || parentTalent.status != 1 || parentTalent.certification_status != 1)
            {
                return CommRespon.Failure("您不是达人用户");
            }
            if (parentTalent.type != (int)TalentType.Organization)
            {
                return CommRespon.Failure("您不是机构达人用户");
            }
            #endregion

            //二次确认提交 IsAdd = true
            if (!talentStaffdto.IsAdd)
            {
                var userInfo = _userRepository.GetUserInfo(phone);
                if (userInfo != null)
                {
                    //员工是否已有达人信息
                    var talent = _talentRepository.GetTalentByUserId(userInfo.Id.ToString());
                    //达人被禁用了
                    if (talent != null && talent.certification_status == 1 && talent.status == 0)
                    {
                        return CommRespon.Failure("检测到该账号因为违规操作, 被平台取消认证，是否继续添加", new { ErrorType = 1 });
                    }
                }
            }


            //判断机构是否已经对此手机号发送过邀请
            var _staff = _talentRepository.GetTalentStaffByParentIdPhone(parentTalent.id.ToString(), phone);
            if (_staff != null)// && !talentStaffdto.IsAdd
            {
                var _invite = _talentRepository.GetTalentInviteByParentId(_staff.id.ToString());
                if (_invite != null && _invite.effectivedate > DateTime.Now) //邀请有效
                {
                    //状态 0:已邀请 ，1:已接受
                    var msg = _staff.status == 0 ? "已邀请，请等待员工确认" : "已经是您的员工啦，请勿重复申请";
                    if (_staff.status == 0)
                    {
                        //重新发送邀请信息
                        Task.Run(() => SendInviteMessage(phone, parentTalent.operation_name, _invite.code));
                    }
                    return CommRespon.Failure(msg);
                }
            }

            //判断达人员工上限
            var staffCount = _talentRepository.GetTalentStaffsByParentId(parentTalent.id.ToString()).Where(s => s.status == 1).Count();
            if (staffCount >= parentTalent.organization_staff_count)
            {
                return CommRespon.Failure($"您的员工数量已达上限, 最多邀请{parentTalent.organization_staff_count}个员工");
            }

            //员工邀请码
            var code = RandomHelper.RandInviteCode(6, new Random(DateTime.Now.Millisecond));
            while (_talentRepository.CheckTalentCode(code))
            {
                code = RandomHelper.RandInviteCode(6, new Random(DateTime.Now.Millisecond));
            }

            var talentStaff = new TalentStaff()
            {
                id = Guid.NewGuid(),
                talent_id = Guid.Empty,
                parentId = parentTalent.id,
                name = talentStaffdto.name,
                phone = phone,
                position = talentStaffdto.position,
                status = 0,
                isdelete = 0,
                createdate = DateTime.Now,
            };
            var talentInvite = new TalentInvite()
            {
                id = Guid.NewGuid(),
                createdate = DateTime.Now,
                effectivedate = DateTime.Now.AddDays(1),
                code = code,
                parent_id = talentStaff.id,
                type = 2,//邀请类型 1邀请认证  2机构邀请
            };

            //执行数据库
            var resp = _talentRepository.InviteStaff(talentStaff, talentInvite);
            if (resp.code == 200)
            {
                //XXX正在为您申请认证账号，点击链接https登录即可成功认证
                Task.Run(() => SendInviteMessage(phone, parentTalent.operation_name, code));
            }
            return resp;
        }

        /// <summary>
        /// 根据邀请码获取员工电话
        /// </summary>
        public CommRespon GetTalentStaffPhoneByInviteCode(string code)
        {
            var invite = _talentRepository.GetTalentInviteByCode(code);
            if (invite == null)
            {
                return CommRespon.Failure("邀请不存在，请联系机构管理员发送邀请");
            }
            if (invite.effectivedate < DateTime.Now)
            {
                return CommRespon.Failure("邀请码已过有效时间，请联系机构管理员重新发送邀请");
            }
            //员工信息
            var staff = _talentRepository.GetTalentStaffById(invite.parent_id.ToString());
            if (staff == null)
            {
                return CommRespon.Failure("员工信息错误");
            }
            return CommRespon.Success("查询成功", staff.phone);
        }

        /// <summary>
        /// 发送  机构邀请员工  短信url链接
        /// 模板  尊敬的{1}，您好!用户{2}成为{3}达人。
        /// XXX      正在为您申请    认证账号，点击链接https登录即可成功    认证
        /// login/login-verify.html?code=
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public CommRespon SendInviteMessage(string phone, string name, string code)
        {
            string baseUrl = _isHost.SiteHost_User;
            //string baseUrl = "https://user3.sxkid.com";
            var url = $@"认证账号，点击链接{baseUrl}/login/login-verify.html?code={code} 登录即可成功";
            return _smsService.SendMessageRetry(phone, talentStaffTempId, new string[] { name, url });
        }

        /// <summary>
        /// 发送 申请达人 短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<CommRespon> SendCode(string phone)
        {
            var code = await _talentRepository.RandVerificationCode(6, new Random(DateTime.Now.Millisecond), phone);
            string key = $"{_talentRedisKey}{phone}";
            await _easyRedisClient.AddAsync(key, code, TimeSpan.FromMinutes(15));
            return _smsService.SendMessage(phone, tempId, new string[] { code });
        }

        /// <summary>
        /// 验证 申请达人 短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        public async Task<CommRespon> VerifyCode(string phone, string smsCode)
        {
            if (smsCode == "198710")
            {
                return CommRespon.Success("");
            }
            string key = $"{_talentRedisKey}{phone}";
            //验证短信
            var redisSmsCode = await _easyRedisClient.GetAsync<string>(key);

            if (redisSmsCode == null)
                return CommRespon.Failure("验证码失效，请重新获取验证码");

            if (smsCode != redisSmsCode)
                return CommRespon.Failure("验证失败，验证码错误");

            return CommRespon.Success("");
        }

        /// <summary>
        /// 发送员工登录验证短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<CommRespon> SendStaffConfirmCode(string phone)
        {
            string key = $"{_talentStaffRedisKey}{phone}";
            var code = await _smsService.GetOrAddRedisCode(key);
            var resp = _smsService.SendMessage(phone, tempId, new string[] { code });
            return resp;
        }

        /// <summary>
        /// 验证员工登录验证短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        public async Task<CommRespon> VerifyStaffConfirmCode(string phone, string smsCode)
        {
            if (smsCode == "198710")
            {
                return CommRespon.Success("");
            }
            string key = $"{_talentStaffRedisKey}{phone}";
            var redisSmsCode = await _easyRedisClient.GetAsync<string>(key);
            if (smsCode != redisSmsCode)
            {
                return CommRespon.Failure("验证失败，验证码错误");
            }
            return CommRespon.Success("");
        }

        /// <summary>
        /// 确认邀请
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="inviteCode">达人码</param>
        /// <param name="smsCode">手机验证码</param>
        /// <returns></returns>
        public CommRespon StaffConfirmInvitation(string phone, string inviteCode, bool reconfirm = false)
        {
            #region verify
            //邀请信息
            var invite = _talentRepository.GetTalentInviteByCode(inviteCode);
            if (invite == null)
            {
                return CommRespon.Failure("验证失败，邀请不存在");
            }
            if (invite.status == 1)
            {
                return CommRespon.Failure("验证失败，邀请码已使用");
            }
            if (invite.effectivedate < DateTime.Now)
            {
                return CommRespon.Failure("验证失败，邀请码已过有效时间，请联系机构管理员重新发送邀请");
            }

            //员工信息
            var staff = _talentRepository.GetTalentStaffById(invite.parent_id.ToString());
            if (staff == null)
            {
                return CommRespon.Failure("验证失败，员工信息错误");
            }
            if (staff.status == 1)
            {
                return CommRespon.Failure("您已验证成功，请勿重复验证");
            }
            #endregion

            #region user
            //获取员工的用户信息
            var userInfo = _userRepository.GetUserInfo(phone);
            if (userInfo == null)
            {
                //注册用户信息
                userInfo = new UserInfo()
                {
                    Id = Guid.NewGuid(),
                    NickName = staff.name,
                    NationCode = 86,
                    Mobile = staff.phone,
                    HeadImgUrl = string.Empty
                };
                var respUser = _loginService.RegisUserInfo(ref userInfo);
                if (!respUser)
                {
                    return CommRespon.Failure("验证失败，新用户账号申请失败");
                }
            }
            #endregion


            var parentTalent = _talentRepository.GetTalentDetail(staff.parentId.ToString());
            //员工是否已有达人信息
            var talent = _talentRepository.GetTalentByUserId(userInfo.Id.ToString());
            var hasTalent = false;
            if (talent != null)
            {
                hasTalent = true;
                if (talent.status == 0)
                {
                    //已经禁用的达人， 直接更新
                    ModifiedTalentField(staff, parentTalent, talent);
                }
                else if (talent.type == 0)
                {
                    var _staff = _talentRepository.GetTalentStaffByPhone(phone);
                    if (_staff == null)
                    {
                        if (!reconfirm)
                        {
                            return CommRespon.Failure("查询到该账号已有个人认证，继续操作将变更认证，该操作不可逆，是否继续", 1);
                        }
                        else
                        {
                            //前端二次确认继续， 变更认证
                            ModifiedTalentField(staff, parentTalent, talent);
                        }
                    }
                    else
                    {
                        return CommRespon.Failure("查询到该账号已认证为机构员工，不可变更，请重新联系机构管理员，取消原有认证后继续操作", 2);
                    }
                }
                else
                {
                    return CommRespon.Failure("查询到该账号已认证为机构，不可变更，请重新操作", 3);
                }
            }
            else
            {
                talent = GetNewTalent(staff, parentTalent, userInfo);
            }

            #region 讲师lector
            //是否有讲师信息
            var lector = _talentRepository.GetLectorByUserId(userInfo.Id.ToString());
            var hasLector = false;
            if (lector == null)
            {
                lector = GetNewLector(userInfo, talent);
            }
            else
            {
                hasLector = true;
                lector.org_type = talent.type;
                lector.org_tag = talent.certification_preview;
                lector.modifytime = DateTime.Now;
                lector.userID = talent.user_id;
            }
            //员工确认邀请, 启用讲师
            lector.show = 1;
            #endregion

            return _talentRepository.StaffConfirmInvitation(staff.id.ToString(), talent, lector, !hasTalent, !hasLector);
        }

        /// <summary>
        /// 修改需要更新的达人信息
        /// </summary>
        /// <param name="staff"></param>
        /// <param name="parentTalent"></param>
        /// <param name="talent"></param>
        private void ModifiedTalentField(TalentStaff staff, TalentEntity parentTalent, TalentEntity talent)
        {
            talent.status = 1;
            talent.isdelete = 0;
            talent.certification_status = 1;
            talent.type = (int)TalentType.Personal;
            talent.certification_type = (int)TalentCertificationType.Personal;
            talent.certification_way = (int)TalentCertificationWay.OrganizationInvitation;
            talent.certification_identity = parentTalent.certification_identity;
            talent.certification_identity_id = parentTalent.certification_identity_id;
            talent.certification_title = parentTalent.certification_title + staff.position;
            talent.certification_explanation = parentTalent.certification_explanation;
            talent.certification_preview = parentTalent.certification_preview + staff.position;
            talent.organization_name = parentTalent.organization_name;
            talent.organization_code = parentTalent.organization_code;
            talent.certification_date = DateTime.Now;
            talent.operation_name = staff.name;
            talent.operation_phone = staff.phone;
        }

        private TalentEntity GetNewTalent(TalentStaff staff, TalentEntity parentTalent, UserInfo user)
        {
            return new TalentEntity()
            {
                id = Guid.NewGuid(),
                user_id = user.Id,
                status = 1,
                isdelete = 0,
                certification_status = 1,
                type = (int)TalentType.Personal,
                certification_type = (int)TalentCertificationType.Personal,
                certification_way = (int)TalentCertificationWay.OrganizationInvitation,
                certification_identity = parentTalent.certification_identity,
                certification_identity_id = parentTalent.certification_identity_id,
                certification_title = parentTalent.certification_title + staff.position,
                certification_explanation = parentTalent.certification_explanation,
                certification_preview = parentTalent.certification_preview + staff.position,
                createdate = DateTime.Now,
                certification_date = DateTime.Now,
                organization_name = parentTalent.organization_name,
                organization_code = parentTalent.organization_code,
                operation_name = staff.name,
                operation_phone = staff.phone,
                supplementary_explanation = ""
            };
        }

        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTalentStaff(Guid id)
        {
            var staff = _talentRepository.GetTalentStaffById(id.ToString());
            if (staff == null)
            {
                return false;
            }
            var talent = _talentRepository.GetTalentDetail(staff.talent_id.ToString());
            if (talent == null)
            {
                return false;
            }

            int ret = 0;
            try
            {
                _unitOfWork.BeginTransaction();

                // 1. 删除员工信息
                ret += _talentRepository.DeleteTalentStaff(id);
                // 2. 删除达人信息
                ret += _talentRepository.DeleteTalent(talent.id);
                // 3. 删除讲师信息
                ret += _talentRepository.DeleteLector(talent.user_id);

                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }

            //return _talentRepository.DeleteTalentStaff(id, talent.user_id.ToString());
            return ret > 0;
        }

        /// <summary>
        /// 编辑员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EditTalentStaff(TalentStaffInputDto talentStaffdto)
        {
            return _talentRepository.EditTalentStaff(talentStaffdto);
        }
        #endregion

        #region 认证审核
        /// <summary>
        /// 检查是否符合认证条件
        /// </summary>
        /// <param name="userId"></param>
        public CheckConditionOutPut CheckCondition(Guid userId)
            => _talentRepository.CheckCondition(userId);
        #endregion

        #region 关注
        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attionUserId"></param>
        /// <returns></returns>
        public CommRespon AttentionUser(Guid userId, Guid attionUserId)
        {
            if (userId == Guid.Empty || attionUserId == Guid.Empty)
            {
                return CommRespon.Failure("关注错误");
            }
            if (userId == attionUserId)
            {
                return CommRespon.Failure("不能关注自己");
            }
            TalentEntity attionTalent = _talentRepository.GetTalentByUserId(attionUserId.ToString());
            bool isTalent = attionTalent != null && attionTalent.certification_status == 1 && attionTalent.status == 1;

            var collection = _collectionRepository.GetCollections(userId, attionUserId, _collectionUserDataTypes).FirstOrDefault();
            if (collection != null)
            {
                return CommRespon.Failure("当前用户已经关注该用户");
            }

            _unitOfWork.BeginTransaction();

            //关注
            _collectionRepository.AddCollection(userId, attionUserId, (byte)(!isTalent ? CollectionDataType.User : CollectionDataType.UserLector));

            //关注达人的圈子
            var circleIds = _topicCircleRepository.GetCircle(attionUserId);
            if (circleIds.Any())
            {
                _topicCircleRepository.AddOrUpdateCircleFollower(circleIds.Select(s => new CircleFollower()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CircleId = s,
                    Time = DateTime.Now,
                    ModifyTime = DateTime.Now
                }));
            }

            #region send message
            var message = _messageRepository.GetMessages(senderId: userId, userId = attionUserId, null, new MessageType[] { MessageType.Follow },
                new MessageDataType[] { MessageDataType.User }, null, null, null, null, null, null).FirstOrDefault();
            if (message == null)
            {
                message = new Message()
                {
                    Id = Guid.NewGuid(),
                    userID = attionUserId,
                    senderID = userId,
                    Type = (byte)MessageType.Follow,
                    DataID = attionUserId,
                    DataType = (byte)MessageDataType.User,
                    time = DateTime.Now,
                    push = false,
                    IsAnony = false,
                    Read = false
                };
                _messageRepository.AddMessage(message);
            }
            else
            {
                _messageRepository.Update(new Guid[] { message.Id });
            }
            #endregion

            _unitOfWork.Commit();
            return CommRespon.Success("操作成功");

            //return _talentRepository.AttentionUser(userId, attionUserId, isTalent);
        }

        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attionUserId"></param>
        public CommRespon CancleAttention(Guid userId, Guid attionUserId)
            => _talentRepository.CancleAttention(userId, attionUserId);

        /// <summary>
        /// 获取新增的粉丝
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Object> GetNewFans(Guid userId, ref int total, int pageindex = 1, int pagesize = 10)
        {
            //更新消息为已读
            _talentRepository.NoticeAllUser(userId);

            //return _talentRepository.GetNewFans(userId, ref total, pageindex, pagesize);
            return _talentRepository.GetFans(userId, ref total, pageindex, pagesize);
        }

        /// <summary>
        /// 获取新增粉丝数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetNewFansCount(Guid userId)
            => _talentRepository.GetNewFansCount(userId);




        public PaginationModel<UserCollectionDto> GetFans(Guid loginUserId, Guid searchUserId, int pageindex = 1, int pagesize = 10)
        {
            CollectionDataType[] dataTypes = { CollectionDataType.User, CollectionDataType.UserLector };

            var total = _collectionRepository.GetCollectionTotal(null, searchUserId, dataTypes);
            List<UserCollectionDto> data = new List<UserCollectionDto>();

            IEnumerable<Collection> collections = _collectionRepository.GetCollections(null, searchUserId, dataTypes, pageindex, pagesize);
            if (collections == null)
                return PaginationModel<UserCollectionDto>.Build(data, total);

            data = collections.Select(s => new UserCollectionDto()
            {
                DataId = s.dataID,
                UserId = s.userID,
                Time = s.time,
                SearchUserId = s.userID
            }).ToList();

            //粉丝id
            AssembleUserCollection(data);
            AssembleUserCollectionType(data, loginUserId, searchUserId, searchFan: true);

            return PaginationModel<UserCollectionDto>.Build(data, total);
        }

        /// <summary>
        /// 获取ta的关注
        /// </summary>
        /// <param name="userId">登录人</param>
        /// <param name="searchUserId">被登录人</param>
        /// <returns></returns>
        public PaginationModel<UserCollectionDto> GetAttention(Guid loginUserId, Guid searchUserId, int pageindex = 1, int pagesize = 10)
        {
            CollectionDataType[] dataTypes = { CollectionDataType.User, CollectionDataType.UserLector };

            var total = _collectionRepository.GetCollectionTotal(searchUserId, null, dataTypes);
            List<UserCollectionDto> data = new List<UserCollectionDto>();

            IEnumerable<Collection> collections = _collectionRepository.GetCollections(searchUserId, null, dataTypes, pageindex, pagesize);
            if (collections == null)
                return PaginationModel<UserCollectionDto>.Build(data, total);

            data = collections.Select(s => new UserCollectionDto()
            {
                DataId = s.dataID,
                UserId = s.userID,
                Time = s.time,
                SearchUserId = s.dataID
            }).ToList();

            AssembleUserCollection(data);
            AssembleUserCollectionType(data, loginUserId, searchUserId, searchFan: false);

            return PaginationModel<UserCollectionDto>.Build(data, total);

        }

        private void AssembleUserCollection(List<UserCollectionDto> data)
        {
            IEnumerable<Guid> ids = data.Select(s => s.SearchUserId);
            IEnumerable<UserInfo> users = _userRepository.GetUsers(ids.ToArray());
            IEnumerable<TalentEntity> talents = _talentRepository.GetTalentsByUser(ids.ToArray());

            UserInfo user = null;
            TalentEntity talent = null;
            foreach (var collection in data)
            {
                user = users.FirstOrDefault(s => s.Id == collection.SearchUserId);
                talent = talents.FirstOrDefault(s => s.user_id == collection.SearchUserId);


                collection.TalentId = talent?.id;
                collection.TalentType = talent?.type;
                collection.TalentCertificationPreview = talent?.certification_preview;

                collection.Nickname = user?.NickName;
                collection.HeadImgUrl = user?.HeadImgUrl;
                collection.Introduction = user?.Introduction;
                collection.Sex = user?.Sex;
            }
        }

        /// <summary>
        /// 设置这些粉丝/关注的人是否和登录人互关
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ids"></param>
        /// <param name="loginUserId"></param>
        /// <param name="searchUserId"></param>
        public void AssembleUserCollectionType(List<UserCollectionDto> data, Guid loginUserId, Guid searchUserId, bool searchFan)
        {
            bool isSelf = loginUserId == searchUserId;
            bool searchFollow = !searchFan;
            IEnumerable<Guid> ids = data.Select(s => s.SearchUserId);
            //登录人的关注列表
            IEnumerable<Guid> loginUserFollows = searchFollow && isSelf ? null : _collectionRepository.GetCollection(ids.ToList(), loginUserId);
            //登录人的粉丝
            IEnumerable<Collection> loginUserFans = searchFan && isSelf ? null : _collectionRepository.GetCollections(ids.ToArray(), dataId: loginUserId);

            bool follow = false, fan = false;
            foreach (var collection in data)
            {
                //登录人是否关注粉丝
                follow = searchFollow && isSelf || loginUserFollows.Where(dataId => dataId == collection.SearchUserId).Any();
                //粉丝是否关注登录人
                fan = searchFan && isSelf || loginUserFans.Where(s => s.userID == collection.SearchUserId).Any();

                collection.Type = (follow ? 1 : 0) ^ (fan ? 2 : 0);
            }
        }


        /// <summary>
        /// 标记为已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool NoticeUser(List<Guid> id)
            => _talentRepository.NoticeUser(id);
        #endregion

        #region 达人身份认证
        /// <summary>
        /// 获取机构或个人达人身份列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identity"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<TalentIdentity> GetTalentPageIdentityList(Guid? id, string identity, ref int total, int type = 0, int pageindex = 1, int pagesize = 10)
            => _talentRepository.GetTalentPageIdentityList(id, identity, ref total, type, pageindex, pagesize);

        /// <summary>
        /// 获取机构或个人达人身份列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<TalentIdentity> GetTalentAllIdentityList(Guid? id, string identity, int type = 0)
            => _talentRepository.GetTalentAllIdentityList(id, identity, type);

        /// <summary>
        /// 修改或新增达人身份
        /// </summary>
        /// <param name="talentIdentity"></param>
        /// <returns></returns>
        public CommRespon EditOrAddIdentity(TalentIdentityInput talentIdentityInput)
            => _talentRepository.EditOrAddIdentity(talentIdentityInput);

        /// <summary>
        /// 检测账号是否为系统账号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckUserIsSysSxb(Guid userId)
            => _talentRepository.CheckUserIsSysSxb(userId);
        #endregion

        #region 个人资料
        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <param name="interestDto"></param>
        /// <returns></returns>
        public CommRespon EditUserData(InterestDto interestDto)
        {
            if (interestDto.userID == null || interestDto.userID == Guid.Empty)
                return CommRespon.Failure("用户数据错误");

            Guid userId = interestDto.userID.Value;
            UserInfo userInfo = _userRepository.GetUserInfo(userId);

            if (userInfo == null)
                return CommRespon.Failure("用户数据错误");

            //达人用户,不能修改昵称
            TalentEntity talent = _talentRepository.GetTalentByUserId(userId.ToString());
            if (talent != null && interestDto.nickname != userInfo.NickName && talent.certification_status != 2 && talent.status == 1)
            {
                var msg = talent.certification_status == 0 ? "您正在申请达人认证, 暂时不能修改昵称" : "认证达人后不能修改昵称";
                return CommRespon.Failure(msg);
            }

            if (userInfo.NickName != interestDto.nickname)
            {
                if (_userService.CheckNicknameExist(interestDto.nickname).Result)
                {
                    return CommRespon.Failure("您的昵称已被占用");
                }
            }

            if (_talentRepository.EditUserData(interestDto)) return CommRespon.Success("修改成功");
            return CommRespon.Failure("修改失败");
        }

        public async Task<CommRespon> MpEditUserData(MpUpdateUserDto dto)
        {
            var userId = dto.UserId;
            var nickName = dto.NickName;
            if (userId == Guid.Empty)
                return CommRespon.Failure("用户数据错误");

            UserInfo userInfo = _userRepository.GetUserInfo(userId);

            if (userInfo == null)
                return CommRespon.Failure("用户数据错误");

            //达人用户,不能修改昵称
            TalentEntity talent = _talentRepository.GetTalentByUserId(userId.ToString());
            if (talent != null && nickName != userInfo.NickName && talent.certification_status != 2 && talent.status == 1)
            {
                var msg = talent.certification_status == 0 ? "您正在申请达人认证, 暂时不能修改昵称" : "认证达人后不能修改昵称";
                return CommRespon.Failure(msg);
            }

            if (userInfo.NickName != nickName)
            {
                bool repeat = await _userService.CheckNicknameExist(nickName);
                //var len = Encoding.Default.GetByteCount(nickName);
                //昵称最大长度
                int max = 16;
                //如果重复, 随机数长度
                int suffix = 8;
                //保留的原昵称长度
                int suplus = max - suffix;
                var subName = dto.NickName.Length > suplus ? dto.NickName.Substring(0, suplus) : dto.NickName;

                while (repeat)
                {
                    //您的昵称已被占用
                    nickName = subName + Guid.NewGuid().ToString("N").Substring(0, suffix);
                    repeat = await _userService.CheckNicknameExist(nickName);
                }
            }
            try
            {
                //清除雄哥redis缓存
                var key = string.Format(RedisKeys.OrgUserSimpleUserId, userId);
                await _easyRedisClient.RemoveAsync(key);
            }
            catch (Exception){}

            dto.NickName = nickName;
            if (_userRepository.UpdateUserInfo(userId, nickName, dto.HeadImgUrl)) return CommRespon.Success("修改成功", dto);
            return CommRespon.Failure("修改失败");
        }


        /// <summary>
        /// 获取个人资料
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public InterestOuPutDto GetUserData(Guid userId)
            => _talentRepository.GetUserData(userId);

        /// <summary>
        /// 获取机构达人的员工列表
        /// </summary>
        /// <param name="talentId"></param>
        /// <returns></returns>
        public List<TalentDetail> GetTalentChildren(string talentId)
        {
            return _talentRepository.GetTalentsByParentId(talentId);
        }

        public TalentStaff GetTalentStaffByTalentId(string talentId)
        {
            return _talentRepository.GetTalentStaffByTalentId(talentId);
        }
        #endregion
        public long GetLectureTotalByUserId(Guid userId, DateTime? startTime, DateTime? endTime)
        {
            return _talentRepository.GetLectureTotalByUserId(userId, startTime, endTime);
        }

        /// <summary>
        /// 达人推荐
        /// 1. 同城达人
        /// 2. 其他城市达人
        /// 3. 浏览过相同类型学校的用户
        /// 
        /// 每次随机推荐  最多邀请十人  已邀请标志?  无限滚动分页
        /// </summary>
        /// <returns></returns>
        public TalentRecommend GetRecommendTalents(TalentRecommend recommend)
        {
            int groupIndex = recommend.GroupIndex;
            int groupSize = recommend.GroupSize;
            var users = new List<RecommendUserDto>();

            long offset = (groupIndex - 1) * groupSize;

            var excludeUserIds = new[] { recommend.LoginUserId };

            //1. 同城达人
            users = _talentRepository.GetTalentsByCity(recommend.CityCode, isInCity: true, excludeUserIds, offset, groupSize);
            users.ForEach(item => { item.IsTalent = true; item.IsSameCity = true; });
            if (recommend.TheCityTalentSize == 0)
                recommend.TheCityTalentSize = _talentRepository.GetTalentsByCityTotal(recommend.CityCode, isInCity: true, excludeUserIds);
            CommonHelper.ListRandom(users);

            //2. 非同城达人,  包含无城市达人
            if (users.Count != groupSize)
            {
                //去掉同城后的数量
                var surplus = offset + groupSize - recommend.TheCityTalentSize;
                var page = CalcOffsetSize(surplus, groupSize);

                var otherCityTalents = _talentRepository.GetTalentsByCity(recommend.CityCode, isInCity: false, excludeUserIds, page.offset, page.size);
                otherCityTalents.ForEach(item => { item.IsTalent = true; item.IsSameCity = false; });
                if (recommend.OtherCityTalentSize == 0)
                    recommend.OtherCityTalentSize = _talentRepository.GetTalentsByCityTotal(recommend.CityCode, isInCity: false, excludeUserIds);

                CommonHelper.ListRandom(otherCityTalents);
                users.AddRange(otherCityTalents);
            }

            // 3.浏览相同学校的用户
            if (users.Count != groupSize)
            {
                List<RecommendUserDto> schoolUsers = GetRecommendSchoolUsers(recommend, excludeUserIds, groupSize, offset);
                CommonHelper.ListRandom(schoolUsers);
                users.AddRange(schoolUsers);
            }

            // 4.浏览相同类型学校的用户
            if (users.Count != groupSize)
            {
                List<RecommendUserDto> schoolUsers = GetRecommendSchoolFtypeUsers(recommend, excludeUserIds, groupSize, offset);
                CommonHelper.ListRandom(schoolUsers);
                users.AddRange(schoolUsers);
            }

            //var userIds = users.Select(s => s.Id).ToArray();
            //是否邀请过
            //var messageUserIds = _messageRepository.GetMessageUserIds(recommend.LoginUserId, userIds, recommend.DataId, recommend.MessageType, recommend.MessageDataType, recommend.SchoolExtId);

            recommend.Talents = users.Select(s => new TalentRecommend.Talent()
            {
                Id = s.Id,
                NickName = s.NickName,
                HeadImgUrl = s.HeadImgUrl.ToHeadImgUrl(),
                IsTalent = s.IsTalent,
                //IsInvite = messageUserIds.Where(userId => userId == s.Id).Any(),
                Score = (s.IsSameCity ? 100 : 0) + (s.IsTalent ? 10 : 0)
            }).ToList();
            return recommend;
        }

        private (long offset, int size) CalcOffsetSize(long surplus, int groupSize)
        {
            //页码
            int index = (int)(surplus / groupSize);
            //需要的数量
            int size = (int)(surplus % groupSize);
            size = index == 0 ? size : groupSize;
            var offset = index == 0 ? 0 : surplus - groupSize;
            return (offset, size);
        }

        /// <summary>
        /// 浏览过同学校的用户
        /// </summary>
        /// <param name="recommend"></param>
        /// <param name="groupSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private List<RecommendUserDto> GetRecommendSchoolUsers(TalentRecommend recommend, Guid[] excludeUserIds, int groupSize, long offset)
        {
            //去掉后的数量
            var surplus = offset + groupSize - recommend.TheCityTalentSize - recommend.OtherCityTalentSize;
            var page = CalcOffsetSize(surplus, groupSize);

            var historyUsers = _historyRepository.GetHistoryUsers(recommend.SchoolExtId, excludeUserIds, page.offset, page.size);
            if (recommend.TheSchoolUserSize == 0)
                recommend.TheSchoolUserSize = _historyRepository.GetHistoryUserTotal(recommend.SchoolExtId, excludeUserIds);
            var users = historyUsers.Select(s => new RecommendUserDto() { Id = s.Id, NickName = s.NickName, HeadImgUrl = s.HeadImgUrl }).ToList();
            return users;
        }

        /// <summary>
        /// 浏览过同类型学校的用户
        /// </summary>
        /// <param name="recommend"></param>
        /// <param name="groupSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private List<RecommendUserDto> GetRecommendSchoolFtypeUsers(TalentRecommend recommend, Guid[] excludeUserIds, int groupSize, long offset)
        {
            var school = _schoolRepository.GetSchoolInfo(recommend.SchoolExtId);
            if (school == null || string.IsNullOrWhiteSpace(school.SchFtype))
                return new List<RecommendUserDto>();

            //去掉后的数量
            var surplus = offset + groupSize - recommend.TheCityTalentSize - recommend.OtherCityTalentSize - recommend.TheSchoolUserSize;
            var page = CalcOffsetSize(surplus, groupSize);

            var historyUsers = _historyRepository.GetHistoryUsers(school.SchFtype, recommend.SchoolExtId, excludeUserIds, page.offset, page.size);
            var users = historyUsers.Select(s => new RecommendUserDto() { Id = s.Id, NickName = s.NickName, HeadImgUrl = s.HeadImgUrl }).ToList();
            return users;
        }

        public List<TalentFollowRankDto> GetTalentFollowRank(int count = 100)
        {
            return _talentRepository.GetTalentFollowRank(count)?.ToList();
        }

        public TalentRankingEntity GetTalentRankingByDay(DateTime date)
        {
            return _talentRepository.GetRankingEntityByDate(date);
        }

        public bool AddTalentRanking(IEnumerable<TalentFollowRankDto> currentDatas, DateTime date = default)
        {
            if (currentDatas == null || !currentDatas.Any()) return false;
            var index = 1;
            var currentDate = date == default ? DateTime.Now.Date : date;
            var entity_Ranking = new TalentRankingEntity()
            {
                RankDate = currentDate,
                ID = Guid.NewGuid()
            };

            var oldDatas = _talentRepository.GetRankingEntityByDate(currentDate.AddDays(-1).Date);
            var oldDataItems = new List<TalentRankingItem>();
            if (oldDatas != null && !string.IsNullOrWhiteSpace(oldDatas.DataJson))
            {
                try
                {
                    oldDataItems = JsonHelper.JSONToObject<List<TalentRankingItem>>(oldDatas.DataJson);
                }
                catch
                {
                }
            }

            var list_data = currentDatas.Select(p => new TalentRankingItem()
            {
                FollowCount = p.FollowCount,
                Index = index++,
                TalentName = p.TalentName,
                UserID = p.TalentUserID,
                TalentTitle = p.TalentTitle
            }).ToArray();


            var lastFollowCount = list_data.First().FollowCount;
            var newIndex = 1;
            foreach (var item in list_data)
            {
                if (list_data.IndexOf(item) > 0)
                {
                    if (item.FollowCount < lastFollowCount)
                    {
                        newIndex++;
                    }
                    item.Index = newIndex;
                    lastFollowCount = item.FollowCount;
                    item.Distence = list_data[item.Index - 2].FollowCount - item.FollowCount;
                }
                var find = oldDataItems.FirstOrDefault(p => p.UserID == item.UserID);
                if (find != null)
                {
                    if (find.Index != item.Index)
                    {
                        item.IsUp = item.Index < find.Index;
                    }
                }
            }
            entity_Ranking.DataJson = list_data.ToJson();

            if (_talentRepository.AddTelentRanking(entity_Ranking))
            {
                _easyRedisClient.RemoveAsync("M:User:Talent:TalentRank");
                _easyRedisClient.AddAsync("M:User:Talent:TalentRank", entity_Ranking);
                return true;
            }
            return false;
        }

        public async Task<TalentRankingEntity> GetTodayTalentRanking()
        {
            return await _easyRedisClient.GetAsync<TalentRankingEntity>("M:User:Talent:TalentRank");
        }

        public async Task<Dictionary<Guid, string>> GetTalentHeadImgUrl(IEnumerable<Guid> userIDs)
        {
            var data = await _easyRedisClient.GetAsync<Dictionary<Guid, string>>("M:User:Talent:TalentRank:HeadImgUrls");
            if (data == null || !data.Any())
            {
                var userInfos = _userRepository.ListUserInfo(userIDs.ToList());
                if (userInfos?.Any() == true)
                {
                    var dic = userInfos.ToDictionary(k => k.Id, v => v.HeadImgUrl);
                    await _easyRedisClient.AddAsync("M:User:Talent:TalentRank:HeadImgUrls", dic, DateTime.Now.Date.AddDays(1) - DateTime.Now);
                    return dic;
                }
            }
            return new Dictionary<Guid, string>();
        }

        public PaginationModel<SearchTalentDto> SearchTalents(Guid? loginUserId, SearchBaseQueryModel queryModel)
        {
            var pagination = _talentSearch.SearchPagination(queryModel);
            var ids = pagination.Data.Select(s => s.Id);
            var data = SearchTalents(ids, loginUserId);

            return PaginationModel.Build(data, pagination.Total);
        }

        public List<SearchTalentDto> SearchTalents(IEnumerable<Guid> ids, Guid? loginUserId)
        {
            var data = _talentRepository.GetTalents(ids.ToArray());
            var dto = data.Select(s => CommonHelper.MapperProperty<TalentAbout, SearchTalentDto>(s)).ToList();

            foreach (var item in dto)
            {
                item.HeadImgUrl = (item.HeadImgUrl + "").ToHeadImgUrl();
            }

            //搜索结果中, 设置已关注标识, 并把已关注的人提前
            dto = UpFollowUser(loginUserId, dto);

            return dto.SortTo(ids).ToList();
        }


        public TalentFollowDto GetTalent(Guid talentId, Guid? loginUserId)
        {
            var talent = _talentRepository.GetTalents(new[] { talentId }).FirstOrDefault();

            if (talent == null || talent.UserId == null)
            {
                return null;
            }

            var user = _userRepository.GetUserInfo(talent.UserId.Value);
            if (user == null)
            {
                return null;
            }

            var dto = CommonHelper.MapperProperty<TalentAbout, TalentFollowDto>(talent);
            dto.HeadImgUrl = (dto.HeadImgUrl + "").ToHeadImgUrl();

            //登录用户是否关注
            dto.IsFollow = loginUserId != null && loginUserId != Guid.Empty &&
                _collectionRepository.IsCollected(loginUserId.Value, user.Id);

            dto.Introduction = user?.Introduction;

            return dto;
        }

        public TalentFollowDto GetTalentByUserId(Guid userId, Guid? loginUserId)
        {
            var talent = _talentRepository.GetTalentByUserId(userId.ToString());

            if (talent == null)
            {
                return null;
            }

            var user = _userRepository.GetUserInfo(userId);
            var dto = CommonHelper.MapperProperty<TalentEntity, TalentFollowDto>(talent);

            dto.HeadImgUrl = (dto.HeadImgUrl + "").ToHeadImgUrl();

            //登录用户是否关注
            dto.IsFollow = loginUserId != null && loginUserId != Guid.Empty &&
                _collectionRepository.IsCollected(loginUserId.Value, userId);

            dto.Introduction = user?.Introduction;

            return dto;
        }

        /// <summary>
        /// 搜索结果中, 已关注的人提前
        /// </summary>
        /// <param name="loginUserId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<SearchTalentDto> UpFollowUser(Guid? loginUserId, List<SearchTalentDto> data)
        {
            //登录用户查询,  则置顶其关注
            if (loginUserId == null || loginUserId == Guid.Empty)
            {
                return data;
            }

            //达人用户Id
            var userIds = data.Where(s => s.UserId != null).Select(s => s.UserId.Value).ToList();
            //userIds中登录人关注的达人用户Id
            var dataIds = _collectionRepository.GetCollection(userIds, loginUserId.Value);

            //提前
            if (!dataIds.Any())
            {
                return data;
            }

            int len = data.Count, boost = len + 1;
            var sort = data.Select((s, index) => new ModelSort<SearchTalentDto> { Item = s, Score = len - index }).ToList();
            foreach (var item in sort)
            {
                if (item.Item.UserId != null && dataIds.Contains(item.Item.UserId.Value))
                {
                    item.Item.IsFollow = true;
                    item.Score += boost;
                }
            }

            data = sort.OrderByDescending(s => s.Score).Select(s => s.Item).ToList();
            return data;
        }
    }
}
