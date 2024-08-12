using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using PMS.OperationPlateform.Domain.Enums;

namespace PMS.OperationPlateform.Domain.Entitys
{
    [Serializable]
    [Table("DataPacket")]
    public partial class DataPacket
    {
        [ExplicitKey]
        public Guid Id { get; set; }

        /// <summary>
        /// 扫码编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 扫码用户
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 学校/文章id
        /// </summary>
        public Guid? DataId { get; set; }
        /// <summary>
        /// 扫码页面
        /// </summary>
        public string ScanPage { get; set; }
        /// <summary>
        /// 扫码次数
        /// </summary>
        public int ScanCount { get; set; }
        /// <summary>
        /// 扫码时间
        /// </summary>
        public DateTime? ScanTime { get; set; }
        /// <summary>
        /// 状态  1 扫码  2 识别  3 关注
        /// </summary>
        public DataPacketStatus Status { get; set; }
        ///// <summary>
        ///// 按住识别二维码时间
        ///// </summary>
        //public DateTime? IdentifyTime { get; set; }
        /// <summary>
        /// 关注时间 = 按住识别二维码时间
        /// </summary>
        public DateTime? SubscribeTime { get; set; }
        /// <summary>
        /// 0 无 1 关注消息  2 24小时消息 3 36小时消息 4 48小时消息
        /// </summary>
        public DataPacketStep Step { get; set; }
    }
}
