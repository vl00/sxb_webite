using MediatR;
using PMS.PaidQA.Domain.Enums;
using System;

namespace PMS.MediatR.Events.PaidQA
{

    /// <summary>
    /// 订单结束事件
    /// </summary>
    public class AssessStatisticsEvent : INotification
    {
        public Guid? UserID { get; set; }
        public AssessType AssessType { get; set; }
        public int StatisticsType { get; set; }
        public AssessStatisticsEvent(AssessType assessType, int statisticsType, Guid? userID)
        {
            UserID = userID;
            AssessType = assessType;
            StatisticsType = statisticsType;
        }
    }
}
