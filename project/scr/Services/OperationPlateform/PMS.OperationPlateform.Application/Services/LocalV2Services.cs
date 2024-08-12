using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    public class LocalV2Services: ILocalV2Services
    {
        ILocalV2Repository currentRepository;

        public LocalV2Services(ILocalV2Repository localV2Repository)
        {
            currentRepository = localV2Repository;
        }


        public local_v2 GetById(int id)
        {
            return this.currentRepository.GetById(id);
        }

        public IEnumerable<local_v2> GetByParent(int pid)
        {
            return this.currentRepository.GetByParent(pid);

        }
    }
}
