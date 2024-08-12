using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface IHistoryRepository
    {
        bool AddHistory(Guid userID, Guid dataID, byte dataType);
        bool RemoveHistory(Guid userID, Guid dataID);

        List<Guid> GetUserHistory(Guid userID, byte dataType, int page = 1, int pageSize = 10);

        bool ChangeHistoryState(List<Guid> Ids, Guid UserId);
        bool ClearAllHistory(Guid userId,int type);
        List<UserInfo> GetHistoryUsers(string schFtype, Guid schoolExtId, Guid[] excludeUserIds, long offset, int size);
        List<UserInfo> GetHistoryUsers(Guid schoolExtId, Guid[] excludeUserIds, long offset, int size);
        long GetHistoryUserTotal(Guid schoolExtId, Guid[] excludeUserIds);
        Task<IEnumerable<Guid>> GetHistoryTopId(EnumSet.MessageDataType dataType, DateTime? startTime, DateTime? endTime, int size = 100);
        Task<IEnumerable<HistoryTopIdQueryDto>> GetHistoryTop(EnumSet.MessageDataType dataType, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize = 100);
    }
}
