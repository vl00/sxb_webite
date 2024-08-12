using System;
namespace PMS.OperationPlateform.Application.Dtos
{
    public class HuaneAskAnswerDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string CreateTime { get; set; }
        public int FloorNumber { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
