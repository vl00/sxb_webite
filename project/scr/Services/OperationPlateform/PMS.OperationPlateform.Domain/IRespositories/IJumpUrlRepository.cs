using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IJumpUrlRepository
    {
        Task<JumpUrl> GetJumpUrl(string Id);
        Task<bool> AddJumpUrl(string Id, string url, string fw);
        Task<bool> IncreJumpUrl(string Id);
    }
}
