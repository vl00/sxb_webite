using ProductManagement.Framework.MSSQLAccessor.WebExample.Core;
using ProductManagement.Framework.MSSQLAccessor.WebExample.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.MSSQLAccessor.WebExample.Service
{
    public class GroupOrderProgressService
    {
        private readonly GroupOrderProgressRepository _groupOrderProgressRepository;
        public GroupOrderProgressService(GroupOrderProgressRepository groupOrderProgressRepository)
        {
            this._groupOrderProgressRepository = groupOrderProgressRepository;
        }

        public GroupOrderProgressDomain GetGroupOrderProgressByOrderId(string orderid)
        {
            return _groupOrderProgressRepository.GetGroupOrderProgressByOrderId(orderid);
        }
        //public GroupOrderProgressDomain GetGroupOrderProgressByOrderId(string orderid, IDbTransaction transaction)
        //{
        //    return _groupOrderProgressRepository.GetGroupOrderProgressByOrderId(orderid,transaction);
        //}
        public int UpdatePayStatus(string orderid)
        {
            return _groupOrderProgressRepository.UpdatePayStatus(orderid);
        }
        //public int UpdatePayStatus(string orderid, IDbTransaction transaction)
        //{
        //    return _groupOrderProgressRepository.UpdatePayStatus(orderid, transaction);
        //}
    }
}
