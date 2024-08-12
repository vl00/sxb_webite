using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.UserManage.Repository.Repositories
{
    public class VerifyRepository : IVerifyRepository
    {
        private readonly UserDbContext _dbcontext;

        public VerifyRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public List<Verify> GetVerifies(Guid userID)
        {
            return _dbcontext.Query<Verify>("select * from verify where userID=@userID", new { userID }).ToList();
        }

        public bool SubmitVerify(Verify verifyInfo)
        {
            return _dbcontext.Execute(@"insert into verify 
(id, userID, realName, idType, idNumber, valid, time, verifyType, intro1, intro2, platform, account)
values
(@id, @userID, @realName, @idType, @idNumber, @valid, @time, @verifyType, @intro1, @intro2, @platform, @account)", verifyInfo) > 0;

        }
    }
}
