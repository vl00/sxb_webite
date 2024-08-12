namespace PMS.OperationPlateform.Domain.DTOs
{
    using Entitys;
    using PMS.OperationPlateform.Domain.Enums;

    public class SchoolRankDto : SchoolRank
    {
        public SchoolGradePY Grade { get; set; }
    }
}
