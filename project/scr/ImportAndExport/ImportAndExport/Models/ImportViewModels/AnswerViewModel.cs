using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Models.ImportViewModels
{
    public class AnswerViewModel
    {
        public AnswerViewModel() 
        {
            IsAttend = true;
            IsAnony = false;
        }

        /// <summary>
        /// 问题Id
        /// </summary>
        [Description("问题Id")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// 邀请者Id
        /// </summary>
        [Description("邀请者Id")]
        public Guid InviteUserId { get; set; }

        /// <summary>
        /// 写入者【用户池随机获取用户id】，写入者信息随机获取，需要先提交一条该用户邀请的达人进行回复，且进行消息推送一条
        /// </summary>
        [Description("受邀达人Id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 回答者Id
        /// </summary>
        [Description("回答者Id")]
        public Guid RespondentId { get; set; }

        /// <summary>
        /// 回答内容
        /// </summary>
        [Description("回答内容")]
        public string AnswerContent { get; set; }

        /// <summary>
        /// 是否入读
        /// </summary>
        [Description("是否为过来人发布[1:是 0:否]")]
        public bool IsAttend { get; set; }

        /// <summary>
        /// 是否为过来人发布[1:是 0:否]
        /// </summary>
        [Description("是否匿名发布[1:是 0:否]")]
        public bool IsAnony { get; set; }
    }
}
