using PMS.CommentsManage.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using Sxb.UserCenter.Models.QuestionViewModel;
using Sxb.UserCenter.Models.SchoolViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;

namespace Sxb.UserCenter.Utils.DtoToViewModel
{
    public static class QuestionDtoToVoHelper
    {
        public static List<QuestionInfoViewModel> QuestionDtoToVo(string ImagePath, List<QuestionDto> QuestionDtos,List<UserViewModel> UserInfoVos,List<SchoolQuestionCardViewModel> SchoolCard = null,List<Guid> checkCollection = null) 
        {
            List<QuestionInfoViewModel> QuestionVo = new List<QuestionInfoViewModel>();

            foreach (var question in QuestionDtos)
            {
                QuestionInfoViewModel questionInfo = new QuestionInfoViewModel();
                questionInfo.No = UrlShortIdUtil.Long2Base32(question.No);
                questionInfo.Id = question.Id;
                questionInfo.UserId = question.UserId;
                questionInfo.LikeCount = question.LikeCount;
                questionInfo.AnswerCount = question.AnswerCount;
                questionInfo.Content = question.QuestionContent;
                questionInfo.AddTime = question.QuestionCreateTime.ConciseTime();
                questionInfo.Images = question.Images?.Select(x=> ImagePath + x)?.ToList();
                questionInfo.School = SchoolCard?.Where(x => x.ExtId == question.SchoolSectionId).FirstOrDefault();
                questionInfo.ExId = question.SchoolSectionId;
                questionInfo.Sid = question.SchoolId;
                questionInfo.UserInfoVo = CopyUserModel(UserInfoVos.Where(x => x.Id == question.UserId).FirstOrDefault())?.ToAnonyUserName(question.IsAnony);

                if(checkCollection!= null && checkCollection.Any()) 
                {
                    questionInfo.IsCollection = checkCollection.Where(x => x == question.Id).FirstOrDefault() != Guid.Empty;
                }
                QuestionVo.Add(questionInfo);
            }
            return QuestionVo;
        }
        private static UserViewModel CopyUserModel(UserViewModel user)
        {
            return user == null ? null : new UserViewModel
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
