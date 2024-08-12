using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Domain.Dtos
{

    /// <summary>
    /// 标签
    /// </summary>
    public class SimpleTagDto
    {
        /// <summary> 
        /// 标签
        /// </summary> 
        public int Id { get; set; }

        /// <summary> 
        /// 标签名称 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        /// 父级标签 
        /// </summary> 
        public int ParentId { get; set; }
        /// <summary> 
        /// 父级标签 
        /// </summary> 
        public string ParentName { get; set; }
    }
}
