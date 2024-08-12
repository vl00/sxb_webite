using PMS.School.Domain.Common;
using Sxb.Web.Common;
using System;
using System.Collections.Generic;

namespace Sxb.Web.ViewModels.Article
{
    public class QACardViewModel
    {

        public Guid QID { get; set; }

        /// <summary>
        /// 问题标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 学部全称
        /// </summary>
        public string sname { get; set; }

        /// <summary>
        /// 学部下的问答数量
        /// </summary>
        public int sqacount { get; set; }

        public List<Reply> replys { get; set; }


        /// <summary>
        /// 问答详情的链接
        /// </summary>
        public string qaUrl { get; set; }

        /// <summary>
        /// 回复总数量
        /// </summary>
        public int replyTotalCount { get; set; }
        public string ShortNo { get; set; }
        public string ShortSchoolNo { get; set; }
        public LodgingEnum LodgingType { get; set; }
        public string LodgingTypeName => LodgingType.GetDescription();
        public SchoolType SchoolType { get; set; }
        public string SchoolTypeName=> SchoolType.GetDescription();

        /// <summary>
        /// 回复
        /// </summary>
        public class Reply
        {

            public Guid id { get; set; }

            /// <summary>
            /// 回复者名称
            /// </summary>
            public string uname { get; set; }

            /// <summary>
            /// 回复内容
            /// </summary>
            public string content { get; set; }
        }
    }
}
