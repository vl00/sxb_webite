using PMS.CommentsManage.Application.ModelDto;
using Sxb.UserCenter.Models.QuestionViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Utils.DtoToViewModel
{
    public static class AnswerInfoDtoToVoHelper
    {
        /// <summary>
        /// 问题回答领域实体转 视图实体
        /// </summary>
        /// <param name="AnswerInfoDtos"></param>
        /// <param name="UserVos"></param>
        /// <returns></returns>
        public static List<QuestionAnswerViewModel> AnswerInfoDtoToVo(List<AnswerInfoDto> AnswerInfoDtos,List<UserViewModel> UserVos,List<QuestionInfoViewModel> Questions = null) 
        {
            List<QuestionAnswerViewModel> AnswerVos = new List<QuestionAnswerViewModel>();
            foreach (var item in AnswerInfoDtos)
            {
                QuestionAnswerViewModel AnswerViewModel = new QuestionAnswerViewModel();
                AnswerViewModel.Id = item.Id;
                AnswerViewModel.Content = item.AnswerContent;
                AnswerViewModel.LikeTotal = item.LikeCount;
                AnswerViewModel.AnswerTotal = item.ReplyCount;
                AnswerViewModel.AddTime = item.AddTime;
                AnswerViewModel.UserInfoVo = CopyUserModel(UserVos.Where(x => x.Id == item.UserId).FirstOrDefault())?.ToAnonyUserName(item.IsAnony);
                AnswerViewModel.IsAttend = item.IsAttend;
                AnswerViewModel.IsSchoolPublish = item.IsSchoolPublish;
                AnswerViewModel.RumorRefuting = item.RumorRefuting;
                AnswerViewModel.IsAnony = item.IsAnony;
                AnswerViewModel.IsLike = item.IsLike;
                AnswerViewModel.QuestionId = item.QuestionId;

                if (Questions != null) 
                {
                    AnswerViewModel.QuestionVo = Questions.Where(x => x.Id == item.QuestionId).FirstOrDefault();
                }
                AnswerVos.Add(AnswerViewModel);
            }
            return AnswerVos;
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
