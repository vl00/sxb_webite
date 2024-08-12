using PMS.CommentsManage.Application.ModelDto;
using Sxb.PCWeb.Models.User;
using Sxb.PCWeb.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Utils.CommentHelper
{
    public static class AnswerInfoDtoToVoHelper
    {
        /// <summary>
        /// 问题回答领域实体转 视图实体
        /// </summary>
        /// <param name="AnswerInfoDtos"></param>
        /// <param name="UserVos"></param>
        /// <returns></returns>
        public static List<QuestionAnswerViewModel> AnswerInfoDtoToVo(List<AnswerInfoDto> AnswerInfoDtos,List<UserInfoVo> UserVos) 
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
                AnswerViewModel.UserInfoVo = UserVos.Where(x => x.Id == item.UserId).FirstOrDefault()?.ToAnonyUserName(item.IsAnony);
                AnswerViewModel.IsAttend = item.IsAttend;
                AnswerViewModel.IsSchoolPublish = item.IsSchoolPublish;
                AnswerViewModel.RumorRefuting = item.RumorRefuting;
                AnswerViewModel.IsAnony = item.IsAnony;
                AnswerViewModel.IsLike = item.IsLike;
                AnswerViewModel.QuestionId = item.QuestionId;
                

                AnswerVos.Add(AnswerViewModel);
            }
            return AnswerVos;
        }

    }
}
