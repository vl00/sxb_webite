using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Article
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
