using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    /// <summary>
    /// 讲师
    /// </summary>
    [Table("lector")]
    public class Lector
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid userID { get; set; }
        /// <summary>
        /// 讲师名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 讲师介绍
        /// </summary>
        public string intro { get; set; }
        /// <summary>
        /// 删除 0:删除 1:正常
        /// </summary>
        public int? show { get; set; }
        /// <summary>
        /// 排序权重
        /// </summary>
        public int? weight { get; set; }
        /// <summary>
        /// 个性签名
        /// </summary>
        public string autograph { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 认证类型
        /// </summary>
        public int? org_type { get; set; }
        /// <summary>
        /// 认证身份头衔
        /// </summary>
        public string org_tag { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string head_imgurl { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? jointime { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? modifytime { get; set; }

    }
}
