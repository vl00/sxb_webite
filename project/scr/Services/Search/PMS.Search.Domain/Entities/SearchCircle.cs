using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Entities
{
    public class SearchCircle : SearchPUV
    {
        /// <summary> 
        /// 主键 
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary> 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        /// </summary> 
        public Guid? UserId { get; set; }

        /// <summary> 
        /// 是否未启用 
        /// </summary> 
        public bool IsDisable { get; set; }
        /// <summary>
        /// 是否删除(预构造的, 默认未删除)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary> 
        /// </summary> 
        public string Intro { get; set; }

        /// <summary> 
        /// </summary> 
        public DateTime? ModifyTime { get; set; }

        /// <summary> 
        /// </summary> 
        public DateTime? CreateTime { get; set; }


        /// <summary>
        /// 圈主昵称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 圈粉数
        /// </summary>
        public long FollowCount { get; set; }
    }
}
