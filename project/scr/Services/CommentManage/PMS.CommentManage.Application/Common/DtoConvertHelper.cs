using PMS.CommentsManage.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.CommentsManage.Application.Common
{
    public static class DtoConvertHelper
    {
        /// <summary>
        /// 点评评分转换
        /// </summary>
        /// <param name="CommentScoreDtos"></param>
        /// <returns></returns>
        public static List<SchoolCmScoreDto> CommentScoreToStar(List<CommentScoreDto> CommentScoreDtos)
        {
            List<SchoolCmScoreDto> CommentScoreViews = new List<SchoolCmScoreDto>();
            if (!CommentScoreDtos.Any())
            {
                return CommentScoreViews;
            }

            foreach (var item in CommentScoreDtos)
            {
                if (item == null) continue;
                SchoolCmScoreDto commentScoreView = new SchoolCmScoreDto();
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

