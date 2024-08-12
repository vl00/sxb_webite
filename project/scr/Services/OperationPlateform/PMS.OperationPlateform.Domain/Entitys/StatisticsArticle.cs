using System;
namespace PMS.OperationPlateform.Domain.Entitys
{
    public class StatisticsArticle
    {
        public Guid ShareArticleId { get; set; }
        public int PV { get; set; }
        public int UV { get; set; }
        public int ShareCount { get; set; }
        public int JumpCount { get; set; }
        public int TimeSpent { get; set; }
        public int Day { get; set; }


        public Guid ArticleId { get; set; }
        public string Fw { get; set; }
        public string ArticleUrl { get; set; }
        public DateTime StatisticsDate { get; set; }
    }
}
