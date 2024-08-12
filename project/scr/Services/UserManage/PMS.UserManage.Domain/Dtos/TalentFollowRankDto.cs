using System;

namespace PMS.UserManage.Domain.Dtos
{
    public class TalentFollowRankDto
    {
        /// <summary>
        /// 达人USERID
        /// </summary>
        public Guid TalentUserID { get; set; }
        /// <summary>
        /// 达人名称
        /// </summary>
        public string TalentName { get; set; }
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int FollowCount { get; set; }
        /// <summary>
        /// 达人描述
        /// </summary>
        public string TalentTitle { get; set; }
    }

}
