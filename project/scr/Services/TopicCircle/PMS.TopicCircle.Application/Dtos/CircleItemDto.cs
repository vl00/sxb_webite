using System;

namespace PMS.TopicCircle.Application.Dtos
{
    public class CircleItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Intro { get; set; }

        public bool IsDisable { get; set; }

        public string Cover { get; set; }

        public int FollowCount { get; set; }
        public string CircleMasterName { get; set; }

        public string BGColor { get; set; }
    }
}
