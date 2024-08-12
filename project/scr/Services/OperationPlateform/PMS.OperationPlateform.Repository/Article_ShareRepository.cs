using System;
using System.Collections.Generic;
using System.Linq;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;

namespace PMS.OperationPlateform.Repository
{
    public class Article_ShareRepository: IArticle_ShareRepository
    {

        private readonly OperationCommandDBContext _dbcontext;

        public Article_ShareRepository(OperationCommandDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        /// <summary>
        /// 获取需要统计的分享文章链接和日期
        /// </summary>
        public List<StatisticsArticle> GetUnstatisticsArticle()
        {
            string sql = @"select sa.Id as ShareArticleId,sa.ArticleId,sa.ArticleUrl,sc.Code as Fw,dd.value + 1 as [day] ,
            dateadd(day,CONVERT(int, dd.[VALUE]),sa.ShareTime) as [StatisticsDate]
            FROM share_articles sa
            CROSS APPLY STRING_SPLIT('0,1,2,6',',') dd
            left join share_channel sc on sc.id = sa.ChannelId
            left join share_statistics ss on ss.ShareArticleId = sa.ID and ss.Day = CONVERT(int, dd.[VALUE]) + 1
            where ss.id is null 
            and dateadd(day,CONVERT(int, dd.[VALUE]) ,sa.ShareTime) <  CONVERT(datetime,convert(char(10),GetDate(),120))
            --and sa.Id = '603F2A4E-A31B-4B21-AD29-61C6C73D06B2';";

            return _dbcontext.Query<StatisticsArticle>(sql, new { }).ToList();
        }

        public void InsertStatisticsArticle(List<StatisticsArticle> statisticsArticle)
        {
            using (var transaction = _dbcontext.BeginTransaction())
            {
                try
                {
                    string sql = @" INSERT INTO [dbo].[share_statistics]
                        ([Id]  ,[ShareArticleId]  ,[PV] ,[UV] ,[ShareCount] ,[JumpCount]  ,[TimeSpent]  ,[Day]  ,[AddTime])  VALUES 
                        (NEWID(), @ShareArticleId, @PV, @UV, @ShareCount, @JumpCount, @TimeSpent, @Day,  getdate()); ";
                    var result = _dbcontext.Execute(sql, statisticsArticle); //直接传送list对象
                    _dbcontext.Commit();
                }
                catch (Exception)
                {
                    _dbcontext.Rollback();
                }
            }
        }
    }
}
