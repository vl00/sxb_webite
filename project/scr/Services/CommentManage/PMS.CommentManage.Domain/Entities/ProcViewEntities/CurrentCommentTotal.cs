using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ProcViewEntities
{
    /// <summary>
    /// 分部点评统计总数
    /// </summary>
    public class CurrentCommentTotal
    {
        /// <summary>
        /// 总点评
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 精选点评
        /// </summary>
        public int SelectedTotal { get; set; }
        /// <summary>
        /// 过来人点评
        /// </summary>
        public int ComeHereTotal { get; set; }
        /// <summary>
        /// 辟谣点评
        /// </summary>
        public int RefutingTotal { get; set; }
        /// <summary>
        /// 好评
        /// </summary>
        public int GoodTotal { get; set; }
        /// <summary>
        /// 差评
        /// </summary>
        public int BadTotal { get; set; }
        /// <summary>
        /// 有图
        /// </summary>
        public int ImageTotal { get; set; }
        /// <summary>
        /// 高中部
        /// </summary>
        public int HighSchoolTotal { get; set; }
        /// <summary>
        /// 国际部
        /// </summary>
        public int InternationalTotal { get; set; }
    }
}
