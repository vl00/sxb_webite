using System;
using System.Collections.Generic;
using System.Text;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Domain.Dtos
{
    public class TalentAbout
    {
        /// <summary> 
        /// 主键达人id
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary> 
        /// 达人用户名称
        /// </summary> 
        public string NickName { get; set; }

        /// <summary>
        ///达人用户头像
        /// </summary>
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 认证名称
        /// </summary>
        public string AuthTitle { get; set; }

        /// <summary> 
        /// </summary> 
        public Guid? UserId { get; set; }

        /// <summary>
        /// 达人粉丝数
        /// </summary>
        public long FansTotal { get; set; }

        /// <summary>
        /// 达人回答问题数
        /// 仅算回答问题, 回复回答不算
        /// </summary>
        public long AnswersTotal { get; set; }

        public TalentType Type { get; set; }

    }

    public class TalentUser
    {
        /// <summary> 
        /// </summary> 
        public Guid UserId { get; set; }

        /// <summary> 
        /// 达人用户名称
        /// </summary> 
        public string NickName { get; set; }

        /// <summary>
        ///达人用户头像
        /// </summary>
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 认证名称
        /// </summary>
        public string AuthTitle { get; set; }


        public TalentType? Type { get; set; }

    }

}
