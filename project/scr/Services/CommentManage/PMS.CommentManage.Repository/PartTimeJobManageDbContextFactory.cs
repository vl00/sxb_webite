using Microsoft.EntityFrameworkCore;

namespace PMS.CommentsManage.Repository
{
    public class PartTimeJobManageDbContextFactory 
    {


        public static PartTimeJobManageDbContext _dbContext;

        static PartTimeJobManageDbContextFactory()
        {
        }

        public static PartTimeJobManageDbContext GetDbContext()
        {
            if (_dbContext == null)
            {

                //var option = new DbContextOptions<PartTimeJobManageDbContext>();

                var builder = new DbContextOptionsBuilder<PartTimeJobManageDbContext>();
                //builder.UseLazyLoadingProxies().UseSqlServer("Server=mssql.8mb.xyz;Database=Test2;User ID=sa;Password=abc123..");
                _dbContext = new PartTimeJobManageDbContext(builder.Options);
            }
            return _dbContext;
        }
    }
}
