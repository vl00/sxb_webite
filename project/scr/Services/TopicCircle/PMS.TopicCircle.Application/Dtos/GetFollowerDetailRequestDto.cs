using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{

    public enum SortEnum
    {

        /// <summary>
        /// 升序
        /// </summary>
        JoinDayAsc = 1,
        /// <summary>
        /// 降序
        /// </summary>
        JoinDayDesc = 2,
        /// <summary>
        /// 最后登录时间
        /// </summary>
        EndLogTime = 3

    }
    public class GetFollowerDetailRequestDto
    {
        public Guid CircleId { get; set; }

        public SortEnum SortType { get; set; }

        public string SearchContent { get; set; }
    }
}
