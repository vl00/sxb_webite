using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.UserManage.Repository.Repositories
{
    public class InviteStatusRepository : IInviteStatusRepository
    {
        private readonly UserDbContext _dbcontext;
        public InviteStatusRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void AddInviteStatus(List<InviteStatus> models)
        {
            _dbcontext.Execute(@"insert into InviteStatus(Type,UserId,SenderId,DataId,Content,Eid,SendTime,IsRead) 
                    values(@Type,@UserId,@SenderId,@DataId,@Content,@Eid,default,@IsRead)", models);
        }


        public bool UpdateInviteStatu(bool IsRead ,Guid UserId = default,Guid SenderId = default,List<Guid> DataId = default) 
        {
            string sql = "update InviteStatus set IsRead = @IsRead where ";

            if (UserId != default)
            {
                sql += " UserId = @UserId and DataId in @DataId";
                return _dbcontext.Execute(sql, new { IsRead, UserId, DataId }) > 0;
            }
            else 
            {
                sql += " SenderId = @SenderId and DataId in @DataId ";
                return _dbcontext.Execute(sql, new { IsRead, SenderId, DataId }) > 0;
            }
        }


        public List<InviteSelf> GetMessageBySenderUser(Guid userId, int page, int size)
        {
            return _dbcontext.Query<InviteSelf>(@"select 
	                i.Type,
	                i.DataId,
	                i.Content,
	                s.name+'-'+e.name as SchName,
	                (select count(1) from InviteStatus where senderID = i.SenderId and DataID = i.DataId) as InviteTotal,
	                case  when (select count(1) from InviteStatus where senderID = i.SenderId and IsRead = 1) > 0 then 1 else 0 end as IsRead
                from InviteStatus as i
	                right join 
                (select 
		                id,
		                row_number () OVER ( partition BY DataID ORDER BY SendTime DESC ) AS Take
	                from InviteStatus 
		                where senderID = @userId) as m on i.id = m.id
	                left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e on e.id = i.Eid
	                left join [iSchoolData].[dbo].[OnlineSchool] as s on s.id = e.sid
	                where m.Take = 1
		                order by SendTime desc
		                offset (@page-1)*@size rows fetch next @size row only
            ", new { userId, page, size })?.ToList();
        }

        public List<InviteUserInfo> GetInviteUserInfos(List<Guid> dataId, Guid senderId)
        {
            return _dbcontext.Query<InviteUserInfo>(@"select 
			        m.dataID,
			        m.type,
			        u.id,
			        u.HeadImgUrl
		        from userInfo as u 
			        RIGHT JOIN
				        (select 
					        userID,
					        dataID,
					        type,
					        row_number () OVER ( partition BY dataID ORDER BY [sendTime] DESC ) as take
				        from InviteStatus where dataID in @dataId
				         and senderID = @senderId) as m on u.id = m.userID
				        where m.take <= 5", new { dataId, senderId })?.ToList();
        }



    }
}
