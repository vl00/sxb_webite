using PMS.CommentsManage.Application.ModelDto;
using Sxb.PCWeb.Models.User;
using Sxb.PCWeb.ViewModels.Comment;
using Sxb.PCWeb.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;

namespace Sxb.PCWeb.Utils.CommentHelper
{
    /// <summary>
    /// 点评工具类
    /// </summary>
    public static class CommentDtoToVoHelper
    {
        /// <summary>
        /// 点评实体转视图点评实体
        /// </summary>
        /// <param name="ImagePath">图片域名</param>
        /// <param name="CommentDto">点评实体</param>
        /// <param name="Users">实体用户是</param>
        /// <param name="CommentScoreVo">学校卡片</param>
        /// <returns></returns>
        public static List<CommentInfoViewModel> CommentDtoToViewModel(string ImagePath,List<SchoolCommentDto> CommentDto,List<UserInfoVo> Users,List<CommentScoreViewModel> CommentScoreVo,List<SchoolCommentCardViewModel> ScoolCommentCard = null,List<Guid> Collection = null) 
        {
            List<CommentInfoViewModel> CommentVo = new List<CommentInfoViewModel>();
            foreach (var item in CommentDto)
            {
                UserInfoVo User = Users.Where(x => x.Id == item.UserId).FirstOrDefault();
                CommentInfoViewModel Comment = new CommentInfoViewModel();
                Comment.Id = item.Id;
                Comment.No = UrlShortIdUtil.Long2Base32(item.No);
                Comment.Images = item.Images.Select(x=> ImagePath+x).ToList();
                Comment.IsAnony = item.IsAnony;
                Comment.IsEssence = item.IsSelected;
                Comment.IsLike = item.IsLike;
                Comment.UserInfo = User?.ToAnonyUserName(item.IsAnony);
                Comment.Content = item.Content;
                Comment.LikeTotal = item.LikeCount;
                Comment.ReplyTotal = item.ReplyCount;
                Comment.RumorRefuting = item.IsRumorRefuting;
                Comment.AddTime = item.CreateTime.ConciseTime();
                Comment.ExId = item.SchoolSectionId;
                Comment.Sid = item.SchoolId;
                Comment.CommentScore = CommentScoreVo.Where(x => x.CommentId == item.Id).FirstOrDefault() ?? new CommentScoreViewModel();
                Comment.School = ScoolCommentCard?.Where(x => x.ExtId == item.SchoolSectionId).FirstOrDefault();

                if (Collection != null && Collection.Any()) 
                {
                    Comment.IsCollection = Collection.Where(x => x == item.Id).FirstOrDefault() != Guid.Empty;
                }

                CommentVo.Add(Comment);
            }
            return CommentVo;
        }
    }
}
