using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IJumpUrlService
    {
        Task AddCount(string url, string fw);
    }
}
