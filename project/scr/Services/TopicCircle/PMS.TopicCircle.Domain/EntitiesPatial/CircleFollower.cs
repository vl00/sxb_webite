using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Domain.Entities
{
    public partial class CircleFollower
    {
        /// <summary>
        /// 在圈子里的发帖数量
        /// </summary>
        [Write(false)]
        public int SendCount { get; set; }

        /// <summary>
        /// 关注者昵称
        /// </summary>
        [Write(false)]
        public string NickName { get; set; }

        /// <summary>
        /// 关注者头像
        /// </summary>
        [Write(false)]
        public string HeadImgUrl { get; set; }


        /// <summary>
        /// 计算加入圈子的天数
        /// </summary>
        [Write(false)]
        public int JoinDays { get; set; }

        /// <summary>
        /// 计算加入圈子的天数
        /// </summary>
        [Write(false)]
        public DateTime LoginTime { get; set; }



    }
}
