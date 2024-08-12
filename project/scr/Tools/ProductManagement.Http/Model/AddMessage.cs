using System;
using System.Collections.Generic;
using System.Text;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace ProductManagement.API.Http.Model
{
    public class AddMessage
    {
        /// <summary>
        /// 受邀人id
        /// </summary>
        public Guid userId { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public MessageType type { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 详情id
        /// </summary>
        public Guid dataID { get; set; }
        /// <summary>
        /// 详情id类型
        /// </summary>
        public MessageDataType dataType { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid eID { get; set; }
        /// <summary>
        /// cookie值
        /// </summary>
        public string iSchoolAuth { get; set; }
        /// <summary>
        /// 是否未匿名发布
        /// </summary>
        public bool IsAnony { get; set; }
    }
}
