using PMS.UserManage.Domain.Common;
using Sxb.UserCenter.Models.CommentViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using Sxb.UserCenter.Models.UserInfoViewModel;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.CommentsManage.Application.ModelDto;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.UserCenter.Utils.DtoToViewModel
{
    public class LikeDataDtoToVoHelper
    {
        public static List<DataMsgViewModel> LikeDataDtosToVoHelper(List<DataMsg> dataMsgs,List<UserViewModel> users, List<ParentReply> parents,
            List<PMS.CommentsManage.Domain.Entities.ViewEntities.AnswerReply> answers,List<LikeCountDto> likeCounts) 
        {
            List<DataMsgViewModel> likeDatas = new List<DataMsgViewModel>();
            foreach (var item in dataMsgs)
            {
                DataMsgViewModel likeData = new DataMsgViewModel();
                likeData.Type = item.Type;
                likeData.Time = item.Time.ConciseTime();
                likeData.SName = item.SName;
                likeData.LikeCount = likeCounts.FirstOrDefault(q => q.SourceId == item.DataId)?.Count??0;
                //操作者
                likeData.User = new UserViewModel() { Id = item.OUserId, HeadImage = item.OImage , UserName = item.OName };

                UserViewModel userView = users.Where(x => x.Id == item.DataUserId).FirstOrDefault();
                if (item.DataType == (int)MessageDataType.Comment)
                {
                    //点评
                    likeData.Comment = new Comment()
                    {
                        Content = item.Content,
                        Id = item.DataId,
                        CommentUser = userView
                    };
                }
                else if (item.DataType == (int)MessageDataType.CommentReply)
                {
                    //回复
                    var reply = parents.Where(x => x.ReplyId == item.DataId || x.ParentId == item.DataId).FirstOrDefault();
                    if(reply != null) 
                    {
                        //回复
                        likeData.CommentReply = new CommentReply()
                        {
                            CommentId = reply.CommentId,
                            CommentContent = reply.CommentContent,
                            ReplyId = reply.ReplyId,
                            ReplyConent = reply.ReplyContent,
                            ReplyParentContent = reply.ParentReplyContent,
                            ReplyParentId = reply.ParentId,
                            ReplyCount = reply.ReplyCount,
                            ReplyType = item.Type != 0 ? reply.Type : reply.ParentId == Guid.Empty ? 1 : 0,
                            CommentUser = CopyUserModel(users.Where(x => x.Id == reply.CommentUserId).FirstOrDefault())?.ToAnonyUserName(reply.ReplyIsAnony),
                            ReplyUser = CopyUserModel(users.Where(x => x.Id == reply.ReplyUserId).FirstOrDefault())?.ToAnonyUserName(reply.ReplyIsAnony),
                            ReplyParentUser = CopyUserModel(users.Where(x => x.Id == reply.ParentUserId).FirstOrDefault())?.ToAnonyUserName(reply.ParentIsAnony)
                        };
                    }

                    if (item.Type == 0)
                    {
                        likeData.Type = 3;
                    }
                }
                else if (item.DataType == (int)MessageDataType.Answer) 
                {
                    //回答
                    var answer = answers.Where(x => x.AnswerId == item.DataId || x.ReplyId == item.DataId).FirstOrDefault();
                    if(answer != null) 
                    {
                            likeData.AnswerReply = new Models.CommentViewModel.AnswerReply()
                            {
                                QuestionContent = answer.QuestionContent,
                                QuestionId = answer.QuestionId,
                                AnswerContent = answer.ReplyContent,
                                AnswerId = answer.ReplyId,
                                ReplyContent = answer.AnswerContent,
                                ReplyCount = answer.ReplyCount,
                                AnswerReplyId = answer.AnswerId,
                                AnswerType = item.Type != 0 ? answer.Type : answer.AnswerId == Guid.Empty ? 1 : 0,
                                QuestionUser = CopyUserModel(users.Where(x => x.Id == answer.QuestionUserId).FirstOrDefault())?.ToAnonyUserName(answer.QuestionIsAnony),
                                AnswerUser = CopyUserModel(users.Where(x => x.Id == answer.AnswerUserId).FirstOrDefault())?.ToAnonyUserName(answer.AnswerIsAnony),
                                AnswerReplyUser = CopyUserModel(users.Where(x => x.Id == answer.ReplyUserId).FirstOrDefault())?.ToAnonyUserName(answer.ReplyIsAnony)
                            };
                    }

                    if (item.Type == 0)
                    {
                        likeData.Type = 2;
                    }
                }
                likeDatas.Add(likeData);
            }
            return likeDatas;
        }

        private static UserViewModel CopyUserModel(UserViewModel user)
        {
            return user==null?null: new UserViewModel
            {
                Id = user.Id,
                AuthTitle = user.AuthTitle,
                UserName = user.UserName,
                IsAuth = user.IsAuth,
                Role = user.Role,
                HeadImage = user.HeadImage,
                Introduction = user.Introduction
            };
        }
    }
}
