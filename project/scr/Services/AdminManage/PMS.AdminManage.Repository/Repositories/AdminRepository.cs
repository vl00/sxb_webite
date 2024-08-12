using System;
using PMS.AdminManage.Domain.Entities;
using PMS.AdminManage.Domain.IRepositories;

namespace PMS.AdminManage.Repository.Repositories
{
    public class AdminRepository: IAdminRepository
    {
        private readonly AdminDbContext _dbcontext;
        public AdminRepository(AdminDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public AdminInfo GetUserInfo(Guid userId)
        {
            return _dbcontext.QuerySingle<AdminInfo>("SELECT [Id], [NickName],[HeadImager], [Phone], [Role] FROM [dbo].[UserInfo] WHERE [Id]=@userId",
                 new { userid = userId });
        }

        public bool AddUserInfo(AdminInfo user)
        {
            var result = _dbcontext.Execute("INSERT INTO [dbo].[UserInfo]([Id], [NickName], [Phone], [Role],[HeadImager]) " +
                "VALUES (@userid, @username, @phone, @role,@headImager); ",
                 new
                 {
                     userid = (user.Id == Guid.Empty ? Guid.NewGuid() : user.Id),
                     username = user.NickName,
                     phone = user.Phone,
                     role = user.Role,
                     headImager = user.HeadImager
                 });
            if (result < 1)
            {
                throw new Exception("新增失败！");
            }
            return result > 0;
        }

        public bool UpdateUserInfo(AdminInfo user)
        {
            var result = _dbcontext.Execute("UPDATE [dbo].[UserInfo] SET [NickName] =  @username, [Phone] =  @phone, [Role] = @role,[HeadImager] = @headImager " +
                "WHERE [Id] = @userid; ",
                 new { userid = user.Id, username = user.NickName, phone = user.Phone, role = user.Role, headImager = user.HeadImager });
            if (result < 1)
            {
                throw new Exception("修改失败！");
            }
            return result > 0;
        }
    }
}
