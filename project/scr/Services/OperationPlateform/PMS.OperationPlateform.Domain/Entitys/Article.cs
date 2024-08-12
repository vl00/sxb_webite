using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
    [Serializable]
    [Table("article")]
    public partial class article
    {
        [ExplicitKey]
        public Guid id { get; set; }

        public string title { get; set; }

        public string author { get; set; }

        public string author_origin { get; set; }

        public DateTime? time { get; set; }

        public string url { get; set; }

        public string type { get; set; }

        public string url_origin { get; set; }

        public byte layout { get; set; }

        public string img { get; set; }

        public string overview { get; set; }

        public string html { get; set; }

        public bool toTop { get; set; }

        public bool show { get; set; }

        public bool? linkOnly { get; set; }

        public int? viewCount { get; set; }

        public int? viewCount_r { get; set; }

        public bool? assistant { get; set; }

        public string city { get; set; }

        public DateTime? createTime { get; set; }

        public string creator { get; set; }

        public DateTime? updateTime { get; set; }

        public string updator { get; set; }

        public int No { get; set; }

        public bool IsHideInList { get; set; }

        /// <summary> 
        /// 置顶时间 
        /// </summary> 
        public DateTime? TopTime { get; set; }

        /// <summary>
        /// 作者类型  0 普通作者 1 达人作者
        /// </summary>
        public int AuthorType { get; set; }

        /// <summary> 
        /// 是否删除
        /// </summary> 
        public bool IsDeleted { get; set; }
    }
}