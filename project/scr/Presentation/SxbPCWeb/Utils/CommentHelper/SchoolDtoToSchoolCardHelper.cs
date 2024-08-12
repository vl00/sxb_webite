using PMS.CommentsManage.Application.ModelDto;
using Sxb.PCWeb.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using Unity.Injection;

namespace Sxb.PCWeb.Utils.CommentHelper
{
    public static class SchoolDtoToSchoolCardHelper
    {
        /// <summary>
        /// 学校Dto 转 学校点评卡片
        /// </summary>
        /// <param name="SchoolInfoDtos"></param>
        /// <returns></returns>
        public static List<SchoolCommentCardViewModel> SchoolDtoToSchoolCommentCard(List<SchoolInfoDto> SchoolInfoDtos)
        {
            List<SchoolCommentCardViewModel> Cards = new List<SchoolCommentCardViewModel>();

            if (!SchoolInfoDtos.Any())
            {
                return Cards;
            }

            foreach (var item in SchoolInfoDtos)
            {
                SchoolCommentCardViewModel model = new SchoolCommentCardViewModel();
                model.CommentTotal = item.SectionCommentTotal;
                model.ExtId = item.SchoolSectionId;
                model.Sid = item.SchoolId;
                model.SchoolName = item.SchoolName;
                model.Stars = item.SchoolStars;
                model.IsAuth = item.IsAuth;
                model.SchoolType = item.SchoolType;
                model.LodgingType = (int)item.LodgingType;
                model.LodgingReason = item.LodgingType.Description();
                model.ShortSchoolNo = item.ShortSchoolNo;
                Cards.Add(model);
            }
            return Cards;
        }

        /// <summary>
        /// 学校Dto 转 学校提问卡片
        /// </summary>
        /// <param name="SchoolInfoDtos"></param>
        /// <returns></returns>
        public static List<SchoolQuestionCardViewModel> SchoolDtoToSchoolQuestionCard(List<SchoolInfoDto> SchoolInfoDtos)
        {
            List<SchoolQuestionCardViewModel> Cards = new List<SchoolQuestionCardViewModel>();
            foreach (var item in SchoolInfoDtos)
            {
                SchoolQuestionCardViewModel model = new SchoolQuestionCardViewModel();
                model.QuestionTotal = item.SectionQuestionTotal;
                model.ExtId = item.SchoolSectionId;
                model.Sid = item.SchoolId;
                model.SchoolName = item.SchoolName;
                model.Type = (int)item.SchoolType;
                model.LodgingType = (int)item.LodgingType;
                model.LodgingReason = item.LodgingType.Description();
                model.International = item.SchoolType == PMS.School.Domain.Common.SchoolType.International;
                model.Auth = item.IsAuth;
                model.ShortSchoolNo = item.ShortSchoolNo;
                Cards.Add(model);
            }
            return Cards;
        }

        /// <summary>
        /// 热评学校
        /// </summary>
        /// <param name="HottestSchool"></param>
        /// <returns></returns>
        public static List<SchoolCommentCardViewModel> HottestSchoolItem(List<HotCommentSchoolDto> HottestSchool)
        {
            List<SchoolCommentCardViewModel> Items = new List<SchoolCommentCardViewModel>();
            if (!HottestSchool.Any())
            {
                return Items;
            }

            foreach (var item in HottestSchool)
            {
                SchoolCommentCardViewModel cardViewModel = new SchoolCommentCardViewModel();
                cardViewModel.CommentTotal = item.Total;
                cardViewModel.SchoolName = item.SchoolName;
                cardViewModel.ExtId = item.SchoolSectionId;
                cardViewModel.Sid = item.SchoolId;
                cardViewModel.ShortSchoolNo = item.ShortSchoolNo;
                Items.Add(cardViewModel);
            }
            return Items;
        }

        /// <summary>
        /// 热问学校
        /// </summary>
        /// <param name="HottestSchool"></param>
        /// <returns></returns>
        public static List<SchoolQuestionCardViewModel> HottestQuestionSchoolItem(List<HotQuestionSchoolDto> HottestSchool)
        {
            List<SchoolQuestionCardViewModel> Items = new List<SchoolQuestionCardViewModel>();
            if (HottestSchool == null || !HottestSchool.Any())
            {
                return Items;
            }

            foreach (var item in HottestSchool)
            {
                SchoolQuestionCardViewModel cardViewModel = new SchoolQuestionCardViewModel()
                {
                    ShortSchoolNo = UrlShortIdUtil.Long2Base32(item.SchoolNo).ToLower()
                };
                cardViewModel.QuestionTotal = item.Total;
                cardViewModel.SchoolName = item.SchoolName;
                cardViewModel.ExtId = item.SchoolSectionId;
                cardViewModel.Sid = item.SchoolId;
                Items.Add(cardViewModel);
            }
            return Items;
        }


    }
}
