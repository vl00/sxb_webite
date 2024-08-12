using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    public class PartTimeJobAdminVo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 电话（账号）
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 系统内昵称（允许修改）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 微信个人信息（关注公众号后得到）
        /// </summary>
        public Guid WeChatId { get; set; }
        /// <summary>
        /// 1：兼职，2：兼职领队，3：供应商，4：审核，5：管理员
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 第一次登录系统密码为邀请码，则password为空
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 注册邀请码
        /// </summary>
        public string RegesitCode { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
        /// <summary>
        /// 结算类型，1：微信现结，2：另结
        /// </summary>
        public int SettlementType { get; set; }
        /// <summary>
        /// 是否禁用该账号，ture：禁用
        /// </summary>
        public bool Prohibit { get; set; }
        /// <summary>
        /// 注册日期，激活日期（查询日志表）
        /// </summary>
        public string RegesitTime { get; set; }
        /// <summary>
        /// 问题审核
        /// </summary>
        public List<QuestionExamineVo> QuestionExamineVos { get; set; }
        /// <summary>
        /// 答题审核
        /// </summary>
        public List<QuestionsAnswerExamineVo> QuestionsAnswerExamineVos { get; set; }
        /// <summary>
        /// 问题详情
        /// </summary>
        public List<QuestionInfoVo> QuestionInfoVos { get; set; }
        /// <summary>
        /// 答题详情
        /// </summary>
        public List<QuestionsAnswersInfoVo> QuestionsAnswersInfoVos { get; set; }
    }
}
