using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IKeyValueRespository:IBaseRepository<KeyValue>
    {
        KeyValue GetKey(string key);

    }
}
