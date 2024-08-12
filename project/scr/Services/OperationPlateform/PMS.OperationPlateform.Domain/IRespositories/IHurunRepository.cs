using PMS.OperationPlateform.Domain.Entitys.Signup;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IHurunRepository
    {
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Add(HurunReportSignup entity);

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="contact">联系方式</param>
        Task<bool> CheckExist(string name, string contact);
    }
}
