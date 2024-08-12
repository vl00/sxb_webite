using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Domain.Dtos
{
    /// <summary>
    /// 存储过程USP_QUERYMYCIRCLES的查询结果
    /// </summary>
    public class USPQUERYMYCIRCLESDto
    {
        public Guid Id { get; set; }

        /// <summary> 
        ///  
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        ///  
        /// </summary> 
        public Guid? UserId { get; set; }

        /// <summary> 
        ///  
        /// </summary> 
        public string Intro { get; set; }

        /// <summary> 
        ///  
        /// </summary> 
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 是否有新动态
        /// </summary>
        public bool HASNEWS { get; set; }

        /// <summary>
        /// 新贴数
        /// </summary>
        public int NEWTOPICCOUNT { get; set; }

        /// <summary>
        /// 新回复数
        /// </summary>
        public int NEWREPLYCOUNT { get; set; }


        /// <summary>
        /// 关注者数量
        /// </summary>
        public int FOLLOWERCOUNT { get; set; }


        public static implicit operator Circle(USPQUERYMYCIRCLESDto dto)
        {
            return new Circle()
            {
                Id = dto.Id,
                Name = dto.Name,
                UserId = dto.UserId
            };
        }

    }
}
