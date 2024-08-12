using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PMS.UserManage.Repository.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private static readonly object wxlock = new object();
        private static readonly object qqlock = new object();

        private INewAccountRepository _account;

        private readonly UserDbContext _dbcontext;
        public LoginRepository(UserDbContext dbcontext, INewAccountRepository account)
        {
            _dbcontext = dbcontext;
            _account = account;
        }

        public bool RegisUserInfo(ref UserInfo userInfo)
        {
            if (userInfo.Id == Guid.Empty)
            {
                userInfo.Id = Guid.NewGuid();
            }

            if (_dbcontext.QuerySingle<int>("Select Count(1) From UserInfo Where NickName = @name", new { name = userInfo.NickName }) > 0)
            {
                var newNickname = userInfo.NickName + userInfo.Id.ToString().Substring(0, 8);
                if (_dbcontext.QuerySingle<int>("Select Count(1) From UserInfo Where NickName = @name", new { name = newNickname }) > 0)
                {
                    userInfo.Id = Guid.Empty;
                    return false;
                }
                userInfo.NickName = newNickname;
            }

            userInfo.RegTime = DateTime.Now;
            userInfo.LoginTime = userInfo.RegTime;
            userInfo.HeadImgUrl = string.IsNullOrWhiteSpace(userInfo.HeadImgUrl) ? "https://cos.sxkid.com/images/head.png" : userInfo.HeadImgUrl;
            return _dbcontext.Execute(@"insert into userInfo 
(id,nationCode,mobile,password,nickname,regTime,loginTime,blockage,headImgUrl,sex,city,channel) values 
(@id,@nationCode,@mobile,@password,@nickname,@regTime,@loginTime,@blockage,@headImgUrl,@sex,@city,@channel)", userInfo) > 0;
        }
        
        
        public bool QQUnionIDLogin(Guid unionID, Guid openID, string appName, ref UserInfo userInfo)
        {
            lock (qqlock)
            {
                var info = _dbcontext.Query<UserInfo>(@"select * from userInfo where exists 
(select 1 from unionid_qq where userID=id and unionID=@unionID)", new { unionID }).FirstOrDefault();
                if (info == null)
                {
                    QQOpenIDLogin(openID, appName, ref userInfo);
                    _account.BindQQUnionID(unionID, userInfo.Id);
                }
                else
                {
                    if (info.Blockage) return false;
                    userInfo = info;
                    _account.BindQQOpenID(openID, userInfo.Id, appName);
                }
                _dbcontext.Execute("update userInfo set channel=null, loginTime = @loginTime where id=@id", new { id = userInfo.Id, loginTime = DateTime.Now });
                return true;
            }
        }
        public bool QQOpenIDLogin(Guid openID, string appName, ref UserInfo userInfo)
        {
            lock (qqlock)
            {
                var info = _dbcontext.Query<UserInfo>(@"select * from userInfo where exists 
(select 1 from openid_qq where userID=id and openid=@openid)", new { openID }).FirstOrDefault();
                if (info == null)
                {
                    userInfo.Id = Guid.NewGuid();
                    RegisUserInfo(ref userInfo);
                    _account.BindQQOpenID(openID, userInfo.Id, appName);
                }
                else
                {
                    if (info.Blockage) return false;
                    userInfo = info;
                }
                _dbcontext.Execute("update userInfo set channel=null, loginTime = @loginTime where id=@id", new { id = userInfo.Id, loginTime = DateTime.Now });
                return true;
            }
        }


    }
}
