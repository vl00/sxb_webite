using System;

namespace PMS.School.Domain.Dtos
{
    /// <summary>
    /// 达人阵容
    /// </summary>
    public class SpecialTopicUserDto
    {
        /// <summary>
        /// 达人名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 达人头像
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 达人ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 达人描述
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 达人类型
        /// <para>
        /// 0个人 1机构  
        /// </para>
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 达人粉丝
        /// </summary>
        public int FollowCount { get; set; }
    }
}
