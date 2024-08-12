using System;

namespace PMS.UserManage.Application.Services
{
    public class SubscribeRemindAddDto
    {
        public static string WeChatRecruitGroupCode = "BigK";

        public string GroupCode { get; set; }
        public Guid UserId { get; set; }
        public Guid SubjectId { get; set; }
        /// <summary> 
        /// 开始提醒时间 
        /// </summary> 
        public DateTime? StartTime { get; set; }

        /// <summary> 
        /// 结束提醒时间 
        /// </summary> 
        public DateTime? EndTime { get; set; }
    }
}
