using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ProductManagement.Infrastructure.AppService
{
    public abstract class AppService<DataObject> : IAppService<DataObject>
    {
        public abstract int Delete(Guid Id);

        public abstract IEnumerable<DataObject> GetList(Expression<Func<DataObject, bool>> where = null);

        public abstract DataObject GetModelById(Guid Id);

        public abstract int Insert(DataObject model);

        public abstract bool isExists(Expression<Func<DataObject, bool>> where);

        public abstract int Update(DataObject model);
    }
}
