using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;

namespace PMS.PaidQA.Domain.EntityExtend
{
    public class TalentExtend
    {
        /// <summary>
        /// 达人id
        /// </summary>
        public Guid TalentId { get; set; }

        /// <summary>
        /// 达人昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 达人标签
        /// </summary>
        public string  Tag { get; set; }
        public int? TalentType { get; set; }
    }
}
