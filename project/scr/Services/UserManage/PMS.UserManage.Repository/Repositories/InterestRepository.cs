using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.UserManage.Repository.Repositories
{
    public class InterestRepository: IInterestRepository
    {
        private readonly UserDbContext _dbcontext;
        public InterestRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public bool SetUserInterest(Interest interest)
        {
            if (interest.UserID != null)
            {
                if (_dbcontext.Execute(@"merge into interest
using (select 1 as o) t
on interest.userID=@userID
when matched then update set
grade_1=@grade_1, grade_2=@grade_2, grade_3=@grade_3, grade_4=@grade_4,
nature_1=@nature_1, nature_2=@nature_2, nature_3=@nature_3,nature_4=@nature_4
lodging_0=@lodging_0, lodging_1=@lodging_1,
uuID = @uuID;
", interest) == 0)
                {
                    return SetDeviceInterest(interest);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return SetDeviceInterest(interest);
            }
        }
        public bool SetDeviceInterest(Interest interest)
        {
            return _dbcontext.Execute(@"merge into interest
using (select 1 as o) t
on interest.uuID=@uuID
when not matched then insert
(grade_1, grade_2, grade_3, grade_4, nature_1, nature_2, nature_3, lodging_0, lodging_1, uuid, userID) values
(@grade_1, @grade_2, @grade_3, @grade_4, @nature_1, @nature_2, @nature_3, @lodging_0, @lodging_1, @uuid, @userID)
when matched then update set
grade_1=@grade_1, grade_2=@grade_2, grade_3=@grade_3, grade_4=@grade_4, 
nature_1=@nature_1, nature_2=@nature_2, nature_3=@nature_3,
lodging_0=@lodging_0, lodging_1=@lodging_1, userID=@userID;
", interest) > 0;
        }
        public bool UserInterestExists(Guid userID)
        {
            return _dbcontext.Query<int>(@"select count(*) from interest where userID=@userID", new { userID }).FirstOrDefault() > 0;
        }
        public Interest GetUserInterest(Guid? userID, Guid? uuID)
        {
            if (userID != null && userID.Value != Guid.Empty)
            {
                var result = _dbcontext.Query<Interest>(@"select * from interest where userID=@userID", new { userID }).FirstOrDefault();
                if (result == null)
                {
                    return _dbcontext.Query<Interest>(@"select * from interest where uuID=@uuID", new { uuID }).FirstOrDefault();
                }
                return result;
            }
            else
            {
                return _dbcontext.Query<Interest>(@"select * from interest where uuID=@uuID", new { uuID }).FirstOrDefault();
            }
        }
        public List<InterestItem> GetInterestColumns()
        {
            return _dbcontext.Query<InterestItem>(@"SELECT B.name AS [Key],CONVERT(varchar(200), cp.value) AS [Value]	
FROM sys.tables A INNER JOIN sys.columns B	ON B.object_id = A.object_id LEFT JOIN sys.extended_properties cp	
ON cp.major_id = B.object_id AND cp.minor_id = B.column_id
where a.name = 'interest'", new { }).ToList();
        }
    }
}
