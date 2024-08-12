using System;
using System.Collections.Generic;
using PMS.Infrastructure.Domain.Entities;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface IMetroInfoRepository
    {
        List<MetroInfo> GetMetroInfoList(int cityCode);
        List<MetroInfo> GetMetroInfoList(List<Guid> metroLineIds);
    }
}
