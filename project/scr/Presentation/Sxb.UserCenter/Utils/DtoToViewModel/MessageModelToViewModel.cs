using PMS.UserManage.Application.ModelDto.Message;
using PMS.UserManage.Domain.Entities;
using Sxb.UserCenter.Models.MessageViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using Sxb.UserCenter.Models.ArticleViewModel;
using Sxb.UserCenter.Models.QuestionViewModel;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.CommentsManage.Application.ModelDto;
using static PMS.UserManage.Domain.Common.EnumSet;
using PMS.CommentsManage.Domain.Entities;

namespace Sxb.UserCenter.Utils.DtoToViewModel
{
    /// <summary>
    /// 消息实体转换
    /// </summary>
    public static class MessageModelToViewModel
    {
        public static List<InviteMessageViewModel> MessageModelToViewModels(List<ApiMessageModel> message)
        {
            List<InviteMessageViewModel> viewModels = new List<InviteMessageViewModel>();
            if (message == null)
            {
                return viewModels;
            }

            foreach (var item in message)
            {
                InviteMessageViewModel temp = new InviteMessageViewModel();
                temp.Id = item.Id;
                temp.DataId = item.DataID;
                temp.InviteTime = item.Time;
                temp.Type = item.Type;
                temp.DataType = item.DataType;
                temp.Content = item.Content;
                temp.InviteUser = new Models.UserInfoViewModel.UserViewModel() { Id = item.SenderID, UserName = item.Nickname, HeadImage = item.HeadImgUrl };
                temp.School = item.SchoolModel == null ? new Models.SchoolViewModel.SchoolInfoViewModel() : new Models.SchoolViewModel.SchoolInfoViewModel()
                {
                    SchoolType = (int)item.SchoolModel.SchoolType,
                    Eid = item.SchoolModel.SchoolSectionId,
                    Sid = item.SchoolModel.SchoolId,
                    Star = item.SchoolModel.SchoolStars,
                    CommentTotal = item.SchoolModel.CommentTotal,
                    QuestionTotal = item.SchoolModel.SectionQuestionTotal,
                    LodgingType = (int)item.SchoolModel.LodgingType,
                    Name = item.SchoolModel.SchoolName
                };
                //问答
                if (item.Type == (int)MessageType.InviteQuestion)
                {
                    temp.InviteTitle = "邀请你提问";
                }
                else if (item.Type == (int)MessageType.InviteAnswer)
                {
                    temp.InviteTitle = "邀请你回答";
                }
                //点评
                else if (item.Type == (int)MessageType.InviteComment)
                {
                    temp.InviteTitle = "邀请你点评";
                }
                viewModels.Add(temp);
            }
            return viewModels;
        }


        public static List<SysMessageViewModel> SysMessageTips(List<SysMessageTips> messages)
        {
            List<SysMessageViewModel> sysMessages = new List<SysMessageViewModel>();

            if (!messages.Any())
            {
                return sysMessages;
            }

            foreach (var item in messages)
            {
                string content;
                if (item.Content == "" || item.Content == null || item.Type == PMS.UserManage.Domain.Common.SysMessageType.Article)
                {
                    content = SysMessageTypeToContent(item.Type);
                }
                else
                {
                    content = item.Content;
                }
                sysMessages.Add(new SysMessageViewModel()
                {
                    UserId = item.SenderUserId,
                    HeadImager = item.HeadImgUrl,
                    UserName = item.nickname,
                    TipsTotal = item.TipsTotal,
                    SenderTime = item.PushTime.ConciseTime(),
                    Message = content

                });
            }
            return sysMessages;
        }

        public static List<MessageDialogueViewModel> MessageDialogues(List<SysMessageDetail> messages, List<Data> article = null, List<AnswerInfoDto> answers = null,
            List<QuestionInfo> questions = null, List<SchoolComment> comments = null, List<SchoolInfoDto> schools = null)
        {
            List<MessageDialogueViewModel> models = new List<MessageDialogueViewModel>();
            if (!messages.Any())
            {
                return models;
            }

            foreach (var item in messages)
            {
                MessageDialogueViewModel message = new MessageDialogueViewModel();
                message.DataId = item.DataId;
                message.Type = (int)item.Type;
                message.DataType = (int?)item.DataType;
                message.OriType = (int?)item.OriType;
                message.Time = item.PushTime.ConciseTime();
                message.SenderNickname = item.SenderNickname;
                message.SenderHeadImgUrl = item.SenderHeadImgUrl;

                var school = schools?.Where(s => s.SchoolSectionId == item.EId).FirstOrDefault() ?? new SchoolInfoDto();
                message.SchoolName = school.SchoolName;
                message.LodgingType = school.LodgingType;
                message.SchoolType = school.SchoolType;
                message.SchoolStars = school.SchoolStars;
                message.Title = item.Title;
                message.Content = item.Content;

                if (item.Type == PMS.UserManage.Domain.Common.SysMessageType.AuthSuccess ||
                    item.Type == PMS.UserManage.Domain.Common.SysMessageType.LiveQuestion)
                {
                    message.Content = SysMessageTypeToContent(item.Type);
                }
                else if (item.Type == PMS.UserManage.Domain.Common.SysMessageType.AuthFail)
                {
                    message.Content = item.Content;
                }
                else if (item.Type == PMS.UserManage.Domain.Common.SysMessageType.Article)
                {
                    var temp = article?.Where(x => x.id == item.DataId).FirstOrDefault() ?? new Data();
                    //message.Title = temp.title;
                    //message.Content = temp.digest;
                    message.Title = "发表了文章";
                    message.Content = temp.title;
                    message.ArticleViewCount = temp.viewCount;

                    if (temp.covers?.Length > 0)
                    {
                        message.ArticleCovers = temp.covers[0];
                    }
                }
                else if (item.Type == PMS.UserManage.Domain.Common.SysMessageType.Live)
                {
                    message.Title = item.Title;
                    message.Content = item.Content;
                }
                else if (item.Type == PMS.UserManage.Domain.Common.SysMessageType.InviteChange)
                {
                    message.Title = item.Content;
                    message.Content = item.OriContent;

                    if (item.DataType == MessageDataType.Answer)
                    {
                        var temp = answers?.Where(x => x.Id == message.DataId).FirstOrDefault() ?? new AnswerInfoDto();
                        message.Content = temp.AnswerContent;

                        temp.questionDto ??= new QuestionDto();
                        message.No = UrlShortIdUtil.Long2Base32(temp.questionDto.No);
                    }
                    else if (item.DataType == MessageDataType.Question)
                    {
                        var temp = questions?.Where(x => x.Id == message.DataId).FirstOrDefault() ?? new QuestionInfo();
                        message.Content = temp.Content;
                        message.No = UrlShortIdUtil.Long2Base32(temp.No);
                    }
                    else if (item.DataType == MessageDataType.Comment)
                    {
                        var temp = comments?.Where(x => x.Id == message.DataId).FirstOrDefault() ?? new SchoolComment();
                        message.Content = temp.Content;
                        message.No = UrlShortIdUtil.Long2Base32(temp.No);
                    }
                }

                models.Add(message);
            }
            return models;
        }


        public static string SysMessageTypeToContent(PMS.UserManage.Domain.Common.SysMessageType Type)
        {
            string content = "";
            if (Type == PMS.UserManage.Domain.Common.SysMessageType.AuthSuccess)
            {
                content = "您申请的认证已通过，可以在个人主页查看您的认证称号。";
            }
            else if (Type == PMS.UserManage.Domain.Common.SysMessageType.Article)
            {
                content = "发布了新的文章，点击查看";
            }
            else if (Type == PMS.UserManage.Domain.Common.SysMessageType.Live)
            {
                content = "微课直播快要开始了，点击收听";
            }
            else if (Type == PMS.UserManage.Domain.Common.SysMessageType.InviteChange)
            {
                content = "您的邀请有了新的反馈，点击查看";
            }
            else if (Type == PMS.UserManage.Domain.Common.SysMessageType.LiveQuestion)
            {
                content = "您的微课有新的提问还没解答，点击查看";
            }
            else if (Type == PMS.UserManage.Domain.Common.SysMessageType.AuthFail)
            {
                content = "您申请的认证审核失败，点击查看具体原因";
            }

            return content;
        }

    }
}
