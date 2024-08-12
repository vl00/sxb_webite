using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Dtos
{
    /// <summary>
    /// 用户关注/粉丝列表
    /// </summary>
    public class UserCollectionDto
    {

        /// <summary>
        /// 查询人和被查数据的关系
        /// 0 无关 1 关注 2 粉丝 
        /// 互粉 1 ^ 2 = 3
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 被关注人
        /// </summary>
        public Guid DataId { get; set; }
        /// <summary>
        /// 关注人(粉丝)
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 被查人的达人Id
        /// 查关注的人, 这是关注的人的talentId
        /// 查粉丝, 这是粉丝的talentId
        /// 下同
        /// </summary>
        public Guid? TalentId { get; set; }
        /// <summary>
        /// 达人类型 0个人 1机构  
        /// </summary>
        public int? TalentType { get; set; }
        public Guid SearchUserId { get; set; }
        /// <summary>
        /// =SearchUserId兼容前端
        /// </summary>
        public Guid Id { 
            get { return SearchUserId; }
            set { Id = value; }
        }

        /// <summary>
        /// 达人称号
        /// </summary>
        public string TalentCertificationPreview { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImgUrl { get; set; }
        public string Introduction { get; set; }
        public int? Sex { get; set; }
        public DateTime? Time { get; set; }
    }
}
