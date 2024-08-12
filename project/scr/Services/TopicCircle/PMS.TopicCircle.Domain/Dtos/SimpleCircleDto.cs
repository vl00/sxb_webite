using PMS.TopicCircle.Domain.Entities;

namespace PMS.TopicCircle.Domain.Dtos
{
    public class SimpleCircleDto : Circle
    {
        public string UserName { get; set; }
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsFollowed { get; set; }
    }
}
