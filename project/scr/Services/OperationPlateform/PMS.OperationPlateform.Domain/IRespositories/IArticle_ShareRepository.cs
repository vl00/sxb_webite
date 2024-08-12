using System;
using System.Collections.Generic;
using PMS.OperationPlateform.Domain.Entitys;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IArticle_ShareRepository
    {
        List<StatisticsArticle> GetUnstatisticsArticle();
        void InsertStatisticsArticle(List<StatisticsArticle> statisticsArticle);
    }
}
