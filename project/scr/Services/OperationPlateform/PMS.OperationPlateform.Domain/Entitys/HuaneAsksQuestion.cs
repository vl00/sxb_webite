using System;
namespace PMS.OperationPlateform.Domain.Entitys
{
    public class HuaneAsksQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int ClickTimes { get; set; }
        public DateTime CreateTime { get; set; }

        public string Answer { get; set; }
    }
}
