using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface ICollectionRepository
    {
        int GetCollectionCount(Guid userID);
        bool AddCollection(Guid userID, Guid dataID, byte dataType);
        bool RemoveCollection(Guid userID, Guid dataID);
        bool IsCollected(Guid userID, Guid dataID, int dataType);
        bool IsCollected(Guid userID, Guid dataID);
        List<Guid> GetUserCollection(Guid userID, byte dataType, int page = 1, int pageSize = 10);
        List<Guid> GetPageUserCollection(Guid userID, byte dataType, ref int total, int page = 1, int pageSize = 10);
        /// <summary>
        /// 这个userId有没有关注这些dataIds = Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<Guid> GetCollection(List<Guid> ids, Guid UserId);
        FollowFansTotal GetUserFollowFans(Guid userId);
        MydynamicTotal MydynamicTotal(Guid userId);
        long GetCollectionTotal(List<Guid> ids);
        long GetCollectionTotal(Guid? userId, Guid? dataId, EnumSet.CollectionDataType[] dataTypes);
        IEnumerable<Collection> GetCollections(Guid? userId, Guid? dataId, EnumSet.CollectionDataType[] dataTypes, int page = 1, int pageSize = 10);
        /// <summary>
        /// 这些userIds有没有关注dataId
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="dataId"></param>
        /// <returns></returns>
        IEnumerable<Collection> GetCollections(Guid[] userIds, Guid dataId);
        IEnumerable<CollectionDto> GetPagination(Guid[] userId, Guid[] dataId, EnumSet.CollectionDataType[] dataTypes, int pageIndex = 1, int pageSize = 10);
        IEnumerable<CollectionDto> GetPagination(Guid[] userIds, Guid? dataId, EnumSet.CollectionDataType dataType, int pageIndex = 1, int pageSize = 10);
    }
}
