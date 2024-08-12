using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public  class CricleFollowerSearchRequestDto
    {
        /// <summary>
        /// 粉丝圈ID
        /// </summary>
        public Guid CircleId { get; set; }

        /// <summary>
        /// 关注者昵称
        /// </summary>
        public string NickName { get; set; }

    }
}
