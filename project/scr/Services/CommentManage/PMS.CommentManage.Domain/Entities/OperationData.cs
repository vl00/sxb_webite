using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 数据源【点赞数 | 回复数 | 是否点赞】
    /// </summary>
    public class OperationData
    {
        /// <summary>
        /// 数据源Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsPraise { get; set; }
        /// <summary>
        /// 点赞总数
        /// </summary>
        public int LikeTotal { get; set; }
        /// <summary>
        /// 回复总数
        /// </summary>
        public int ReplyTotal { get; set; }
    }
}
