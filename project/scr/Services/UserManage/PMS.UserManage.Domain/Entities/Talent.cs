using System;

namespace PMS.UserManage.Domain.Entities
{
    public class Talent
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public int? Role { get; set; }
        /// <summary>
        /// 用户简介
        /// </summary>
        public string Introduction { get; set; }
        /// <summary>
        /// 认证title
        /// </summary>
        public string Certification_title { get; set; }
        /// <summary>
        /// 认证title预览
        /// </summary>
        public string Certification_preview { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsAuth { get; set; }
        /// <summary>
        /// 是否员工
        /// </summary>
        public bool IsTalentStaff { get; set; }
        /// <summary>
        /// 员工对应的机构userid
        /// </summary>
        public Guid? ParentUserId { get; set; }
        /// <summary>
        /// 机构名
        /// </summary>
        public string Organization_name { get; set; }
        /// <summary>
        /// 学部id
        /// </summary>
        public Guid Eid { get; set; }
        /// <summary>
        /// 是否内部达人
        /// </summary>
        public bool IsInternal { get; set; }
    }
}
