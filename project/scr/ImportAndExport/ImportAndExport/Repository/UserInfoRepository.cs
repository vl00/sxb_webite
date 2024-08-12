using ImportAndExport.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Repository
{
    public class UserInfoRepository
    {

        private DataDbContext _context;

        public UserInfoRepository(DataDbContext context)
        {
            _context = context;
        }

        public List<InviteComment> InviteComments(DateTime startTime,DateTime endTime) 
        {
            string sql = @"select 
	                            m.[time] as InviteTime,
                                u.nickname as DarenName,
	                            m.senderID as InviteUser,
	                            m.userID as ReceiveUser,
	                            m.eID as Eid,
	                            s.id as Sid,
	                            s.name as Sname,
	                            e.name as Ename
                            from message as m 
                            left JOIN [iSchoolUser].dbo.userInfo as u on u.id = m.userID
                            left join [iSchoolUser].dbo.verify as v on u.id = v.userID
                            left join [iSchoolData].dbo.OnlineSchoolExtension as e on e.id = m.eID
                            left join [iSchoolData].dbo.OnlineSchool as s on e.sid = s.id
                            where m.type = 2 and m.[time] >= @startTime and m.[time] <= @endTime
                             and v.verifyType = 1 and u.channel = 'sys'";

            return _context.Query<InviteComment>(sql, new { startTime, endTime })?.ToList();
        }


        public List<InviteAnswer> InviteAnswers(DateTime startTime, DateTime endTime)
        {
            string sql = @"select 
	                            m.[time]  as InviteTime,
                                u.nickname as DarenName,
	                            m.senderID as InviteUser,
	                            m.userID as ReceiveUser,
	                            m.eID as Eid,
	                            s.id as Sid,
	                            s.name as Sname,
	                            e.name as Ename,
	                            m.dataID as QuestionId,
	                            q.content as Content
                            from message as m
                            left JOIN [iSchoolUser].dbo.userInfo as u on u.id = m.userID
                            left join [iSchoolUser].dbo.verify as v on u.id = v.userID
                            left join [iSchoolProduct].dbo.QuestionInfos as q on m.dataID = q.id
                            left join [iSchoolData].dbo.OnlineSchoolExtension as e on e.id = m.eID
                            left join [iSchoolData].dbo.OnlineSchool as s on e.sid = s.id
                            where m.type = 4 and m.[time] >= @startTime and m.[time] <= @endTime
                             and v.verifyType = 1 and u.channel = 'sys' ";

            return _context.Query<InviteAnswer>(sql, new { startTime, endTime })?.ToList();
        }

    }
}
