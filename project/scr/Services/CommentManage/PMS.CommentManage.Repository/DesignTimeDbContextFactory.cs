using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PMS.CommentsManage.Repository
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CommentsManageDbContext>
    {
        public CommentsManageDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CommentsManageDbContext>();
            builder.UseSqlServer("Server=mssql.8mb.xyz,1434;Database=Test1;User ID=sa;Password=abc123..");
            return new CommentsManageDbContext(builder.Options);
        }
    }
}
