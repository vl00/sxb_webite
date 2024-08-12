using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    using IServices;
    using Domain.IRespositories;
    using PMS.OperationPlateform.Domain.Entitys;

    public class AdvertisingOptionService:IAdvertisingOptionService
    {
        IAdvertisingOptionRepository repository;

        public AdvertisingOptionService(IAdvertisingOptionRepository repository)
        {
            this.repository = repository;
        }




        public IEnumerable<AdvertisingOption> GetAdvertisingOptions(int location)
        {
           return this.repository.Select(" [location]=@location and show=1 and uptime<getdate() and EXPIRETIME>GETDATE() ", new { location }, " sort desc");
        }

        public IEnumerable<AdvertisingOption> GetAdvertisingOptions(int[] locations)
        {
            return this.repository.Select(" [location] in @locations and show=1 and uptime<getdate() and EXPIRETIME>GETDATE() ", new { locations }, " sort desc");
        }
    }

}
