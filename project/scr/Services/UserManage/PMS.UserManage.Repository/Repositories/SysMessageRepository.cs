using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Repository.Repositories
{
    public class SysMessageRepository : ISysMessageRepository
    {
        private readonly UserDbContext _dbcontext;

        public SysMessageRepository(UserDbContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public List<SysMessage> GetMessageByType(Guid SenderUserId, int Type, int Page, int Size)
        {
            return _dbcontext.Query<SysMessage>(@"select * from SysMessage where SenderUserId = @SenderUserId and type = @Type and IsOver = 1
	order by PushTime desc offset (@Page - 1) * @Size rows fetch next @Size rows only", new { SenderUserId, Type, Page, Size }).ToList();
        }

        public bool AddSysMessage(List<SysMessage> message)
        {
            return _dbcontext.Execute($@"
insert into SysMessage 
(Id, Type, DataId, SenderUserId, UserId, Content, IsRead, PushTime, IsOver, OriContent, OriSenderUserId, EId, DataType, OriType)
values
(@Id,@Type,@DataId,@SenderUserId,@UserId,@Content,0,@PushTime,0, @OriContent, @OriSenderUserId, @EId, @DataType, @OriType)", message) > 0;
        }

        public async Task<int> AddSysMessageState(List<SysMessageState> messageStates)
        {
            return await _dbcontext.ExecuteAsync("insert into SysMessageState values(@MessageId,@UserId,default)", messageStates);
        }

        public async Task<int> UpdateMessageRead(List<Guid> dataIds, Guid UserId)
        {
            return await _dbcontext.ExecuteAsync("update SysMessage set IsRead = 1 where DataId in @dataIds and UserId = @UserId", new { dataIds, UserId });
        }

        public List<Guid> CheckMessageIdIsExist(List<Guid> DataIds, Guid UserId)
        {
            return _dbcontext.Query<Guid>("select MessageId from SysMessageState where MessageId in @DataIds and UserId = @UserId ", new { DataIds, UserId }).ToList();
        }

        public List<SysMessageTips> GetLiveMessageTips(Guid userId, int page, int size)
        {
            string sql = @"
                        select 
                            m.DataId,
	                        m.SenderUserID,
	                        m.Type,
	                        m.Content,
	                        m.PushTime,
	                        u.nickname,
	                        u.HeadImgUrl
                        from SysMessage as m
	                        RIGHT JOIN 
                        (select  m.Id from SysMessage as m
	                                   left join collection as c on c.dataID = m.SenderUserID
									   left join SysMessageState as s on s.MessageId = m.Id and s.UserId = @userId
                                            where  m.Type = 5 and c.userID = @userId and (IsOver is null or IsOver = 0) and s.Id is null
														                        order by PushTime desc offset (@page - 1) * @size rows fetch next @size row only 
                        )
	                        as d on m.Id = d.Id
	                        left join userInfo as u on u.id = m.SenderUserID
                        ";

            return _dbcontext.Query<SysMessageTips>(sql, new { userId, page, size }).ToList();
        }

        public List<SysMessageTips> GetSysMessageTips(Guid userId, int page)
        {
            var innerTotalSql = $@"
		SELECT 
			sum(
				CASE 
				WHEN s.Id IS NOT NULL OR sm.IsRead = 1 THEN 0 
				ELSE 1 END
			) 
		FROM 
			SysMessage sm 
			left join SysMessageState as s on s.MessageId = sm.Id and s.UserId = @userId
		WHERE 
			(
				sm.SenderUserId = m.SenderUserId 
				AND sm.UserId = m.UserId
			)
";
            string sql = $@"
SELECT
	m.*,
	u.nickname,
	u.HeadImgUrl,
	(
      {innerTotalSql}
	) AS TipsTotal
FROM
(
	select 
		m.*,
        row_number() OVER (
			partition BY m.SenderUserId
			ORDER BY m.PushTime desc
		) AS rn
	from
		SysMessage as m
	where 
		m.type != 5 and m.UserId = @userId
) AS m
left join userInfo as u on u.id = m.SenderUserID
WHERE
	m.rn = 1
order BY m.PushTime DESC 
offset (@page - 1) * 10 rows 
fetch next 10 row only 
";
            return _dbcontext.Query<SysMessageTips>(sql, new { userId, page }).ToList();
        }


        public List<DynamicItem> GetDynamicItems(Guid userId, int page, int size, bool IsSelf = true)
        {
            var selfSql = IsSelf ? "" : " and IsAnony  = 0 ";

            var sql = $@"
select 
    * 
from (
    select Id,AddTime as PushTime,1 as Type from [iSchoolProduct].[dbo].[SchoolComments] 
    where CommentUserId = @userId {selfSql}

    union all 
    select Id,CreateTime as PushTime,2 as Type from [iSchoolProduct].[dbo].[QuestionsAnswersInfos] 
    where UserId = @userId and ParentId is null {selfSql}

    union all 
    select Id,CreateTime as PushTime,3 as Type from [iSchoolProduct].[dbo].[QuestionInfos] 
    where UserId = @userId {selfSql}

    union all
    SELECT distinct article.id,time as PushTime,4 as Type  
    FROM  
        iSchoolArticle.dbo.article article
        JOIN(SELECT ArticleID, t.id, t.user_id FROM iSchoolArticle.dbo.Article_Talent[at]
        JOIN iSchoolUser.dbo.talent t on[at].TalentID = t.id) talent on article.id = talent.ArticleID
    WHERE 
        article.time <= GETDATE() AND article.show = 1 AND talent.user_id = @userId

    union all 
    select lecture.id ,lecture.time_start as PushTime,5 as Type  
	from iSchoolLive.dbo.lecture_v2 lecture
	LEFT JOIN iSchoolLive.dbo.lector on lector.id = lecture.lector_id
	where lecture.show = 1 and lecture.status = 5 
	and lector.userID = @userId
) as dynamic 
ORDER BY 
    dynamic.PushTime DESC 
OFFSET (@page - 1) * @size 
ROWS FETCH NEXT @size ROWS ONLY
            ";
            return _dbcontext.Query<DynamicItem>(sql, new { userId, page, size }).ToList();
        }

        public List<DynamicItem> GetDynamicItems(List<Guid> userIds, int page, int size, bool IsSelf = true)
        {
            var selfSql = IsSelf ? "" : " and IsAnony  = 0 ";

            var sql = $@"
select 
    * 
from (
    select Id,AddTime as PushTime,1 as Type from [iSchoolProduct].[dbo].[SchoolComments] 
    where CommentUserId in @userIds {selfSql}

    union all 
    select Id,CreateTime as PushTime,2 as Type from [iSchoolProduct].[dbo].[QuestionsAnswersInfos] 
    where UserId in @userIds {selfSql}

    union all 
    select Id,CreateTime as PushTime,3 as Type from [iSchoolProduct].[dbo].[QuestionInfos] 
    where UserId in @userIds {selfSql}

    union all 
    select Id,CreateTime as PushTime,6 as Type from [iSchoolTopicCircle].[dbo].[Topic] 
    where Creator in @userIds and IsDeleted = 0

    union all 
    select Id,CreateTime as PushTime,7 as Type from [iSchoolProduct].[dbo].[SchoolCommentReplies] 
    where UserId in @userIds {selfSql}
) as dynamic 
ORDER BY 
    dynamic.PushTime DESC 
OFFSET (@page - 1) * @size 
ROWS FETCH NEXT @size ROWS ONLY
            ";
            return _dbcontext.Query<DynamicItem>(sql, new { userIds, page, size }).ToList();
        }


        /// <summary>
        /// 查看消息对话框
        /// </summary>
        /// <param name="senderUserId"></param>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<SysMessageDetail> GetSysMessages(Guid? senderId, Guid? userId, SysMessageType[] types, bool? isRead, DateTime? startTime, DateTime? endTime, int page = 1)
        {
            //兼容isread可能为空
            var readSql = isRead != null && !isRead.Value ? " SM.IsRead is null or  " : "";
            var sql = $@"
select
    SM.*,
	U.nickname as SenderNickname,
	U.HeadImgUrl as SenderHeadImgUrl
from 
    SysMessage SM
    left join userInfo as U on U.id = SM.SenderUserID
where 
    1=1
	and (@senderId is null or SM.SenderUserId = @senderId)
	and (@userId is null or SM.UserId = @userId)
	and (@types is null or SM.Type in @types)
	and (@isRead is null or ( {readSql} SM.IsRead = @isRead))
	and (@startTime is null or SM.PushTime >= @startTime)
	and (@endTime is null or SM.PushTime <= @endTime) and SM.Type != 5
order by 
    SM.PushTime desc 
offset (@page - 1) * 10 rows 
fetch next 10 rows only 
";
            return _dbcontext.Query<SysMessageDetail>(sql, new { senderId, userId, types, isRead, startTime, endTime, page }).ToList();
        }

        public long GetSysMessageTotal(Guid? senderId, Guid? userId, SysMessageType[] types, bool? isRead, DateTime? startTime, DateTime? endTime)
        {
            //兼容isread可能为空
            var readSql = isRead != null && !isRead.Value ? " SM.IsRead is null or  " : "";
            var sql = $@"
select
	count(1) as total
from
	SysMessage SM
where
    1=1
	and (@senderId is null or SM.SenderUserId = @senderId)
	and (@userId is null or SM.UserId = @userId)
	and (@types is null or SM.Type in @types)
	and (@isRead is null or ( {readSql} SM.IsRead = @isRead))
	and (@startTime is null or SM.PushTime >= @startTime)
	and (@endTime is null or SM.PushTime <= @endTime) and SM.Type != 5
";
            return _dbcontext.QuerySingle<long>(sql, new { senderId, userId, types, isRead, startTime, endTime });
        }
    }
}
