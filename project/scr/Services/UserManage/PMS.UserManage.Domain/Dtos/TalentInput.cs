using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Dtos
{
    public class TalentInput
    {
        /// <summary>
        /// 类型 1为邀请，0为申请
        /// </summary>
        public int InviteType { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string user_id { get; set; }
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
        public string certification_identity_id { get; set; }
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
        /// 认证时间
        /// </summary>
        public DateTime? certification_date { get; set; }
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
        /// 补充说明
        /// </summary>
        public string supplementary_explanation { get; set; }
        /// <summary>
        /// 证明图片
        /// </summary>
        public List<TalentImg> imgs { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 有效时间
        /// </summary>
        public DateTime? effectivedate { get; set; }

        /// <summary>
        /// 达人昵称 == 用户昵称
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 达人头像
        /// </summary>
        public string headImgUrl { get; set; }

        /// <summary>
        /// 达人简介
        /// </summary>
        public string introduction { get; set; }
    }
}
