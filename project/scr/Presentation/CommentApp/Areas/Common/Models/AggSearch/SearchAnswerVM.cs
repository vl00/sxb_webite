using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.Web.Areas.Common.Models
{
    public class SearchAnswerVM
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 写入的用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 达人类型
        /// </summary>
        public TalentType? TalentType { get; set; }

        /// <summary>
        /// 回答内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 写入时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
    }

}
