using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;

namespace PMS.UserManage.Repository.Repositories
{
    /// <summary>
    /// 与账号有关数据仓库
    /// </summary>
    public class NewAccountRepository : INewAccountRepository
    {
        private readonly UserDbContext _dbcontext;
        public NewAccountRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public bool CreateUserInfo(UserInfo userInfo)
        {
            return _dbcontext.Execute(@"insert into userInfo 
            (id,nationCode,mobile,password,nickname,regTime,loginTime,blockage,headImgUrl,sex,city,channel,source,client) values 
            (@id,@nationCode,@mobile,@password,@nickname,getdate(),getdate(),@blockage,@headImgUrl,@sex,@city,@channel,@source,@client)", userInfo) > 0;
        }
        /// <summary>
        /// 检查重复用户名
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public bool CheckRepeatNickName(string nickName)
        {
            return _dbcontext.QuerySingle<int>("Select Count(1) From UserInfo Where NickName = @name", new { name = nickName }) > 0;
        }
        /// <summary>
        /// 获取用户被封禁状态
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int CheckAccountStatus(Guid userID)
        {
            return _dbcontext.QuerySingle<int>("select blockage from userInfo where id=@userID", new { userID });
        }
        /// <summary>
        /// 更新用户基本信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUserInfo(UserInfo user)
        {
            var result = _dbcontext.Execute(
                "UPDATE [dbo].[UserInfo] SET [nickname] =  @username, [mobile] =  @phone,[headImgUrl] = @headImager " +
                "WHERE [id] = @userid; ",
                 new { userid = user.Id, username = user.NickName, phone = user.Mobile, headImager = user.HeadImgUrl });
            if (result < 1)
            {
                throw new Exception("修改失败！");
            }
            return result > 0;
        }
        /// <summary>
        /// 更新用户登录时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLoginTime(Guid userId)
        {
            return _dbcontext.Execute("update userInfo set channel=null, loginTime = getdate() where id=@id", new { id = userId }) > 0;
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserInfo GetUserInfo(Guid userId)
        {
            string sql = @"SELECT top 1 u.id,u.nationCode,u.mobile,u.password,u.nickname,
                u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,u.introduction,
				[type] as verifyTypes
                from
                userInfo u left join talent as t on u.id = t.user_id and t.isdelete = 0
                where u.id = @userid;";

            return _dbcontext.QuerySingle<UserInfo>(sql, new { userid = userId });
        }
        /// <summary>
        /// 批量ID获取用户
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
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

        #region 手机号账号
        /// <summary>
        /// 根据手机号获取用户
        /// </summary>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public List<UserInfo> GetUserInfoByMobile(short nationCode, string mobile)
        {
            string sql = @"SELECT u.id,u.nationCode,u.mobile,u.password,u.nickname,
                u.regTime,u.loginTime,u.blockage,u.headImgUrl,u.sex,u.city,u.channel,u.introduction,
                STUFF((SELECT ','+  CONVERT(varchar(10),[type]+1) from talent 
                where talent.user_id = u.id and talent.isdelete=0 and certification_status = 1
                for xml path('')),1,1,'') as verifyTypes
                from
                userInfo u
                where u.nationCode = @nationCode and u.mobile = @mobile order by u.loginTime desc;";

            return _dbcontext.Query<UserInfo>(sql, new { nationCode, mobile }).ToList();
        }

        /// <summary>
        /// 判断旧手机号是否正确
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool OldMobileMatch(Guid userID, short nationCode, string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return _dbcontext.Query<int>("select count(*) from userInfo where id=@userID and mobile is null", new { userID }).FirstOrDefault() == 1;
            }
            else
            {
                return _dbcontext.Query<int>("select count(*) from userInfo where id=@userID and nationCode=@nationCode and mobile=@mobile", new { userID, nationCode, mobile }).FirstOrDefault() == 1;
            }
        }
        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool BindMobile(Guid userID, short nationCode, string mobile)
        {
            return _dbcontext.Execute("update userInfo set nationCode=@nationCode, mobile=@mobile, channel=null where id=@userID", new { nationCode, mobile, userID }) > 0;
        }
        /// <summary>
        /// 检查手机号是否冲突
        /// </summary>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool CheckMobileConflict(short nationCode, string mobile)
        {
            return _dbcontext.Query<int>("select count(*) from userInfo where nationCode=@nationCode and mobile=@mobile", new { nationCode, mobile }).FirstOrDefault() == 0;
        }
        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool CheckBindMobile(Guid userID, short nationCode, string mobile)
        {
            return _dbcontext.Query<int>("select 1 from userInfo where id = @userID and nationCode=@nationCode and mobile=@mobile", new { nationCode, mobile, userID }).FirstOrDefault() == 1;
        }
        // <summary>
        /// 获取绑定的手机号
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetBindMobile(Guid userID)
        {
            return _dbcontext.Query<string>("select mobile from userInfo where id=@userID", new { userID }).FirstOrDefault();
        }
        /// <summary>
        /// 获取绑定手机号的账号数量
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        public int GetMobileBindCount(string mobileNo)
        {
            var result = 0;
            var str_SQL = $"Select COUNT(1) from userInfo WHERE mobile = @mobileNo";
            result = _dbcontext.QuerySingle<int>(str_SQL, new { mobileNo });
            return result;
        }

        public bool CheckPasswordLogin(short nationCode, string mobile, string password)
        {
            Guid pw = Guid.Parse(password);
            return _dbcontext.QuerySingle<int>("select Count(1) from userInfo where nationCode=@nationCode and mobile = @mobile and password = @pw;", new { nationCode, mobile, pw }) > 0;
        }
        /// <summary>
        /// 修改手机号码
        /// </summary>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ChangePSW(short nationCode, string mobile, Guid password)
        {
            return _dbcontext.Execute("update userInfo set password=@password, channel=null where nationCode=@nationCode and mobile=@mobile", new { nationCode, mobile, password }) > 0;
        }
        /// <summary>
        /// 列出手机号码冲突账号
        /// </summary>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public List<UserInfo> ListMobileConflict(short nationCode, string mobile)
        {
            return _dbcontext.Query<UserInfo>("select id, nickname, headImgUrl from userInfo where nationCode=@nationCode and mobile=@mobile", new { nationCode, mobile }).ToList();
        }
        #endregion

        #region 第三方账号--微信
        /// <summary>
        /// 检查微信UnionID号是否冲突,返回true是没有冲突
        /// </summary>
        /// <param name="unionID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool CheckWXCanBind(string unionID, Guid userID)
        {
            return _dbcontext.Query<int>("select count(*) from unionid_weixin where unionID=@unionID and userID!=@userID and valid = 1;", new { unionID, userID }).FirstOrDefault() == 0;
        }
        /// <summary>
        /// 列出微信冲突账号
        /// </summary>
        /// <param name="unionid"></param>
        /// <returns></returns>
        public List<UserInfo> ListWXConflict(string unionid)
        {
            return _dbcontext.Query<UserInfo>(@"select id, nickname, headImgUrl from userInfo where 
exists (select 1 from unionid_weixin where unionid=@unionid and userID=userInfo.id and valid = 1)", new { unionid }).ToList();
        }
        /// <summary>
        /// 绑定微信OpenID
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="appName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool BindWXOpenID(string openID, Guid userID, string sessionKey, string appName, string appId, bool subscribeStatus = false)
        {
            return _dbcontext.Execute(@"merge into openid_weixin
            using (select 1 as o) t
            on openid_weixin.openid = @openID
            when not matched then insert 
            (openID, userID, appName,appId, sessionKey ,valid,[createtime]) values (@openID, @userID, @appName,@appId,@sessionKey, @valid, getdate())
            when matched then update
            set userID=@userID  ,appName = @appName,appId=@appId,sessionKey = @sessionKey, valid = @valid;", new { openID, userID, appName, appId, sessionKey, valid = subscribeStatus }) > 0;
        }
        /// <summary>
        /// 绑定微信UnionID
        /// </summary>
        /// <param name="unionID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool BindWXUnionID(string unionID, Guid userID, string nickname)
        {
            return _dbcontext.Execute(@"merge into unionid_weixin 
            using (select 1 as o) t
            on unionid_weixin.unionID = @unionID
            when not matched then insert 
            (unionID, userID, nickname) values (@unionID, @userID, @nickname)
            when matched then update
            set userID=@userID, nickname=@nickname, valid = 1;", new { unionID, userID, nickname }) > 0;
        }
        /// <summary>
        /// 获取绑定的微信UnionID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetBindWeixin(Guid userID)
        {
            return _dbcontext.Query<string>("select unionID from unionid_weixin where userID=@userID and [valid] = 1 ;", new { userID }).FirstOrDefault();
        }
        /// <summary>
        /// 微信解绑
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UnboundWx(Guid userID)
        {
            //string unionid_weixin = "delete from unionid_weixin where userID = @userID";
            string openid_weixin = "delete from openid_weixin where userID = @userID";
            string sql = "UPDATE [dbo].[unionid_weixin] SET [valid] = 0 where userid = @userID;";
            return _dbcontext.Tran(dbtransaction =>
            {
                _dbcontext.Execute(openid_weixin, new { userID }, dbtransaction);
                var ret = _dbcontext.Execute(sql, new { userID }, dbtransaction);
                return ret > 0;
            });
        }
        /// <summary>
        /// 获取微信OpenID绑定的用户信息
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public ThirdAuthUser GetWXOpenIDUserInfo(string openID)
        {
            string sql = @"select top 1 ui.*,ow.openID,ow.appId,ow.appName,uw.unionid from userInfo ui
            LEFT JOIN openid_weixin ow on ow.userID =  ui.id
            LEFT JOIN unionid_weixin uw on uw.userID = ui.id and uw.valid = 1
            where ow.openID =@openID;";
            var info = _dbcontext.Query<ThirdAuthUser>(sql, new { openID }).FirstOrDefault();
            return info;
        }
        /// <summary>
        /// 获取微信unionID绑定的用户信息
        /// </summary>
        /// <param name="unionID"></param>
        /// <returns></returns>
        public ThirdAuthUser GetWXUnionIDUserInfo(string unionID, string appId)
        {
            string sql = @"select top 1 ui.*,ow.openID,ow.appId,ow.appName,uw.unionid from userInfo ui
            LEFT JOIN openid_weixin ow on ow.userID = ui.id and ow.appId = @appId
            LEFT JOIN unionid_weixin uw on uw.userID = ui.id and uw.valid = 1
            where uw.unionID = @unionID;";
            var info = _dbcontext.Query<ThirdAuthUser>(sql, new { unionID, appId }).FirstOrDefault();
            return info;
        }
        public OpenIdWeixin GetWeixinOpenId(Guid userId, string appName)
        {
            return _dbcontext.Query<OpenIdWeixin>(@"select top 1 * from openid_weixin where userId=@userId and appName=@appName;", new { userId, appName }).FirstOrDefault();
        }

        public bool UpdateWeiXinSessionKey(string openid, string sessionKey)
        {
            string sql = "UPDATE openid_weixin SET [sessionKey]=@sessionKey WHERE openid=@openid";
            return _dbcontext.Execute(sql, new { openid, sessionKey }) > 0;
        }

        public async Task<bool> UpdateOpenIDUserID(Guid oldUserID, Guid newUserID)
        {
            if (oldUserID == default || newUserID == default) return false;
            var str_SQL = "Update [openid_weixin] Set [userID] = @newUserID Where [userID] = @oldUserID";
            var result = await _dbcontext.ExecuteAsync(str_SQL, new { oldUserID, newUserID });
            return result > 0;
        }

        #endregion

        #region 第三方账号--百度
        /// <summary>
        /// 绑定百度OpenID
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="appName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool BindBDOpenID(string openID, Guid userID, string accessKey, string appName = "baidubox", bool valid = true)
        {
            //return _dbcontext.Execute(@"insert into openid_weixin (openID, userID, appName, valid) values (@openID, @userID, @appName, @valid)", new { openID, userID, appName, valid = false }) > 0;
            return _dbcontext.Execute(@"merge into openid_baidu
            using (select 1 as o) t
            on openid_baidu.openid = @openID
            when not matched then insert 
            ([openID], [userID], [appName], [accessKey],[valid],[createTime]) values
            (@openId ,@userId,  @appName, @accessKey , @valid, getdate())
            when matched then update
            set userID=@userID  ,appName = @appName,accessKey=@accessKey, valid = @valid;",
            new { openID, userID, appName, accessKey, valid }) > 0;
        }

        public UserInfo GetUserByBaiduOpenId(string openId)
        {
            string sql = @"SELECT top 1 *  FROM userInfo
                    WHERE EXISTS(SELECT 1 FROM openid_baidu WHERE openid_baidu.userID = userInfo.id AND openid_baidu.openID = @openId and valid =1)  ";

            return _dbcontext.Query<UserInfo>(sql, new { openId }).FirstOrDefault();
        }
        public OpenidBaidu GetBaiduOpenId(Guid userId)
        {
            string sql = "SELECT top 1 * FROM openid_baidu WHERE userID=@userId AND valid=1;";

            return this._dbcontext.Query<OpenidBaidu>(sql, new { userId }).FirstOrDefault();
        }
        #endregion

        #region 第三方账号--QQ
        /// <summary>
        /// 检查QQUnionID号是否冲突
        /// </summary>
        /// <param name="unionID"></param>
        /// <returns></returns>
        public bool CheckQQConflict(Guid unionID)
        {
            return _dbcontext.Query<int>("select count(*) from unionid_qq where unionID=@unionID", new { unionID }).FirstOrDefault() == 0;
        }
        /// <summary>
        /// 列出QQ冲突账号
        /// </summary>
        /// <param name="unionid"></param>
        /// <returns></returns>
        public List<UserInfo> ListQQConflict(Guid unionid, Guid openid)
        {
            return _dbcontext.Query<UserInfo>(@"select id, nickname, headImgUrl from userInfo where 
exists (select 1 from unionid_qq where unionid=@unionid and userID=userInfo.id)
or exists (select 1 from openid_qq where openid=@openid and userID=userInfo.id)
", new { unionid, openid }).ToList();
        }
        /// <summary>
        /// 绑定QQOpenID
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="userID"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        public bool BindQQOpenID(Guid openID, Guid userID, string appName)
        {
            return _dbcontext.Execute(@"merge into openid_qq
            using (select 1 as o) t
            on openid_qq.openID=@openID
            when not matched then insert 
            (openID, userID, appName) values (@openID, @userID, @appName)
            when matched then update
            set userID=@userID;", new { openID, userID, appName }) > 0;
        }
        /// <summary>
        /// 绑定QQUnionID
        /// </summary>
        /// <param name="unionID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool BindQQUnionID(Guid unionID, Guid userID)
        {
            return _dbcontext.Execute(@"merge into unionid_qq
            using (select 1 as o) t
            on unionid_qq.unionID=@unionid
            when not matched then insert 
            (unionID, userID) values (@unionID, @userID)
            when matched then update
            set userID=@userID;", new { unionID, userID }) > 0;
        }
        /// <summary>
        /// 获取绑定的QQUnionID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Guid? GetBindQQ(Guid userID)
        {
            return _dbcontext.Query<Guid?>("select unionID from unionid_qq where userID=@userID", new { userID }).FirstOrDefault();
        }
        /// <summary>
        /// 解绑QQ
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UnboundQQ(Guid userID)
        {
            string unionid_qq = "delete from unionid_qq where userID = @userID";
            string openid_qq = "delete from openid_qq where userID = @userID ";
            return _dbcontext.Tran(dbtransaction =>
            {
                var ret = _dbcontext.Execute(unionid_qq, new { userID }, dbtransaction);
                ret += _dbcontext.Execute(openid_qq, new { userID }, dbtransaction);
                return ret > 0;
            });
        }
        #endregion
    }
}
