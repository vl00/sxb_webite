using PMS.Live.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;

namespace PMS.Live.Repository
{
    public class LectorRepository : ILectorRepository
    {
        LiveDBContext _DB;
        public LectorRepository(LiveDBContext dBContext)
        {
            _DB = dBContext;
        }


    }
}
