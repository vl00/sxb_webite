using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using Dapper;

namespace PMS.UserManage.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _dbcontext;

        public UserRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public UserInfo GetUserInfoDetail(Guid userId)
        {
            string sql = @"SELECT * from userInfo
                where id = @userid;";

            return _dbcontext.QuerySingle<UserInfo>(sql, new { userid = userId });
        }
        public UserInfo GetUserInfo(Guid userId)
        {
            string sql = @"SELECT top 1 u.id,u.nationCode,u.mobile,u.password,u.nickname,
                u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,u.introduction,
				[type] as verifyTypes
                from
                userInfo u
									left join talent as t on u.id = t.user_id and t.isdelete = 0
                where u.id = @userid;";

            return _dbcontext.QuerySingle<UserInfo>(sql, new { userid = userId });
        }

        public UserInfo GetUserInfo(string phone)
        {
            string sql = @"SELECT u.id,u.nationCode,u.mobile,u.password,u.nickname,
                u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,u.introduction,
                STUFF((SELECT ','+  CONVERT(varchar(10),[type]+1) from talent 
                where talent.user_id = u.id and talent.isdelete=0 and certification_status = 1
                for xml path('')),1,1,'') as verifyTypes
                from
                userInfo u
                where u.mobile = @phone ;";

            return _dbcontext.Query<UserInfo>(sql, new { phone }).FirstOrDefault();
        }

        public IEnumerable<UserInfo> GetUserInfos(string phone)
        {
            string sql = @"SELECT u.id,u.nationCode,u.mobile,u.password,u.nickname,
                u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,u.introduction,
                STUFF((SELECT ','+  CONVERT(varchar(10),[type]+1) from talent 
                where talent.user_id = u.id and talent.isdelete=0 and certification_status = 1
                for xml path('')),1,1,'') as verifyTypes
                from
                userInfo u
                where u.mobile = @phone ;";

            return _dbcontext.Query<UserInfo>(sql, new { phone });
        }


        public IEnumerable<Guid> GetSamplePhoneUserIds(Guid userId)
        {
            string sql = @" select Id from userInfo u
                where u.mobile = (select mobile from UserInfo where Id = @userId)";

            return _dbcontext.Query<Guid>(sql, new { userId });
        }

        public IEnumerable<UserInfo> GetUserInfosByName(string name)
        {
            string sql = @"SELECT u.id,u.nationCode,u.mobile,u.password,u.nickname,
                u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,u.introduction,
                STUFF((SELECT ','+  CONVERT(varchar(10),[type]+1) from talent 
                where talent.user_id = u.id and talent.isdelete=0 and certification_status = 1
                for xml path('')),1,1,'') as verifyTypes
                from
                userInfo u
                where u.nickname = @name ;";

            return _dbcontext.Query<UserInfo>(sql, new { name });
        }


        public bool AddUserInfo(UserInfo user)
        {
            var result = _dbcontext.Execute("INSERT INTO [dbo].[userInfo](" +
                "[id], [nickname], [mobile], [headImgUrl],[blockage],[regTime]) " +
                "VALUES (@userid, @username, @phone, @headImager , '0', getdate()); ",
                 new
                 {
                     userid = (user.Id == Guid.Empty ? Guid.NewGuid() : user.Id),
                     username = user.NickName,
                     phone = user.Mobile,
                     headImager = user.HeadImgUrl
                 });
            if (result < 1)
            {
                throw new Exception("新增失败！");
            }
            return result > 0;
        }

        public bool UpdateUserInfo(UserInfo user)
        {
            var result = _dbcontext.Execute(
                "UPDATE [dbo].[UserInfo] SET [nickname] =  @username, [mobile] =  @phone,[headImgUrl] = @headImager ,[introduction]=@introduction " +
                "WHERE [id] = @userid; ",
                 new { userid = user.Id, username = user.NickName, phone = user.Mobile, headImager = user.HeadImgUrl, introduction = user.Introduction });
            if (result < 1)
            {
                throw new Exception("修改失败！");
            }
            return result > 0;
        }

        public bool UpdateUserInfo(Guid userid, string nickName, string mobile, string headImgUrl, int? sex)
        {
            var result = _dbcontext.Execute(
                "UPDATE [dbo].[UserInfo] SET [nickname] =  @nickName, [mobile] =  @mobile,[headImgUrl] = @headImgUrl ,[sex] = @sex " +
                "WHERE [id] = @userid; ",
                 new { userid, nickName, mobile, headImgUrl, sex });
            if (result < 1)
            {
                throw new Exception("修改失败！");
            }
            return result > 0;
        }

        public bool UpdateUserInfo(Guid userid, string nickName, string headImgUrl)
        {
            var result = _dbcontext.Execute(
                "UPDATE [dbo].[UserInfo] SET [nickname] =  @nickName, [headImgUrl] = @headImgUrl " +
                "WHERE [id] = @userid; ",
                 new { userid, nickName, headImgUrl });
            if (result < 1)
            {
                throw new Exception("修改失败！");
            }
            return result > 0;
        }

        public List<OpenIdWeixin> ListUserInfoOpenId(List<Guid> userIds)
        {

            string sql = @"select * from [dbo].[openid_weixin]  where userid  IN @userids ;";
            return _dbcontext.Query<OpenIdWeixin>(sql,
                             new { userids = userIds.ToArray() }).ToList();
        }

        public List<UserInfo> ListUserInfo(List<Guid> userIds)
        {

            string sql = @"SELECT u.id,u.nationCode,u.mobile,u.password,u.nickname,
                u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,
                STUFF((SELECT ','+  CONVERT(varchar(10),[type]+1) from talent 
                where talent.user_id = u.id and talent.isdelete=0 and certification_status = 1
                for xml path('')),1,1,'') as verifyTypes
                from
                userInfo u
                WHERE u.[Id] IN @userids ;";


            return _dbcontext.Query<UserInfo>(sql,
                             new { userids = userIds.ToArray() }).ToList();
        }

        public List<UserInfo> ListAllUserInfo()
        {
            string sql = @"SELECT top 10 u.id,u.nationCode,u.mobile,u.password,u.nickname,
                u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,
                STUFF((SELECT ','+  CONVERT(varchar(10),[type]+1) from talent 
                where talent.user_id = u.id and talent.isdelete=0 and certification_status = 1
                for xml path('')),1,1,'') as verifyTypes
                from
                userInfo u;";

            return _dbcontext.Query<UserInfo>(sql, new { }).ToList();
        }

        public List<UserInvite> GetUserInfoByHistory(List<Guid> SchoolIds, UserRole Role, int PageIndex, int PageSize, int type, int dataType, Guid senderID, Guid dataID, Guid eID, Guid userId, int city)
        {
            string sql = sql = @"SELECT
	                        u.id,u.nickname,u.headImgUrl,STUFF((SELECT ','+  CONVERT(varchar(10),[type]+1) from talent 
                where talent.user_id = u.id and talent.isdelete=0 and certification_status = 1
                for xml path('')),1,1,'') as verifyTypes,
	                    case
		                    when inviteUserId is null then 0
		                    when inviteUserId is not null then 1
	                    end as isInvite ";

            if (Role != 0)
            {
                sql += @",v.intro1,
                        v.intro2 ";
            }

            sql += @" FROM
	                    userInfo AS u 
	                    ";

            if ((int)Role == 0)
            {
                sql += " RIGHT JOIN ( SELECT userID FROM history AS h WHERE h.dataType = 1 AND h.dataID in @dataId GROUP BY h.userID ) AS h ON u.id = h.userID ";
            }

            if ((int)Role != 0)
            {
                sql += " left join verify as v on u.id = v.userID and v.valid=1";
            }

            sql += @" LEFT JOIN ( select userID as inviteUserId from  message 
		                    where senderID = @senderID
			                    and type = @type
			                    and dataID = @sourceDataId
			                    and dataType = @dataType
			                    and eID = @eID) AS i on u.id = i.inviteUserId
	                     ";
            sql += " where u.id <> @userId ";
            if ((int)Role != 0)
            {
                sql += " and v.verifyType = @role and u.city = @city order by NEWID() OFFSET ( @pageIndex - 1 ) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY";
            }
            else
            {
                sql += " and u.id not in (select userID from verify) order by u.id OFFSET ( @pageIndex - 1 ) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY ";
            }

            if ((int)Role != 0)
            {
                return _dbcontext.Query<UserInvite>(sql, new { dataId = SchoolIds.ToArray(), type, senderID, sourceDataId = dataID, eID, dataType, pageIndex = PageIndex, pageSize = PageSize, role = (int)Role, userId, city }).ToList();
            }
            else
            {
                return _dbcontext.Query<UserInvite>(sql, new { dataId = SchoolIds.ToArray(), type, senderID, sourceDataId = dataID, eID, dataType, pageIndex = PageIndex, pageSize = PageSize, userId }).ToList();
            }
        }

        public List<UserInvite> GetUserInfoByHistory(int Role, int pageIndex, int pageSize, int type, int dataType, Guid senderID, Guid dataID, Guid eID, Guid userId, int city)
        {
            string sql = @"
                        SELECT 
                            *,STUFF((SELECT ','+  CONVERT(varchar(10),[type]+1) from talent 
                where talent.user_id = u.id and talent.isdelete=0 and certification_status = 1
                for xml path('')),1,1,'') as verifyTypes,
                        ";

            if (Role != 0)
            {
                sql += @"v.intro1,v.intro2,";
            }

            sql += @"
	                        case
		                        when inviteUserId is null then 0
		                        when inviteUserId is not null then 1
	                        end as isInvite 
                        FROM
	                        userInfo AS u
	                        LEFT JOIN ( select userID as inviteUserId from  message 
		                        where senderID = @senderID  
			                        and type = @type
			                        and dataID = @dataID
			                        and dataType = @dataType
			                        and eID = @eID) AS i on u.id = i.inviteUserId";
            if (Role != 0)
            {
                sql += " LEFT JOIN verify AS v ON u.id = v.userID and v.valid=1 ";
            }
            else
            {
                sql += " LEFT JOIN verify as v on v.userID = u.id ";
            }

            sql += " where u.id <> @userId ";

            if (Role != 0)
            {
                sql += "  and v.verifyType = @Role and u.city = @city order by NEWID() OFFSET @pageIndex * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY";
            }
            else
            {
                sql += " and v.userId is null order by u.id OFFSET @pageIndex * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY ";
            }


            if (Role != 0)
            {
                return _dbcontext.Query<UserInvite>(sql, new { Role, pageIndex, pageSize, senderID, type, dataID, dataType, eID, userId, city })?.ToList();
            }
            else
            {
                return _dbcontext.Query<UserInvite>(sql, new { pageIndex, pageSize, senderID, type, dataID, dataType, eID, userId })?.ToList();
            }
        }

        public List<UserInfo> GetOrdinaryUserInfo(int pageIndex, int pageSize, Guid userId)
        {
            string sql = "select * from userInfo where id <> @userId order by id OFFSET @pageIndex * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY";
            return _dbcontext.Query<UserInfo>(sql, new { pageIndex, pageSize, userId })?.ToList();
        }

        public List<string> CheckTodayInviteSuccessTotal(Guid userId, Guid eID, Guid dataID, int type, int dataType, DateTime todayTime)
        {
            string sql = @"select u.headImgUrl from message as m
		                          left join userInfo as u on m.userID = u.id
                                where 
	                        senderID = @userId and 
	                        type = @type and
	                        eID = @eID and
	                        dataType = @dataType and 
	                        dataID = @dataID
	                        and [time] >= @beginTime and [time] <= @endTime";

            DateTime beginTime = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, 0, 0, 0);
            DateTime endTime = new DateTime(todayTime.Year, todayTime.Month, todayTime.Day, 23, 59, 59);
            return _dbcontext.Query<string>(sql, new { userId, type, eID, dataType, dataID, beginTime, endTime })?.ToList();
        }

        public bool CheckUserisInvite(Guid userId, int type, int dataType, Guid senderID, Guid dataID, Guid eID)
        {
            string sql = @"select count(id) from message where userID = @userId and senderID = @senderID and type = @type and dataID = @dataID and dataType = @dataType and eID = @eID";
            return _dbcontext.Query<int>(sql, new { userId, type, dataType, senderID, dataID, eID }).FirstOrDefault() > 0 ? true : false;
        }

        public ThirdAuthUser CheckIsBinding(Guid UserId)
        {
            string sql = @"
                            select 
	                            top 1 
		                            t.nickname,
		                            t.id,
		                            t.mobile,
		                            t.cOpneId as OpneId
                            from 
                            (
                            select n.nickname,
			                            u.id,
			                            u.mobile,
			                            case 
				                            WHEN o.appName = 'fwh' then 2
				                            WHEN o.appName = 'app' then 1
				                            WHEN o.appName is null then 0
				                            else -1
			                            end as cOpneId
			                            from userInfo as u
								                            left join unionid_weixin as n on n.userID = u.id
                                            left join openid_weixin as o on u.id = o.userID
                                            where 
	                                            u.id = @UserId
					
                            ) as t order by t.cOpneId desc";

            SqlParameter[] para = {
                new SqlParameter("@UserId",UserId)
            };

            return _dbcontext.Query<ThirdAuthUser>(sql, new { UserId }).FirstOrDefault();
        }

        public List<Guid> GetShlCollectionList(Guid UserId, int dataType)
        {
            List<Guid> SclGids = new List<Guid>();

            string sql = @"select dataID from collection where userID = @UserId and dataType = @dataType
            order by time desc";

            SclGids = _dbcontext.Query<Guid>(sql, new { UserId, dataType })?.ToList();

            return SclGids;
        }

        public bool IsCollection(Guid dataID, int dataType)
        {
            List<Guid> Gids = new List<Guid>();
            string sql = @"select userID from collection where dataID = @dataID and dataType = @dataType
            order by time desc";

            Gids = _dbcontext.Query<Guid>(sql, new { dataID, dataType })?.ToList();
            if (Gids.Count > 0)
                return true;
            else
                return false;
        }

        public List<Guid> GetHistoryIds(string userId, int dataType) => _dbcontext.Query<Guid>("select dataID from history where userID = @userId and dataType=@dataType", new { userId, dataType })?.ToList();

        public Talent GetTalentDetail(Guid UserId)
        {
            return _dbcontext.Query<Talent>(@"
            select top 1
		        u.id,
		        u.nickname,
		        u.HeadImgUrl,
		        t.type as role,
		        u.introduction,
		        t.certification_title,
		        t.certification_preview,
		        e.eid,
		        t.organization_name,
		        case when t.certification_status = 1 and t.isdelete = 0 and t.status = 1 then 1 else 0 end as IsAuth,
		        case when ts.talent_id is not null then 1 else 0 end as IsTalentStaff,
		        pt.user_id as ParentUserId
	        from  userInfo as u 
		        left join talent as t on t.user_id = u.id and t.isdelete = 0
		        left join talent_staff as ts on ts.talent_id= t.id and ts.isdelete= 0
		        left join talent as pt on pt.id = ts.parentId and pt.isdelete = 0
		        left join talentSchoolExtension as e on t.id = e.talentId
		        where u.id = @UserId ", new { UserId }).FirstOrDefault();
        }

        public IEnumerable<Talent> GetTalentDetails(IEnumerable<Guid> UserIDs)
        {
            return _dbcontext.Query<Talent>(@"
            select
		        u.id,
		        u.nickname,
		        u.HeadImgUrl,
		        t.type as role,
		        u.introduction,
		        t.certification_title,
		        t.certification_preview,
		        e.eid,
		        t.organization_name,
		        case when t.certification_status = 1 and t.isdelete = 0 and t.status = 1 then 1 else 0 end as IsAuth,
		        case when ts.talent_id is not null then 1 else 0 end as IsTalentStaff,
		        pt.user_id as ParentUserId
	        from  userInfo as u 
		        left join talent as t on t.user_id = u.id
		        left join talent_staff as ts on ts.talent_id= t.id
		        left join talent as pt on pt.id = ts.parentId
		        left join talentSchoolExtension as e on t.id = e.talentId
		        where u.id in @UserIDs and t.isdelete = 0", new { UserIDs });
        }

        public IEnumerable<UserInfo> GetUsers(Guid[] ids)
        {
            string sql = @"
                SELECT 
                    u.id,u.nationCode,u.mobile,u.password,u.nickname,
                    u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,u.introduction
                from
                    userInfo u
                where 
                    u.id in @ids ;";

            return _dbcontext.Query<UserInfo>(sql, new { ids });
        }

        public openid_weixinDto GetOpenId(Guid userId, string type = "fwh")
        {
            string sql = "SELECT * FROM openid_weixin WHERE userID=@userId AND [valid] = 1";
            if (!string.IsNullOrWhiteSpace(type)) sql += " AND appName = @appName";

            return this._dbcontext.Query<openid_weixinDto>(sql, new { appName = type, userId = userId }).FirstOrDefault();
        }

        public openid_weixinDto GetOpenWeixin(string openId)
        {
            string sql = "SELECT * FROM openid_weixin WHERE openId=@openId AND [valid] = 1";
            return this._dbcontext.Query<openid_weixinDto>(sql, new { openId }).FirstOrDefault();
        }

        public UserInfo GetUserByWeixinOpenId(string openId)
        {
            string sql = @"SELECT  *  FROM userInfo
                    WHERE
                    EXISTS(SELECT 1 FROM openid_weixin WHERE openid_weixin.userID = userInfo.id AND openid_weixin.openID=@openId) ";

            return this._dbcontext.Query<UserInfo>(sql, new { openId }).FirstOrDefault();
        }

        public string GetUserWeixinNickName(Guid userId)
        {
            string sql = @"SELECT nickname FROM unionid_weixin WHERE userID = @userId;";

            return this._dbcontext.Query<string>(sql, new { userId }).FirstOrDefault();
        }


        public UserInfo GetUserByBaiduOpenId(string openId)
        {
            string sql = @"SELECT top 1 *  FROM userInfo
                    WHERE
                    EXISTS(SELECT 1 FROM openid_baidu WHERE openid_baidu.userID = userInfo.id AND openid_baidu.openID = @openId and valid =1)  ";

            return _dbcontext.Query<UserInfo>(sql, new { openId }).FirstOrDefault();
        }

        public OpenidBaidu GetBaiduOpenId(Guid userId)
        {
            string sql = "SELECT top 1 * FROM openid_baidu WHERE userID=@userId AND valid=1;";

            return this._dbcontext.Query<OpenidBaidu>(sql, new { userId }).FirstOrDefault();
        }

        public async Task<IEnumerable<UserInfo>> GetRandomUsers(int count)
        {
            string sql = $@"SELECT TOP {count} id into #temp  FROM userInfo 
ORDER BY NEWID()
SELECT * FROM userInfo
WHERE
EXISTS(SELECT 1 FROM #temp WHERE userInfo.id=#temp.id)
DROP TABLE #temp";
            var userInfos = await _dbcontext.QueryAsync<UserInfo>(sql, null);
            return userInfos;
        }


        public async Task<bool> CheckNicknameExist(string nickname)
        {
            var str_SQL = $"Select Count(1) from userInfo WHERE nickname = @nickname";
            return await _dbcontext.QuerySingleAsync<int>(str_SQL, new { nickname }) > 0;
        }

        public async Task<IEnumerable<UserInfo>> GetLastLoginUsersByStartTime(DateTime startTime)
        {
            return await _dbcontext.GetByAsync<UserInfo>("LoginTime >= @startTime", new { startTime }, order: "loginTime", fileds: new string[] { "ID", "NickName", "RegTime", "LoginTime" });
        }

        public string GetUnionWeixinNickName(Guid userId)
        {
            string sql = @"SELECT top 1 nickname from unionid_weixin where userId = @userId;";
            return _dbcontext.QuerySingle<string>(sql, new { userId = userId });
        }

        public bool SetDeviceToken(DeviceToken model)
        {
            return _dbcontext.Execute(@"merge into deviceToken
            using (select 1 as o) t
            on deviceToken.uuID = @uuid or deviceToken.deviceToken=@deviceToken
            when not matched then insert 
            (uuID, userID, deviceToken, type, city) values (@uuID, @userID, @deviceToken, @type, @city)
            when matched then update
            set deviceToken=@deviceToken, userID=@userID, type=@type, city=@city;", model) > 0;
        }
        public async Task<IEnumerable<UserInfo>> GetUserInfosByNames(IEnumerable<string> nicknames)
        {
            string sql = @"SELECT u.id,u.nationCode,u.mobile,u.password,u.nickname,
                u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,u.introduction,
                STUFF((SELECT ','+  CONVERT(varchar(10),[type]+1) from talent 
                where talent.user_id = u.id and talent.isdelete=0 and certification_status = 1
                for xml path('')),1,1,'') as verifyTypes
                from
                userInfo u
                where u.nickname in @nicknames ;";

            return await _dbcontext.QueryAsync<UserInfo>(sql, new { nicknames });
        }

        public async Task<string> GetWXUnionId(Guid userId)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            List<string> or = new List<string>();
            string sql = @"SELECT unionID FROM unionid_weixin
JOIN userInfo ON unionid_weixin.userID = userInfo.id
";
            or.Add("userInfo.id=@userId");
            dynamicParameters.Add("userId", userId);
            sql += $" where  valid =1 and ({string.Join(" or ",or)})";
            return await _dbcontext.ExecuteScalarAsync<string>(sql, dynamicParameters);
        }

        public async Task<bool> DeleteUser(Guid userID)
        {
            var entity_User = GetUserInfo(userID);
            if (entity_User?.Id != default)
            {
                var str_SQL = @"INSERT INTO [dbo].[RemovedUserInfo] (
	                                [id],
	                                [nationCode],
	                                [mobile],
	                                [password],
	                                [nickname],
	                                [regTime],
	                                [loginTime],
	                                [blockage],
	                                [headImgUrl],
	                                [sex],
	                                [city],
	                                [channel],
	                                [introduction],
	                                [source],
	                                [client],
                                    [RemoveTime]
                                )
                                VALUES
	                                (
	                                @id,
	                                @nationCode,
	                                @mobile,
	                                @password,
	                                @nickname,
	                                @regTime,
	                                @loginTime,
	                                @blockage,
	                                @headImgUrl,
	                                @sex,
	                                @city,
	                                @channel,
	                                @introduction,
	                                @source,
	                                @client,
                                    @removeTime
                                    );";
                var insertResult = await _dbcontext.ExecuteAsync(str_SQL, new
                {
                    id = entity_User.Id,
                    nationCode = entity_User.NationCode,
                    mobile = entity_User.Mobile,
                    password = entity_User.Password,
                    nickname = entity_User.NickName,
                    regTime = entity_User.RegTime,
                    loginTime = entity_User.LoginTime,
                    blockage = entity_User.Blockage,
                    headImgUrl = entity_User.HeadImgUrl,
                    sex = entity_User.Sex,
                    city = entity_User.City,
                    channel = entity_User.City,
                    introduction = entity_User.Introduction,
                    source = entity_User.Source,
                    client = entity_User.Client,
                    removeTime = DateTime.Now
                });
                if (insertResult > 0)
                {
                    str_SQL = "Delete From [UserInfo] Where [ID] = @userID;";
                    return (await _dbcontext.ExecuteAsync(str_SQL, new { userID })) > 0;
                }
            }
            return false;
        }
    }
}
