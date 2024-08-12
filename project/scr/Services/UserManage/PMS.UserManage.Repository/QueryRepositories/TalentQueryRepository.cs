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
    public partial class TalentRepository
    {

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
        {
            var sql = "";
            var condition = new StringBuilder();
            dynamic param = new ExpandoObject();
            param.type = type;
            param.pageindex = pageindex;
            param.pagesize = pagesize;
            condition.Append(" ta.isdelete=0 ");
            condition.Append(" and ta.type=@type ");
            if (!string.IsNullOrEmpty(userId))
            {
                condition.Append(" and u.user_id=@userId ");
                param.userId = userId;
            }
            if (!string.IsNullOrEmpty(userName))
            {
                condition.Append(" and u.nickname=@userName ");
                param.userName = userName;
            }
            if (!string.IsNullOrEmpty(phone))
            {
                condition.Append(" and u.mobile=@phone ");
                param.phone = phone;
            }
            if (certificationType != null)
            {
                condition.Append(" and ta.certification_type=@certificationType ");
                param.certificationType = certificationType;
            }
            if (begainDate != null && endDate != null)
            {
                condition.Append(" and ta.certification_date begian @begainDate and @endDate");
                param.begainDate = begainDate;
                param.endDate = endDate;
            }
            if (status != -1)
            {
                condition.Append(" and ta.certification_status=@status ");
                param.status = status;
            }
            if (isInvite == 0)
            {
                sql = $"select t.* from (SELECT ta.*,u.mobile,u.nickname,ROW_NUMBER() OVER(order by createdate desc) AS Row_Index FROM talent ta left join userInfo u on ta.user_id=u.id where {condition.ToString()})t where  Row_Index between ((@pageindex - 1)* @pagesize + 1) and (@pageindex*@pagesize) ";
                total = _dbcontext.Query<int>($" select count(1) from talent ta left join userInfo u on ta.user_id=u.id where {condition.ToString()}", new { userId, type, userName, phone, certificationType, begainDate, endDate }).FirstOrDefault();
            }
            else
            {
                sql = $"select t.* from (SELECT ta.*,i.code,u.mobile,u.nickname,ROW_NUMBER() OVER(order by createdate desc) AS Row_Index FROM talent_invite i left join talent ta on i.parent_id=ta.id left join userInfo u on ta.user_id=u.id where {condition.ToString()})t where  Row_Index between ((@pageindex - 1)* @pagesize + 1) and (@pageindex*@pagesize) ";
                total = _dbcontext.Query<int>($" select count(1) from  talent_invite i left join talent ta on i.parent_id=ta.id left join userInfo u on ta.user_id=u.id where {condition.ToString()}", new { userId, type, userName, phone, certificationType, begainDate, endDate }).FirstOrDefault();

            }
            var talents = _dbcontext.Query<TalentEntity>(sql, param);
            return talents;
        }


        public IEnumerable<TalentEntity> GetTalentsByUser(Guid[] userIds)
        {
            var sql = $@"
select * from talent T
where 
    T.isdelete = 0
    and T.status = 1
    and T.certification_status = 1
    and T.user_id in @userIds
";
            return _dbcontext.Query<TalentEntity>(sql, new { userIds });
        }

        public IEnumerable<TalentAbout> GetTalents(Guid[] ids)
        {
            var sql = $@"
SELECT
	T.Id AS id,
    T.Type,
	UI.Id AS userId,
	UI.nickName,
	UI.headImgUrl,
	T.certification_preview AS authTitle,
	(
		SELECT COUNT(1) FROM iSchoolUser.dbo.Collection C WHERE C.DataId = T.user_Id AND C.DataType IN (4, 5)
	) AS fansTotal,
	(
		SELECT COUNT(1) FROM iSchoolProduct.dbo.QuestionsAnswersInfos QA WHERE QA.UserId = T.user_Id
	) AS answersTotal
FROM
	iSchoolUser.dbo.Talent T
	INNER JOIN iSchoolUser.dbo.UserInfo UI ON UI.id = T.user_id
WHERE
	T.isdelete = 0
	AND T.status = 1
	AND T.certification_status = 1
    and T.Id in @ids
";
            return _dbcontext.Query<TalentAbout>(sql, new { ids });
        }

        public async Task<IEnumerable<TalentUser>> GetTalentUsers(IEnumerable<Guid> userIds)
        {

            var sql = $@"
SELECT
	UI.Id AS userId,
	UI.nickName,
	UI.headImgUrl,
    T.Type,
	T.certification_preview AS authTitle
FROM
	iSchoolUser.dbo.UserInfo UI
	LEFT JOIN iSchoolUser.dbo.Talent T ON UI.id = T.user_id AND T.isdelete = 0 AND T.status = 1 AND T.certification_status = 1
WHERE
    UI.Id in @userIds
";
            return await _dbcontext.QueryAsync<TalentUser>(sql, new { userIds });
        }

        /// <summary>
        /// 获取达人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TalentEntity GetTalentDetail(string id)
        {
            var talent = _dbcontext.Query<TalentEntity>(" select * from talent where id=@id and isdelete = 0", new { id }).FirstOrDefault();
            if (talent != null)
            {
                talent.imgs = _dbcontext.Query<TalentImg>(" select * from talent_img where talent_id=@talent_id", new { talent_id = id }).ToList();
            }
            return talent;
        }

        /// <summary>
        /// 根据达人获取其所有的员工达人
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<TalentDetail> GetTalentsByParentId(string parentId)
        {
            var sql = $@"
select 
    T.*,
    UI.nickname,
    TS.name as staffname
from 
    talent T
    inner join talent_staff TS on TS.talent_id = T.id and TS.isdelete = 0 and TS.status = 1
    left join userinfo UI on UI.id = T.user_id
where
    T.isdelete = 0
    and T.status = 1
    and T.certification_status = 1
    and TS.parentId = @parentId
";
            return _dbcontext.Query<TalentDetail>(sql, new { parentId }).ToList();
        }

        public TalentEntity GetTalent(string id)
        {
            return _dbcontext.Query<TalentEntity>(" select * from talent where id=@id and isdelete = 0 ", new { id }).FirstOrDefault();
        }
        #endregion
        public Lector GetLectorByUserId(string userId)
        {
            return _dbcontext.Query<Lector>($" select * from {_schoolLiveDb}.[lector] where userID = @userId ", new { userId }).FirstOrDefault();
        }

        public long GetLectureTotalByUserId(Guid userId, DateTime? startTime, DateTime? endTime)
        {
            return _dbcontext.QuerySingle<long>($@" 
select count(1) as total 
from 
    {_schoolLiveDb}.[lecture_v2] LE
    left join {_schoolLiveDb}.[lector] LR on LE.lector_id = LR.id AND LR.show = 1
WHERE 
	LR.userID = @userId
	and LE.show = 1 
	and LE.status IN (4, 5)
    and (@startTime is null or LE.time_start >= @startTime)
    and (@endTime is null or LE.time_start <= @endTime)
", new { userId, startTime, endTime });
        }

        /// <summary>
        /// 员工列表
        /// </summary>
        /// <param name="talentId"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<TalentStaff> GetStaffList(string userId, ref int total, int pageindex = 1, int pagesize = 10)
        {
            var parentId = _dbcontext.Query<TalentEntity>(" select * from talent where user_id=@userId", new { userId }).FirstOrDefault()?.id;
            if (parentId == null)
            {
                total = 0;
                return new List<TalentStaff>();
            }
            total = _dbcontext.Query<int>("select count(1) FROM talent_staff where parentId=@parentId and status=1 and isdelete=0 ", new { parentId, pageindex, pagesize }).First();

            var dataSql = $@"
select t.* 
from (
    SELECT 
		TS.*,
		UI.id as userId, UI.headImgUrl,
		ROW_NUMBER() OVER(order by TS.createdate desc) AS Row_Index 
    FROM 
		talent_staff TS
		left join talent T on T.id = TS.talent_id and TS.isdelete = 0
		left join userInfo UI on UI.id = T.user_id
    where 
        TS.parentId=@parentId and TS.status=1 and TS.isdelete=0
)t  
where 
    Row_Index between ((@pageindex - 1)* @pagesize + 1) and (@pageindex*@pagesize)

";
            return _dbcontext.Query<TalentStaff>(dataSql, new { parentId, pageindex, pagesize }).ToList();
        }

        /// <summary>
        /// 根据电话获取员工
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public TalentStaff GetTalentStaffByPhone(string phone, int status = 1)
        {
            return _dbcontext.Query<TalentStaff>("select * from talent_staff where phone=@phone and status = @status and isdelete=0 ", new { phone, status }).FirstOrDefault();
        }

        public TalentStaff GetTalentStaffById(string id, int isdelete = 0)
        {
            return _dbcontext.Query<TalentStaff>(" select * from talent_staff where id=@id and isdelete=@isdelete ", new { id, isdelete }).FirstOrDefault();
        }

        public TalentStaff GetTalentStaffByTalentId(string talentId, int isdelete = 0)
        {
            return _dbcontext.Query<TalentStaff>(" select * from talent_staff where talent_id=@talentId and isdelete=@isdelete ", new { talentId, isdelete }).FirstOrDefault();
        }

        public List<TalentStaff> GetTalentStaffsByParentId(string parentId, int isdelete = 0)
        {
            return _dbcontext.Query<TalentStaff>("select * from talent_staff where parentId = @parentId and isdelete=@isdelete  ", new { parentId, isdelete }).ToList();
        }

        /// <summary>
        /// 根据机构和电话获取员工
        /// </summary>
        /// <param name="parentId">归属机构</param>
        /// <param name="phone"></param>
        /// <param name="isdelete"></param>
        /// <returns></returns>
        public TalentStaff GetTalentStaffByParentIdPhone(string parentId, string phone, int isdelete = 0)
        {
            return _dbcontext.Query<TalentStaff>("select * from talent_staff where parentId = @parentId and phone=@phone and isdelete=@isdelete  ", new { parentId, phone, isdelete }).FirstOrDefault();
        }

        /// <summary>
        /// 获取员工邀请
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public TalentInvite GetTalentInviteByParentId(string parentId)
        {
            return _dbcontext.Query<TalentInvite>("select * from talent_invite where parent_id=@parent_id order by createdate desc",
                 new { parent_id = parentId }).FirstOrDefault();
        }

        /// <summary>
        /// 获取员工邀请
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public TalentInvite GetTalentInviteByCode(string code)
        {
            return _dbcontext.Query<TalentInvite>(" select * from talent_invite where code=@code order by createdate desc ",
                 new { code }).FirstOrDefault();
        }

        /// <summary>
        /// 根据用户ID获取达人
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isdelete"></param>
        /// <returns></returns>
        public TalentEntity GetTalentByUserId(string userId, int isdelete = 0)
        {
            return _dbcontext.Query<TalentEntity>(" select * from talent where user_id=@userId and isdelete=@isdelete ", new { userId, isdelete }).FirstOrDefault();
        }

        public bool IsTalent(Guid userId)
        {
            return _dbcontext.Query<int>(" select count(1) from talent where user_id=@userId and isdelete=0 and certification_status=1 and status=1 ", new { userId }).FirstOrDefault() > 0;
        }

        /// <summary>
        /// 检查是否符合认证条件
        /// </summary>
        /// <param name="userId"></param>
        public CheckConditionOutPut CheckCondition(Guid userId)
            => _dbcontext.Query<CheckConditionOutPut>(" select top 100 nationCode,mobile,headImgUrl,(select count(1) from collection where dataType in @dataTypes and userID=u.id) as attentionCount,(select count(1) from [iSchoolProduct].[dbo].QuestionsAnswersInfos where userID=u.id and IsAnony=0) as answerCount from userInfo u  where u.id=@userId ", new { userId, dataTypes = _collectionUserDataTypes }).FirstOrDefault();


        /// <summary>
        /// 获取新增的粉丝
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Object> GetNewFans(Guid userId, ref int total, int pageindex = 1, int pagesize = 10)
        {
            _dbcontext.BeginTransaction();
            total = _dbcontext.Query<int>("select count(1) FROM message where userID=@userId and dataType=5 and type=16 group by userID ", new { userId }).FirstOrDefault();
            var newsFans = _dbcontext.Query<dynamic>($@"
select
	m.time,m.id,nickname,headImgUrl,introduction,senderID,isattion,sex 
from 
(
	select 
		u.id,u.nickname,u.headImgUrl,u.introduction,m.time,m.senderID,
		(
			select (case when count(1)>0 THEN 'true' ELSE 'false' END) 
			from collection co 
			where dataType in @dataTypes and co.userID=m.dataID and co.dataID=m.senderID
		)as isattion,
		u.sex,
		ROW_NUMBER() OVER(order by time desc) AS Row_Index 
	from 
		message m 
		join userInfo u on m.senderID=u.id  
	where 
		userID=@userId and dataType=5 and type=16
)m 
where 
	m.Row_Index between ((@pageindex - 1)* @pagesize + 1) and (@pageindex*@pagesize) 
", new { userId, dataTypes = _collectionUserDataTypes, pageindex, pagesize }).ToList();
            _dbcontext.Execute(" update message set [Read]=1 where id=@id ", newsFans);
            var date = DateTime.Now;
            newsFans.ForEach(n =>
            {
                //判断是否同年
                if (n.time.ToString("yyyy") == date.ToString("yyyy"))
                {
                    if (n.time.ToString("MM-dd") == date.ToString("MM-dd"))
                    {
                        n.time = "今天";
                    }
                    else if (n.time.ToString("MM-dd") == date.AddDays(-1).ToString("MM-dd"))
                    {
                        n.time = "昨天";
                    }
                    else
                    {
                        n.time = n.time.ToString("MM月-dd日");
                    }
                }
                else
                {
                    n.time = n.time.ToString("yyyy年MM月dd日");
                }
            });
            _dbcontext.Commit();
            return newsFans;
        }

        /// <summary>
        /// 获取新增粉丝数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetNewFansCount(Guid userId)
            => _dbcontext.Query<int>("select count(1) FROM message where userID=@userId and [Read]=0 and dataType=5 and type=16 group by userID ", new { userId }).First();

        /// <summary>
        /// 获取ta的粉丝
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Object> GetFans(Guid userId, ref int total, int pageindex = 1, int pagesize = 10)
        {
            total = _dbcontext.Query<int>($"select count(1) FROM collection where dataID=@userId and dataType in @dataTypes", new { userId, dataTypes = _collectionUserDataTypes, pageindex, pagesize }).FirstOrDefault();
            return _dbcontext.Query<Object>($@" 
select 
    m.* 
from (
    select 
        ROW_NUMBER() OVER(order by time desc) AS Row_Index,
        t.certification_preview as talentCertificationPreview, t.id AS talentId,
        u.nickname,u.headImgUrl,u.introduction,u.id,u.sex, CONVERT(VARCHAR(10), c.time, 23) as time,
        (
            select (case  when count(1)>0 THEN 'true' ELSE 'false' END) 
            from collection co 
            where dataType in @dataTypes and co.userID=c.dataID and co.dataID=c.userID
        )as isattion 
    from 
        collection c 
        join userInfo u on c.userID=u.id
        LEFT JOIN talent t ON t.user_id = c.userID and t.isdelete = 0 and t.certification_status = 1 and t.status = 1
    where c.dataID=@userId and dataType in @dataTypes
)m 
where 
    m.Row_Index between ((@pageindex - 1)* @pagesize + 1) and (@pageindex*@pagesize) ", new { userId, dataTypes = _collectionUserDataTypes, pageindex, pagesize }).ToList();
        }

        /// <summary>
        /// 获取ta的关注
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Object> GetAttention(Guid userId, ref int total, int pageindex = 1, int pagesize = 10)
        {
            total = _dbcontext.Query<int>($@"select count(1) FROM collection where userID=@userId and dataType in @dataTypes", new { userId, dataTypes = _collectionUserDataTypes, pageindex, pagesize }).FirstOrDefault();
            return _dbcontext.Query<Object>($@" 
SELECT m.*
FROM (
	SELECT ROW_NUMBER() OVER (ORDER BY time DESC) AS Row_Index, u.nickname, u.headImgUrl, u.introduction, u.id,
		u.sex, c.time,
        t.certification_preview as talentCertificationPreview, t.id AS talentId,
		(
			SELECT CASE 
					WHEN COUNT(1) > 0 THEN 'true'
					ELSE 'false'
				END
			FROM collection co
			WHERE (dataType in @dataTypes
				AND co.userID = c.dataID
				AND co.dataID = c.userID)
		) AS isattion
	FROM collection c
		JOIN userInfo u ON c.dataID = u.id
        LEFT JOIN talent t ON t.user_id = c.dataID and t.isdelete = 0 and t.certification_status = 1 and t.status = 1
	WHERE c.userID = @userId
		AND dataType in @dataTypes
) m
WHERE m.Row_Index BETWEEN ((@pageindex - 1) * @pagesize + 1) AND @pageindex * @pagesize ", new { userId, dataTypes = _collectionUserDataTypes, pageindex, pagesize }).ToList();
        }


        /// <summary>
        /// 获取机构或个人达人身份列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identity"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<TalentIdentity> GetTalentPageIdentityList(Guid? id, string identity, ref int total, int type = 0, int pageindex = 1, int pagesize = 10)
        {
            var condition = new StringBuilder();
            dynamic param = new ExpandoObject();
            param.type = type;
            param.pageindex = pageindex;
            param.pagesize = pagesize;
            condition.Append(" isdelete=0 ");
            condition.Append(" and type=@type ");
            if (id != null)
            {
                condition.Append(" and id=@id ");
                param.id = id;
            }
            if (!string.IsNullOrEmpty(identity))
            {
                identity = $"%{identity}%";
                condition.Append(" and identity_name like @identity ");
                param.identity = identity;
            }
            var sql = $"select t.* from (SELECT *,ROW_NUMBER() OVER(order by createdate desc) AS Row_Index FROM talent_identity where {condition.ToString()})t where  Row_Index between ((@pageindex - 1)* @pagesize + 1) and (@pageindex*@pagesize) ";
            var talents = _dbcontext.Query<TalentIdentity>(sql, param);
            total = _dbcontext.Query<int>($" select count(1) from talent_identity where {condition.ToString()} ", new { id, identity, type }).FirstOrDefault();
            return talents;
        }

        /// <summary>
        /// 获取机构或个人达人身份列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<TalentIdentity> GetTalentAllIdentityList(Guid? id, string identity, int type = 0)
        {
            var condition = new StringBuilder();
            dynamic param = new ExpandoObject();
            param.type = type;
            //是否启用 0为否 1为是
            condition.Append(" isdelete=0 and enable = 1 ");
            condition.Append(" and type=@type ");
            if (id != null)
            {
                condition.Append(" and id=@id ");
                param.id = id;
            }
            if (!string.IsNullOrEmpty(identity))
            {
                identity = $"%{identity}%";
                condition.Append(" and identity_name like @identity ");
                param.identity = identity;
            }
            var sql = $"SELECT * FROM talent_identity where {condition.ToString()}";
            var talents = _dbcontext.Query<TalentIdentity>(sql, param);
            return talents;
        }

        public TalentIdentity GetTalentIdentityByNameType(string identityName, int? type = 0)
        {
            var sql = $@" SELECT * FROM talent_identity where isdelete=0 and enable = 1 and identity_name=@identityName and type=@type ";
            return _dbcontext.Query<TalentIdentity>(sql, new { identityName, type }).FirstOrDefault();
        }

        /// <summary>
        /// 检测是否为上学帮系统账号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckUserIsSysSxb(Guid userId)
        {
            return _dbcontext.Query<int>("select count(1) from talent where user_id = @userId and certification_type = 99 and type = 99", new { userId }).FirstOrDefault() > 0;
        }

        /// <summary>
        /// 获取个人资料
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public InterestOuPutDto GetUserData(Guid userId)
        {
            var userData = _dbcontext.Query<TUserData>($@" 
select u.id userID,u.nickname,u.headImgUrl,u.introduction,k.name,u.city,i.* 
from userInfo u left join [iSchoolData].[dbo].[KeyValue] k on u.city=k.id left join interest i on u.id=i.userID where u.id=@userId ", new { userId }).FirstOrDefault();

            if (userData == null)
            {
                return null;
            }
            var outPutData = new InterestOuPutDto()
            {
                cityName = userData.name,
                introduction = userData.introduction,
                nickname = userData.nickname,
                headImgUrl = userData.headImgUrl,
                CityCode = userData.city
            };

            var talent = GetTalentByUserId(userId.ToString());
            if (talent != null && talent.certification_status == 1 && talent.status == 1)
            {
                outPutData.isTalent = true;
                outPutData.talentId = talent.id.ToString();
                outPutData.talentType = talent.type;
                outPutData.talentCertificationPreview = talent.certification_preview;

                var talentStaff = GetTalentStaffByTalentId(talent.id.ToString());
                outPutData.isTalentStaff = talentStaff != null && talentStaff.isdelete == 0 && talentStaff.status == 1;
            }

            outPutData.initGrade(userData.grade_1, userData.grade_2, userData.grade_3, userData.grade_4);
            outPutData.initType(userData.nature_1, userData.nature_2, userData.nature_3);
            outPutData.initLodging(userData.lodging_0, userData.lodging_1);
            outPutData.UserID = userId;
            return outPutData;
        }


        private class TUserData
        {
            public string name { get; set; }
            public string introduction { get; set; }
            public string nickname { get; set; }
            public string headImgUrl { get; set; }
            public int? city { get; set; }
            public bool? grade_1 { get; set; }
            public bool? grade_2 { get; set; }
            public bool? grade_3 { get; set; }
            public bool? grade_4 { get; set; }
            public bool? nature_1 { get; set; }
            public bool? nature_2 { get; set; }
            public bool? nature_3 { get; set; }
            public bool? lodging_0 { get; set; }
            public bool? lodging_1 { get; set; }
        }

        /// <summary>
        /// 查询达人邀请码是否被使用过
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckTalentCode(string code)
        {
            return _dbcontext.Query<int>(@" select count(1) from talent_invite a where a.code=@code ",
                new { code }).FirstOrDefault() > 0;
        }

        /// <summary>
        /// 查询员工邀请码是否被使用过
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckTalentStaffCode(string phone, string code)
        {
            return _dbcontext.Query<int>(@"
select count(1) 
from talent_staff t join talent_invite a on t.id=a.parent_id 
where t.phone=@phone and a.code=@code ", new { phone, code }).FirstOrDefault() > 0;
        }

        public List<RecommendUserDto> GetTalentsByCity(int cityCode, bool isInCity, Guid[] excludeUserIds, long offset, int size)
        {
            var citySql = " and UI.city = @cityCode ";
            if (!isInCity)
                citySql = " and (UI.city is null or UI.city != @cityCode) ";

            var sql = $@"
select 
    UI.*
from 
    talent T
    inner join userinfo UI on UI.id = T.user_id
where
    T.isdelete = 0
    and T.status = 1
    and T.certification_status = 1
    and T.user_id not in @excludeUserIds
    {citySql}
ORDER BY
	T.id
offset @offset rows
FETCH next @size rows only
";
            return _dbcontext.Query<RecommendUserDto>(sql, new { cityCode, excludeUserIds, offset, size }).ToList();
        }

        public long GetTalentsByCityTotal(int cityCode, bool isInCity, Guid[] excludeUserIds)
        {
            var citySql = " and UI.city = @cityCode ";
            if (!isInCity)
                citySql = " and (UI.city is null or UI.city != @cityCode) ";

            var sql = $@"
select 
    Count(1) as Total
from 
    talent T
    inner join userinfo UI on UI.id = T.user_id
where
    T.isdelete = 0
    and T.status = 1
    and T.certification_status = 1
    and T.user_id not in @excludeUserIds
    {citySql}
";
            return _dbcontext.Query<long>(sql, new { cityCode, excludeUserIds }).FirstOrDefault();
        }
    }
}
