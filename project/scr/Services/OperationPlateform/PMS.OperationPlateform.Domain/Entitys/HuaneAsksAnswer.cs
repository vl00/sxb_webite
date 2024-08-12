using System;
namespace PMS.OperationPlateform.Domain.Entitys
{
    public class HuaneAsksAnswer
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public int FloorNumber { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
