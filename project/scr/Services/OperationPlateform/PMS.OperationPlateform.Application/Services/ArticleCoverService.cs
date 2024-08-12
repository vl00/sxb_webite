using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    using PMS.OperationPlateform.Application.IServices;
    using PMS.OperationPlateform.Domain.DTOs;
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.IRespositories;
    using System.Linq;

    public class ArticleCoverService : IArticleCoverService
    {
        IArticleCoverRepository _articleCoverRepository;
        public ArticleCoverService(IArticleCoverRepository articleCoverRepository)
        {
            this._articleCoverRepository = articleCoverRepository;

        }


        public IEnumerable<article_cover> GetCoversByIds(Guid[] articleIds)
        {
            return this._articleCoverRepository.GetCoversByIds(articleIds);
        }
    }
}
