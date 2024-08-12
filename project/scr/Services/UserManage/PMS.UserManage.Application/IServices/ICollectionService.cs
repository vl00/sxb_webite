using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.IServices
{
    public interface ICollectionService
    {
        int GetCollectionCount(Guid userID);
        List<Guid> GetUserCollection(Guid userID, byte dataType, int page = 1, int pageSize = 10);
        List<Guid> GetCollection(List<Guid> ids, Guid UserId);
        bool AddCollection(Guid userID, Guid dataID, byte dataType);
        bool RemoveCollection(Guid userID, Guid dataID);
        void RemoveCollections(Guid userID, List<Guid> dataIDs);
        /// <summary>
        /// 判断是否收藏
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="dataID">数据ID</param>
        /// <param name="dataType">0：文章、1:学校学部、2:问答、3:点评</param>
        /// <returns></returns>
        bool IsCollected(Guid userID, Guid dataID, CollectionDataType dataType);
        bool IsCollected(Guid userID, Guid dataID);
        List<Guid> GetUserCollection(Guid userID, byte dataType, int page);
        List<Guid> GetPageUserCollection(Guid userID, byte dataType, ref int total, int page = 1, int pageSize = 10);
        List<PMS.UserManage.Application.ModelDto.ModelVo.SchoolModel> GetSchoolCollection(Guid userID, double? lat = null, double? lng = null, int page = 1);
        VoBase GetCommentCollection(Guid userID, string cookieStr, int page = 1);
        List<QuestionVo> GetQACollection(Guid userID, string cookieStr, int page = 1);
        List<Data> GetArticleCollection(Guid userID, int page = 1);
        RootModel GetSchoolCollectionID(Guid userID);
        FollowFansTotal GetUserFollowFans(Guid userId);
        MydynamicTotal MydynamicTotal(Guid userId);
        long GetCollectionUserTotal(Guid userId, DateTime? startDate, DateTime? endDate);
    }
}
