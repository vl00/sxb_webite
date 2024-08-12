using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class TagHotDto
    {
        /// <summary>
        /// 话题ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 话题词语
        /// </summary>
        public string Name { get; set; }


        public int PId { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public string PName { get; set; }

    }
}
