using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PMS.UserManage.Domain.Common;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Repository.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly UserDbContext _dbcontext;

        public MessageRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public int GetMessageCount(Guid userID)
        {
            return _dbcontext.Query<int>("select count(*) from message where userID=@userID", new { userID }).FirstOrDefault();
        }
        public bool Update(Guid[] ids)
        {
            string sql = " update message set [Read]=1, ReadChangeTime=GETDATE() where id in @ids and ([Read] is null or [Read] = 0) ";
            return _dbcontext.ExecuteUow(sql, new { ids }) > 0;
        }

        public void UpdateMessageState(List<int> type, Guid userId)
        {
            string sql = " update message set [Read]=1, ReadChangeTime=GETDATE() where type in @type  and UserId = @userId and ([Read] is null or [Read] = 0) ";
            _dbcontext.Execute(sql, new { type, userId });
        }

        public bool UpdateReadState(bool IsRead, MessageType type, MessageDataType dataType, Guid UserId = default, Guid SenderId = default, List<Guid> DataId = default)
        {
            string sql = $@"
update message 
set [Read] = @IsRead,ReadChangeTime = GETDATE()
where 
    type = @type
    and dataType = @dataType
";

            if (UserId != default)
            {
                sql += " and UserId = @UserId and DataId in @DataId";
                return _dbcontext.Execute(sql, new { IsRead, type, dataType, UserId, DataId }) > 0;
            }
            else
            {
                sql += " and SenderId = @SenderId and DataId in @DataId ";
                return _dbcontext.Execute(sql, new { IsRead, type, dataType, SenderId, DataId }) > 0;
            }
        }

        public bool UpdateMessageHandled(bool handled, bool read, MessageType type, MessageDataType dataType, Guid? userId, Guid? senderId, List<Guid> dataId)
        {
            string sql = $@"
update message 
set [handled] = @handled, [Read] = @read, ReadChangeTime = GETDATE()
where 
    type = @type
    and dataType = @dataType
    and (@userId is null or UserId = @userId)
    and (@senderId is null or SenderId = @senderId)
    and DataId in @dataId 
";
            return _dbcontext.Execute(sql, new { handled, read, type, userId, dataType, senderId, dataId }) > 0;
        }

        public bool UpdateMessageIgnore(bool ignore, bool read, MessageType type, MessageDataType dataType, Guid? userId, Guid? senderId, List<Guid> dataId)
        {
            string sql = $@"
update message 
set [ignore] = @ignore, [Read] = @read, ReadChangeTime = GETDATE()
where 
    type = @type
    and dataType = @dataType
    and (@userId is null or UserId = @userId)
    and (@senderId is null or SenderId = @senderId)
    and DataId in @dataId 
";
            return _dbcontext.Execute(sql, new { ignore, read, type, userId, dataType, senderId, dataId }) > 0;
        }

        public bool AddMessage(Message model)
        {
            return _dbcontext.ExecuteUow(@"insert into message 
(id, userID, senderID, type, title, content, dataID, dataType, eID, time,push,IsAnony,[Read],Ignore,ReadChangeTime)
values
(@id, @userID, @senderID, @type, @title, @content, @dataID, @dataType, @eID, default,@push,@IsAnony,@Read,null,@ReadChangeTime)", model) > 0;
        }
        public bool RemoveMessage(Guid msgID, Guid userID)
        {
            return _dbcontext.Execute(@"delete from message where id=@msgID and userID=@userID"
, new { msgID, userID }) > 0;
        }
        public List<Message> GetUserMessage(Guid userID, byte[] type, int page, int pageSize = 10, bool isAuth = false, bool? read = null, bool? handled = null)
        {
            string sql = "";

            if (isAuth)
            {
                sql = " and (Ignore is null or Ignore = 0) ";
            }

            return _dbcontext.Query<Message>($@"
select * from message 
where 
    userID=@userID {sql} 
    and (@read is null or [read] = @read) 
    and (@handled is null or [handled] = @handled) 
    and type in @type 
    order by time desc
offset (@page-1)*@pageSize rows fetch next @pageSize row only", new { userID, type = type.ToList(), page, pageSize, read, handled }).ToList();
        }

        public Push GetPushSetting(Guid userID)
        {
            return _dbcontext.Query<Push>("select * from push where userID=@userID", new { userID }).FirstOrDefault();
        }
        public bool SetPushSetting(Push model)
        {
            return _dbcontext.Execute(@"merge into push
using (select 1 as o) t
on push.userID=@userID
when not matched then insert 
(userID, article, school, invite, reply) values (@userID, @article, @school, @invite, @reply)
when matched then update
set article=@article, school=@school, invite=@invite, reply=@reply;", model) > 0;
        }


        public List<InviteSelf> GetMessageBySenderUser(Guid userId, List<int> type, int page, int size)
        {
            return _dbcontext.Query<InviteSelf>(@"select 
                        m.dataID as DataId,
                        m.type as Type,
                        (select count(1) from message where senderID = m.senderID and dataID = m.dataID) as InviteTotal
                            from message as m
	                RIGHT JOIN
                (select 
	                id,row_number () OVER ( partition BY dataID ORDER BY [time] DESC ) AS Take
                from message
	                where senderID = @userId  and type in @type and dataType in (2,3) as t on m.id = t.id
		                where t.Take = 1
                    order by time desc
	                    offset (@page-1)*@size rows fetch next @size row only
            ", new { userId, type, page, size })?.ToList();
        }

        public List<InviteUserInfo> GetInviteUserInfos(List<Guid> dataId, Guid senderId)
        {
            return _dbcontext.Query<InviteUserInfo>(@"
select 
    m.dataID,
    m.type,
    u.id,
    u.HeadImgUrl,
    m.userID
from 
    userInfo as u 
    RIGHT JOIN
    (
        select 
            userID, dataID, type,
            row_number () OVER ( partition BY type,DataID ORDER BY [time] DESC ) AS Take
        from message 
        where dataID in @dataId and senderID = @senderId
    ) as m on u.id = m.userID
				       ", new { dataId, senderId })?.ToList();
            //where m.take <= 5", new { dataId, senderId })?.ToList();
        }

        public List<InviteSelf> GetMessageBySenderUser(Guid userId, int page, int size)
        {
            //邀请
            var types = new List<byte>() { (byte)MessageType.InviteAnswer, (byte)MessageType.InviteComment,
                    (byte)MessageType.InviteQuestion };//, (byte)MessageType.InviteReplyComment
            return _dbcontext.Query<InviteSelf>(@"
select 
    i.Type,
    i.DataId,
    i.Content,
    s.name+'-'+e.name as SchName,
    (select count(1) from message where senderID = i.SenderId and DataID = i.DataId) as InviteTotal,
    case  when (select count(1) from message where senderID = i.SenderId and DataID = i.DataId and [Read] = 0) > 0 THEN 0 ELSE 1 end as IsRead
from 
    message as i
    right join 
    (
        select 
            id,
            row_number () OVER ( partition BY type,DataID ORDER BY [time] DESC ) AS Take
        from message 
        where senderID = @userId  and type in @types
    ) as m on i.id = m.id
    left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e on e.id = i.Eid
    left join [iSchoolData].[dbo].[OnlineSchool] as s on s.id = e.sid
where m.Take = 1
    order by [time] desc
    offset (@page-1)*@size rows fetch next @size row only
            ", new { userId, types, page, size })?.ToList();
        }

        public List<DataMsg> GetDataMsgs(Guid userId, MessageType? type, MessageDataType? dataType, bool? ignore, int page, int size)
        {
            string ignoreSql = "";
            if (ignore == false)
                ignoreSql = " m.ignore is null or ";

            string sql = $@"	select 
	                        m.type,
                            m.dataType,
	                        m.dataID,
	                        o.name+'-'+e.name as sname,
	                        m.[time],
	                        m.title,
	                        m.content,
                            m.userID as DataUserId,
                            u.id as OUserId,
	                        u.nickname as oName,
	                        u.headImgUrl as oImage
                        from message as m
	                        left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e on m.eID = e.Id
	                        left join [iSchoolData].[dbo].[OnlineSchool] as o on e.sid = o.id
	                        left join userInfo as u on u.id = m.senderID
	                        left join UserInfo as u1 on u1.id = m.UserId
	                        where 
                                1=1
                                and (@ignore is null or ({ignoreSql} m.ignore = @type))
                                and (@type is null or m.type = @type)
                                and (@dataType is null or m.dataType = @dataType)
                                and m.UserId = @userId
                                order by m.[time] desc
                            offset (@page - 1) * @size rows fetch next @size row only";
            return _dbcontext.Query<DataMsg>(sql, new { userId, type, dataType, ignore, page, size }).ToList();
        }


        public List<MessageTipsTotal> TotalTips(Guid UserId, DateTime? startTime)
        {
            var inviteAnswer = MessageType.InviteAnswer;
            var inviteQuestion = MessageType.InviteQuestion;
            var inviteReplyComment = MessageType.InviteReplyComment;
            var inviteComment = MessageType.InviteComment;
            // or m.type = @inviteReplyComment

            var like = MessageType.Like;
            var follow = MessageType.Follow;
            var reply = MessageType.Reply;
            string sql = $@"	select 
		                    d.type,
		                    count(d.type) as total
	                    from 
	                    (
		                    select 
			                    case 
				                    when m.type = @inviteAnswer or m.type = @inviteQuestion or m.type = @inviteComment then 1
				                    when m.type = @like and (m.ignore is null or m.ignore = 0) then 2
				                    when m.type = @follow then 3
				                    when m.type = @reply then 4
			                    end as type
		                    from message as m
			                    where
			                    m.userID = @UserId 
                                and (@startTime is null or m.time >= @startTime)
		                    and m.type in (@inviteAnswer, @inviteQuestion, @inviteReplyComment, @inviteComment, @like, @follow, @reply) 
                            AND (m.[Read] IS NULL or m.[Read] = 0)
	                    ) as d
		                    GROUP BY d.type";

            return _dbcontext.Query<MessageTipsTotal>(sql, new
            {
                UserId,
                startTime,
                inviteAnswer,
                inviteQuestion,
                inviteReplyComment,
                inviteComment,
                like,
                follow,
                reply
            }).ToList();
        }

        /// <summary>
        /// 忽略邀请接口
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userId"></param>
        /// <param name="isAll">是否忽略当前用户的所有邀请信息</param>
        public void IgnoreInvite(List<Guid> ids, Guid userId, bool isAll)
        {
            if (isAll)
            {
                _dbcontext.Execute(@"update message set Ignore = 1 where userID = @userId", new { userId });
            }
            else
            {
                _dbcontext.Execute(@"update message set Ignore = 1 where Id in @ids and userID = @userId", new { ids, userId });
            }
        }


        public List<DataMsg> GetReplyME(Guid UserID, int page, int size)
        {
            string sql = @"	select
                                m.[time],
                                m.type,
		                        m.dataID,
		                        m.dataType,
                                o.name+'-'+e.name as sname,
                                u.id as OUserId,
	                            u.nickname as oName,
	                            u.headImgUrl as oImage
	                        from message as m
                                left join userInfo as u on u.id = m.senderID
                                left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e on m.eID = e.Id
	                            left join [iSchoolData].[dbo].[OnlineSchool] as o on e.sid = o.id
		                    where m.type = @type and m.userID = @UserID 
			                order by [time] desc offset (@page - 1 ) * @size rows fetch next @size rows only";

            return _dbcontext.Query<DataMsg>(sql, new { UserID, type=MessageType.Reply, page, size }).ToList();
        }

        /// <summary>
        /// 获取需要推送的消息
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<Message> GetMessageJob(DateTime startTime, DateTime endTime, int page, int size)
        {
            //回复和已处理的邀请, MessageType.InviteReplyComment 
            var types = new MessageType[] { MessageType.Reply, MessageType.InviteComment, MessageType.InviteQuestion, MessageType.InviteAnswer};
            var replyType = MessageType.Reply;
            string sql = @"	select * from message 
		                        where type in @types
                                    and (ignore is null or ignore = 0)
                                    and (type = @replyType or handled = 1)
                                    and [ReadChangeTime] >= @startTime and [ReadChangeTime] <= @endTime
			                    order by [time] desc offset (@page - 1 ) * @size rows fetch next @size rows only";
            return _dbcontext.Query<Message>(sql, new { types, replyType, startTime, endTime, page, size }).ToList();
        }

        /// <summary>
        /// 获取需要推送的消息数量
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public int GetMessageJobTotal(DateTime startTime, DateTime endTime)
        {
            //回复和已处理的邀请, MessageType.InviteReplyComment
            var types = new MessageType[] { MessageType.Reply, MessageType.InviteComment, MessageType.InviteQuestion, MessageType.InviteAnswer };
            var replyType = MessageType.Reply;
            return _dbcontext.Query<int>(@"select count(1) from message 
		                        where type in @types
                                    and (ignore is null or ignore = 0)
                                    and (type = @replyType or handled = 1)
				                    and [ReadChangeTime] >= @startTime and [ReadChangeTime] <= @endTime", new { types, replyType, startTime, endTime }).FirstOrDefault();
        }

        public long GetMessageTotal(Guid? senderId, Guid? userId, MessageType[] types, MessageDataType[] dataTypes, bool? read, bool? ignore, DateTime? startTime, DateTime? endTime, DateTime? pushStartTime, DateTime? pushEndTime)
        {
            //兼容read可能为空
            var readSql = read != null && !read.Value ? " M.[read] is null or  " : "";
            var ignoreSql = read != null && !read.Value ? " M.[ignore] is null or  " : "";
            var sql = $@"
select
	count(1) as total
from
	message M
where
    1=1
	and (@senderId is null or M.senderID = @senderId)
	and (@userId is null or M.userID = @userId)
	and (@dataTypes is null or M.dataType in @dataTypes)
	and (@types is null or M.type in @types)
	and (@read is null or ({readSql} M.[read] = @read))
	and (@ignore is null or ({ignoreSql} M.[ignore] = @ignore))
	and (@startTime is null or M.time >= @startTime)
	and (@endTime is null or M.time <= @endTime)
	and (@pushStartTime is null or M.ReadChangeTime >= @pushStartTime)
	and (@pushEndTime is null or M.ReadChangeTime <= @pushEndTime)
";
            var param = new
            {
                senderId,
                userId,
                types,
                dataTypes,
                read,
                ignore,
                startTime,
                endTime,
                pushStartTime,
                pushEndTime
            };
            return _dbcontext.QuerySingle<long>(sql, param);
        }

        public IEnumerable<Message> GetMessages(Guid? senderId, Guid? userId, Guid[] dataIds, MessageType[] types, MessageDataType[] dataTypes, bool? read, bool? ignore, DateTime? startTime, DateTime? endTime, DateTime? pushStartTime, DateTime? pushEndTime)
        {
            //兼容read可能为空
            var readSql = read != null && !read.Value ? " M.[read] is null or  " : "";
            var ignoreSql = read != null && !read.Value ? " M.[ignore] is null or  " : "";
            var sql = $@"
select
	*
from
	message M
where
    1=1
	and (@senderId is null or M.senderID = @senderId)
	and (@userId is null or M.userID = @userId)
	and (@dataIds is null or M.dataId in @dataIds)
	and (@dataTypes is null or M.dataType in @dataTypes)
	and (@types is null or M.type in @types)
	and (@read is null or ({readSql} M.[read] = @read))
	and (@ignore is null or ({ignoreSql} M.[ignore] = @ignore))
	and (@startTime is null or M.time >= @startTime)
	and (@endTime is null or M.time <= @endTime)
	and (@pushStartTime is null or M.ReadChangeTime >= @pushStartTime)
	and (@pushEndTime is null or M.ReadChangeTime <= @pushEndTime)
";
            var param = new
            {
                senderId,
                userId,
                dataIds,
                types,
                dataTypes,
                read,
                ignore,
                startTime,
                endTime,
                pushStartTime,
                pushEndTime
            };
            return _dbcontext.Query<Message>(sql, param);
        }

        public IEnumerable<Guid> GetMessageUserIds(Guid senderId, Guid[] userIds, Guid dataId, MessageType type, MessageDataType dataType, Guid? eId)
        {

            var userIdSql = userIds == null ?  "" : " and M.userID in @userIds ";
            var sql = $@"
select UserId from message M 
where
    1=1
    and M.senderID = @senderId
    {userIdSql}
    and M.type = @type
    and M.dataType = @dataType
    and M.dataID = @dataId
	and (@eId is null or M.eId = @eId)
    and M.Time > @time
";
            var param = new
            {
                senderId, userIds, dataId, type, dataType, eId, time = DateTime.Today
            };
            return _dbcontext.Query<Guid>(sql, param);
        }



        /// <summary>
        /// 我的点赞列表
        /// <para>modified by Labbor on 20200709 添加条件ignore</para>
        /// </summary>
        /// <param name="searchType">0 点评/点评回复  1问答</param>
        /// <param name="userID"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<DataMsg> GetMyLike(int searchType, Guid userID, int page, int size)
        {
            string sql = @"	select
                            m.type,
                            m.dataType,
	                        m.dataID,
	                        o.name+'-'+e.name as sname,
	                        m.[time],
	                        m.title,
	                        m.content,
                            m.userID as DataUserId,
                            u.id as OUserId,
	                        u.nickname as oName,
	                        u.headImgUrl as oImage
	                        from message as m
                            left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e on m.eID = e.Id
	                        left join [iSchoolData].[dbo].[OnlineSchool] as o on e.sid = o.id
	                        left join userInfo as u on u.id = m.senderID
	                        left join UserInfo as u1 on u1.id = m.UserId
		                        where m.type = @type and m.dataType in @dataTypes and m.senderID = @UserID
                                    and (m.ignore is null or m.ignore = 0)
			                    order by [time] desc offset (@page - 1 ) * @size rows fetch next @size rows only";

            List<int> dataTypes;
            if (searchType == 0)
            {
                dataTypes = new List<int> { (int)MessageDataType.Comment, (int)MessageDataType.CommentReply };
            }
            else
            {
                dataTypes = new List<int> { (int)MessageDataType.Answer };
            }

            return _dbcontext.Query<DataMsg>(sql, new { type = MessageType.Like, dataTypes, userID, page, size }).ToList();
        }
        /// <summary>
        /// 我的评论
        /// </summary>
        /// <param name="type">0 点评/点评回复  1问答</param>
        /// <param name="userID"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<DataMsg> GetMyReply(int searchType, Guid userID, int page, int size)
        {
            string sql = @"	select
                                m.[time],
		                        m.dataID,
		                        m.dataType,
                                m.type,
                                o.name+'-'+e.name as sname,
                                u.id as OUserId,
	                            u.nickname as oName,
	                            u.headImgUrl as oImage
	                        from message as m
                            left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e on m.eID = e.Id
	                        left join [iSchoolData].[dbo].[OnlineSchool] as o on e.sid = o.id
                                    left join userInfo as u on u.id = m.senderID
		                        where m.dataType = @dataTypes and m.type = 1 and m.senderID = @UserID
			                    order by [time] desc offset (@page - 1 ) * @size rows fetch next @size rows only";
            int dataTypes;
            if (searchType == 0)
            {
                dataTypes = (int)MessageDataType.CommentReply;
            }
            else
            {
                dataTypes = (int)MessageDataType.Answer;
            }
            return _dbcontext.Query<DataMsg>(sql, new { dataTypes, userID, page, size }).ToList();
        }
    }
}
