using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Common
{
    public static class SchoolScoreToStart
    {
        public static int GetCurrentSchoolstart(decimal scoreTemp)
        {
            if (scoreTemp > 20)
                scoreTemp = scoreTemp / 20;
            int start = 0, score = (int)Math.Round(scoreTemp);
            if (score <= 5) 
            {
                score = score * 20;
            }
            
            if (score >= 1 && score <= 20)
            {
                start = 1;
            }
            else if (score >= 21 && score <= 40)
            {
                start = 2;
            }
            else if (score >= 41 && score <= 60)
            {
                start = 3;
            }
            else if (score >= 61 && score <= 80)
            {
                start = 4;
            }
            else if(score >= 81 && score<= 100)
            {
                start = 5;
            }
            return start;
        }

        public static string scoreValue(decimal score)
        {
            string value = "";
            if (score >= 0 && score <= 20)
            {
                value = "不佳";
            }
            else if (score >= 21 && score <= 40)
            {
                value = "一般";
            }
            else if (score >= 41 && score <= 60)
            {
                value = "不错";
            }
            else if (score >= 61 && score <= 80)
            {
                value = "满意";
            }
            else
            {
                value = "超棒";
            }
            return value;
        }
    }
}
