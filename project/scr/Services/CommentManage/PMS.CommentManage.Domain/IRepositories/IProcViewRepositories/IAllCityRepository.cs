using PMS.CommentsManage.Domain.Entities.AdCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories
{
    public interface IAllCityRepository
    {
        CityCode GetAllCity();
    }
}
