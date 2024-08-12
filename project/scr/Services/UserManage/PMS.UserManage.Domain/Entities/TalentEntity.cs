using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    [Table("talent")]
    public class TalentEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid user_id { get; set; }
        ///// <summary>
        ///// 用户昵称
        ///// </summary>
        //public string user_name { get; set; }
        ///// <summary>
        ///// 绑定手机号
        ///// </summary>
        //public string user_phone { get; set; }
        /// <summary>
        /// 认证类型
        /// </summary>
        public int? certification_type { get; set; }
        /// <summary>
        /// 认证方式
        /// </summary>
        public int? certification_way { get; set; }
        /// <summary>
        /// 认证身份
        /// </summary>
        public string certification_identity { get; set; }
        /// <summary>
        /// 认证身份id
        /// </summary>
        public Guid certification_identity_id { get; set; }
        /// <summary>
        /// 认证称号
        /// </summary>
        public string certification_title { get; set; }
        /// <summary>
        /// 认证说明类型 0：认证称号，1前缀+认证称号
        /// </summary>
        public int? certification_explanation { get; set; }
        /// <summary>
        /// 认证说明预览
        /// </summary>
        public string certification_preview { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdate { get; set; }
        /// <summary>
        /// 认证时间
        /// </summary>
        public DateTime? certification_date { get; set; }
        /// <summary>
        /// 删除标记 0否，1是
        /// </summary>
        public int? isdelete { get; set; }
        /// <summary>
        /// 类型 0个人 1机构
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 机构全称
        /// </summary>
        public string organization_name { get; set; }
        /// <summary>
        /// 统一社会信用码
        /// </summary>
        public string organization_code { get; set; }
        /// <summary>
        /// 运营人员姓名
        /// </summary>
        public string operation_name { get; set; }
        /// <summary>
        /// 运营人员手机号
        /// </summary>
        public string operation_phone { get; set; }
        /// <summary>
        /// 认证状态 0未审核，1已通过，2已驳回
        /// </summary>
        public int? certification_status { get; set; }
        /// <summary>
        /// 状态 0:禁用 1启用
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 邀请状态 0 非邀请  1 邀请中 2 邀请成功
        /// </summary>
        public int? invite_status { get; set; }
        /// <summary>
        /// 补充说明
        /// </summary>
        public string supplementary_explanation { get; set; }
        /// <summary>
        /// 机构员工最大值
        /// </summary>
        public int organization_staff_count { get; set; }
        /// <summary>
        /// 简称
        /// </summary>
        [NotMapped]
        public string nickname { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [NotMapped]
        public string mobile { get; set; }
        /// <summary>
        /// 证明图片
        /// </summary>
        [NotMapped]
        public List<TalentImg> imgs { get; set; }
        /// <summary>
        /// 达人码
        /// </summary>
        [NotMapped]
        public string code { get; set; }
    }
}
