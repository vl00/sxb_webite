using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys.Signup;
using PMS.OperationPlateform.Domain.IRespositories;
using PMS.School.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.Services
{
    public class HurunService : IHurunService
    {
        IHurunRepository _hurunRepository;
        public HurunService(IHurunRepository hurynRepository)
        {
            _hurunRepository = hurynRepository;
        }

        public async Task<(bool, string)> Add(HurunReportSignup entity)
        {
            await Task.Run(() =>
            {
                if (entity.ID == Guid.Empty) entity.ID = Guid.NewGuid();
            });

            if (await _hurunRepository.CheckExist(entity.Name.Trim(), entity.Contact.Trim()))
            {
                return (false, "数据已存在");
            }
            try
            {
                var schoolCount = 0;
                if (!string.IsNullOrWhiteSpace(entity.EastSchools))
                {
                    IEnumerable<string> object_East = JsonHelper.JSONToObject<IEnumerable<string>>(entity.EastSchools);//东
                    schoolCount += object_East.Count();
                }
                if (!string.IsNullOrWhiteSpace(entity.SouthSchools))
                {
                    IEnumerable<string> object_South = JsonHelper.JSONToObject<IEnumerable<string>>(entity.SouthSchools);//南
                    schoolCount += object_South.Count();
                }
                if (!string.IsNullOrWhiteSpace(entity.WesternSchools))
                {
                    IEnumerable<string> object_Western = JsonHelper.JSONToObject<IEnumerable<string>>(entity.WesternSchools);//西
                    schoolCount += object_Western.Count();
                }
                if (!string.IsNullOrWhiteSpace(entity.NorthernSchools))
                {
                    IEnumerable<string> object_Northern = JsonHelper.JSONToObject<IEnumerable<string>>(entity.NorthernSchools);//北
                    schoolCount += object_Northern.Count();
                }
                if (schoolCount < 5) return (false, "学校数量不足");
            }
            catch
            {
            }

            if (await _hurunRepository.Add(entity))
            {
                return (true, "success");
            }
            else
            {
                return (false, "insert db error");
            }

        }
    }
}
