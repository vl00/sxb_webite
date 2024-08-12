using Newtonsoft.Json;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Result;
using ProductManagement.API.Http.Service;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Repository.Repositories
{
    public partial class TalentRepository : ITalentRepository
    {
        private readonly UserDbContext _dbcontext;
        private readonly IOperationClient _operationClient;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly string _redisKey = "talent:SMSCode:";
        //private readonly string tempId = "617496";

        private static string _schoolLiveDb = "[iSchoolLive].[dbo]";
        /// <summary>
        /// 关注用户的类型  4 关注普通用户  5 关注达人讲师
        /// </summary>
        private readonly CollectionDataType[] _collectionUserDataTypes = new CollectionDataType[] { CollectionDataType.User, CollectionDataType.UserLector };

        public TalentRepository(
            UserDbContext dbcontext
            , IOperationClient operationClient
            , IEasyRedisClient easyRedisClient
            )
        {
            _dbcontext = dbcontext;
            _operationClient = operationClient;
            _easyRedisClient = easyRedisClient;
        }


        /// <summary>
        /// 修改达人信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public CommRespon UpdateTalent(TalentEntity talent)
        {
            var result = new CommRespon() { code = 200, message = "操作成功" };
            var currentTalent = _dbcontext.Query<TalentEntity>("select * from talent  where id=@id", new { id = talent.id }).FirstOrDefault();
            if (currentTalent == null)
            {
                return new CommRespon() { code = 400, message = "达人不存在" };
            }
            currentTalent.organization_code = talent.organization_code;
            currentTalent.organization_name = talent.organization_name;
            currentTalent.certification_preview = talent.certification_preview;
            currentTalent.certification_explanation = talent.certification_explanation;
            currentTalent.supplementary_explanation = talent.supplementary_explanation;
            _dbcontext.Execute("update talent set organization_code=@organization_code,organization_name=@organization_name,certification_preview=@certification_preview,certification_explanation=@certification_explanation,supplementary_explanation=@supplementary_explanation where id=@id", currentTalent);
            return result;
        }

        /// <summary>
        /// 修改达人状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool ChangTalentStatus(string id, int status)
            => _dbcontext.Execute("update talent set status=@status where id=@id", new { id, status }) > 0;

        /// <summary>
        /// 审核达人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <param name="userId"></param>
        public void AuditTalent(string id, int status, string reason, string userId)
        {
            _dbcontext.BeginTransaction();
            _dbcontext.Execute("update talent set status=@status where id=@id", new { id, status });
            _dbcontext.Execute(@"insert into talent_audit(id,talent_id,createdate,status,audit_date,audit_reason,audit_userid) 
                    values(@id,@talent_id,@createdate,@status,@audit_date,@audit_reason,@audit_userid)", new { id = Guid.NewGuid(), talent_id = id, createdate = DateTime.Now, status, audit_date = DateTime.Now, reason, audit_userid = userId });
            _dbcontext.Commit();
        }

        /// <summary>
        /// 达人码认证
        /// </summary>
        /// <param name="talent"></param>
        /// <param name="talentAudit"></param>
        /// <returns></returns>
        public CommRespon InviteTalentByCode(TalentEntity talent, TalentAudit talentAudit, TalentEntity oldTalent, Lector lector, bool isNewLector)
        {
            return _dbcontext.Tran(tran =>
            {

                //消费达人码
                _dbcontext.Execute(@"
update talent_invite 
set status=1
where parent_id=@parent_id", new { parent_id = talent.id }, tran);

                //添加审核记录
                _dbcontext.Execute(@"
insert into talent_audit(
id,talent_id,createdate,status,audit_date,audit_reason,audit_userid) 
values(
@id,@talent_id,@createdate,@status,@audit_date,@audit_reason,@audit_userid)
", talentAudit, tran);

                //更新达人信息
                _dbcontext.Execute(@"
update talent 
set 
    user_id=@user_id,
    certification_date=@certification_date,
    status=@status,
    invite_status=@invite_status,
    certification_status=@certification_status 
where id=@id", talent, tran);

                if (oldTalent != null)
                {
                    //如果有旧的达人信息, 则删除
                    _dbcontext.Execute(@"
update talent 
set isdelete=@isdelete 
where id=@id", oldTalent, tran);
                }

                if (isNewLector)
                {
                    AddLector(lector, tran);
                }
                else
                {
                    UpdateLector(lector, tran);
                }
                return new CommRespon() { code = 200, message = "操作成功" };

            });
        }

        public bool AddTalent(TalentEntity talent, IDbTransaction _dbtransaction)
        {
            var talentSql = $@"
                insert into talent(
	                id,user_id,certification_type,certification_way,certification_identity,certification_identity_id,
	                certification_title,certification_explanation,certification_preview,createdate,certification_date,
	                isdelete,type,organization_name,organization_code,operation_name,
	                operation_phone,certification_status,status,supplementary_explanation
                ) 
                values(
	                @id,@user_id,@certification_type,@certification_way,@certification_identity,@certification_identity_id,
	                @certification_title,@certification_explanation,@certification_preview,@createdate,@certification_date,
	                @isdelete,@type,@organization_name,@organization_code,@operation_name,
	                @operation_phone,@certification_status,@status,@supplementary_explanation
                )
            ";
            _dbcontext.Execute(talentSql, talent, _dbtransaction);
            return true;
        }

        /// <summary>
        /// 更新达人
        /// </summary>
        /// <param name="talent"></param>
        /// <param name="_dbtransaction"></param>
        /// <returns></returns>
        public bool UpdateTalent(TalentEntity talent, IDbTransaction _dbtransaction)
        {
            var talentSql = $@"
                update talent 
                set 
                    user_id=@user_id,
                    certification_type=@certification_type,
                    certification_way=@certification_way,
                    certification_identity=@certification_identity,
                    certification_identity_id=@certification_identity_id,

                    certification_title=@certification_title,
                    certification_explanation=@certification_explanation,
                    certification_preview=@certification_preview,
                    createdate=@createdate,
                    certification_date=@certification_date,

                    isdelete=@isdelete,
                    type=@type,
                    organization_name=@organization_name,
                    organization_code=@organization_code,
                    operation_name=@operation_name,

                    operation_phone=@operation_phone,
                    certification_status=@certification_status,
                    status=@status,
                    supplementary_explanation=@supplementary_explanation
                where 
                    id=@id
            ";
            _dbcontext.Execute(talentSql, talent, _dbtransaction);
            return true;
        }

        public bool AddLector(Lector lector, IDbTransaction _dbtransaction)
        {
            var sql = $@"
                insert into {_schoolLiveDb}.[lector](
	                id,userID,name,intro,show,
	                weight,jointime,autograph,mobile,modifytime,
	                org_type,org_tag,head_imgurl
                ) 
                values(
	                @id,@userID,@name,@intro,@show,
	                @weight,@jointime,@autograph,@mobile,@modifytime,
	                @org_type,@org_tag,@head_imgurl
                )
            ";
            return _dbcontext.Execute(sql, lector, _dbtransaction) > 0;
        }

        public bool UpdateLector(Lector lector, IDbTransaction _dbtransaction)
        {
            var sql = $@"
                update {_schoolLiveDb}.[lector]
                set
                   show = @show,
                   org_type = @org_type,
                   org_tag = @org_tag,
                   modifytime = @modifytime
                where
                   userID = @userID
            ";
            return _dbcontext.Execute(sql, lector, _dbtransaction) > 0;
        }


        public bool UpdateLector(Lector lector,string[] fields, IDbTransaction _dbtransaction)
        {
            if (fields?.Any() == false) {
                return false;
            }
            var sql = $@"
                update {_schoolLiveDb}.[lector]
                set
                 {string.Join(",",fields.Select(field => $"{field} = @{field}"))}
                where
                   userID = @userID
            ";
            return _dbcontext.Execute(sql, lector, _dbtransaction) > 0;
        }

        public bool AddTalentImgs(List<TalentImg> talentImgs, IDbTransaction _dbtransaction)
        {
            _dbcontext.Execute(@"insert into talent_img(id,talent_id,url,type) 
            values(@id,@talent_id,@url,@type)", talentImgs, _dbtransaction);
            return true;
        }

        public bool UpdateTalentImgs(Guid talentId, List<TalentImg> talentImgs, IDbTransaction _dbtransaction)
        {
            //删除所有旧图片
            _dbcontext.Execute(@" delete from talent_img where talent_id = @talentId ", new { talentId }, _dbtransaction);

            //插入新图片
            _dbcontext.Execute(@"insert into talent_img(id,talent_id,url,type) 
                values(@id,@talent_id,@url,@type)", talentImgs, _dbtransaction);
            return true;
        }

        /// <summary>
        /// 邀请/申请达人
        /// </summary>
        /// <param name="talentInput"></param>
        /// <param name="code">邀请达人码</param>
        /// <returns></returns>
        [Obsolete]
        public async Task<CommRespon> InviteTalent(TalentInput talentInput, string code)
        {
            var result = new CommRespon() { code = 200, message = "操作成功" };
            var _dbtransaction = _dbcontext.BeginTransaction();
            try
            {
                var userInfo = _dbcontext.Query<UserInfo>(" select * from userInfo where id=@id", new { id = talentInput.user_id }).FirstOrDefault();
                if (userInfo == null)
                    return new CommRespon() { code = 400, message = "用户不存在" };
                var talent = _dbcontext.Query<TalentEntity>(" select * from talent where user_id=@user_id", new { talentInput.user_id }).FirstOrDefault();
                if (talent != null) // && talentInput.InviteType != 1
                    return new CommRespon() { code = 400, message = "当前用户已经申请达人认证" };

                if (talentInput.imgs == null || talentInput.imgs.Count == 0)
                    return new CommRespon() { code = 400, message = "请选择证明图片" };
                if (talentInput.imgs.Count > 5)
                    return new CommRespon() { code = 400, message = "证明图片不能超过5张" };
                if (talentInput.type == 1 && string.IsNullOrEmpty(userInfo.Mobile) && talentInput.InviteType == 0)
                {
                    return new CommRespon() { code = 400, message = "未绑定手机号，请先绑定" };
                }
                talent = new TalentEntity()
                {
                    id = Guid.NewGuid(),
                    createdate = DateTime.Now,
                    status = 1,
                    isdelete = 0,
                    certification_status = 0,
                    user_id = userInfo.Id,
                    certification_type = talentInput.certification_type,
                    certification_way = talentInput.certification_way,
                    certification_identity = talentInput.certification_identity,
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

                if (talentInput.InviteType == 1)
                {
                    result.data = code;
                    talent.certification_way = 1;
                    talent.certification_date = talentInput.effectivedate;
                    _dbcontext.Execute(@"insert into talent_invite(id,createdate,effectivedate,code,parent_id,type)
                    values(@id,@createdate,@effectivedate,@talentcode,@parent_id,@type)", new { id = Guid.NewGuid(), createdate = DateTime.Now, talentInput.effectivedate, code, parent_id = talent.id, type = 0 }, _dbtransaction);
                }
                else
                {
                    talent.certification_way = 0;
                    //验证短信
                    var data = await _easyRedisClient.GetAsync<string>($"{_redisKey}{talentInput.operation_phone}");
                    if (data == null)
                        return new CommRespon() { code = 400, message = "验证码失效，请重新获取验证码" };
                    if (data != talentInput.code)
                        return new CommRespon() { code = 400, message = "验证码错误" };
                }

                AddTalent(talent, _dbtransaction);

                var lector = GetLectorByUserId(talent.user_id.ToString());
                if (lector == null)
                {
                    lector = new Lector()
                    {
                        id = Guid.NewGuid(),
                        userID = talent.user_id,
                        name = userInfo.NickName,
                        intro = userInfo.Introduction,
                        show = 1,
                        weight = 0,
                        jointime = userInfo.RegTime,
                        autograph = "",
                        mobile = userInfo.Mobile,
                        modifytime = DateTime.Now,
                        org_type = talent.type,
                        org_tag = talent.certification_preview,
                        head_imgurl = userInfo.HeadImgUrl
                    };
                    AddLector(lector, _dbtransaction);
                }
                else
                {
                    lector.org_type = talent.type;
                    lector.org_tag = talent.certification_preview;
                    lector.modifytime = DateTime.Now;
                    UpdateLector(lector, _dbtransaction);
                }

                talentInput.imgs.ForEach(i =>
                {
                    i.id = Guid.NewGuid();
                    i.talent_id = talent.id;
                });
                AddTalentImgs(talentInput.imgs, _dbtransaction);

                _dbtransaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                _dbtransaction.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// 申请达人
        /// </summary>
        /// <param name="talent"></param>
        /// <param name="lector"></param>
        /// <param name="isNewTalent"></param>
        /// <param name="isNewLector"></param>
        /// <returns></returns>
        public CommRespon ApplyTalent(TalentEntity talent, Lector lector, bool isNewTalent, bool isNewLector)
        {
            return _dbcontext.Tran(tran =>
            {
                if (isNewTalent)
                {
                    //创建达人
                    AddTalent(talent, tran);

                    if (talent.imgs != null && talent.imgs.Count > 0)
                    {
                        AddTalentImgs(talent.imgs, tran);
                    }
                }
                else
                {
                    //删除旧的员工
                    _dbcontext.Execute(" update talent_staff set isdelete=1 where talent_id=@talentId", new { talentId = talent.id }, tran);

                    //变更认证
                    UpdateTalent(talent, tran);

                    if (talent.imgs != null && talent.imgs.Count > 0)
                    {
                        UpdateTalentImgs(talent.id, talent.imgs, tran);
                    }
                }

                if (isNewLector)
                {
                    AddLector(lector, tran);
                }
                else
                {
                    UpdateLector(lector, tran);
                }

                return CommRespon.Success("申请成功");
            });
        }

        public CommRespon EnableTalent(string id, string lectorId, string staffId = null)
        {
            return _dbcontext.Tran(tran =>
            {
                int ret = 0;
                //启用达人  状态 0:禁用 1启用
                ret += _dbcontext.Execute("update talent set status=@status where id=@id", new { id, status = 1 }, tran);

                //启用员工信息
                if (!string.IsNullOrWhiteSpace(staffId))
                {
                    ret += _dbcontext.Execute("update talent_staff set status=@status where id=@staffId", new { staffId, status = 1 }, tran);
                }

                //启用讲师
                if (!string.IsNullOrWhiteSpace(lectorId))
                {
                    ret += _dbcontext.Execute($"update {_schoolLiveDb}.[lector] set show=@show where id=@lectorId", new { lectorId, show = 1 }, tran);
                }
                return ret > 0 ? CommRespon.Success("操作成功") : CommRespon.Failure("操作失败");
            });
        }

        public CommRespon DisableTalent(string id, string lectorId, string staffId = null)
        {
            return _dbcontext.Tran(tran =>
            {
                int ret = 0;
                //禁用达人 状态 0:禁用 1启用
                ret += _dbcontext.Execute("update talent set status=@status where id=@id", new { id, status = 0 }, tran);

                //禁用员工信息
                if (!string.IsNullOrWhiteSpace(staffId))
                {
                    ret += _dbcontext.Execute("update talent_staff set status=@status where id=@staffId", new { staffId, status = 0 }, tran);
                }

                //禁用讲师
                if (!string.IsNullOrWhiteSpace(lectorId))
                {
                    ret += _dbcontext.Execute($"update {_schoolLiveDb}.[lector] set show=@show where id=@lectorId", new { lectorId, show = 0 }, tran);
                }
                return ret > 0 ? CommRespon.Success("操作成功") : CommRespon.Failure("操作失败");
            });
        }

        /// <summary>
        /// 删除达人证明图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        public CommRespon DeleteTalentImg(List<TalentImg> imgs)
            => (_dbcontext.Execute(@"delete talent_img where id=@id", imgs)) > 0 ? new CommRespon() { code = 200, message = "操作成功" } : new CommRespon() { code = 400, message = "操作失败" };

        /// <summary>
        /// 新增达人证明图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        public CommRespon AddTalentImg(List<TalentImg> imgs)
        {
            imgs.ForEach(i =>
            {
                i.id = Guid.NewGuid();
            });
            return _dbcontext.Execute(@"insert into talent_img(id,talent_id,url,type) 
                    values(@id,@talent_id,@url,@type)", imgs) > 0 ? new CommRespon() { code = 200, message = "操作成功" } : new CommRespon() { code = 400, message = "操作失败" };
        }



        /// <summary>
        /// 邀请员工
        /// 1. 验证员工信息
        /// 2. 验证机构达人信息
        /// 3. 添加talent, talentInvite, talentStaff
        /// </summary>
        /// <param name="talent"></param>
        /// <param name="effectivedate"></param>
        /// <returns></returns>
        public CommRespon InviteStaff(TalentStaff talentStaff, TalentInvite talentInvite)
        {
            return _dbcontext.Tran(tran =>
            {
                int ret = 0;

                //删除同一个机构对同一个手机旧的邀请
                ret += _dbcontext.Execute(@" 
                            update talent_staff 
                            set isdelete = 1
                            where parentId = @parentId and phone = @phone
                            ", new { talentStaff.parentId, talentStaff.phone }, tran);

                //新增邀请信息
                ret += _dbcontext.Execute(@"insert into talent_staff(id,talent_id,parentId,name,position,phone,status,isdelete,createdate)
                    values(@id,@talent_id,@parentId,@name,@position,@phone,@status,@isdelete,@createdate)", talentStaff, tran);

                ret += _dbcontext.Execute(@"insert into talent_invite(id,createdate,effectivedate,code,parent_id,type)
                values(@id,@createdate,@effectivedate,@code,@parent_id,@type)", talentInvite, tran);

                if (ret > 0)
                {
                    return CommRespon.Success("操作成功");
                }
                else
                {
                    return CommRespon.Failure("操作失败");
                }
            });
        }

        /// <summary>
        /// 确认邀请
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <param name="isNewTalent"></param>
        /// <returns></returns>
        public CommRespon StaffConfirmInvitation(string staffId, TalentEntity talent, Lector lector, bool isNewTalent, bool isNewLector)
        {
            return _dbcontext.Tran(tran =>
            {
                //消费达人码
                _dbcontext.Execute(@"
update talent_invite 
set status=1
where parent_id=@parent_id", new { parent_id = staffId }, tran);

                if (isNewTalent)
                {
                    //创建达人
                    AddTalent(talent, tran);
                }
                else
                {
                    //变更认证
                    _dbcontext.Execute($@"
                                        update talent 
                                        set 
                                            status=@status,
                                            isdelete=@isdelete,
                                            certification_status=@certification_status,
                                            type=@type,
                                            certification_type=@certification_type,

                                            certification_way=@certification_way,
                                            certification_identity=@certification_identity,
                                            certification_identity_id=@certification_identity_id,
                                            certification_title=@certification_title,
                                            certification_explanation=@certification_explanation,
                                            certification_preview=@certification_preview,
                                            organization_code=@organization_code,
                                            organization_name=@organization_name,
                                            operation_name=@operation_name,
                                            operation_phone=@operation_phone,
                                            certification_date=@certification_date
                                        where 
                                            id=@id", talent, tran);
                }

                if (isNewLector)
                {
                    AddLector(lector, tran);
                }
                else
                {
                    UpdateLector(lector, tran);
                }

                //更新员工信息
                _dbcontext.Execute(" update talent_staff set talent_id = @talentId, status=1, isdelete=0 where id=@id ", new { id = staffId, talentId = talent.id }, tran);

                //添加审核通过信息
                _dbcontext.Execute(@"
                    insert into talent_audit(
                        id,talent_id,createdate,status,audit_date,audit_reason,audit_userid) 
                    values(
                        @id,@talent_id,@createdate,@status,@audit_date,@audit_reason,@audit_userid)
                    ", new { id = Guid.NewGuid(), talent_id = talent.id, createdate = DateTime.Now, status = 1, audit_date = DateTime.Now, audit_reason = "机构员工认证", audit_userid = talent.user_id });
                return CommRespon.Success("操作成功");
            });
        }

        /// <summary>
        /// 编辑员工
        /// 1.修改员工信息
        /// 2.修改用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EditTalentStaff(TalentStaffInputDto talentStaffdto)
        {
            return _dbcontext.Execute(" update talent_staff set name=@name,position=@position where id=@id", talentStaffdto) > 0;
        }

        /// <summary>
        /// 删除员工
        /// 1. 删除员工信息
        /// 2. 删除达人信息
        /// 3. 删除讲师信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTalentStaff(Guid id, string userId)
        {
            return _dbcontext.Tran(tran =>
            {
                var ret = 0;
                //删除员工
                ret += _dbcontext.Execute(" update talent_staff set isdelete=1 where id=@id", new { id }, tran);
                //删除达人
                ret += _dbcontext.Execute(" update talent set isdelete=1 where id in (select talent_id from talent_staff where id = @id) ", new { id }, tran);
                //删除讲师
                ret += _dbcontext.Execute($" update {_schoolLiveDb}.[lector] set show=0 where userID = @userId ", new { userId }, tran);
                return ret > 0;
            });
        }

        /// <summary>
        /// 删除员工信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteTalentStaff(Guid id)
        {
            return _dbcontext.ExecuteUow(" update talent_staff set isdelete=1 where id=@id", new { id });
        }

        /// <summary>
        ///  删除达人信息
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public int DeleteTalent(Guid id)
        {
            return _dbcontext.ExecuteUow(" update talent set isdelete=1 where id = @id ", new { id });
        }

        /// <summary>
        ///  删除讲师信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int DeleteLector(Guid userId)
        {
            return _dbcontext.ExecuteUow($" update {_schoolLiveDb}.[lector] set show=0 where userID = @userId ", new { userId });
        }

        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attionUserId"></param>
        /// <returns></returns>
        public CommRespon AttentionUser(Guid userId, Guid attionUserId, bool isTalent)
        {
            var result = new CommRespon() { code = 200, message = "操作成功" };
            var collection = _dbcontext.Query<Collection>($@" select * from collection where userID=@userId and dataID=@attionUserId and dataType in  @dataTypes ",
                new { userId, attionUserId, dataTypes = _collectionUserDataTypes }).FirstOrDefault();

            if (collection != null)
            {
                return CommRespon.Failure("当前用户已经关注该用户");
            }

            collection = new Collection()
            {
                dataID = attionUserId,
                dataType = (int)(!isTalent ? CollectionDataType.User : CollectionDataType.UserLector),
                userID = userId,
                time = DateTime.Now
            };

            var queryMessageSql = $"select * from  message where senderID=@senderID and userID=@userID and type=@type and dataType=@dataType ";
            var message = _dbcontext.Query<Message>(queryMessageSql, new { senderID = userId, userID = attionUserId, type = MessageType.Follow, dataType = MessageDataType.User }).FirstOrDefault();

            return _dbcontext.Tran(tran =>
            {
                _dbcontext.Execute(@"insert into collection(dataID,dataType,userID,time) 
                values(@dataID,@dataType,@userID,@time)", collection, tran);
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
                    _dbcontext.Execute(@"insert into message(id,userID,senderID,type,dataID,dataType,time,push,IsAnony,[Read]) 
                values(@Id,@userID,@senderID,@Type,@DataID,@DataType,@time,@push,@IsAnony,@Read)", message, tran);
                }
                else
                {
                    _dbcontext.Execute("update message set [Read]=1 where id=@id  ", new { id = message.Id, tran });
                }
                return CommRespon.Success("操作成功");
            });

        }

        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attionUserId"></param>
        public CommRespon CancleAttention(Guid userId, Guid attionUserId)
        {
            var result = new CommRespon() { code = 200, message = "操作成功" };
            _dbcontext.Execute($@"delete collection where dataID=@dataID and dataType in @dataType and userID=@userID", new { dataID = attionUserId, dataType = _collectionUserDataTypes, userID = userId });
            //设为已读
            var updateMessageSql = $@"update message set [Read]=1 where senderID=@senderID and userID=@userID and [Read]=0  and type=@type and dataType=@dataType";
            _dbcontext.Execute(updateMessageSql, new { senderID = userId, userID = attionUserId, type = MessageType.Follow, dataType = MessageDataType.User });
            return result;
        }


        /// <summary>
        /// 标记为已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool NoticeUser(List<Guid> id)
            => _dbcontext.Execute("update message set [Read]=1 where id=@id", new { id }) > 0;

        /// <summary>
        /// 标记为已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool NoticeAllUser(Guid userId)
        {
            return _dbcontext.Execute($@"
UPDATE 
	message 
SET 
	[Read]=1 
FROM 
	message M
 	INNER JOIN collection C ON C.dataID = M.userId AND C.dataType in @collectionUserDataTypes
where 
  C.dataID = @userId 
  AND M.type = @type
  AND M.dataType in @dataTypes
", new { userId, collectionUserDataTypes = _collectionUserDataTypes, type = MessageType.Follow, dataTypes = new[] { MessageDataType.User, MessageDataType.Lecture } }) > 0;
        }


        /// <summary>
        /// 修改或新增达人身份
        /// </summary>
        /// <param name="talentIdentity"></param>
        /// <returns></returns>
        public CommRespon EditOrAddIdentity(TalentIdentityInput talentIdentityInput)
        {
            var result = new CommRespon() { code = 200, message = "操作成功" };
            var talentIdentity = new TalentIdentity()
            {
                identity_name = talentIdentityInput.identity_name,
                type = talentIdentityInput.type,
                enable = talentIdentityInput.enable,
                identity_description = talentIdentityInput.identity_description
            };
            if (talentIdentityInput.id == null)
            {
                talentIdentity.id = Guid.NewGuid();
                talentIdentity.isdelete = 0;
                talentIdentity.createdate = DateTime.Now;
                _dbcontext.Execute(@"insert into talent_identity(id,identity_name,type,enable,identity_description,isdelete,createdate)
                    values(@id,@identity_name,@type,@enable,@identity_description,@isdelete,@createdate)", talentIdentity);
            }
            else
            {
                talentIdentity.id = (Guid)talentIdentityInput.id;
                _dbcontext.Execute("update talent_identity set identity_name=@identity_name,type=@type,enable=@enable,identity_description=@identity_description where id=@id", new { talentIdentity.id, talentIdentity.identity_name, talentIdentity.type, talentIdentity.enable, talentIdentity.identity_description });
            }
            return result;
        }


        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <param name="interestDto"></param>
        /// <returns></returns>
        public bool EditUserData(InterestDto interestDto)
        {
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(interestDto));
            var interest = _dbcontext.Query<Interest>("select * from interest  where userID=@userID", new { userID = interestDto.userID }).FirstOrDefault();
            System.Diagnostics.Debug.WriteLine(interest == null);
            if (interest == null)
            {
                interestDto.uuID = Guid.NewGuid();
                var ret = _dbcontext.Execute(@"insert into interest(uuID,userID,grade_1,grade_2,grade_3,grade_4,nature_1,nature_2,nature_3,lodging_0,lodging_1)
                    values(@uuID,@userID,@grade_1,@grade_2,@grade_3,@grade_4,@nature_1,@nature_2,@nature_3,@lodging_0,@lodging_1)", interestDto);
                System.Diagnostics.Debug.WriteLine(ret);
            }
            else
            {
                var ret = _dbcontext.Execute($@"
update interest 
set 
    uuID=@uuID,
    grade_1=@grade_1,grade_2=@grade_2,grade_3=@grade_3,grade_4=@grade_4,
    nature_1=@nature_1,nature_2=@nature_2,nature_3=@nature_3,
    lodging_0=@lodging_0,lodging_1=@lodging_1 
where userID=@userID", interestDto);
                System.Diagnostics.Debug.WriteLine(ret);
            }

            _dbcontext.Execute($@"
update userInfo 
set
    introduction = @introduction,
    city = @city,
    nickname = ISNULL(@nickname, nickname),
    headImgUrl = ISNULL(@headImgUrl, headImgUrl)
where id=@userId", new { userId = interestDto.userID, interestDto.introduction, city = interestDto.cityCode, interestDto.nickname, interestDto.headImgUrl });

            _dbcontext.Execute($@"
            UPDATE [iSchoolLive].[dbo].[lector] SET 
            [name] =ISNULL(@nickname, name), [intro] = @introduction, [modifytime] = getdate(), 
            [head_imgurl] = ISNULL(@headImgUrl, head_imgurl)
            WHERE 
            [userID] = @userId;",
            new
            {
                userId = interestDto.userID,
                interestDto.introduction,
                interestDto.nickname,
                interestDto.headImgUrl
            });
            return true;
        }

        #region 公用方法

        /// <summary>
        /// 获取用验证码(随机数字)
        /// </summary>
        /// <param name="n">生成长度</param>
        /// <returns></returns>
        public async Task<string> RandVerificationCode(int n, Random random, string phone)
        {
            var code = RandomHelper.RandVerificationCode(n, random);
            //判断是否重复
            var exitCode = await _easyRedisClient.GetAsync<string>($"{_redisKey}{phone}");
            if (code == exitCode)
            {
                return await RandVerificationCode(n, random, phone);
            }
            else
            {
                if (exitCode == null)
                {
                    await _easyRedisClient.AddAsync($"{_redisKey}{phone}", code, TimeSpan.FromMilliseconds(15));
                }
                else
                {
                    await _easyRedisClient.ReplaceAsync($"{_redisKey}{phone}", code, TimeSpan.FromMilliseconds(15));
                }
            }
            return code;
        }

        #endregion

        #region 达人榜相关
        public IEnumerable<TalentFollowRankDto> GetTalentFollowRank(int count = 100)
        {
            var str_Top = $"Top {count}";
            if (count < 1) str_Top = string.Empty;
            var str_SQL = $@"SELECT {str_Top} 
                                C.dataID as TalentUserID,
	                            U.nickname as TalentName,
                                T.certification_title as TalentTitle,
	                            COUNT ( C.dataID ) AS FollowCount 
                            FROM
	                            collection AS C
	                            LEFT JOIN talent AS T ON C.dataID = T.user_id 
	                            INNER JOIN userInfo as U ON c.dataID = U.id
                            WHERE
	                            C.dataType IN ( 4, 5 ) 
	                            AND T.status = 1 
	                            AND T.certification_status = 1 
	                            AND T.isdelete = 0 
                            GROUP BY
	                            C.dataID ,
	                            U.nickname,
                                T.certification_title
                            ORDER BY
	                            FollowCount DESC;";
            var finds = _dbcontext.Query<TalentFollowRankDto>(str_SQL, new { });
            if (finds?.Any() == true) return finds;
            return null;
        }

        public TalentRankingEntity GetRankingEntityByDate(DateTime date)
        {
            var str_SQL = $@"SELECT
	                            ID,
	                            RankDate,
	                            DataJson 
                            FROM
	                            TalentRanking
	                            WHERE RankDate = @date";
            return _dbcontext.QuerySingle<TalentRankingEntity>(str_SQL, new { date });
        }

        public bool AddTelentRanking(TalentRankingEntity entity)
        {
            var str_SQL = $"Insert Into TalentRanking Values(@ID,@RankDate,@DataJson)";
            return _dbcontext.Execute(str_SQL, new { entity.ID, entity.RankDate, entity.DataJson }) > 0;
        }
        #endregion
    }
}
