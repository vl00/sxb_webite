using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
   public  class TagDto
    {
        /// <summary>
        /// 话题ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 话题词语
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public int Pid { get; set; }

        public static implicit operator TagDto(Tag tag)
        {
            return new TagDto()
            {
                Id = tag.Id,
                Name = tag.Name,
                Pid = tag.ParentId
            };
        }

    }
}
