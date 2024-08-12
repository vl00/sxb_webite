using System;
using System.Collections.Generic;
using System.Linq;
//using ImportAndExport.Entity;

namespace ImportAndExport.Repository
{
    public class PublishRepository
    {
        private DataDbContext _context;
        public PublishRepository(DataDbContext context)
        {
            _context = context;
        }


//        public List<PublishComment> GetPublishComments()
//        {
//            string sql = @"select top 1000 NEWID() as Id, scc.Eid as SchoolSectionId,scc.Sid as SchoolId,scc.Id as RecordId,
//				scc.CommentText as Content ,scc.UserId as CommentUserId,
//				scs.star * 20 as AggScore,scs.shizi * 20 as TeachScore,scs.yingjian * 20 as HardScore,
//				scs.zhoubian * 20 as EnvirScore,scs.xiaofeng * 20 as ManageScore,scs.xiaoyuan * 20 as LifeScore
//                        from [iSchoolData].[dbo].SchoolCommentCreate as scc
//												inner join [iSchoolData].[dbo].SchoolCommentScore scs on scs.commentID = scc.Id
//                        left join [iSchoolData].[dbo].SchoolCommentPublished scp on scp.commentID = scc.Id
//												where scp.commentID is null ORDER BY NEWID();";
//            var result = _context.Query<PublishComment>(sql, new { });
//            return result.ToList();
//        }


//        public bool PublishComment(List<SchoolComment> comments)
//        {
//            string sql = @" INSERT INTO [iSchoolProduct].[dbo].[SchoolComments]
//            ([Id], [SchoolId], [SchoolSectionId], [CommentUserId], [Content], [State], [IsTop], [ReplyCount],
//            [LikeCount], [IsSettlement], [IsAnony], [RumorRefuting], [AddTime], [IsHaveImagers],
//            [PostUserRole], [UserInfoExId]) VALUES
//            (@Id, @SchoolId, @SchoolSectionId, @CommentUserId, @Content,@State , @IsTop, @ReplyCount,
//            @LikeCount, @IsSettlement, @IsAnony, @RumorRefuting, @AddTime, @IsHaveImagers, 0, NULL);";
//            var result = _context.Execute(sql, comments); //直接传送list对象
//            return result >= 1;
//        }

//        public bool PublishCommentScore(List<SchoolCommentScore> comments)
//        {
//            string sql = @"INSERT INTO [iSchoolProduct].[dbo].[SchoolCommentScores]([Id], [CommentId], [IsAttend], [AggScore], [TeachScore],
//[HardScore], [EnvirScore], [ManageScore], [LifeScore])
//VALUES (@Id, @CommentId,@IsAttend, @AggScore, @TeachScore, @HardScore,@EnvirScore, @ManageScore, @LifeScore);";
//            var result = _context.Execute(sql, comments); //直接传送list对象
//            return result >= 1;
//        }

//        public bool PublishCommentRecord(List<PublishComment> comments)
//        {
//            string sql = @"INSERT INTO [dbo].[SchoolCommentPublished]([commentId]) VALUES (@RecordId);";
//            var result = _context.Execute(sql, comments); //直接传送list对象
//            return result >= 1;
//        }
    }
}
