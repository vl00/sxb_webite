using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public interface IHotTypeService : IApplicationService<HotType>
    {
        IEnumerable<HotType> GetAll(int type = 1);
        Task<IEnumerable<Guid>> GetAllOrderIDs();
        Task<IEnumerable<Guid>> GetRandomOrderIDs(int pageIndex = 1, int pageSize = 10, bool flag = false);
        Task<IEnumerable<Guid>> GetOrderIDByHotTypeID(Guid hotTypeID, int pageIndex = 1, int pageSize = 10, int sortType = 1);
        Task<IEnumerable<(Guid, int)>> GetViewCountByOrderIDs(IEnumerable<Guid> ids);
        /// <summary>
        /// 修改浏览量
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="step">幅度</param>
        /// <param name="isUp">是否上增</param>
        /// <returns></returns>
        Task<bool> ChangeViewCount(Guid id, int step = 1, bool isUp = true);

        Task<HotQuestionExtend> GetHotQuestionDetail(Guid orderID);
        Task<IEnumerable<Guid>> GetRandomOrderIDByGrade(int gradeType);

        Task<IEnumerable<Guid>> GetRandomOrderIDByGrade(IEnumerable<int> gradeTypes);
        /// <summary>
        /// 根据学校类型从Redis获取推荐达人
        /// </summary>
        /// <param name="schFTypeCode">学校类型Code</param>
        /// <param name="userID">当前UserID</param>
        /// <returns></returns>
        Task<TalentDetailExtend> GetRecommendTalentBySchFTypeCode(string schFTypeCode, Guid userID);
    }
}
