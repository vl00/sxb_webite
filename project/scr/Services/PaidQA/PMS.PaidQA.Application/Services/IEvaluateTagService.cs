using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using PMS.PaidQA.Domain.EntityExtend;

namespace PMS.PaidQA.Application.Services
{
    public interface IEvaluateTagsService : IApplicationService<EvaluateTags>
    {
        /// <summary>
        /// 获取评价标签
        /// </summary>
        /// <param name="ids">评论ID</param>
        /// <returns>评论ID, 评论标签</returns>
        Task<IEnumerable<KeyValuePair<Guid, IEnumerable<EvaluateTags>>>> GetByEvaluateIDs(IEnumerable<Guid> ids);
    }
}
