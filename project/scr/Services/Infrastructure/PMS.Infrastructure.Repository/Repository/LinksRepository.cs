using System;
using System.Collections.Generic;
using System.Linq;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;

namespace PMS.Infrastructure.Repository.Repository
{
    public class LinksRepository:ILinksRepository
    {
        private JcDbContext _dbContext;
        public LinksRepository(JcDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AddLinks(string title, string link, int sort = 0)
        {
            var result = _dbContext.Execute("INSERT INTO [dbo].[links](" +
                "[title], [link],[sort], [addTime],[updateTime]) " +
                "VALUES (@title, @link,@sort, getdate(), getdate()); ",
                 new { title , link, sort });
            return result > 0;
        }

        public bool ExsitLinks(string title)
        {
            string sql = "select count(1) from [dbo].[links] where title = @title; ";
            return _dbContext.QuerySingle<int>(sql, new { title }) > 0;
        }

        public List<Links> GetLinks()
        {
            return _dbContext.Query<Links>("select title,link as href,sort from [dbo].[links] order by sort asc;", new { }).ToList();
        }

        public bool RemoveLinks(string title)
        {
            string sql = "delete from [dbo].[links] where title = @title; ";
            return _dbContext.Execute(sql, new { title }) > 0;
        }

        public bool UpdateLinks(string title, string link , int sort = 0)
        {
            string sql = "update [dbo].[links] set link = @link,sort = @sort,updateTime = getdate()  where title = @title; ";
            return _dbContext.Execute(sql, new { title , link , sort }) > 0;
        }
   }
}
