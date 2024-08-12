using PMS.OperationPlateform.Domain.Entitys.Signup;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IHurunService
    {
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<(bool, string)> Add(HurunReportSignup entity);
    }
}
