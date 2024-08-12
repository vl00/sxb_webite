using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    public class LSRFLeveInfoService : ILSRFLeveInfoService
    {
        private ILSRFLeveInfoRepository _repository;

        public LSRFLeveInfoService(ILSRFLeveInfoRepository repository)
        {
            _repository = repository;
        }

        public bool Insert(LSRFLeveInfo lSRFLeveInfo)
        {
            return _repository.Insert(lSRFLeveInfo);
        }
    }
}
