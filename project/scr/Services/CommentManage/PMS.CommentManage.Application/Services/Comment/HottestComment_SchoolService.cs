using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace PMS.CommentsManage.Application.Services.Comment
{
    public class HottestComment_SchoolService : IHottestComment_SchoolService
    {
        private readonly ISchoolCommentService _commentService;
        private readonly IGiveLikeService _likeservice;
        private readonly IEasyRedisClient _easyRedisClient;

        public HottestComment_SchoolService(ISchoolCommentService commentService,
            IGiveLikeService likeservice,
            IEasyRedisClient easyRedisClient) 
        {
            _commentService = commentService;
            _easyRedisClient = easyRedisClient;
            _likeservice = likeservice;
        }

        public async Task<List<SchoolCommentDto>> HotComment(HotCommentQuery CommentQuery, Guid UserId)
        {
            string key = $"Comment:Hottest_City:{CommentQuery.City}_Grade:{CommentQuery.Grade}_Type:{CommentQuery.Type}_Discount:{Convert.ToInt32(CommentQuery.Discount)}_Diglossia:{Convert.ToInt32(CommentQuery.Diglossia)}_Chinese:{Convert.ToInt32(CommentQuery.Chinese)}";
            var HotComment = await _easyRedisClient.GetOrAddAsync(key, () => {
                return _commentService.HotComment(CommentQuery, UserId);
            });

            if (HotComment.Count == 0) 
            {
                return new List<SchoolCommentDto>();
            }
            else 
            {
                var LikeComment = _likeservice.CheckLike(HotComment.GroupBy(x=>x.Id).Select(x=>x.Key).ToList(),UserId);
                HotComment.ForEach(x => {
                    if (LikeComment.Contains(x.Id))
                    {
                        x.IsLike = true;
                    }
                    else 
                    {
                        x.IsLike = false;
                    }
                });
            }
            return HotComment;
        }

        public List<SchoolCommentDto> HottestComment(DateTime StartTime, DateTime EndTime)
        {
            return _commentService.HottestComment(StartTime, EndTime);
        }

        public async Task<List<HotCommentSchoolDto>> HottestSchool(HotCommentQuery query, bool queryAll)
        {
            string key = $"Comment:HottestSchool_City:{query.City}_Grade:{query.Grade}_Type:{query.Type}_Discount:{Convert.ToInt32(query.Discount)}_Diglossia:{Convert.ToInt32(query.Diglossia)}_Chinese:{Convert.ToInt32(query.Chinese)}";
            return await _easyRedisClient.GetOrAddAsync(key, () => {
                return _commentService.HottestSchool(query, queryAll);
            });
        }

    }
}
