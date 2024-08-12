using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.ModelDto;
using Sxb.PCWeb.ViewModels.Comment;
using Sxb.PCWeb.ViewModels.Common;
using Sxb.PCWeb.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Utils.CommentHelper
{
    public static class ScoreToStarHelper
    {
        /// <summary>
        /// 学校点评总分转换
        /// </summary>
        /// <param name="SchoolScoreDtos"></param>
        /// <returns></returns>
        public static List<SchoolCmScoreViewModel> SchoolScoreToStar(List<SchoolScoreDto> SchoolScoreDtos) 
        {
            if (SchoolScoreDtos.Count() == 0)
                return null;

            List<SchoolCmScoreViewModel> score = new List<SchoolCmScoreViewModel>();
            foreach (var item in SchoolScoreDtos)
            {
                SchoolCmScoreViewModel SchoolcmScore = new SchoolCmScoreViewModel();
                SchoolcmScore.AggStar = SchoolScoreToStart.GetCurrentSchoolstart(item.AggScore);
                SchoolcmScore.EnvirStar = SchoolScoreToStart.GetCurrentSchoolstart(item.EnvirScore);
                SchoolcmScore.HardStar = SchoolScoreToStart.GetCurrentSchoolstart(item.HardScore);
                SchoolcmScore.LifeStar = SchoolScoreToStart.GetCurrentSchoolstart(item.LifeScore);
                SchoolcmScore.ManageStar = SchoolScoreToStart.GetCurrentSchoolstart(item.ManageScore);
                SchoolcmScore.TeachStar = SchoolScoreToStart.GetCurrentSchoolstart(item.TeachScore);
                SchoolcmScore.Sid = item.SchoolId;
                SchoolcmScore.ExtId = item.SchoolSectionId;
                SchoolcmScore.CommentTotal = item.CommentCount;
                SchoolcmScore.QuestionTotal = item.QuestionCount;
                score.Add(SchoolcmScore);
            }
            return score;
        }

        /// <summary>
        /// 点评评分转换
        /// </summary>
        /// <param name="CommentScoreDtos"></param>
        /// <returns></returns>
        public static List<CommentScoreViewModel> CommentScoreToStar(List<CommentScoreDto> CommentScoreDtos)
        {
            List<CommentScoreViewModel> CommentScoreViews = new List<CommentScoreViewModel>();
            if (!CommentScoreDtos.Any()) 
            {
                return CommentScoreViews;
            }

            foreach (var item in CommentScoreDtos)
            {
                if (item == null) continue;
                CommentScoreViewModel commentScoreView = new CommentScoreViewModel();
                commentScoreView.AggStar = SchoolScoreToStart.GetCurrentSchoolstart(item.AggScore);
                commentScoreView.EnvirStar = SchoolScoreToStart.GetCurrentSchoolstart(item.EnvirScore);
                commentScoreView.HardStar = SchoolScoreToStart.GetCurrentSchoolstart(item.HardScore);
                commentScoreView.LifeStar = SchoolScoreToStart.GetCurrentSchoolstart(item.LifeScore);
                commentScoreView.ManageStar = SchoolScoreToStart.GetCurrentSchoolstart(item.ManageScore);
                commentScoreView.TeachStar = SchoolScoreToStart.GetCurrentSchoolstart(item.TeachScore);
                commentScoreView.CommentId = item.CommentId;
                commentScoreView.IsAttend = item.IsAttend;
                CommentScoreViews.Add(commentScoreView);
            }
            return CommentScoreViews;
        }

    }
}
