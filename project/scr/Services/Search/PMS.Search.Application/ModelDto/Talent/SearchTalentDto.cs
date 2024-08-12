using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Application.ModelDto.Talent
{
    /// <summary>
    /// 达人
    /// </summary>
    public class SearchTalentDto
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
        /// 认证名称
        /// </summary>
        public string AuthTitle { get; set; }

        /// <summary> 
        /// </summary> 
        public Guid? UserId { get; set; }

        /// <summary>
        /// 达人所在城市编码
        /// </summary>
        public int? CityCode { get; set; }

        /// <summary> 
        /// 是否启用 
        /// </summary> 
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否删除(预构造的)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 达人粉丝数
        /// </summary>
        public long FansTotal { get; set; }

        /// <summary>
        /// 达人回答问题数
        /// 仅算回答问题, 回复回答不算
        /// </summary>
        public long AnswersTotal { get; set; }

    }
}
