using Dapper.Contrib.Extensions;
using System;

namespace PMS.TopicCircle.Domain.Entities
{
    public partial class Circle
    {

        /// <summary>
        /// 关注人数
        /// </summary>
        [Write(false)]
        public int FollowerCount { get; set; }



        /// <summary>
        /// 背景图
        /// </summary>
        [Write(false)]
        public CircleCover Cover { get; set; }


        /// <summary>
        /// 城市
        /// </summary>
        [Write(false)]
        public int? City { get; set; }

        /// <summary>
        /// 最新动态时间
        /// </summary>
        [Write(false)]
        public DateTime? DynamicTime { get; set; }
        /// <summary>
        /// 圈主名称
        /// </summary>
        [Write(false)]
        public string UserName{ get; set; }
    }
}
