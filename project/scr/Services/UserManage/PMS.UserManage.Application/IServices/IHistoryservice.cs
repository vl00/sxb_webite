using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.IServices
{
    public interface IHistoryService
    {
        bool AddHistory(Guid userID, Guid dataID, byte dataType);
        bool RemoveHistory(Guid userID, Guid dataID);
        List<Guid> GetUserHistory(Guid userID, byte dataType, int page = 1, int pageSize = 10);

        List<Data> GetArticleHistory(Guid userID, int page = 1);
        VoBase GetQAHistory(Guid userID, string cookieStr, int page = 1);
        VoBase GetCommentHistory(Guid userID, string cookieStr, int page = 1);
        List<SchoolModel> GetSchoolHistory(Guid userID, double? lat = null, double? lng = null, int page = 1);
        List<Guid> GetUserHistory(Guid userID, byte dataType, int page);

        bool ChangeHistoryState(List<Guid> Ids, Guid UserId);

        List<Data> ApiArticle(List<Guid> Ids);
        bool ClearAllHistory(Guid userId,int type);
        Task<IEnumerable<Guid>> RefreshAndGetTop(EnumSet.MessageDataType dataType);
        Task<IEnumerable<Guid>> GetRandomTop(EnumSet.MessageDataType dataType, int takeSize, IEnumerable<Guid> excludeIds);
        Task<IEnumerable<Guid>> GetTop(EnumSet.MessageDataType dataType, int takeSize);
        Task<bool> BulkSchoolViewCount();
    }
}
