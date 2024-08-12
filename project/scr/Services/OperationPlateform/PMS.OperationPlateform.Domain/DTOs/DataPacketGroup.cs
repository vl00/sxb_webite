using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.DTOs
{
    public class DataPacketGroupDto
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 扫码页面
        /// </summary>
        public string ScanPage { get; set; }

        /// <summary>
        /// 扫码次数
        /// </summary>
        public long ScanCount { get; set; }

        /// <summary>
        /// 关注数
        /// </summary>
        public long SubcribeCount { get; set; }
    }

    public class DataPacketUserDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        public string OpenId { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 当前关注状态
        /// </summary>
        public bool IsSubcribe { get; set; }

        /// <summary>
        /// 关注时间
        /// </summary>
        public DateTime SubscribeTime { get; set; }

        /// <summary>
        /// 取关时间
        /// </summary>
        public DateTime? LastUnSubscribeTime { get; set; }
    }
}
