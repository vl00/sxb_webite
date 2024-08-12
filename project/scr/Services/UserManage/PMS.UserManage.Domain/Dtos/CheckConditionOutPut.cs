using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Dtos
{
    public class CheckConditionOutPut
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string headImgUrl { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 区号
        /// </summary>
        public string nationCode { get; set; }
        /// <summary>
        /// 关注数
        /// </summary>
        public int? attentionCount { get; set; }
        /// <summary>
        /// 回答数
        /// </summary>
        public int? answerCount { get; set; }
    }
}
