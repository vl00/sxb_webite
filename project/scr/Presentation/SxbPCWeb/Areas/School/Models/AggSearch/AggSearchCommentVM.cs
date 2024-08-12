using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.ModelDto;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.PCWeb.Areas.School.Models
{

    public class AggSearchCommentVM
    {
        /// <summary>
        /// 点评Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 短链
        /// </summary>
        public string ShortNo { get; set; }

        /// <summary>
        /// 写入点评的用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 达人类型
        /// </summary>
        public TalentType? TalentType { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }

        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }

        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }

        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }

        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsCollection { get; set; }

        /// <summary>
        /// 点亮
        /// </summary>
        public int Stars { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public IEnumerable<string> Images { get; set; }

        /// <summary>
        /// 学校信息
        /// </summary>
        //public SchoolInfoDto SchoolInfo { get; set; }
        public CommentSchoolInfoVM School { get; set; }

        internal static AggSearchCommentVM Convert(UserCommentDto s)
        {
            var vm = new AggSearchCommentVM()
            {
                Id = s.Id,
                ShortNo = UrlShortIdUtil.Long2Base32(s.No),
                UserId = s.UserId,
                NickName = s.User?.NickName,
                HeadImgUrl = s.User?.HeadImgUrl,
                TalentType = s.User?.Type,
                Content = s.Content,
                CreateTime = s.CreateTime.ConciseTime(),
                ReplyCount = s.ReplyCount,
                LikeCount = s.LikeCount,
                IsAnony = s.IsAnony,
                IsTop = s.IsTop,
                IsAttend = s.IsAttend,
                RumorRefuting = s.RumorRefuting,
                IsLike = s.IsLike,
                IsCollection = s.IsCollection,
                Stars = s.Stars,
                Images = s.Images,
                School = new CommentSchoolInfoVM()
            };

            if (s.School != null)
            {
                vm.School.ShortNo = s.School.ShortSchoolNo;
                vm.School.SchoolName = s.School.SchoolName;
                vm.School.CommentCount = s.SchoolCommentCount;
                vm.School.SchoolStars = SchoolScoreToStart.GetCurrentSchoolstart(s.School.Score);
            }
            return vm;
        }
    }

}
